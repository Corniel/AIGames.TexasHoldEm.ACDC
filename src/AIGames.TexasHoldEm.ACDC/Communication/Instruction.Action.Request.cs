using System;
using System.Diagnostics;

namespace AIGames.TexasHoldEm.ACDC.Communication
{
	public struct RequestMoveInstruction : IInstruction
	{
		public RequestMoveInstruction(PlayerName name, TimeSpan time) 
		{
			m_Name = name;
			m_Time = time; }

		public TimeSpan Time { get { return m_Time; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TimeSpan m_Time;

		public PlayerName Name { get { return m_Name; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private PlayerName m_Name;

		public override string ToString()
		{
			return String.Format("Action {0} {1:0}",Name, Time.TotalMilliseconds);
		}

		internal static IInstruction Parse(string[] splited)
		{
			var name = Player.TryParse(splited[1]);
			int ms;
			if (name != PlayerName.None && splited.Length == 3 && Int32.TryParse(splited[2], out ms))
			{
				return new RequestMoveInstruction(name, TimeSpan.FromMilliseconds(ms));
			}
			return null;
		}
	}
}
