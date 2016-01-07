using System;
using System.Diagnostics;

namespace AIGames.TexasHoldEm.ACDC.Communication
{
	public struct ActionInstruction : IInstruction
	{
		public ActionInstruction(PlayerName name, GameAction value)
		{
			m_Name = name;
			m_Value = value; 
		}

		public PlayerName Name { get { return m_Name; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private PlayerName m_Name;

		public GameAction Value { get { return m_Value; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private GameAction m_Value;

		public override string ToString() { return String.Format("{0} {1}", Name, Value); }

		internal static IInstruction Parse(PlayerName name, string[] splited)
		{
			var type = (GameActionType)Enum.Parse(typeof(GameActionType), splited[1], true);

			int value;
			if (Int32.TryParse(splited[2], out value) && value >= 0)
			{
				switch (type)
				{
					case GameActionType.fold: return new ActionInstruction(name, GameAction.Fold);
					case GameActionType.call: return new ActionInstruction(name, GameAction.Call);
					case GameActionType.check: return new ActionInstruction(name, GameAction.Check);
					case GameActionType.raise: return new ActionInstruction(name, GameAction.Raise(value));
				}
			}
			return null;
		}
	}
}
