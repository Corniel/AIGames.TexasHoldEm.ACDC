using System;
namespace AIGames.TexasHoldEm.ACDC.Actors
{
	public class Actor
	{
		public virtual GameAction GetAction(ActorState state)
		{
			if (state.AmountToCall > 0 && state.Odds >= 0.5) { return GameAction.Call; }
			if (state.Odds >= 0.5)
			{
				if (state.OwnStack >= state.BigBlind && state.Odds > state.Rnd.NextDouble())
				{
					var max = Math.Min(state.OwnStack, state.BigBlind);
					var raise = state.Rnd.Next(state.BigBlind, max);
					return GameAction.Raise(raise);
				}
				else
				{
					return GameAction.Check;
				}
			}
			return GameAction.Fold;
		}

		public static Actor Get(Match match)
		{
			switch (match.SubRound)
			{
				case SubRoundType.PreFlop: return new PreFlopActor();
				case SubRoundType.Flop:
				case SubRoundType.Turn:
				case SubRoundType.River:
				default: return new Actor();
			}
		}
	}
}
