using AIGames.TexasHoldEm.ACDC.Analysis;
using System.Collections.Generic;
using Troschuetz.Random.Generators;
using System.Linq;

namespace AIGames.TexasHoldEm.ACDC.Actors
{
	public class ActionOptions: List<ActionOption>
	{
		private static readonly MT19937Generator rnd = new MT19937Generator(17);

		public void Add(GameAction action)
		{
			if (action == GameAction.Fold)
			{
				Add(new ActionOption(action, 0, 1));
			}
			else
			{
				var index = rnd.Next(Count + 1);

				if (rnd.Next(10000) == 0)
				{
					// Once in a while promote a random action.
					Insert(index, new ActionOption(action, 100, 1));
				}
				else
				{
					Insert(index, new ActionOption(action));
				}
			}
		}

		public void Sort(Record test, IEnumerable<Record> items)
		{
			if (Count == 1) { return; }

			foreach (var item in items)
			{
				foreach (var option in this)
				{
					var type = option.ActionType;
					if (type == item.Action.ActionType &&
						item.HasAmountToCall == test.HasAmountToCall)
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

		public GameAction Action { get { return Best.Action; } }

		public ActionOption Best
		{
			get
			{
				if (Count == 0) { return new ActionOption(GameAction.Check); }
				return this[0];
			}
		}
	}
}
