using AIGames.TexasHoldEm.ACDC.Analysis;
using System;
using System.Collections.Generic;

namespace AIGames.TexasHoldEm.ACDC.Actors
{
	public class Actor
	{
		public Actor(IList<Record> records)
		{
			Records = records;
			Buffer = new List<Record>();
		}

		public IList<Record> Records { get; private set; }

		private List<Record> Buffer { get; set; }

		public void ApplyProfit(int profit)
		{
			var win = profit > 0;
			foreach (var record in Buffer)
			{
				var costs = win ? record.Profit : -record.Profit;
				record.Profit = (short)(profit - costs);
				Records.Add(record);
			}
			Buffer.Clear();
		}

		public ActionOptions GetAction(ActorState state)
		{
			var options = new ActionOptions();

			var nOdds = 1 - state.Odds;

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

			Record test = state.ToRecord();
			options.Sort(test, Records);
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
