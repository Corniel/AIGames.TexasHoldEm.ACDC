using AIGames.TexasHoldEm.ACDC.Actors;
using AIGames.TexasHoldEm.ACDC.Analysis;
using AIGames.TexasHoldEm.ACDC.Communication;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Troschuetz.Random.Generators;

namespace AIGames.TexasHoldEm.ACDC
{
	[DebuggerDisplay("{DebuggerDisplay}")]
	public class ACDCBot : IBot
	{
		public ACDCBot() : this(Nodes.Get()) { }
		public ACDCBot(IList<Node> nodes)
		{
			Actor = new Actor(nodes);
		}

		public Actor Actor { get; set; }
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
			var profit = instruction.Value;
			if (Settings.YourBot != instruction.Name) { profit = -profit; }
			Actor.ApplyProfit(profit);
		}

		public Cards Table { get { return Current.Table; } }
		public Cards Hand { get { return Own.Hand; } }
		private int TableCount;

		public PlayerState Own { get { return Current[Settings.YourBot]; } }
		public PlayerState Other { get { return Current[Settings.YourBot.Other()]; } }

		public BotResponse GetResponse(TimeSpan time)
		{
			if (TableCount != Table.Count)
			{
				Own.Odds = null;
				TableCount = Table.Count;
			}
			if (!Own.Odds.HasValue)
			{
				Own.Odds = PokerHandEvaluator.GetOdds(Hand, Table);
			}

			var state = new ActorState()
			{
				Round = Current.Round,
				SubRound = Current.SubRound,
				Step = 1 + Own.Actions.Count,
				BigBlind = Matches.Current.BigBlind,
				Odds = Own.Odds.Value,
				OwnStack = Own.Stack,
				OwnPot = Own.Pot,
				OtherStack = Other.Stack,
				OtherPot = Other.Pot,
			};

			var option = Actor.GetOption(state);

			var response = new BotResponse()
			{
				Action = option.Action,
				Log = GetLog(state, option),
			};
			return response;
		}

		private string GetLog(ActorState state, ActionOption best)
		{
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

			return log.ToString();
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never), ExcludeFromCodeCoverage]
		private string DebuggerDisplay { get { return "Bot: " + Settings.YourBot.ToString(); } }
	}
}
