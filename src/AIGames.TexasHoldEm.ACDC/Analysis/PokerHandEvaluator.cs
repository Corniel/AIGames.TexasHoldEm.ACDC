using System;
using Troschuetz.Random.Generators;

namespace AIGames.TexasHoldEm.ACDC.Analysis
{
	/// <summary>Evaluates poker hands.</summary>
	public static class PokerHandEvaluator
	{
		/// <summary>Calculates the winning change of a hand.</summary>
		public static double GetOdds(Cards hand, Cards table)
		{
			var mask = hand.GetMask();
			var board = table.GetMask();

			double[] player = new double[9];
			double[] opponent = new double[9];

			Hand.HandPlayerOpponentOdds(mask, board, ref player, ref opponent);

			var score = 0.0;

			for (var type = Hand.HandTypes.HighCard; type <= Hand.HandTypes.StraightFlush; type++)
			{
				var p = player[(int)type];
				score += p;
			}
			return score;
		}

		public static PlayerName GetOutcome(Cards player1, Cards player2, Cards table)
		{
			var p1 = String.Join(" ", player1);
			var p2 = String.Join(" ", player2);
			var tb = String.Join(" ", table);

			var h1 = new Hand(p1, tb);
			var h2 = new Hand(p2, tb);

			if (h1 == h2) { return PlayerName.None; }
			return h1 > h2 ? PlayerName.player1 : PlayerName.player2;
		}
	}
}
