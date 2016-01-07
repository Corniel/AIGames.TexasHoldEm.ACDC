using AIGames.TexasHoldEm.ACDC.Analysis;

namespace AIGames.TexasHoldEm.ACDC.Actors
{
	public class Actor
	{
		public Actor(Records records)
		{
			this.Records = records;
		}

		public Records Records { get; private set; }

		public ActionOption GetAction(ActorState state)
		{
			var options = new ActionOptions();

			var nOdds = 1 - state.Odds;

			if (state.NoAmountToCall)
			{
				options.Add(GameAction.Check, state.Pot * state.Odds);
			}
			else
			{
				options.Add(GameAction.Fold, 0);
				options.Add(GameAction.Call, state.Pot * state.Odds - state.AmountToCall * nOdds);
			}
			if (state.AmountToCall != state.SmallBlind)
			{
				var step = 1 + (state.MaximumRaise - state.BigBlind) / 13;

				for (var raise = state.BigBlind; raise <= state.MaximumRaise; raise += step)
				{
					options.Add(GameAction.Raise(raise), state.Pot * state.Odds - (state.AmountToCall - raise) * nOdds);
				}
			}

			Record test = state.ToRecord();
			var best = Records.Select(test, options);
			test.Profit = (short)(-state.AmountToCall - best.Action.Amount);
			Records.Add(test);
			return best;
		}
	}
}
