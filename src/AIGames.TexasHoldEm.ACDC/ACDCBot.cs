using AIGames.TexasHoldEm.ACDC.Analysis;
using AIGames.TexasHoldEm.ACDC.Communication;
using McCulloch.Networks;
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
		public ACDCBot() : this(null) { }
        public ACDCBot(NeuralNetwork<ActionOption> network) : this(network, new MT19937Generator()) { }

		public ACDCBot(NeuralNetwork<ActionOption> network, MT19937Generator rnd)
		{
            Network = network;
            Rnd = rnd;
		}

        private MT19937Generator Rnd { get; }
        private NeuralNetwork<ActionOption> Network { get;}

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
            // Nothing do do.
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

			var state = new ActionState()
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

            var options = new ActionOptions(Network, Rnd);
            var option = options.Select(state);
            Current.Action = option.Action;

			var response = new BotResponse()
			{
				Action = option.Action,
				Log = GetLog(option),
			};
			return response;
		}

		private string GetLog(ActionOption action)
		{
			var log = new StringBuilder();
			
			log.AppendFormat("{0:00}.{1,-5}", Current.Round, Current.SubRound)
				.Append(' ').Append(Hand);
			
			if (Current.SubRound != SubRoundType.Pre)
			{
				log.AppendFormat(", {0}", Table);
			}

            log.AppendFormat(", {0:0.0%}", action.Odds)
                .Append(", ")
                .Append(action.Action)
                .Append(", ")
                .Append(action.Result[true].ToString("0.00%"));


            return log.ToString();
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never), ExcludeFromCodeCoverage]
		private string DebuggerDisplay { get { return "Bot: " + Settings.YourBot.ToString(); } }
    }
}
