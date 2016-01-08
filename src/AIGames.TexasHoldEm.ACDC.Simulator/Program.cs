﻿using AIGames.TexasHoldEm.ACDC.Analysis;
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

			int recordSize = 0;

			if (args.Length == 0 || !Int32.TryParse(args[0], out recordSize))
			{
				recordSize = 50000;
			}
			Console.Clear();
			Console.WriteLine("Record size: {0:#,##0.0}k", recordSize / 1000.0);

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
				runs++;

				// We don't want this feature for the simulator.
				foreach (var record in records) { record.IsNew = false; }

				simulator.Simulate(records, rnd);
				Write(records, sw, runs, shrinks);

				if ((runs & 15) == 15)
				{
					records.Save(file);
				}
				if (records.Count > recordSize)
				{
					simulator.Shrink(records);
					shrinks++;
					Write(records, sw, runs, shrinks, true);
				}
			}
		}

		private static void Write(List<Record> records, Stopwatch sw, long runs, int shrinks, bool writeLine = false)
		{

			Console.Write("\r{0} {1:#,##0} ({2:#,##0.00}/s), {3} ({4})",
				sw.Elapsed,
				runs,
				runs / sw.Elapsed.TotalSeconds,
				records.Count,
				shrinks);

			if (writeLine)
			{
				Console.WriteLine();
			}
		}
	}
}
