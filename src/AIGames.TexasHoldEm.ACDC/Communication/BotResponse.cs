using System;

namespace AIGames.TexasHoldEm.ACDC.Communication
{
	public class BotResponse
	{
		public GameAction Action { get; set; }
		public string Log { get; set; }

		public override string ToString()
		{
			return String.Format("Move: {0}, Log: {1}", Action, Log);
		}
	}
}
