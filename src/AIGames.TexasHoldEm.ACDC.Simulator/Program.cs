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

			int recordSize = 0;

			var records = LoadRecords(file);
			Merge(records);

			if (Log(args, records)) { return; }

			if (args.Length == 0 || !Int32.TryParse(args[0], out recordSize))
			{
				recordSize = 50000;
			}
			Console.Clear();
			Console.WriteLine("Record size: {0:#,##0.0}k", recordSize / 1000.0);

			var simulator = new Simulator();
			var rnd = new MT19937Generator();

			var sw = Stopwatch.StartNew();
			long runs = 0;
			var shrinks = 0;
			while (true)
			{
				runs++;

				ClearNewStatus(records);

				simulator.Simulate(records, rnd);
				Write(records, sw, runs, shrinks);

				if ((runs & 15) == 15)
				{
					records.Save(file);
					Merge(records);
				}
				if (records.Count > recordSize)
				{
					simulator.Shrink(records);
					shrinks++;
					Write(records, sw, runs, shrinks, true);
				}
			}
		}

		private static void Merge(List<Record> records)
		{
			var dir = new DirectoryInfo(".");
			foreach (var file in dir.GetFiles("*.bin"))
			{
				if (file.Name != "data.bin")
				{
					Console.WriteLine();
					records.AddRange(Records.Load(file));
					Console.WriteLine("Merged {0}", file);
					file.Delete();
				}
			}
		}

		private static List<Record> LoadRecords(FileInfo file)
		{
			var records = new List<Record>();
			if (file.Exists)
			{
				records.AddRange(Records.Load(file));
			}
			records.Sort();
			return records;
		}

		private static bool Log(string[] args, List<Record> records)
		{
			if (args.Length == 1 && args[0].ToUpperInvariant() == "LOG")
			{
				using (var writer = new StreamWriter("data.log"))
				{
					foreach (var record in records)
					{
						writer.WriteLine(record);
					}
				}
				return true;
			}
			return false;
		}

		private static void ClearNewStatus(List<Record> records)
		{
			// We don't want this feature for the simulator.
			foreach (var record in records) { record.IsNew = false; }
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
