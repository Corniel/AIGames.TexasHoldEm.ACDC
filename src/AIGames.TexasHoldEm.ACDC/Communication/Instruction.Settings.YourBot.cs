﻿using System;
using System.Diagnostics;

namespace AIGames.TexasHoldEm.ACDC.Communication
{
	public struct YourBotInstruction : IInstruction
	{
		public YourBotInstruction(PlayerName name) { m_Name = name; }

		public PlayerName Name { get { return m_Name; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private PlayerName m_Name;

		public override string ToString() { return String.Format("Settings your_bot {0}", Name); }

		internal static IInstruction Parse(string[] splitted)
		{
			PlayerName name;
			if (Enum.TryParse<PlayerName>(splitted[2], true, out name))
			{
				return new YourBotInstruction(name);
			}
			return null;
		}
	}
}
