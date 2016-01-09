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
			foreach (var odd in GetPreFlopOdds())
			{
				var state = new ActorState()
				{
					BigBlind = 20,
					Odds = odd,
					OwnPot = 20,
					OtherPot = 20,
					OwnStack = 1980,
					OtherStack = 1980,
					Round = 1,
					Step = 1,
					SubRound = SubRoundType.Pre,
				};

				var option = Act.GetAction(state);
				Console.WriteLine("{0:00.0%} {1}",odd, option);
			}
		}
		[Test]
		public void PreFlop_Call10_PreFlopOdds()
		{
			foreach (var odd in GetPreFlopOdds())
			{
				var state = new ActorState()
				{
					BigBlind = 20,
					Odds = odd,
					OwnPot = 10,
					OtherPot = 20,
					OwnStack = 1980,
					OtherStack = 1980,
					Round = 1,
					Step = 1,
					SubRound = SubRoundType.Pre,
				};

				var option = Act.GetAction(state);
				Console.WriteLine("{0:00.0%} {1}", odd, option);
			}
		}
		[Test]
		public void PreFlop_Call20Round11_PreFlopOdds()
		{
			foreach (var odd in GetPreFlopOdds())
			{
				var state = new ActorState()
				{
					BigBlind = 40,
					Odds = odd,
					OwnPot = 20,
					OtherPot = 40,
					OwnStack = 1960,
					OtherStack = 1960,
					Round = 11,
					Step = 1,
					SubRound = SubRoundType.Pre,
				};

				var option = Act.GetAction(state);
				Console.WriteLine("{0:00.0%} {1}", odd, option);
			}
		}

		[Test]
		public void PreFlop_NoCallPot1000Response_PreFlopOdds()
		{
			var sw = Stopwatch.StartNew();
			foreach (var odd in GetPreFlopOdds())
			{
				var state = new ActorState()
				{
					BigBlind = 20,
					Odds = odd,
					OwnPot = 500,
					OtherPot = 500,
					OwnStack = 1500,
					OtherStack = 1500,
					Round = 1,
					Step = 4,
					SubRound = SubRoundType.Pre,
				};

				var option = Act.GetAction(state);
				Console.WriteLine("{0:00.0%} {1}", odd, option);
			}
			Console.WriteLine(sw.Elapsed);
		}
	}
}
