using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace AIGames.TexasHoldEm.ACDC.Game
{
	public struct GameStatus
	{
		private PlayerStatus m_Player1;
		private PlayerStatus m_Player2;
		private short m_SmallBlind;
		private short m_Round;
		private short m_Data;

		/// <summary>Gets the current round.</summary>
		public int Round { get { return 1 + (m_Round >> 3); } }
		/// <summary>Gets the current sub round.</summary>
		public SubRoundType SubRound { get { return (SubRoundType)(m_Round & 3); } }
		/// <summary>The name of the bot that currently has the dealer button.</summary>
		public PlayerType PlayerToMove { get { return (PlayerType)((m_Data >> 1) & 1); } }

		/// <summary>The name of the bot that currently has the dealer button.</summary>
		public PlayerType Button { get { return (PlayerType)(m_Data & 1); } }

		/// <summary>The current size of the small blind.</summary>
		public int SmallBlind { get { return m_SmallBlind; } }
		/// <summary>The current size of the big blind.</summary>
		public int BigBlind { get { return m_SmallBlind << 1; } }

		/// <summary>Represents player1 specific data.</summary>
		public PlayerStatus Player1 { get { return m_Player1; } }
		/// <summary>Represents player2 specific data.</summary>
		public PlayerStatus Player2 { get { return m_Player2; } }

		/// <summary>Get player based on the player type.</summary>
		public PlayerStatus this[PlayerType tp]
		{
			get
			{
				switch (tp)
				{
					case PlayerType.player1: return Player1;
					case PlayerType.player2: default: return Player2;
				}
			}
			private set
			{
				switch (tp)
				{
					case PlayerType.player1: m_Player1 = value; break;
					case PlayerType.player2: m_Player2 = value; break;
				}
			}
		}

		/// <summary>The amount of chips your bot has to put in to call.</summary>
		public int AmountToCall { get { return Math.Abs(Player1.Pot - Player2.Pot); } }

		/// <summary>Returns true if their is no amount to call, otherwise false.</summary>
		public bool NoAmountToCall { get { return Player1.Pot == Player2.Pot; } }

		/// <summary>Test the minimum amount to raise.</summary>
		public int MinimumRaise
		{
			get
			{
				// Don't raise on small blind.
				if (AmountToCall == SmallBlind) { return 0; }
				return SmallestStack < BigBlind ? 0 : BigBlind;
			}
		}
		/// <summary>Test the maximum amount to raise.</summary>
		public int MaxinumRaise
		{
			get
			{
				// If minimum raise is 0, maximum raise is zero too.
				if (MinimumRaise == 0) { return 0; }
				return Math.Min(SmallestStack, Player1.Pot + Player2.Pot + AmountToCall);
			}
		}

		/// <summary>Returns true if there is an option to raise, otherwise false.</summary>
		public bool CanRaise { get { return MinimumRaise > 0; } }

		private int SmallestStack
		{
			get
			{
				var atc = Player1.Pot - Player2.Pot;
				var p1 = Player1.Stack - atc <= 0 ? 0 : -atc;
				var p2 = Player2.Stack - atc >= 0 ? 0 : +atc;
				return Math.Min(p1, p2);
			}
		}

		public GameStatus Apply(PlayerType playerToMove, GameAction action)
		{
			if (PlayerToMove != playerToMove) { throw new ArgumentOutOfRangeException(); }
			if (action == GameAction.Check && !NoAmountToCall) { throw new ArgumentException(); }
			if (action == GameAction.Call && NoAmountToCall) { throw new ArgumentException(); }
			if (action.ActionType == GameActionType.raise && !CanRaise) { throw new ArgumentException(); }
			if (action.ActionType == GameActionType.raise && action.Amount < MinimumRaise) { throw new ArgumentException(); }
			if (action.ActionType == GameActionType.raise && action.Amount > MaxinumRaise) { throw new ArgumentException(); }

			bool endOfRound = false;
			bool endOfSubround = false;

			var state = new GameStatus()
			{
				m_Player1 = m_Player1,
				m_Player2 = m_Player2,
				m_SmallBlind = m_SmallBlind,
				m_Data = m_Data,
			};

			// Handles a check.
			if (action == GameAction.Check)
			{
				endOfSubround = (m_Data & 3) > 1;
			}
			// handle the call.
			else if (action == GameAction.Call)
			{
				state[playerToMove] = state[playerToMove].Call(AmountToCall);
			}
			// Handle the raise.
			else if (action.ActionType == GameActionType.raise)
			{
				state[playerToMove] = state[playerToMove].Raise(action.Amount, AmountToCall);
			}
			// Handle the win.
			else if (action == GameAction.Fold)
			{
				endOfRound = true;

				var pot = this.Pot;
				state[playerToMove] = state[playerToMove].Fold();
				state[playerToMove.Other()] = state[playerToMove.Other()].Win(pot);
			}

			if (endOfSubround)
			{
				if (state.SubRound == SubRoundType.River)
				{
					endOfRound = true;
				}
				else
				{
					state.m_Round = (short)((Round << 3) + (int)state.SubRound - 7);
					var button = (int)Button;
					state.m_Data = (short)((int)button | ((int)button << 1));
				}
			}
			if (endOfRound)
			{
				state.m_Round = (short)(Round << 3);
				state.m_Data ^= 3;
				var button = (int)Button ^ 1;
				state.m_Data = (short)((int)button | ((int)button << 1));
			}
			else
			{
				if (state.PlayerToMove != state.Button)
				{
					state.m_Data |= 4;
				}
				state.m_Data ^= 2;
			}
			return state;
		}

		/// <summary>Gets the pot.</summary>
		public int Pot { get { return this.Player1.Pot + this.Player2.Pot; } }
		/// <summary>Gets the total of chips of the game.</summary>
		public int Chips { get { return this.Player1.Chips + this.Player2.Chips; } }

		public static GameStatus Create(int initialStack, int smallBlind, PlayerType button)
		{
			var p1 = PlayerStatus.Create(initialStack, PlayerType.player1);
			var p2 = PlayerStatus.Create(initialStack, PlayerType.player2);

			p1 = p1.Post(button == PlayerType.player1 ? smallBlind : smallBlind << 1);
			p2 = p2.Post(button == PlayerType.player2 ? smallBlind : smallBlind << 1);

			var state = new GameStatus()
			{
				m_Player1 = p1,
				m_Player2 = p2,
				m_SmallBlind = (short)smallBlind,
				m_Data = (short)((int)button | ((int)button << 1)),
			};

			return state;
		}
	}

	
}
