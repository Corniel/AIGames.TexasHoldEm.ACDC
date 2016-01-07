using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace AIGames.TexasHoldEm.ACDC.Game
{
	[DebuggerDisplay("{DebuggerDisplay}")]
	public struct PlayerStatus
	{
		private uint m_Pot;
		private int m_Stack;

		/// <summary>The player (type).</summary>
		public PlayerType Player { get { return (PlayerType)(m_Pot & 1); } }
		
		/// <summary>The stack of the player.</summary>
		public int Stack { get { return m_Stack; } }
		/// <summary>The chips in the pot of the player.</summary>
		public int Pot { get { return (int)(m_Pot >> 1); } }
		/// <summary>Gets the stack and the pot.</summary>
		public int Chips { get { return this.Stack + this.Pot; } }

		/// <summary>Handles a post.</summary>
		internal PlayerStatus Post(int post)
		{
			return ToPot(post);
		}
		/// <summary>Handles a call.</summary>
		internal PlayerStatus Call(int call)
		{
			return ToPot(call);
		}
		/// <summary>Handles a raise.</summary>
		internal PlayerStatus Raise(int raise, int call)
		{
			return ToPot(raise + call);
		}
		/// <summary>moves chips from the stack to the pot.</summary>
		private PlayerStatus ToPot(int amount)
		{
			var pot = Pot + amount;
			var state = new PlayerStatus()
			{
				m_Pot = (m_Pot & 1) | (uint)(pot << 1),
				m_Stack = (m_Stack - amount),
			};
			return state;
		}

		/// <summary>Handles a fold.</summary>
		internal PlayerStatus Fold()
		{
			return new PlayerStatus()
			{
				m_Pot = m_Pot & 1,
				m_Stack = m_Stack,
			};
		}
		/// <summary>Handles a win.</summary>
		internal PlayerStatus Win(int pot)
		{
			return new PlayerStatus()
			{
				m_Pot = m_Pot & 1,
				m_Stack = m_Stack + pot,
			};
		}

		/// <summary>Creates a new player state.</summary>
		public static PlayerStatus Create(int stack, PlayerType player)
		{
			if (stack < 0) { throw new ArgumentNullException("Stack should be positive."); }
			return new PlayerStatus()
			{
				m_Pot = (uint)player,
				m_Stack = stack,
			};
		}

		/// <summary>Represents a player state as a debug string.</summary>
		[ExcludeFromCodeCoverage, DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string DebuggerDisplay
		{
			get
			{
				return String.Format("Player{0}: {1} ({2})", (m_Pot & 1) + 1, this.Stack, this.Pot);
			}
		}
	}
}
