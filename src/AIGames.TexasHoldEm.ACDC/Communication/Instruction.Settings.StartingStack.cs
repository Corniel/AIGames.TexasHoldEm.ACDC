using System;
using System.Diagnostics;

namespace AIGames.TexasHoldEm.ACDC.Communication
{
	public struct StartingStackInstruction : IInstruction
	{
		public StartingStackInstruction(int Stack) { m_Stack = Stack; }

		public Int32 Stack { get { return m_Stack; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Int32 m_Stack;

		public override string ToString() { return String.Format("Settings starting_stack {0}", Stack); }


		internal static IInstruction Parse(string[] splitted)
		{
			int Stack;
			if (Int32.TryParse(splitted[2], out Stack))
			{
				return new StartingStackInstruction(Stack);
			}
			return null;
		}

	}
}
