using AIGames.TexasHoldEm.ACDC.Communication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGames.TexasHoldEm.ACDC
{
	[DebuggerDisplay("{DebuggerDisplay}")]
	public class Match
	{
		public Match(int round)
		{
			Round = round;
			Player1 = new PlayerState(PlayerName.player1);
			Player2 = new PlayerState(PlayerName.player2);
			Table = Cards.Empty;
		}

		public PlayerState this[PlayerName name]
		{
			get
			{
				switch (name)
				{
					case PlayerName.player1: return Player1;
					case PlayerName.player2: return Player2;
					default: return null;
				}
			}
		}

		public int Round { get; private set; }

		public SubRoundType SubRound { get { return (SubRoundType)Table.Count; } }

		public int SmallBlind { get; set; }
		public int BigBlind { get; set; }
		
		public PlayerName OnButton { get; set; }
		public Cards Table
		{
			get { return m_Table; }
			set
			{
				m_Table = value;
				Odds = null;
			}
		}
		private Cards m_Table;

		public PlayerState Player1 { get; set; }
		public PlayerState Player2 { get; set; }

		public double? Odds { get; set; }

		public void SetHand(HandInstruction instruction)
		{
			this[instruction.Name].Hand = instruction.Value;
		}

		public void Stack(StackInstruction instruction)
		{
			this[instruction.Name].Stack = instruction.Value;
		}
		public void Post(PostInstruction instruction)
		{
			this[instruction.Name].Pot = instruction.Value;
			this[instruction.Name].Stack -= instruction.Value;
		}

		public void Act(ActionInstruction instruction)
		{
			var action = instruction.Value;

			if (action.ActionType == GameActionType.call || action.ActionType == GameActionType.raise)
			{
				var amountToCall = this[instruction.Name].GetAmountToCall(this[instruction.Name.Other()]);
				if (amountToCall > 0)
				{
					this[instruction.Name].Call(amountToCall);
				}
			}
			this[instruction.Name].Act(action);
		}

		private string DebuggerDisplay { get { return String.Format("{0} Table: {1:f}", Round, Table); } }
	}
}
