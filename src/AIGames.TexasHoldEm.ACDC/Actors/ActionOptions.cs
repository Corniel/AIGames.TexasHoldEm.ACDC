using AIGames.TexasHoldEm.ACDC.Analysis;
using System.Collections.Generic;

namespace AIGames.TexasHoldEm.ACDC.Actors
{
	public class ActionOptions: List<ActionOption>
	{
		public void Add(GameAction action)
		{
			if (action == GameAction.Fold)
			{
				Add(new ActionOption(action, 0, 1));
			}
			else
			{
				Add(new ActionOption(action));
			}
		}

		public void Sort(Node test, IEnumerable<Node> items)
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
						var match = Matcher.Node(test, item);
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

		internal object Where(System.Func<ActionOption, int, bool> func)
		{
			throw new System.NotImplementedException();
		}
	}
}
