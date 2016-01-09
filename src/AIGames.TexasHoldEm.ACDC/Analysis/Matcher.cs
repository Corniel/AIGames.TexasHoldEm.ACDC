using System;

namespace AIGames.TexasHoldEm.ACDC.Analysis
{
	public static class Matcher
	{
		public static double Record(Record l, Record r)
		{
			var match = SubRound(l.SubRound, r.SubRound);
			if (match == 0) { return 0; }

			match *= AmountToCall(l.AmountToCall, r.AmountToCall);
			if (match == 0) { return 0; }
			
			match *= Odds(l.Odds, r.Odds);
			if (match == 0) { return 0; }
	
			match *= Gap(l.Gap, r.Gap);
			if (match == 0) { return 0; }
			
			match *= Pot(l.Pot, r.Pot);
			if (match == 0) { return 0; }

			match *= Amount(l.Action.Amount, r.Action.Amount);
			if (match == 0) { return 0; }

			// No zero match
			match *= Step(l.Step, r.Step);
			match *= Round(l.Round, r.Round);
			return match;
		}

		public static double SubRound(SubRoundType l, SubRoundType r)
		{
			if (l == r) { return 1; }
			if (l == SubRoundType.Pre ^ r == SubRoundType.Pre) { return 0; }
			// if they differ but are not Pre(Flop).
			return 0.7;
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
		public static double AmountToCall(int l, int r)
		{
			if (l == 0)
			{
				return r == 0 ? 1 : 0;
			}
			if (r == 0) { return 0; }

			var delta = 1.0 - (l - r) * (l - r) / (5000.0);
			return Math.Max(0, delta);
		}
		public static double Gap(int l, int r)
		{
			var delta = 1.0 - (l - r) * (l - r) / (10000.0);
			return Math.Max(0, delta);
		}
		public static double Pot(int l, int r)
		{
			var delta = 1.0 - (l - r) * (l - r) / (5000.0);
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
