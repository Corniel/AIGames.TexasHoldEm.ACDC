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
		}

		public MT19937Generator Rnd { get; set; }
		public Settings Settings { get; set; }
		public Matches Matches  {get; set; }
		public Match Current { get { return Matches.Current; } }

		public void ApplySettings(Settings settings)
		{
			Settings = settings;
		}

		public void Update(Matches matches)
		{
			Matches = matches;
		}

		public Cards Table { get { return Current.Table; } }
		public Cards Hand { get { return Own.Hand; } }

		public PlayerState Own { get { return Current[Settings.YourBot]; } }
		public PlayerState Other { get { return Current[Settings.YourBot.Other()]; } }

		public BotResponse GetResponse(TimeSpan time)
		{
			var state = new ActorState()
			{
				BigBlind = Matches.Current.BigBlind,
				Odds = PokerHandEvaluator.GetOdds(Hand, Table),
				OwnStack =Own.Stack,
				OwnPot = Own.Pot,
				OtherStack = Other.Stack,
				OtherPot = Other.Pot,
				Rnd = Rnd,
			};
			
			var actor = Actor.Get(Matches.Current);
			var action = actor.GetAction(state);

			var log = new StringBuilder();
			log.AppendFormat("{0:00}.{1,-5}", Current.Round, Current.SubRound)
				.Append(' ').Append(Hand);
			if (Current.SubRound != SubRoundType.Pre)
			{
				log.AppendFormat(", {0}", Table);
			}
			log.AppendFormat(", {0:0.0%}", state.Odds).Append(", ").Append(action);

			var response = new BotResponse()
			{
				Action = action,
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
