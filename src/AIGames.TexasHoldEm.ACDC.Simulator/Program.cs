using AIGames.TexasHoldEm.ACDC.Analysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Troschuetz.Random.Generators;

namespace AIGames.TexasHoldEm.ACDC.Simulator
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var file = new FileInfo("data.bin");

			var simulator = new Simulator();
			var rnd = new MT19937Generator();

			var records = new List<Record>();
			if(file.Exists)
			{ 
				records.AddRange(Records.Load(file));
			}

			records.Sort();

			var sw = Stopwatch.StartNew();
			long runs = 0;
			var shrinks = 0;
			while (true)
			{
				if (records.Count > 50000)
				{
					simulator.Shrink(records);
					shrinks++;
				}
				runs++;

				// We don't want this feature for the simulator.
				foreach (var record in records) { record.IsNew = false; }

				simulator.Simulate(records, rnd);

				Console.Write("\r{0} {1:#,##0} ({2:#,##0.00}/s), {3} (shrinked: {4})",
					sw.Elapsed,
					runs,
					runs / sw.Elapsed.TotalSeconds,
					records.Count,
					shrinks);

				if ((runs & 15) == 15)
				{
					records.Save(file);
				}
			}
		}
	}
}
