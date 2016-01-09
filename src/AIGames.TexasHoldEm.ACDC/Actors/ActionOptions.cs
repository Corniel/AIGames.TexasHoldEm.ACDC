using AIGames.TexasHoldEm.ACDC.Analysis;
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
			if (action != GameAction.Fold && rnd.NextDouble() > 0.995)
			{
				// Once in a while promote a random action.
				Insert(index, new ActionOption(action, 100, 1));
			}
			Insert(index, new ActionOption(action));
		}

		public void Sort(Record test, IEnumerable<Record> items)
		{
			if (Count == 1) { return; }

			foreach (var item in items)
			{
				foreach (var option in this)
				{
					var type = option.ActionType;
					if (type == item.Action.ActionType)
					{
						// To match amounts.
						test.Action = option.Action;
						var match = Matcher.Record(test, item);
						if (match > 0)
						{
							if (item.IsNew)
							{
								match *= 10;
							}
							option.Update(item.Profit, match);
						}
					}
				}
			}
			Sort();
		}
	}
}
