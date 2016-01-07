using System;
using System.Diagnostics;

namespace AIGames.TexasHoldEm.ACDC.Communication
{
	public struct RoundInstruction : IInstruction
	{
		public RoundInstruction(int round) { m_Round = round; }

		public int Round { get { return m_Round; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_Round;

		public override string ToString() { return String.Format("Match round {0}", Round); }

		internal static IInstruction Parse(string[] splited)
		{
			int round;
			if (Int32.TryParse(splited[2], out round) && round > 0)
			{
				return new RoundInstruction(round);
			}
			return null;
		}
	}
}
