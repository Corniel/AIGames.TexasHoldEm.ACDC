using AIGames.TexasHoldEm.ACDC.Analysis;
using NUnit.Framework;

namespace AIGames.TexasHoldEm.ACDC.UnitTests.Analysis
{
	[TestFixture]
	public class MatcherTest
	{
		[Test]
		public void SubRound_PreFlopVsNonPreFlop_0()
		{
			Assert.AreEqual(0.0, Matcher.SubRound(SubRoundType.Pre, SubRoundType.River), "Pre flop left.");
			Assert.AreEqual(0.0, Matcher.SubRound(SubRoundType.Turn, SubRoundType.Pre), "Pre flop right");
		}
		[Test]
		public void Match_NonePreFlopVsNonPreFlop_0dot3()
		{
			Assert.AreEqual(0.3, Matcher.SubRound(SubRoundType.Turn, SubRoundType.River), 0.001, "Turn vs River.");
			Assert.AreEqual(0.3, Matcher.SubRound(SubRoundType.Turn, SubRoundType.Flop),0.001, "Turn vs Flop.");
		}

		[Test]
		public void Round_8vs12_0dot5()
		{
			var act = Matcher.Round(8, 12);
			var exp = 0.5;
			Assert.AreEqual(exp, act, 0.01);
		}

		[Test]
		public void Round_28vs2_0dot333()
		{
			var act = Matcher.Round(28, 2);
			var exp = 0.333;
			Assert.AreEqual(exp, act, 0.01);
		}

		[Test]
		public void Step_1vs3_0dot333()
		{
			var act = Matcher.Step(1, 3);
			var exp = 0.333;
			Assert.AreEqual(exp, act, 0.01);
		}

		[Test]
		public void Step_3vs2_0dot5()
		{
			var act = Matcher.Step(3, 2);
			var exp = 0.5;
			Assert.AreEqual(exp, act, 0.01);
		}

		[Test]
		public void Odds_80vs77_0dot932()
		{
			var act = Matcher.Odds(0.8, 0.77);
			var exp = 0.932;
			Assert.AreEqual(exp, act, 0.01);
		}

		[Test]
		public void Odds_80vs70_0dot25()
		{
			var act = Matcher.Odds(0.8, 0.70);
			var exp = 0.25;
			Assert.AreEqual(exp, act, 0.01);
		}

		[Test]
		public void AmountToCall_0vs0_1()
		{
			var act = Matcher.AmountToCall(0, 0);
			var exp = 1;
			Assert.AreEqual(exp, act, 0.01);
		}

		[Test]
		public void AmountToCall_1vs0_0()
		{
			var act = Matcher.AmountToCall(1, 0);
			var exp = 0;
			Assert.AreEqual(exp, act, 0.01);
		}
		[Test]
		public void AmountToCall_0vs1_0()
		{
			var act = Matcher.AmountToCall(0, 1);
			var exp = 0;
			Assert.AreEqual(exp, act, 0.01);
		}
		[Test]
		public void AmountToCall_50vs30_0Dot833()
		{
			var act = Matcher.AmountToCall(50, 30);
			var exp = 0.833;
			Assert.AreEqual(exp, act, 0.01);
		}
	}
}
