using AIGames.TexasHoldEm.ACDC.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGames.TexasHoldEm.ACDC.UnitTests.Analysis
{
	public class NodeStats
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
	}
}
