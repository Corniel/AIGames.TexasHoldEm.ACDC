using AIGames.TexasHoldEm.ACDC.Actors;
using AIGames.TexasHoldEm.ACDC.Analysis;
using AIGames.TexasHoldEm.ACDC.Communication;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Troschuetz.Random.Generators;

namespace AIGames.TexasHoldEm.ACDC
{
	[DebuggerDisplay("{DebuggerDisplay}")]
	public class ACDCBot : IBot
	{
		public ACDCBot()
		{
			Rnd = new MT19937Generator(17);
			Actor = new Actor(new Records());
		}

		public Actor Actor { get; set; }
		public MT19937Generator Rnd { get; set; }
		public Settings Settings { get; set; }
		public Matches Matches { get; set; }
		public Match Current { get { return Matches.Current; } }

		public void ApplySettings(Settings settings)
		{
			Settings = settings;
		}

		public void Update(Matches matches)
		{
			Matches = matches;
		}

		public void Update(WinsInstruction instruction)
		{
			var profit = (short)instruction.Value;
			if (Settings.OppoBot == instruction.Name) { profit = (short)-profit; }

			for (var index = Actor.Records.Count - 1; index >= 0; index--)
			{
				var record = Actor.Records[index];
				if (record.Round != Current.Round) { break; }
				record.Profit += profit;
			}
		}

		public Cards Table { get { return Current.Table; } }
		public Cards Hand { get { return Own.Hand; } }

		public PlayerState Own { get { return Current[Settings.YourBot]; } }
		public PlayerState Other { get { return Current[Settings.YourBot.Other()]; } }

		public BotResponse GetResponse(TimeSpan time)
		{
			if (!Current.Odds.HasValue)
			{
				Current.Odds = PokerHandEvaluator.GetOdds(Hand, Table);
			}

			var state = new ActorState()
			{
				Round = Current.Round,
				SubRound = Current.SubRound,
				Step = 1 + Own.Actions.Count,
				BigBlind = Matches.Current.BigBlind,
				Odds = Current.Odds.Value,
				OwnStack = Own.Stack,
				OwnPot = Own.Pot,
				OtherStack = Other.Stack,
				OtherPot = Other.Pot,
				Rnd = Rnd,
			};

			var best = Actor.GetAction(state);

			var log = new StringBuilder();
			log.AppendFormat("{0:00}.{1,-5}", Current.Round, Current.SubRound)
				.Append(' ').Append(Hand);
			if (Current.SubRound != SubRoundType.Pre)
			{
				log.AppendFormat(", {0}", Table);
			}
			log.AppendFormat(", {0:0.0%}", state.Odds)
				.Append(", ")
				.Append(best.Action)
				.Append(" ")
				.Append(best.Profit > 0 ? "+" : "")
				.Append(best.Profit.ToString("#,##0.0"));

			var response = new BotResponse()
			{
				Action = best.Action,
				Log = log.ToString(),
			};
			return response;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never), ExcludeFromCodeCoverage]
		private string DebuggerDisplay
		{
			get
			{
				return string.Format("Round {0}", Matches.Round);
			}
		}
	}
}
