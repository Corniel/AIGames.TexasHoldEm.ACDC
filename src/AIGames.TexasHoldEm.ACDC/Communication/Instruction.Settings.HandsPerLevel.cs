using System;
using System.Diagnostics;

namespace AIGames.TexasHoldEm.ACDC.Communication
{
	public struct HandsPerLevelInstruction : IInstruction
	{
		public HandsPerLevelInstruction(int hands) { m_Hands = hands; }

		public Int32 Hands { get { return m_Hands; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Int32 m_Hands;

		public override string ToString() { return String.Format("Settings hands_per_level {0}", Hands); }


		internal static IInstruction Parse(string[] splitted)
		{
			int hands;
			if (Int32.TryParse(splitted[2], out hands))
			{
				return new HandsPerLevelInstruction(hands);
			}
			return null;
		}

	}
}
