using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AIGames.TexasHoldEm.ACDC
{
	[DebuggerDisplay("{DebuggerDisplay}")]
	public class PlayerState
	{
		public PlayerState(PlayerName name)
		{
			Name = name;
			Actions = new List<GameAction>();
			Hand = Cards.Empty;
		}
		public PlayerName Name { get; private set; }

		public double? Odds { get; set; }

		public int Stack { get; set; }
		public int Pot { get; set; }

		public Cards Hand { get; set; }

		public List<GameAction> Actions { get; private set; }

		public int GetAmountToCall(PlayerState other)
		{
			if (other.Pot > Pot)
			{
				return other.Pot - Pot;
			}
			return 0;
		}

		public void Act(GameAction action)
		{
			Actions.Add(action);

			switch (action.ActionType)
			{
				case GameActionType.call: break;
				case GameActionType.check: break;

				case GameActionType.fold:
					Pot = 0;
					break;

				case GameActionType.raise:
					Stack -= action.Amount;
					Pot += action.Amount;
					break;
			}
		}

		public void Call(int amountToCall)
		{
			Stack -= amountToCall;
			Pot += amountToCall;
		}

		private string DebuggerDisplay
		{
			get
			{
				return String.Format("{0} {1:f} {2:0.0%} {3} ({4}), Actions: {5}", 
					Name, 
					Hand,
					Odds,
					Stack, 
					Pot,
					Actions.Count);
			}
		}
	}
}
