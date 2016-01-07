using System;
namespace AIGames.TexasHoldEm.ACDC.Actors
{
	public class Actor
	{
		public virtual GameAction GetAction(ActorState state)
		{
			if (state.Odds >= 0.5)
			{
				// raise
				if (state.Odds > state.Rnd.NextDouble())
				{
					var raise = state.Rnd.Next(state.BigBlind, state.MaximumRaise);
					return GameAction.Raise(raise);
				}
				else
				{
					return GameAction.CheckOrCall(state.NoAmountToCall);
				}
			}
			if (state.NoAmountToCall)
			{
				return GameAction.Check;
			}
			if (state.AmountToCall < state.BigBlind * 2 && state.Odds > state.Rnd.NextDouble())
			{
				return GameAction.Call;
			}
			return GameAction.Fold;
		}

		public static Actor Get(Match match)
		{
			switch (match.SubRound)
			{
				case SubRoundType.Pre: return new PreFlopActor();
				case SubRoundType.Flop:
				case SubRoundType.Turn:
				case SubRoundType.River:
				default: return new Actor();
			}
		}
	}
}
