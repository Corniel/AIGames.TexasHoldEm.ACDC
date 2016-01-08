using System;
using System.Collections.Generic;
using Troschuetz.Random.Generators;

namespace AIGames.TexasHoldEm.ACDC.Actors
{
	public class ActionOptions: List<ActionOption>
	{
		private static readonly MT19937Generator rnd = new MT19937Generator(17);

		public void Add(GameAction action, double profit)
		{
			Add(new ActionOption(action, profit));
		}
		public void Add(GameAction action)
		{
			var index = rnd.Next(Count + 1);
			Insert(index, new ActionOption(action));
		}
	}
}
