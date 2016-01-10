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
		public static readonly Actor Act = new Actor(Nodes.Get());

		[Test]
		public void PreFlop_NoCallFirstResponse_PreFlopOdds()
		{
			TestSelect(NodeStats.GetPreFlopOdds(), new ActorState()
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
			TestSelect(NodeStats.GetPreFlopOdds(), new ActorState()
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
			TestSelect(NodeStats.GetPreFlopOdds(), new ActorState()
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
			TestSelect(NodeStats.GetPreFlopOdds(), new ActorState()
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
			TestSelect(NodeStats.Odds, new ActorState()
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
			TestSelect(NodeStats.Odds, new ActorState()
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
			TestSelect(NodeStats.Odds, new ActorState()
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
			TestSelect(NodeStats.Odds, new ActorState()
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

		[Test]
		public void DataEvaluation()
		{
			var rounds = new int[1000];
			
			var pre = 0;
			var flop = 0;
			var turn = 0;
			var river = 0;

			var call = 0;
			var nocall = 0;

			var odds = NodeStats.Odds.ToDictionary(o => o, o => 0);

			foreach (var rec in Act.Nodes.Where(r => r.Round != 0 /*&& r.SubRound == SubRoundType.Pre*/))
			{
				odds[rec.Odds]++;
				rounds[rec.Round]++;
				switch (rec.SubRound)
				{
					case SubRoundType.Pre: pre++; break;
					case SubRoundType.Flop: flop++;break;
					case SubRoundType.Turn: turn++;break;
					case SubRoundType.River: river++;break;
				}
				if (rec.HasAmountToCall)
				{
					call++;
				}
				else
				{
					nocall++;
				}
			}

			Console.WriteLine("ToCall vs NotToCall: {0:#,##0} - {1:#,##0}", call, nocall);
			Console.WriteLine("Pre-flop: {0:#,##0}", pre);
			Console.WriteLine("Flop:     {0:#,##0}", flop);
			Console.WriteLine("Turn:     {0:#,##0}", turn);
			Console.WriteLine("River:    {0:#,##0}", river);
			Console.WriteLine();
			Console.WriteLine("Rounds");
			for (var round = 0; round < rounds.Length; round++)
			{
				if (rounds[round] != 0)
				{
					Console.WriteLine("Round {0,-3}: {1:#,##0}", round, rounds[round]);
				}
			}
			Console.WriteLine();
			Console.WriteLine("Odds");
			foreach(var kvp in odds)
			{
				Console.WriteLine("{0:00.0%} {1:#,##0}", kvp.Key, kvp.Value);
			}
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
