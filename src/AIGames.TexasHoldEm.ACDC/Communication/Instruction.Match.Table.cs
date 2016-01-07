using System;
using System.Diagnostics;

namespace AIGames.TexasHoldEm.ACDC.Communication
{
	public struct TableInstruction : IInstruction
	{
		public TableInstruction(Cards hand)
		{
			m_Value = hand; 
		}

		public Cards Value { get { return m_Value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Cards m_Value;

		public override string ToString() { return String.Format("Match table {0}", Value); }

		internal static IInstruction Parse(string[] splited)
		{
			var table = Cards.Parse(splited[2]);
			return new TableInstruction(table);
		}
	}
}
