using System;
using System.Diagnostics;

namespace AIGames.TexasHoldEm.ACDC.Communication
{
	public struct StackInstruction : IInstruction
	{
		public StackInstruction(PlayerName name, int value)
		{
			m_Name = name;
			m_Value = value; 
		}

		public PlayerName Name { get { return m_Name; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private PlayerName m_Name;

		public int Value { get { return m_Value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_Value;

		public override string ToString() { return String.Format("{0} stack {1}", Name, Value); }

		internal static IInstruction Parse(PlayerName name, string[] splited)
		{
			int value;
			if (Int32.TryParse(splited[2], out value) && value >= 0)
			{
				return new StackInstruction(name, value);
			}
			return null;
		}
	}
}
