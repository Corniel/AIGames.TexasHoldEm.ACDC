using AIGames.TexasHoldEm.ACDC.Actors;
using System.Collections.Generic;
using System.Linq;

namespace AIGames.TexasHoldEm.ACDC.Analysis
{
	public class Records : List<Record>
	{
		public ActionOption Select(Record test, ActionOptions options)
		{
			if (options.Count == 1) { return options.FirstOrDefault(); }

			foreach (var item in this)
			{
				foreach (var option in options)
				{
					var type = option.ActionType;
					if (type == item.Action.ActionType)
					{
						// To match amounts.
						test.Action = option.Action;
						var match = Matcher.Record(test, item);
						if (match > 0)
						{
							option.Update(test.Profit, match);
						}
					}
				}
			}
			options.Sort();
			var best = options.FirstOrDefault();
			test.Action = best.Action;
			return best;
		}
	}
}
