using AIGames.TexasHoldEm.ACDC.Analysis;
using System;
using System.Collections.Generic;

namespace AIGames.TexasHoldEm.ACDC.Actors
{
	public class Actor
	{
		public Actor(IList<Node> nodes)
		{
			Nodes = nodes;
			Buffer = new List<Node>();
		}

		public IList<Node> Nodes { get; private set; }

		private List<Node> Buffer { get; set; }

		public void ApplyProfit(int profit)
		{
			var win = profit > 0;
			foreach (var node in Buffer)
			{
				var costs = win ? node.Profit : -node.Profit;
				node.Profit = (short)(profit - costs);
				Nodes.Add(node);
			}
			Buffer.Clear();
		}

		public ActionOptions GetAction(ActorState state)
		{
			var options = new ActionOptions();

			if (state.NoAmountToCall)
			{
				options.Add(GameAction.Check);
			}
			else
			{
				options.Add(GameAction.Call);
			}
			if (state.AmountToCall != state.SmallBlind)
			{
				var step = 1 + ((state.MaximumRaise - state.BigBlind) >> 4);

				for (var raise = state.BigBlind; raise <= state.MaximumRaise; raise += step)
				{
					options.Add(GameAction.Raise(raise));
				}
			}
			// Only add a fold if we have to call, and there is a change that we 
			// can play a next round.
			if (!state.NoAmountToCall && state.OtherPot >= state.BigBlind)
			{
				options.Add(GameAction.Fold);
			}

			Node test = state.ToNode();
			options.Sort(test, Nodes);
			var best = options[0];
			if (test.Action != GameAction.Fold)
			{
				test.Profit = (short)state.OwnPot;
				test.IsNew = true;
				Buffer.Add(test);
			}
			return options;
		}
	}
}
