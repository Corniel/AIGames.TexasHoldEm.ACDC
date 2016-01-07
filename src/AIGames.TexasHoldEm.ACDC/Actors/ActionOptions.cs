using System.Collections.Generic;

namespace AIGames.TexasHoldEm.ACDC.Actors
{
	public class ActionOptions: List<ActionOption>
	{
		public void Add(GameAction action, double profit)
		{
			Add(new ActionOption(action, profit));
		}
	}
}
