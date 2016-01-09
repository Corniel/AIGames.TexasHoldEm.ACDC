using AIGames.TexasHoldEm.ACDC.Analysis;
using NUnit.Framework;

namespace AIGames.TexasHoldEm.ACDC.UnitTests.Analysis
{
	[TestFixture]
	public class RecordTest
	{
		[Test]
		public void RoundTripByteArray_()
		{
			var exp = new Record()
			{
				Odds = 0.83,
				Gap = 345,
				Action = GameAction.Raise(542),
				Profit = 549,
				Round = 4,
				SubRound = SubRoundType.River,
				Step = 2,
				Pot = 17,
				AmountToCall = 234,
			};

			var bytes = exp.ToByteArray();
			var act = Record.FromByteArray(bytes);

			Assert.AreEqual(exp.Odds, act.Odds, 0.2, "Odds");
			Assert.AreEqual(exp.Gap, act.Gap, "Gap");
			Assert.AreEqual(exp.Action, act.Action, "Action");
			Assert.AreEqual(exp.Profit, act.Profit, "Profit");
			Assert.AreEqual(exp.Round, act.Round, "Round");
			Assert.AreEqual(exp.SubRound, act.SubRound, "Round");
			Assert.AreEqual(exp.Step, act.Step, "Round");
			Assert.AreEqual(exp.Pot, act.Pot, "Pot");
			Assert.AreEqual(exp.AmountToCall, act.AmountToCall, "AmountToCall");
		}
	}
}
