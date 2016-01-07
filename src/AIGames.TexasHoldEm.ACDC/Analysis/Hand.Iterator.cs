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
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

namespace AIGames.TexasHoldEm.ACDC.Analysis
{
	public partial class Hand : IComparable
	{
		/// <summary>
		/// An enumeration value for each of the 169 possible types of pocket cards.
		/// </summary>
		public enum PocketHand169Enum : int
		{
			/// <summary>
			/// Not a PocketPairType
			/// </summary>
			None = -1,
			/// <summary>
			/// Represents a pair of Aces (Pocket Rockets)
			/// </summary>
			PocketAA = 0,
			/// <summary>
			/// Represents a pair of Kings (Cowboys)
			/// </summary>
			PocketKK = 1,
			/// <summary>
			/// Represents a pair of Queens (Ladies)
			/// </summary>
			PocketQQ = 2,
			/// <summary>
			/// Represents a pair of Jacks (Fish hooks)
			/// </summary>
			PocketJJ = 3,
			/// <summary>
			/// Represents a pair of Tens (Rin Tin Tin)
			/// </summary>
			PocketTT = 4,
			/// <summary>
			/// Represents a pair of Nines (Gretzky)
			/// </summary>
			Pocket99 = 5,
			/// <summary>
			/// Represents a pair of Eights (Snowmen)
			/// </summary>
			Pocket88 = 6,
			/// <summary>
			/// Represents a pair of Sevens (Hockey Sticks)
			/// </summary>
			Pocket77 = 7,
			/// <summary>
			/// Represents a pair of Sixes (Route 66)
			/// </summary>
			Pocket66 = 8,
			/// <summary>
			/// Represents a pair of Fives (Speed Limit)
			/// </summary>
			Pocket55 = 9,
			/// <summary>
			/// Represents a pair of Fours (Sailboats)
			/// </summary>
			Pocket44 = 10,
			/// <summary>
			/// Represents a pair of Threes (Crabs)
			/// </summary>
			Pocket33 = 11,
			/// <summary>
			/// Represents a pair of Twos (Ducks)
			/// </summary>
			Pocket22 = 12,
			/// <summary>
			/// Represents Ace/King Suited (Big Slick)
			/// </summary>
			PocketAKs = 13,
			/// <summary>
			/// Represents Ace/King offsuit (Big Slick)
			/// </summary>
			PocketAKo = 14,
			/// <summary>
			/// Represents Ace/Queen Suited (Little Slick)
			/// </summary>
			PocketAQs = 15,
			/// <summary>
			/// Represents Ace/Queen offsuit (Little Slick)
			/// </summary>
			PocketAQo = 16,
			/// <summary>
			/// Represents Ace/Jack suited (Blackjack)
			/// </summary>
			PocketAJs = 17,
			/// <summary>
			/// Represents Ace/Jack offsuit (Blackjack)
			/// </summary>
			PocketAJo = 18,
			/// <summary>
			/// Represents Ace/Ten suited (Johnny Moss)
			/// </summary>
			PocketATs = 19,
			/// <summary>
			/// Represents Ace/Ten offsuit (Johnny Moss)
			/// </summary>
			PocketATo = 20,
			/// <summary>
			/// Represents Ace/Nine suited
			/// </summary>
			PocketA9s = 21,
			/// <summary>
			/// Represents Ace/Nine offsuit
			/// </summary>
			PocketA9o = 22,
			/// <summary>
			/// Represents Ace/Eight suited
			/// </summary>
			PocketA8s = 23,
			/// <summary>
			/// Represents Ace/Eight offsuit
			/// </summary>
			PocketA8o = 24,
			/// <summary>
			/// Represents Ace/seven suited
			/// </summary>
			PocketA7s = 25,
			/// <summary>
			/// Represents Ace/seven offsuit
			/// </summary>
			PocketA7o = 26,
			/// <summary>
			/// Represents Ace/Six suited
			/// </summary>
			PocketA6s = 27,
			/// <summary>
			/// Represents Ace/Six offsuit
			/// </summary>
			PocketA6o = 28,
			/// <summary>
			/// Represents Ace/Five suited
			/// </summary>
			PocketA5s = 29,
			/// <summary>
			/// Represents Ace/Five offsuit
			/// </summary>
			PocketA5o = 30,
			/// <summary>
			/// Represents Ace/Four suited
			/// </summary>
			PocketA4s = 31,
			/// <summary>
			/// Represents Ace/Four offsuit
			/// </summary>
			PocketA4o = 32,
			/// <summary>
			/// Represents Ace/Three suited
			/// </summary>
			PocketA3s = 33,
			/// <summary>
			/// Represents Ace/Three offsuit
			/// </summary>
			PocketA3o = 34,
			/// <summary>
			/// Represents Ace/Two suited
			/// </summary>
			PocketA2s = 35,
			/// <summary>
			/// Represents Ace/Two offsuit
			/// </summary>
			PocketA2o = 36,
			/// <summary>
			/// Represents King/Queen suited
			/// </summary>
			PocketKQs = 37,
			/// <summary>
			/// Represents King/Queen offsuit
			/// </summary>
			PocketKQo = 38,
			/// <summary>
			/// Represents King/Jack suited
			/// </summary>
			PocketKJs = 39,
			/// <summary>
			/// Represents King/Jack offsuit
			/// </summary>
			PocketKJo = 40,
			/// <summary>
			/// Represents King/Ten suited
			/// </summary>
			PocketKTs = 41,
			/// <summary>
			/// Represents King/Ten offsuit
			/// </summary>
			PocketKTo = 42,
			/// <summary>
			/// Represents King/Nine suited
			/// </summary>
			PocketK9s = 43,
			/// <summary>
			/// Represents King/Nine offsuit
			/// </summary>
			PocketK9o = 44,
			/// <summary>
			/// Represents King/Eight suited
			/// </summary>
			PocketK8s = 45,
			/// <summary>
			/// Represents King/Eight offsuit
			/// </summary>
			PocketK8o = 46,
			/// <summary>
			/// Represents King/Seven suited
			/// </summary>
			PocketK7s = 47,
			/// <summary>
			/// Represents King/Seven offsuit
			/// </summary>
			PocketK7o = 48,
			/// <summary>
			/// Represents King/Six suited
			/// </summary>
			PocketK6s = 49,
			/// <summary>
			/// Represents King/Six offsuit
			/// </summary>
			PocketK6o = 50,
			/// <summary>
			/// Represents King/Five suited
			/// </summary>
			PocketK5s = 51,
			/// <summary>
			/// Represents King/Five offsuit
			/// </summary>
			PocketK5o = 52,
			/// <summary>
			/// Represents King/Four suited
			/// </summary>
			PocketK4s = 53,
			/// <summary>
			/// Represents King/Four offsuit
			/// </summary>
			PocketK4o = 54,
			/// <summary>
			/// Represents King/Three suited
			/// </summary>
			PocketK3s = 55,
			/// <summary>
			/// Represents King/Three offsuit
			/// </summary>
			PocketK3o = 56,
			/// <summary>
			/// Represents King/Two suited
			/// </summary>
			PocketK2s = 57,
			/// <summary>
			/// Represents King/Two offsuit
			/// </summary>
			PocketK2o = 58,
			/// <summary>
			/// Represents Queen/Jack suited
			/// </summary>
			PocketQJs = 59,
			/// <summary>
			/// Represents Queen/Jack offsuit
			/// </summary>
			PocketQJo = 60,
			/// <summary>
			/// Represents Queen/Ten suited
			/// </summary>
			PocketQTs = 61,
			/// <summary>
			/// Represents Queen/Ten offsuit
			/// </summary>
			PocketQTo = 62,
			/// <summary>
			/// Represents Queen/Nine suited
			/// </summary>
			PocketQ9s = 63,
			/// <summary>
			/// Represents Queen/Nine offsuit
			/// </summary>
			PocketQ9o = 64,
			/// <summary>
			/// Represents Queen/Eight suited
			/// </summary>
			PocketQ8s = 65,
			/// <summary>
			/// Represents Queen/Eight offsuit
			/// </summary>
			PocketQ8o = 66,
			/// <summary>
			/// Represents Queen/Seven suited
			/// </summary>
			PocketQ7s = 67,
			/// <summary>
			/// Represents Queen/Seven offsuit
			/// </summary>
			PocketQ7o = 68,
			/// <summary>
			/// Represents Queen/Six suited
			/// </summary>
			PocketQ6s = 69,
			/// <summary>
			/// Represents Queen/Six offsuit
			/// </summary>
			PocketQ6o = 70,
			/// <summary>
			/// Represents Queen/Five suited
			/// </summary>
			PocketQ5s = 71,
			/// <summary>
			/// Represents Queen/Five offsuit
			/// </summary>
			PocketQ5o = 72,
			/// <summary>
			/// Represents Queen/Four suited
			/// </summary>
			PocketQ4s = 73,
			/// <summary>
			/// Represents Queen/Four offsuit
			/// </summary>
			PocketQ4o = 74,
			/// <summary>
			/// Represents Queen/Three suited
			/// </summary>
			PocketQ3s = 75,
			/// <summary>
			/// Represents Queen/Three offsuit
			/// </summary>
			PocketQ3o = 76,
			/// <summary>
			/// Represents Queen/Two suited
			/// </summary>
			PocketQ2s = 77,
			/// <summary>
			/// Represents Queen/Two offsuit
			/// </summary>
			PocketQ2o = 78,
			/// <summary>
			/// Represents Jack/Ten suited
			/// </summary>
			PocketJTs = 79,
			/// <summary>
			/// Represents Jack/Ten offsuit
			/// </summary>
			PocketJTo = 80,
			/// <summary>
			/// Represents Jack/Nine suited
			/// </summary>
			PocketJ9s = 81,
			/// <summary>
			/// Represents Jack/Nine offsuit
			/// </summary>
			PocketJ9o = 82,
			/// <summary>
			/// Represents Jack/Eight suited
			/// </summary>
			PocketJ8s = 83,
			/// <summary>
			/// Represents Jack/Eight offsuit
			/// </summary>
			PocketJ8o = 84,
			/// <summary>
			/// Represents Jack/Seven suited
			/// </summary>
			PocketJ7s = 85,
			/// <summary>
			/// Represents Jack/Seven offsuit
			/// </summary>
			PocketJ7o = 86,
			/// <summary>
			/// Represents Jack/Six suited
			/// </summary>
			PocketJ6s = 87,
			/// <summary>
			/// Represents Jack/Six offsuit
			/// </summary>
			PocketJ6o = 88,
			/// <summary>
			/// Represents Jack/Five suited
			/// </summary>
			PocketJ5s = 89,
			/// <summary>
			/// Represents Jack/Five offsuit
			/// </summary>
			PocketJ5o = 90,
			/// <summary>
			/// Represents Jack/Four suited.
			/// </summary>
			PocketJ4s = 91,
			/// <summary>
			/// Represents Jack/Four offsuit
			/// </summary>
			PocketJ4o = 92,
			/// <summary>
			/// Represents Jack/Three suited
			/// </summary>
			PocketJ3s = 93,
			/// <summary>
			/// Represents Jack/Three offsuit
			/// </summary>
			PocketJ3o = 94,
			/// <summary>
			/// Represents Jack/Two suited.
			/// </summary>
			PocketJ2s = 95,
			/// <summary>
			/// Represents Jack/Two offsuit
			/// </summary>
			PocketJ2o = 96,
			/// <summary>
			/// Represents Ten/Nine suited
			/// </summary>
			PocketT9s = 97,
			/// <summary>
			/// Represents Ten/Nine offsuit
			/// </summary>
			PocketT9o = 98,
			/// <summary>
			/// Represents Ten/Eigth suited
			/// </summary>
			PocketT8s = 99,
			/// <summary>
			/// Represents Ten/Eight offsuit
			/// </summary>
			PocketT8o = 100,
			/// <summary>
			/// Represents Ten/Seven suited
			/// </summary>
			PocketT7s = 101,
			/// <summary>
			/// Represents Ten/Seven offsuit
			/// </summary>
			PocketT7o = 102,
			/// <summary>
			/// Represents Ten/Six suited
			/// </summary>
			PocketT6s = 103,
			/// <summary>
			/// Represents Ten/Six offsuit
			/// </summary>
			PocketT6o = 104,
			/// <summary>
			/// Represents Ten/Five suited
			/// </summary>
			PocketT5s = 105,
			/// <summary>
			/// Represents Ten/Five offsuit
			/// </summary>
			PocketT5o = 106,
			/// <summary>
			/// Represents Ten/Four suited
			/// </summary>
			PocketT4s = 107,
			/// <summary>
			/// Represents Ten/Four offsuit
			/// </summary>
			PocketT4o = 108,
			/// <summary>
			/// Represents Ten/Three suited
			/// </summary>
			PocketT3s = 109,
			/// <summary>
			/// Represents Ten/Three offsuit
			/// </summary>
			PocketT3o = 110,
			/// <summary>
			/// Represents Ten/Two suited
			/// </summary>
			PocketT2s = 111,
			/// <summary>
			/// Represents Ten/Two offsuit
			/// </summary>
			PocketT2o = 112,
			/// <summary>
			/// Represents Nine/Eight suited
			/// </summary>
			Pocket98s = 113,
			/// <summary>
			/// Represents Nine/Eight offsuit
			/// </summary>
			Pocket98o = 114,
			/// <summary>
			/// Represents Nine/Seven suited
			/// </summary>
			Pocket97s = 115,
			/// <summary>
			/// Represents Nine/Seven offsuit
			/// </summary>
			Pocket97o = 116,
			/// <summary>
			/// Represents Nine/Six suited
			/// </summary>
			Pocket96s = 117,
			/// <summary>
			/// Represents Nine/Six offsuit
			/// </summary>
			Pocket96o = 118,
			/// <summary>
			/// Represents Nine/Five suited
			/// </summary>
			Pocket95s = 119,
			/// <summary>
			/// Represents Nine/Five offsuit
			/// </summary>
			Pocket95o = 120,
			/// <summary>
			/// Represents Nine/Four suited
			/// </summary>
			Pocket94s = 121,
			/// <summary>
			/// Represents Nine/Four offsuit
			/// </summary>
			Pocket94o = 122,
			/// <summary>
			/// Represents Nine/Three suited
			/// </summary>
			Pocket93s = 123,
			/// <summary>
			/// Represents Nine/Three offsuit
			/// </summary>
			Pocket93o = 124,
			/// <summary>
			/// Represents Nine/Two suited
			/// </summary>
			Pocket92s = 125,
			/// <summary>
			/// Represents Nine/Two offsuit
			/// </summary>
			Pocket92o = 126,
			/// <summary>
			/// Represents Eight/Seven Suited.
			/// </summary>
			Pocket87s = 127,
			/// <summary>
			/// Represents Eight/Seven offsuit
			/// </summary>
			Pocket87o = 128,
			/// <summary>
			/// Represents Eight/Six suited
			/// </summary>
			Pocket86s = 129,
			/// <summary>
			/// Represents Eight/Six offsuit
			/// </summary>
			Pocket86o = 130,
			/// <summary>
			/// Represents Eight/Five suited
			/// </summary>
			Pocket85s = 131,
			/// <summary>
			/// Represents Eight/Five offsuit
			/// </summary>
			Pocket85o = 132,
			/// <summary>
			/// Represents Eight/Four suited
			/// </summary>
			Pocket84s = 133,
			/// <summary>
			/// Represents Eight/Four offsuit
			/// </summary>
			Pocket84o = 134,
			/// <summary>
			/// Represents Eight/Three suited
			/// </summary>
			Pocket83s = 135,
			/// <summary>
			/// Represents Eight/Three offsuit
			/// </summary>
			Pocket83o = 136,
			/// <summary>
			/// Represents Eight/Two suited
			/// </summary>
			Pocket82s = 137,
			/// <summary>
			/// Represents Eight/Two offsuit
			/// </summary>
			Pocket82o = 138,
			/// <summary>
			/// Represents Seven/Six suited
			/// </summary>
			Pocket76s = 139,
			/// <summary>
			/// Represents Seven/Six offsuit
			/// </summary>
			Pocket76o = 140,
			/// <summary>
			/// Represents Seven/Five suited
			/// </summary>
			Pocket75s = 141,
			/// <summary>
			/// Represents Seven/Five offsuit
			/// </summary>
			Pocket75o = 142,
			/// <summary>
			/// Represents Seven/Four suited
			/// </summary>
			Pocket74s = 143,
			/// <summary>
			/// Represents Seven/Four offsuit
			/// </summary>
			Pocket74o = 144,
			/// <summary>
			/// Represents Seven/Three suited
			/// </summary>
			Pocket73s = 145,
			/// <summary>
			/// Represents Seven/Three offsuit
			/// </summary>
			Pocket73o = 146,
			/// <summary>
			/// Represents Seven/Two suited
			/// </summary>
			Pocket72s = 147,
			/// <summary>
			/// Represents Seven/Two offsuit
			/// </summary>
			Pocket72o = 148,
			/// <summary>
			/// Represents Six/Five suited
			/// </summary>
			Pocket65s = 149,
			/// <summary>
			/// Represents Six/Five offsuit
			/// </summary>
			Pocket65o = 150,
			/// <summary>
			/// Represents Six/Four suited
			/// </summary>
			Pocket64s = 151,
			/// <summary>
			/// Represents Six/Four offsuit
			/// </summary>
			Pocket64o = 152,
			/// <summary>
			/// Represents Six/Three suited
			/// </summary>
			Pocket63s = 153,
			/// <summary>
			/// Represents Six/Three offsuit
			/// </summary>
			Pocket63o = 154,
			/// <summary>
			/// Represents Six/Two suited
			/// </summary>
			Pocket62s = 155,
			/// <summary>
			/// Represents Six/Two offsuit
			/// </summary>
			Pocket62o = 156,
			/// <summary>
			/// Represents Five/Four suited
			/// </summary>
			Pocket54s = 157,
			/// <summary>
			/// Represents Five/Four offsuit
			/// </summary>
			Pocket54o = 158,
			/// <summary>
			/// Represents Five/Three suited
			/// </summary>
			Pocket53s = 159,
			/// <summary>
			/// Represents Five/Three offsuit
			/// </summary>
			Pocket53o = 160,
			/// <summary>
			/// Represents Five/Two suited
			/// </summary>
			Pocket52s = 161,
			/// <summary>
			/// Represents Five/Two offsuit
			/// </summary>
			Pocket52o = 162,
			/// <summary>
			/// Represent Four/Three suited
			/// </summary>
			Pocket43s = 163,
			/// <summary>
			/// Represents Four/Three offsuit
			/// </summary>
			Pocket43o = 164,
			/// <summary>
			/// Represents Four/Two suited
			/// </summary>
			Pocket42s = 165,
			/// <summary>
			/// Represents Four/Two offsuit
			/// </summary>
			Pocket42o = 166,
			/// <summary>
			/// Represents Three/Two suited
			/// </summary>
			Pocket32s = 167,
			/// <summary>
			/// Represents Three/Two offsuit
			/// </summary>
			Pocket32o = 168
		};
		
		/// <exclude/>
		static private Dictionary<ulong, PocketHand169Enum> pocketdict = new Dictionary<ulong, PocketHand169Enum>();

		/// <summary>
		/// Given a pocket pair mask, the PocketPairType cooresponding to this mask
		/// will be returned. 
		/// </summary>
		/// <param name="mask"></param>
		/// <returns></returns>
		public static PocketHand169Enum PocketHand169Type(ulong mask)
		{
#if DEBUG
			// mask must contain exactly 2 cards
			if (BitCount(mask) != 2)
				throw new ArgumentOutOfRangeException("mask");
#endif

			// Fill in dictionary
			if (pocketdict.Count == 0)
			{
				for (int i = 0; i < Pocket169Table.Length; i++)
				{
					foreach (ulong tmask in Pocket169Table[i])
					{
						pocketdict.Add(tmask, (PocketHand169Enum)i);
					}
				}
			}

			if (pocketdict.ContainsKey(mask))
				return pocketdict[mask];

			return PocketHand169Enum.None;
		}

		/// <summary>
		/// Enables a foreach command to enumerate all possible ncard hands.
		/// </summary>
		/// <param name="numberOfCards">the number of cards in the hand (must be between 1 and 7)</param>
		/// <returns></returns>
		/// <example>
		/// <code>
		/// // This method iterates through all possible 5 card hands and returns a count.
		/// public static long CountFiveCardHands()
		/// {
		///     long count = 0;
		/// 
		///     // Iterate through all possible 5 card hands
		///     foreach (ulong mask in Hands(5))
		///     {
		///         count++;
		///     }
		///
		///     // Validate results.
		///     System.Diagnostics.Debug.Assert(count == 2598960);
		///     return count;
		/// }
		/// </code>
		/// </example>
		public static IEnumerable<ulong> Hands(int numberOfCards)
		{
			int _i1, _i2, _i3, _i4, _i5, _i6, _i7, length;
			ulong _card1, _n2, _n3, _n4, _n5, _n6;

#if DEBUG
			// We only support 0-7 cards
			if (numberOfCards < 0 || numberOfCards > 7)
				throw new ArgumentOutOfRangeException("numberOfCards");
#endif

			switch (numberOfCards)
			{
				case 7:
					for (_i1 = NumberOfCards - 1; _i1 >= 0; _i1--)
					{
						_card1 = CardMasksTable[_i1];
						for (_i2 = _i1 - 1; _i2 >= 0; _i2--)
						{
							_n2 = _card1 | CardMasksTable[_i2];
							for (_i3 = _i2 - 1; _i3 >= 0; _i3--)
							{
								_n3 = _n2 | CardMasksTable[_i3];
								for (_i4 = _i3 - 1; _i4 >= 0; _i4--)
								{
									_n4 = _n3 | CardMasksTable[_i4];
									for (_i5 = _i4 - 1; _i5 >= 0; _i5--)
									{
										_n5 = _n4 | CardMasksTable[_i5];
										for (_i6 = _i5 - 1; _i6 >= 0; _i6--)
										{
											_n6 = _n5 | CardMasksTable[_i6];
											for (_i7 = _i6 - 1; _i7 >= 0; _i7--)
											{
												yield return _n6 | CardMasksTable[_i7];
											}
										}
									}
								}
							}
						}
					}
					break;
				case 6:
					for (_i1 = NumberOfCards - 1; _i1 >= 0; _i1--)
					{
						_card1 = CardMasksTable[_i1];
						for (_i2 = _i1 - 1; _i2 >= 0; _i2--)
						{
							_n2 = _card1 | CardMasksTable[_i2];
							for (_i3 = _i2 - 1; _i3 >= 0; _i3--)
							{
								_n3 = _n2 | CardMasksTable[_i3];
								for (_i4 = _i3 - 1; _i4 >= 0; _i4--)
								{
									_n4 = _n3 | CardMasksTable[_i4];
									for (_i5 = _i4 - 1; _i5 >= 0; _i5--)
									{
										_n5 = _n4 | CardMasksTable[_i5];
										for (_i6 = _i5 - 1; _i6 >= 0; _i6--)
										{
											yield return _n5 | CardMasksTable[_i6];
										}
									}
								}
							}
						}
					}
					break;
				case 5:
					for (_i1 = NumberOfCards - 1; _i1 >= 0; _i1--)
					{
						_card1 = CardMasksTable[_i1];
						for (_i2 = _i1 - 1; _i2 >= 0; _i2--)
						{
							_n2 = _card1 | CardMasksTable[_i2];
							for (_i3 = _i2 - 1; _i3 >= 0; _i3--)
							{
								_n3 = _n2 | CardMasksTable[_i3];
								for (_i4 = _i3 - 1; _i4 >= 0; _i4--)
								{
									_n4 = _n3 | CardMasksTable[_i4];
									for (_i5 = _i4 - 1; _i5 >= 0; _i5--)
									{
										yield return _n4 | CardMasksTable[_i5];
									}
								}
							}
						}
					}
					break;
				case 4:
					for (_i1 = NumberOfCards - 1; _i1 >= 0; _i1--)
					{
						_card1 = CardMasksTable[_i1];
						for (_i2 = _i1 - 1; _i2 >= 0; _i2--)
						{
							_n2 = _card1 | CardMasksTable[_i2];
							for (_i3 = _i2 - 1; _i3 >= 0; _i3--)
							{
								_n3 = _n2 | CardMasksTable[_i3];
								for (_i4 = _i3 - 1; _i4 >= 0; _i4--)
								{
									yield return _n3 | CardMasksTable[_i4];
								}
							}
						}
					}

					break;
				case 3:
					for (_i1 = NumberOfCards - 1; _i1 >= 0; _i1--)
					{
						_card1 = CardMasksTable[_i1];
						for (_i2 = _i1 - 1; _i2 >= 0; _i2--)
						{
							_n2 = _card1 | CardMasksTable[_i2];
							for (_i3 = _i2 - 1; _i3 >= 0; _i3--)
							{
								yield return _n2 | CardMasksTable[_i3];
							}
						}
					}
					break;
				case 2:
					length = TwoCardTable.Length;
					for (_i1 = 0; _i1 < length; _i1++)
					{
						yield return TwoCardTable[_i1];
					}
					break;
				case 1:
					length = CardMasksTable.Length;
					for (_i1 = 0; _i1 < length; _i1++)
					{
						yield return CardMasksTable[_i1];
					}
					break;
				default:
					Debug.Assert(false);
					yield return 0UL;
					break;
			}
		}

		/// <summary>
		/// Enables a foreach command to enumerate all possible ncard hands.
		/// </summary>
		/// <param name="shared">A bitfield containing the cards that must be in the enumerated hands</param>
		/// <param name="dead">A bitfield containing the cards that must not be in the enumerated hands</param>
		/// <param name="numberOfCards">the number of cards in the hand (must be between 1 and 7)</param>
		/// <returns></returns>
		/// <example>
		/// <code>
		/// // Counts all remaining hands in a 7 card holdem hand.
		/// static long CountHands(string partialHand)
		/// {
		///     long count = 0;
		/// 
		///     // Parse hand and create a mask
		///     ulong partialHandmask = Hand.ParseHand(partialHand);
		/// 
		///     // iterate through all 7 card hands that share the cards in our partial hand.
		///    foreach (ulong handmask in Hand.Hands(partialHandmask, 0UL, 7))
		///    {
		///        count++;
		///    }
		/// 
		///    return count;
		///  }
		/// </code>
		/// </example>
		public static IEnumerable<ulong> Hands(ulong shared, ulong dead, int numberOfCards)
		{
			int _i1, _i2, _i3, _i4, _i5, _i6, _i7, length;
			ulong _card1, _card2, _card3, _card4, _card5, _card6, _card7;
			ulong _n2, _n3, _n4, _n5, _n6;

			dead |= shared;

			switch (numberOfCards - BitCount(shared))
			{
				case 7:
					for (_i1 = NumberOfCards - 1; _i1 >= 0; _i1--)
					{
						_card1 = CardMasksTable[_i1];
						if ((dead & _card1) != 0) continue;
						for (_i2 = _i1 - 1; _i2 >= 0; _i2--)
						{
							_card2 = CardMasksTable[_i2];
							if ((dead & _card2) != 0) continue;
							_n2 = _card1 | _card2;
							for (_i3 = _i2 - 1; _i3 >= 0; _i3--)
							{
								_card3 = CardMasksTable[_i3];
								if ((dead & _card3) != 0) continue;
								_n3 = _n2 | _card3;
								for (_i4 = _i3 - 1; _i4 >= 0; _i4--)
								{
									_card4 = CardMasksTable[_i4];
									if ((dead & _card4) != 0) continue;
									_n4 = _n3 | _card4;
									for (_i5 = _i4 - 1; _i5 >= 0; _i5--)
									{
										_card5 = CardMasksTable[_i5];
										if ((dead & _card5) != 0) continue;
										_n5 = _n4 | _card5;
										for (_i6 = _i5 - 1; _i6 >= 0; _i6--)
										{
											_card6 = CardMasksTable[_i6];
											if ((dead & _card6) != 0) continue;
											_n6 = _n5 | _card6;
											for (_i7 = _i6 - 1; _i7 >= 0; _i7--)
											{
												_card7 = CardMasksTable[_i7];
												if ((dead & _card7) != 0) continue;
												yield return _n6 | _card7 | shared;
											}
										}
									}
								}
							}
						}
					}
					break;
				case 6:
					for (_i1 = NumberOfCards - 1; _i1 >= 0; _i1--)
					{
						_card1 = CardMasksTable[_i1];
						if ((dead & _card1) != 0) continue;
						for (_i2 = _i1 - 1; _i2 >= 0; _i2--)
						{
							_card2 = CardMasksTable[_i2];
							if ((dead & _card2) != 0) continue;
							_n2 = _card1 | _card2;
							for (_i3 = _i2 - 1; _i3 >= 0; _i3--)
							{
								_card3 = CardMasksTable[_i3];
								if ((dead & _card3) != 0) continue;
								_n3 = _n2 | _card3;
								for (_i4 = _i3 - 1; _i4 >= 0; _i4--)
								{
									_card4 = CardMasksTable[_i4];
									if ((dead & _card4) != 0) continue;
									_n4 = _n3 | _card4;
									for (_i5 = _i4 - 1; _i5 >= 0; _i5--)
									{
										_card5 = CardMasksTable[_i5];
										if ((dead & _card5) != 0) continue;
										_n5 = _n4 | _card5;
										for (_i6 = _i5 - 1; _i6 >= 0; _i6--)
										{
											_card6 = CardMasksTable[_i6];
											if ((dead & _card6) != 0)
												continue;
											yield return _n5 | _card6 | shared;
										}
									}
								}
							}
						}
					}
					break;
				case 5:
					for (_i1 = NumberOfCards - 1; _i1 >= 0; _i1--)
					{
						_card1 = CardMasksTable[_i1];
						if ((dead & _card1) != 0) continue;
						for (_i2 = _i1 - 1; _i2 >= 0; _i2--)
						{
							_card2 = CardMasksTable[_i2];
							if ((dead & _card2) != 0) continue;
							_n2 = _card1 | _card2;
							for (_i3 = _i2 - 1; _i3 >= 0; _i3--)
							{
								_card3 = CardMasksTable[_i3];
								if ((dead & _card3) != 0) continue;
								_n3 = _n2 | _card3;
								for (_i4 = _i3 - 1; _i4 >= 0; _i4--)
								{
									_card4 = CardMasksTable[_i4];
									if ((dead & _card4) != 0) continue;
									_n4 = _n3 | _card4;
									for (_i5 = _i4 - 1; _i5 >= 0; _i5--)
									{
										_card5 = CardMasksTable[_i5];
										if ((dead & _card5) != 0) continue;
										yield return _n4 | _card5 | shared;
									}
								}
							}
						}
					}
					break;
				case 4:
					for (_i1 = NumberOfCards - 1; _i1 >= 0; _i1--)
					{
						_card1 = CardMasksTable[_i1];
						if ((dead & _card1) != 0) continue;
						for (_i2 = _i1 - 1; _i2 >= 0; _i2--)
						{
							_card2 = CardMasksTable[_i2];
							if ((dead & _card2) != 0) continue;
							_n2 = _card1 | _card2;
							for (_i3 = _i2 - 1; _i3 >= 0; _i3--)
							{
								_card3 = CardMasksTable[_i3];
								if ((dead & _card3) != 0) continue;
								_n3 = _n2 | _card3;
								for (_i4 = _i3 - 1; _i4 >= 0; _i4--)
								{
									_card4 = CardMasksTable[_i4];
									if ((dead & _card4) != 0) continue;
									yield return _n3 | _card4 | shared;
								}
							}
						}
					}

					break;
				case 3:
					for (_i1 = NumberOfCards - 1; _i1 >= 0; _i1--)
					{
						_card1 = CardMasksTable[_i1];
						if ((dead & _card1) != 0) continue;
						for (_i2 = _i1 - 1; _i2 >= 0; _i2--)
						{
							_card2 = CardMasksTable[_i2];
							if ((dead & _card2) != 0) continue;
							_n2 = _card1 | _card2;
							for (_i3 = _i2 - 1; _i3 >= 0; _i3--)
							{
								_card3 = CardMasksTable[_i3];
								if ((dead & _card3) != 0) continue;
								yield return _n2 | _card3 | shared;
							}
						}
					}
					break;
				case 2:
					length = TwoCardTable.Length;
					for (_i1 = 0; _i1 < length; _i1++)
					{
						_card1 = TwoCardTable[_i1];
						if ((dead & _card1) != 0) continue;
						yield return _card1 | shared;
					}
					break;
				case 1:
					length = CardMasksTable.Length;
					for (_i1 = 0; _i1 < length; _i1++)
					{
						_card1 = CardMasksTable[_i1];
						if ((dead & _card1) != 0) continue;
						yield return _card1 | shared;
					}
					break;
				case 0:
					yield return shared;
					break;
				default:
					yield return 0UL;
					break;
			}
		}
	}
}
