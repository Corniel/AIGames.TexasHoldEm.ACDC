using System;
using System.Diagnostics;

namespace AIGames.TexasHoldEm.ACDC.Communication
{
	public struct TimePerMoveInstruction : IInstruction
	{
		public TimePerMoveInstruction(TimeSpan duration) { m_Duration = duration; }

		public TimeSpan Duration { get { return m_Duration; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TimeSpan m_Duration;

		public override string ToString() { return String.Format("Settings time_per_move {0:0}", Duration.TotalMilliseconds); }


		internal static IInstruction Parse(string[] splitted)
		{
			int ms;
			if (Int32.TryParse(splitted[2], out ms))
			{
				return new TimePerMoveInstruction(TimeSpan.FromMilliseconds(ms));
			}
			return null;
		}

	}
}
