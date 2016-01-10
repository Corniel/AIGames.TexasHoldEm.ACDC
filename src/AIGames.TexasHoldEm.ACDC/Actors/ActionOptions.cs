using AIGames.TexasHoldEm.ACDC.Analysis;
using System.Collections.Generic;
using System.Linq;

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

		public IEnumerable<ActionOption> GetRaises()
		{
			return this.Where(node => node.ActionType == GameActionType.raise);
		}

		public void Sort(Node test, NodeCollection nodes)
		{
			if (Count == 1) { return; }

			UpdateCallOption(test, nodes);
			UpdateCheckOptions(test, nodes);
			Sort();
		}
		private void UpdateCallOption(Node test, NodeCollection nodes)
		{
			var option = this.FirstOrDefault(o => o.Action == GameAction.Call);
			if (option != null)
			{
				foreach (var node in nodes.GetCallNodes(test.SubRound))
				{
					Update(test, node, option);
				}
			}
		}

		private void UpdateCheckOptions(Node test, NodeCollection nodes)
		{
			var options = GetRaises().ToList();

			foreach (var node in nodes.GetRaiseNodes(test.SubRound, test.HasAmountToCall))
			{
				foreach (var option in options)
				{
					Update(test, node, option);
				}
			}
		}

		

		private static void Update(Node test, Node node, ActionOption option)
		{
			// To match amounts.
			test.Action = option.Action;
			var match = Matcher.Node(test, node);
			if (match > 0)
			{
				if (node.IsNew)
				{
					match *= 10;
				}
				option.Update(node.Profit, match);
			}
		}
	}
}
