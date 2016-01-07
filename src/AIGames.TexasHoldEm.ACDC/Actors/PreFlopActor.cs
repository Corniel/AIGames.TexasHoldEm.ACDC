namespace AIGames.TexasHoldEm.ACDC.Actors
{
	public class PreFlopActor: Actor
	{
		public override GameAction GetAction(ActorState state)
		{
			if (state.AmountToCall == 0) { return GameAction.Check; }
			if (state.Odds >= 0.5) { return GameAction.Call; }
			return GameAction.Fold;
		}
	}
}
