﻿using System;
using System.Diagnostics;

namespace AIGames.TexasHoldEm.ACDC.Communication
{
	public struct BigBlindInstruction : IInstruction
	{
		public BigBlindInstruction(int value) { m_Value = value; }

		public int Value { get { return m_Value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_Value;

		public override string ToString() { return String.Format("Match big_blind {0}", Value); }

		internal static IInstruction Parse(string[] splited)
		{
			int value;
			if (Int32.TryParse(splited[2], out value) && value > 0)
			{
				return new BigBlindInstruction(value);
			}
			return null;
		}
	}
}
