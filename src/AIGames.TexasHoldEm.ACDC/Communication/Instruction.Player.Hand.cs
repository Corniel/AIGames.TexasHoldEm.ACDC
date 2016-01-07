using System;
using System.Diagnostics;

namespace AIGames.TexasHoldEm.ACDC.Communication
{
	public struct HandInstruction : IInstruction
	{
		public HandInstruction(PlayerName name, Cards hand)
		{
			m_Name = name;
			m_Value = hand; 
		}

		public PlayerName Name { get { return m_Name; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private PlayerName m_Name;

		public Cards Value { get { return m_Value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Cards m_Value;

		public override string ToString() { return String.Format("{0} hand {1}", Name, Value); }

		internal static IInstruction Parse(PlayerName name, string[] splited)
		{
			var hand = Cards.Parse(splited[2]);
			return new HandInstruction(name, hand);
		}
	}
}
