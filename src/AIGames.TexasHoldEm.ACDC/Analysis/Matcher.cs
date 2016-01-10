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

			// No zero match
			match *= Gap(l.Gap, r.Gap);
			match *= Pot(l.Pot, r.Pot);
			match *= Amount(l.Action.Amount, r.Action.Amount);
			match *= Step(l.Step, r.Step);
			match *= Round(l.Round, r.Round);
			return match;
		}

		public static double SubRound(SubRoundType l, SubRoundType r)
		{
			if (l == r) { return 1; }
			if (l == SubRoundType.Pre ^ r == SubRoundType.Pre) { return 0; }
			// if they differ but are not Pre(Flop).
			return 0.3;
		}
		public static double Round(int l, int r)
		{
			if (l == r) { return 1; }
			var bigL = (l / 10) + 2.0;
			var bigR = (r / 10) + 2.0;

			return 1.0 / (1.0 + Math.Abs(bigL - bigR));
		}
		public static double Step(int l, int r)
		{
			var half = 4.0;
			return half / (half + Math.Abs(r - l));
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

			var half = 100.0;
			return half / (half + Math.Abs(r - l));
		}
		public static double Gap(int l, int r)
		{
			var half = 500.0;
			return half / (half + Math.Abs(r - l));
		}
		public static double Pot(int l, int r)
		{
			var half = 200.0;
			return half / (half + Math.Abs(r - l));
		}

		private static double Amount(int l, int r)
		{
			var half = 100.0;
			return half / (half + Math.Abs(r - l));
		}
	}
}
