using AIGames.TexasHoldEm.ACDC.Communication;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

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
		public Cards Table { get; set; }

		public PlayerState Player1 { get; set; }
		public PlayerState Player2 { get; set; }

		public PlayerState On { get { return this[OnButton]; } }
		public PlayerState Off { get { return this[OnButton.Other()]; } }

		public int Pot { get { return Player1.Pot + Player2.Pot; } }

        public GameAction Action { get; set; }

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

		[DebuggerBrowsable(DebuggerBrowsableState.Never), ExcludeFromCodeCoverage]
		internal string DebuggerDisplay
		{
			get
			{
				var sb = new StringBuilder();
				sb.AppendFormat("{0}.{1,-5} ({2})", Round, SubRound, 1+ Player1.Actions.Count + Player2.Actions.Count);
				if (Table.Any())
				{
					sb.AppendFormat(" {0:f}", Table);
				}
				sb.AppendFormat(" {0:0.0%}-{1:0.0%}", Player1.Odds, Player2.Odds);
				sb.AppendFormat(" {0:0}-{1:0}", Player1.Stack, Player2.Stack);
				sb.AppendFormat(" ({0:0}-{1:0})", Player1.Pot, Player2.Pot);
				sb.AppendFormat(" {0:f}-{1:f})", Player1.Hand, Player2.Hand);

				return sb.ToString();
			}
		}
	}
}
