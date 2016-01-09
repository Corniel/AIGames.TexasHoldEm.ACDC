using AIGames.TexasHoldEm.ACDC.Actors;
using AIGames.TexasHoldEm.ACDC.Analysis;
using NUnit.Framework;
using NUnit.Framework.Compatibility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace AIGames.TexasHoldEm.ACDC.UnitTests.Analysis
{
	[TestFixture, Category(Category.Evaluation)]
	public class SelectTest
	{
		public static readonly double[] Odds = GetOdds();
		public static double[] GetOdds()
		{
			var odds = new List<double>();
			for (int b = byte.MaxValue; b >= byte.MinValue; b--)
			{
				odds.Add(Record.ToOdds((byte)b));
			}
			return odds.ToArray();
		}

		/// <summary>Gets the range of possible pre-flop odds (AA: 85.3%, 32o: 31.2%).</summary>
		public static IEnumerable<double> GetPreFlopOdds()
		{
			return GetOdds().Where(odd => odd >= 0.305 && odd <= 0.86);
		}

		public static readonly Actor Act = new Actor(Records.Get());

		[Test]
		public void PreFlop_NoCallFirstResponse_PreFlopOdds()
		{
			TestSelect(GetPreFlopOdds(), new ActorState()
			{
				BigBlind = 20,
				OwnPot = 20,
				OtherPot = 20,
				OwnStack = 1980,
				OtherStack = 1980,
				Round = 1,
				Step = 1,
				SubRound = SubRoundType.Pre,
			});
		}

		[Test]
		public void PreFlop_Call10_PreFlopOdds()
		{
			TestSelect(GetPreFlopOdds(), new ActorState()
			{
				BigBlind = 20,
				OwnPot = 10,
				OtherPot = 20,
				OwnStack = 1980,
				OtherStack = 1980,
				Round = 1,
				Step = 1,
				SubRound = SubRoundType.Pre,
			});
		}
		[Test]
		public void PreFlop_Call20Round11_PreFlopOdds()
		{
			TestSelect(GetPreFlopOdds(), new ActorState()
			{
				BigBlind = 40,
				OwnPot = 20,
				OtherPot = 40,
				OwnStack = 1960,
				OtherStack = 1960,
				Round = 11,
				Step = 1,
				SubRound = SubRoundType.Pre,
			});
		}

		[Test]
		public void PreFlop_NoCallPot1000Response_PreFlopOdds()
		{
			TestSelect(GetPreFlopOdds(), new ActorState()
			{
				BigBlind = 20,
				OwnPot = 500,
				OtherPot = 500,
				OwnStack = 1500,
				OtherStack = 1500,
				Round = 1,
				Step = 4,
				SubRound = SubRoundType.Pre,
			});
		}

		[Test]
		public void River_NoCallPot1200Response_AllOdds()
		{
			TestSelect(Odds, new ActorState()
			{
				BigBlind = 80,
				OwnPot = 600,
				OtherPot = 600,
				OwnStack = 1300,
				OtherStack = 1500,
				Round = 83,
				Step = 4,
				SubRound = SubRoundType.River,
			});
		}

		[Test]
		public void Flop_NoCallPot160Response_AllOdds()
		{
			TestSelect(Odds, new ActorState()
			{
				BigBlind = 80,
				OwnPot = 80,
				OtherPot = 80,
				OwnStack = 1320,
				OtherStack = 2520,
				Round = 83,
				Step = 1,
				SubRound = SubRoundType.Flop,
			});
		}

		[Test]
		public void Turn_NoCallPot160Response_AllOdds()
		{
			TestSelect(Odds, new ActorState()
			{
				BigBlind = 80,
				OwnPot = 80,
				OtherPot = 80,
				OwnStack = 1320,
				OtherStack = 2520,
				Round = 83,
				Step = 1,
				SubRound = SubRoundType.Turn,
			});
		}

		[Test]
		public void River_NoCallPot160Response_AllOdds()
		{
			TestSelect(Odds, new ActorState()
			{
				BigBlind = 80,
				OwnPot = 80,
				OtherPot = 80,
				OwnStack = 1320,
				OtherStack = 2520,
				Round = 83,
				Step = 1,
				SubRound = SubRoundType.River,
			});
		}

		

		private static void TestSelect(IEnumerable<double> odds, ActorState state)
		{
			var sw = Stopwatch.StartNew();

			foreach (var odd in odds)
			{
				var st = new ActorState()
				{
					BigBlind = state.BigBlind,
					Odds = odd,
					OwnPot = state.OwnPot,
					OtherPot = state.OtherPot,
					OwnStack = state.OwnStack,
					OtherStack = state.OtherStack,
					Round = state.Round,
					Step = state.Step,
					SubRound = state.SubRound,
				};

				var options = Act.GetAction(st);
				Console.WriteLine("{0:00.0%} {1}", odd, options.Best);
			}
			Console.WriteLine(sw.Elapsed);
		}
	}
}
