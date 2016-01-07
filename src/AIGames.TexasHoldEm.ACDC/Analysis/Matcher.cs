using System;

namespace AIGames.TexasHoldEm.ACDC.Analysis
{
	public static class Matcher
	{
		public static double Record(Record l, Record r)
		{
			var match = SubRound(l.SubRound, r.SubRound);
			match *= Round(l.Round, r.Round);
			match *= Step(l.Step, r.Step);
			match *= Odds(l.Odds, r.Odds);
			match *= Gap(l.Gap, r.Gap);
			match *= Amount(l.Action.Amount, r.Action.Amount);
			return match;
		}

		public static double SubRound(SubRoundType l, SubRoundType r)
		{
			if (l == r) { return 1; }
			if (l == SubRoundType.Pre ^ r == SubRoundType.Pre) { return 0; }
			// if they differ but are not Pre(Flop).
			return 0.8;
		}
		public static double Round(int l, int r)
		{
			if (l == r) { return 1; }
			var bigL = (l / 10) + 2.0;
			var bigR = (r / 10) + 2.0;

			var match = bigL / bigR;
			return match > 1 ? 1 / match : match;
		}
		public static double Step(int l, int r)
		{
			if (l == r) { return 1; }
			var match = (double)(l - r) / (l + r);
			return 1 - Math.Abs(match);
		}
		public static double Odds(double l, double r)
		{
			var delta = 1 - (l - r) * (l - r) * 75;
			return Math.Max(0, delta);
		}
		public static double Gap(int l, int r)
		{
			var delta = 1.0 - (l - r) * (l - r) / (10000.0);
			return Math.Max(0, delta);
		}

		private static double Amount(int l, int r)
		{
			if (l == r) { return 1; }
			var match = 2.0 * (l - r) / (l + r);
			return 1 - Math.Abs(match);
		}
	}
}
