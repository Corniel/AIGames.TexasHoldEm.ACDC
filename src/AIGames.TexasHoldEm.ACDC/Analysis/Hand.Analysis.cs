// Much of this code is derived from poker.eval (look for it on sourceforge.net).
// This library is covered by the LGPL Gnu license. See http://www.gnu.org/copyleft/lesser.html 
// for more information on this license.

// This code is a very fast, native C# Texas Holdem hand evaluator (containing no interop or unsafe code). 
// This code can enumarate 35 million 5 card hands per second and 29 million 7 card hands per second on my desktop machine.
// That's not nearly as fast as the heavily macro-ed poker.eval C library. However, this implementation is
// in roughly the same ballpark for speed and is quite usable in C#.

// The speed ups are mostly table driven. That means that there are several very large tables included in this file. 
// The code is divided up into several files they are:
//      HandEvaluator.cs - base hand evaluator
//      HandIterator.cs - methods that support IEnumerable and methods that validate the hand evaluator
//      HandAnalysis.cs - methods to aid in analysis of Texas Holdem Hands.

// Written (ported) by Keith Rule - Sept 2005

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace AIGames.TexasHoldEm.ACDC.Analysis
{
	public partial class Hand : IComparable
	{
		#region Analysis Functions
		/// <summary>
		/// Used to calculate the wining information about each players hand. This function enumerates all 
		/// possible remaining hands and tallies win, tie and losses for each player. This function typically takes
		/// well less than a second regardless of the number of players.
		/// </summary>
		/// <param name="pockets">Array of pocket hand string, one for each player</param>
		/// <param name="board">the board cards</param>
		/// <param name="dead">the dead cards</param>
		/// <param name="wins">An array of win tallies, one for each player</param>
		/// <param name="ties">An array of tie tallies, one for each player</param>
		/// <param name="losses">An array of losses tallies, one for each player</param>
		/// <param name="totalHands">The total number of hands enumarated.</param>
		public static void HandOdds(string[] pockets, string board, string dead, long[] wins, long[] ties, long[] losses, ref long totalHands)
		{
			ulong[] pocketmasks = new ulong[pockets.Length];
			ulong[] pockethands = new ulong[pockets.Length];
			int count = 0, bestcount;
			ulong boardmask = 0UL, deadcards_mask = 0UL, deadcards = Hand.ParseHand(dead, ref count);

			totalHands = 0;
			deadcards_mask |= deadcards;

			// Read pocket cards
			for (int i = 0; i < pockets.Length; i++)
			{
				count = 0;
				pocketmasks[i] = Hand.ParseHand(pockets[i], "", ref count);
				if (count != 2)
					throw new ArgumentException("There must be two pocket cards."); // Must have 2 cards in each pocket card set.
				deadcards_mask |= pocketmasks[i];
				wins[i] = ties[i] = losses[i] = 0;
			}

			// Read board cards
			count = 0;
			boardmask = Hand.ParseHand("", board, ref count);
		   

#if DEBUG
			Debug.Assert(count >= 0 && count <= 5); // The board must have zero or more cards but no more than a total of 5

			// Check pocket cards, board, and dead cards for duplicates
			if ((boardmask & deadcards) != 0)
				throw new ArgumentException("Duplicate between cards dead cards and board");

			// Validate the input
			for (int i = 0; i < pockets.Length; i++)
			{
				for (int j = i + 1; j < pockets.Length; j++)
				{
					if ((pocketmasks[i] & pocketmasks[j]) != 0)
						throw new ArgumentException("Duplicate pocket cards");
				}

				if ((pocketmasks[i] & boardmask) != 0)
					throw new ArgumentException("Duplicate between cards pocket and board");

				if ((pocketmasks[i] & deadcards) != 0)
					throw new ArgumentException("Duplicate between cards pocket and dead cards");
			}
#endif

			// Iterate through all board possiblities that doesn't include any pocket cards.
			foreach (ulong boardhand in Hands(boardmask, deadcards_mask, 5))
			{
				// Evaluate all hands and determine the best hand
				ulong bestpocket = Evaluate(pocketmasks[0] | boardhand, 7);
				pockethands[0] = bestpocket;
				bestcount = 1;
				for (int i = 1; i < pockets.Length; i++)
				{
					pockethands[i] = Evaluate(pocketmasks[i] | boardhand, 7);
					if (pockethands[i] > bestpocket)
					{
						bestpocket = pockethands[i];
						bestcount = 1;
					}
					else if (pockethands[i] == bestpocket)
					{
						bestcount++;
					}
				}

				// Calculate wins/ties/loses for each pocket + board combination.
				for (int i = 0; i < pockets.Length; i++)
				{
					if (pockethands[i] == bestpocket)
					{
						if (bestcount > 1)
						{
							ties[i]++;
						}
						else
						{
							wins[i]++;
						}
					}
					else if (pockethands[i] < bestpocket)
					{
						losses[i]++;
					}
				}

				totalHands++;
			}
		}

		/// <summary>
		/// Returns the number of outs possible with the next card.
		/// </summary>
		/// <param name="player">Players pocket cards</param>
		/// <param name="board">THe board (must contain either 3 or 4 cards)</param>
		/// <param name="opponents">A list of zero or more opponent cards.</param>
		/// <returns>The count of the number of single cards that improve the current hand.</returns>
		public static int Outs(ulong player, ulong board, params ulong[] opponents)
		{
			return BitCount(OutsMask(player, board, opponents));
		}

		/// <summary>
		/// Creates a Hand mask with the cards that will improve the specified players hand
		/// against a list of opponents or if no opponents are list just the cards that improve the 
		/// players current had.
		/// 
		/// Please note that this only looks at single cards that improve the hand and will not specifically
		/// look at runner-runner possiblities.
		/// </summary>
		/// <param name="player">Players pocket cards</param>
		/// <param name="board">The board (must contain either 3 or 4 cards)</param>
		/// <param name="opponents">A list of zero or more opponent pocket cards</param>
		/// <returns>A mask of all of the cards taht improve the hand.</returns>
		public static ulong OutsMask(ulong player, ulong board, params ulong[] opponents)
		{
			ulong retval = 0UL, dead = 0UL;
			int ncards = Hand.BitCount(player | board);
#if DEBUG
			Debug.Assert(Hand.BitCount(player) == 2); // Must have two cards for a legit set of pocket cards
			if (ncards != 5 && ncards != 6) 
				throw new ArgumentException("Outs only make sense after the flop and before the river");
#endif           

			if (opponents.Length > 0)
			{
				// Check opportunities to improve against one or more opponents
				foreach (ulong opp in opponents)
				{
					Debug.Assert(Hand.BitCount(opp) == 2); // Must have two cards for a legit set of pocket cards
					dead |= opp;
				}

				uint playerOrigHandVal = Hand.Evaluate(player | board, ncards);
				uint playerOrigHandType = Hand.HandType(playerOrigHandVal);
				uint playerOrigTopCard = Hand.TopCard(playerOrigHandVal);

				foreach (ulong card in Hand.Hands(0UL, dead | board | player, 1))
				{
					bool bWinFlag = true;
					uint playerHandVal = Hand.Evaluate(player | board | card, ncards + 1);
					uint playerNewHandType = Hand.HandType(playerHandVal);
					uint playerNewTopCard = Hand.TopCard(playerHandVal);
					foreach (ulong oppmask in opponents)
					{
						uint oppHandVal = Hand.Evaluate(oppmask | board | card, ncards + 1);
					   
						bWinFlag = oppHandVal < playerHandVal && (playerNewHandType > playerOrigHandType || (playerNewHandType == playerOrigHandType && playerNewTopCard > playerOrigTopCard));
						if (!bWinFlag)
							break;
					}
					if (bWinFlag)
						retval |= card;
				}
			}
			else
			{
				// Look at the cards that improve the hand.
				uint playerOrigHandVal = Hand.Evaluate(player | board, ncards);
				uint playerOrigHandType = Hand.HandType(playerOrigHandVal);
				uint playerOrigTopCard = Hand.TopCard(playerOrigHandVal);

				// Look ahead one card
				foreach (ulong card in Hand.Hands(0UL, dead | board | player, 1))
				{
					uint playerNewHandVal = Hand.Evaluate(player | board | card, ncards + 1);
					uint playerONewHandType = Hand.HandType(playerNewHandVal);
					uint playerNewTopCard = Hand.TopCard(playerNewHandVal);
					if (playerONewHandType > playerOrigHandType || (playerONewHandType == playerOrigHandType && playerNewTopCard > playerOrigTopCard))
						retval |= card;
				}
			}

			return retval;
		}

		/// <summary>
		/// This function returns true if the cards in the hand are all one suit
		/// </summary>
		/// <param name="mask">hand to check for "suited-ness"</param>
		/// <returns>true if all hands are of the same suit, false otherwise.</returns>
		public static bool IsSuited(ulong mask)
		{
			int cards = BitCount(mask);
		   
			uint sc = CardMask(mask, Clubs);
			uint sd = CardMask(mask, Diamonds);
			uint sh = CardMask(mask, Hearts);
			uint ss = CardMask(mask, Spades);

			return  BitCount(sc) == cards || BitCount(sd) == cards ||
					BitCount(sh) == cards || BitCount(ss) == cards;
		}

		/// <summary>
		/// Returns true if the cards in the two card hand are connected.
		/// </summary>
		/// <param name="mask">the hand to check</param>
		/// <returns>true of all of the cards are next to each other.</returns>
		public static bool IsConnected(ulong mask)
		{
			return GapCount(mask) == 0;
		}

		/// <summary>
		/// Counts the number of empty space between adjacent cards. 0 means connected, 1 means a gap
		/// of one, 2 means a gap of two and 3 means a gap of three.
		/// </summary>
		/// <param name="mask">two card hand mask</param>
		/// <returns>number of spaces between two cards</returns>
		static public int GapCount(ulong mask)
		{
			int start, end;

			if (BitCount(mask) != 2) return -1;

			uint bf = CardMask(mask, Clubs) |
						CardMask(mask, Diamonds) |
						CardMask(mask, Hearts) |
						CardMask(mask, Spades);

			if (BitCount(bf) != 2) return -1;

			for (start = 12; start >= 0; start--)
			{
				if ((bf & (1UL << start)) != 0)
					break;
			}

			for (end = start - 1; end >= 0; end--)
			{
				if ((bf & (1UL << end)) != 0)
					break;
			}

			// Handle wrap
			if (start == 12 && end == 0) return 0;
			if (start == 12 && end == 1) return 1;
			if (start == 12 && end == 2) return 2;
			if (start == 12 && end == 3) return 3;

			return start-end-1;
		}

		/// <summary>
		/// Given a set of pocket cards and a set of board cards this function returns the odds of winning or tying for a player and a random opponent.
		/// </summary>
		/// <param name="ourcards">Pocket mask for the hand.</param>
		/// <param name="board">Board mask for hand</param>
		/// <param name="player">Player odds as doubles</param>
		/// <param name="opponent">Opponent odds as doubles</param>
		public static void HandPlayerOpponentOdds(ulong ourcards, ulong board, ref double[] player, ref double[] opponent)
		{
			uint ourbest, oppbest;
			int count = 0;
			int cards = BitCount(ourcards | board);
			int boardcount = BitCount(board);

			// Preconditions
			if (BitCount(ourcards) != 2) throw new ArgumentOutOfRangeException("pocketcards");
			if (boardcount > 5) throw new ArgumentOutOfRangeException("boardcards");
			if (player.Length != opponent.Length || player.Length != 9) throw new ArgumentOutOfRangeException();

			// Use precalcuated results for pocket cards
			if (boardcount == 0)
			{
				int index = (int)Hand.PocketHand169Type(ourcards);
				player = Hand.PreCalcPlayerOdds[index];
				opponent = Hand.PreCalcOppOdds[index];
				return;
			}

			// initialize return values
			for (int i = 0; i < player.Length; i++)
			{
				player[i] = opponent[i] = 0.0;
			}

			// Calculate results
			foreach (ulong oppcards in Hands(0UL, ourcards | board, 2))
			{
				foreach (ulong handmask in Hands(board, ourcards | oppcards, 5))
				{
					ourbest = Evaluate(ourcards | handmask, 7);
					oppbest = Evaluate(oppcards | handmask, 7);
					if (ourbest > oppbest)
					{
						player[(uint)HandType(ourbest)] += 1.0;
						count++;
					}
					else if (ourbest == oppbest)
					{
						player[(uint)HandType(ourbest)] += 0.5;
						opponent[(uint)HandType(oppbest)] += 0.5;
						count++;
					}
					else
					{
						opponent[(uint)HandType(oppbest)] += 1.0;
						count++;
					}
				}
			}

			for (int i = 0; i < 9; i++)
			{
				player[i] = player[i] / count;
				opponent[i] = opponent[i] / count;
			}
		}

		/// <summary>
		/// Given a set of pocket cards and a set of board cards this function returns the odds of winning or tying for a player and a random opponent.
		/// </summary>
		/// <param name="pocketcards">Pocket cards in ASCII</param>
		/// <param name="boardcards">Board cards in ASCII</param>
		/// <param name="player">Player odds as doubles</param>
		/// <param name="opponent">Opponent odds as doubles</param>
		public static void HandPlayerOpponentOdds(string pocketcards, string boardcards, ref double[] player, ref double[] opponent)
		{
			HandPlayerOpponentOdds(Hand.ParseHand(pocketcards), Hand.ParseHand(boardcards), ref player, ref opponent);
		}

		/// <summary>
		/// Internal function used by HandPotential.
		/// </summary>
		/// <param name="ourcards"></param>
		/// <param name="board"></param>
		/// <param name="oppcards"></param>
		/// <param name="index"></param>
		/// <param name="HP"></param>
		private static void HandPotentialOpp(ulong ourcards, ulong board, ulong oppcards, int index, ref int[,] HP)
		{
			const int ahead = 2;
			const int tied = 1;
			const int behind = 0;
			ulong dead_cards = ourcards | board | oppcards;
			uint ourbest, oppbest;

			foreach (uint handmask in Hands(0UL, ourcards | board | oppcards, 7-BitCount(ourcards | board))) 
			{
				ourbest = Evaluate(ourcards | board | handmask, 7);
				oppbest = Evaluate(oppcards | board | handmask, 7);
				if (ourbest > oppbest)
					HP[index, ahead]++;
				else if (ourbest == oppbest)
					HP[index, tied]++;
				else
					HP[index, behind]++;
			}
		}

		/// <summary>
		/// Returns the positive and negative potential of the current hand. This funciton
		/// is described in Aaron Davidson's masters thesis (davidson.msc.pdf).
		/// </summary>
		/// <param name="pocket">Hold Cards</param>
		/// <param name="board">Community cards</param>
		/// <param name="ppot">Positive Potential</param>
		/// <param name="npot">Negative Potential</param>
		public static void HandPotential(ulong pocket, ulong board, out double ppot, out double npot)
		{
			const int ahead = 2;
			const int tied = 1;
			const int behind = 0;

			int[,] HP = new int[3, 3];
			int[] HPTotal = new int[3];
			int cards = BitCount(pocket | board);
			double mult = (cards == 5 ? 990.0 : 45.0);

			if (cards < 5 || cards > 7)
				throw new ArgumentOutOfRangeException();

			// Initialize
			for (int i = 0; i < 3; i++)
			{
				HPTotal[i] = 0;
				for (int j = 0; j < 3; j++)
				{
					HP[i, j] = 0;
				}
			}

			// Rank our hand
			uint ourrank = Evaluate(pocket | board, BitCount(pocket | board));

			// Mark known cards as dead.
			ulong dead_cards = pocket | board;

			// Iterate through all possible opponent pocket cards
			foreach (uint oppPocket in Hands(0UL, dead_cards, 2)) {
				// Note Current State
				uint opprank = Evaluate(oppPocket | board, BitCount(oppPocket | board));
				if (ourrank > opprank)
				{
					HandPotentialOpp(pocket, board, oppPocket, ahead, ref HP);
					HPTotal[ahead]++;
				}
				else if (ourrank == opprank)
				{
					HandPotentialOpp(pocket, board, oppPocket, tied, ref HP);
					HPTotal[tied]++;
				}
				else
				{
					HandPotentialOpp(pocket, board, oppPocket, behind, ref HP);
					HPTotal[behind]++;
				}
			}

			double den1 = (mult * (HPTotal[behind] + (HPTotal[tied] / 2.0)));
			double den2 = (mult * (HPTotal[ahead] + (HPTotal[tied] / 2.0)));
			if (den1 > 0)
				ppot = (HP[behind, ahead] + (HP[behind, tied] / 2) + (HP[tied, ahead] / 2)) / (double)den1;
			else
				ppot = 0;
			if (den2 > 0)
				npot = (HP[ahead, behind] + (HP[ahead, tied] / 2) + (HP[tied, behind] / 2)) / (double)den2;
			else
				npot = 0;
		}
		#endregion
	}
}
