using AIGames.TexasHoldEm.ACDC.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using Troschuetz.Random.Generators;

namespace AIGames.TexasHoldEm.ACDC.Actors
{
	public class Actor
	{
		public Actor(IList<Node> nodes) 
			: this(nodes, new MT19937Generator(17)) { }

		public Actor(IList<Node> nodes, MT19937Generator rnd)
		{
			Nodes = new NodeCollection(nodes);
			Buffer = new List<Node>();
			Rnd = rnd;
		}

		public NodeCollection Nodes { get; private set; }
		public MT19937Generator Rnd { get; private set; }
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

		public ActionOption GetOption(ActorState state)
		{
			var options = new ActionOptions();

			if (state.NoAmountToCall)
			{
				options.Add(new ActionOption(GameAction.Check, state.Odds * state.Pot, 1));
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

			var best = SelectOption(options);

			if (best.Action != GameAction.Fold)
			{
				if (best.Action != GameAction.Check)
				{
					test.Profit = (short)state.OwnPot;
					test.Action = best.Action;
					test.IsNew = true;
					Buffer.Add(test);
				}
			}
			else if(options.Count > 1)
			{
				best = new ActionOption(GameAction.Fold, options[1].Profit, options[1].Weight);
			}
			return best;
		}

		/// <summary>Select the option to play.</summary>
		/// <remarks>
		/// Returns a random profitable if any, otherwise the best case.
		/// </remarks>
		private ActionOption SelectOption(ActionOptions options)
		{
			var best = options[0];

			var profitable = options
				.Where(option => option.IsProfitable)
				.ToArray();

			if (profitable.Length > 1)
			{
				var weights = new double[profitable.Length];
				var sum = 0.0;
				for (var index = 0; index < weights.Length; index++)
				{
					weights[index] = sum;
					sum += profitable[index].WeightedProfit;
				}

				var pick = Rnd.NextDouble(sum);

				for (var index = 0; index < weights.Length; index++)
				{
					if (pick < weights[index]) { break; }
					best = profitable[index];
				}
			}
			return best;
		}
	}
}
