using System;
using System.Collections.Generic;

namespace AIGames.TexasHoldEm.ACDC.Actors
{
	public class ActionOptions: List<ActionOption>
	{
		public void Add(GameAction action, double profit)
		{
			Add(new ActionOption(action, profit));
		}
		public void Add(GameAction action)
		{
			var index = (int)(DateTime.Now.Ticks % (Count + 1));
			Insert(index, new ActionOption(action));
		}
	}
}
