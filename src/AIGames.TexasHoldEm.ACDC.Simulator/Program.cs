using AIGames.TexasHoldEm.ACDC.Analysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Troschuetz.Random.Generators;

namespace AIGames.TexasHoldEm.ACDC.Simulator
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var file = new FileInfo("data.bin");

			int nodeSize = 0;

			var collection = new NodeCollection(LoadNodes(file));

			if (Log(args, collection)) { return; }

			if (args.Length == 0 || !Int32.TryParse(args[0], out nodeSize))
			{
				nodeSize = 50000;
			}
			Console.Clear();
			Console.WriteLine("Node size: {0:#,##0.0}k", nodeSize / 1000.0);

			var simulator = new Simulator();
			var rnd = new MT19937Generator();

			
			Merge(collection);

			var sw = Stopwatch.StartNew();
			long runs = 0;
			var shrinks = 0;
			while (true)
			{
				runs++;

				ClearNewStatus(collection);

				simulator.Simulate(collection, rnd);
				Write(collection, sw, runs, shrinks);

				if ((runs & 15) == 15)
				{
					collection.Save(file);
					Merge(collection);
				}
				if (collection.Count > nodeSize)
				{
					var nodes = collection.ToList();
					nodes.Shrink();
					collection.Clear();
					collection.AddRange(nodes);
					collection.Save(file);
					shrinks++;
					Write(collection, sw, runs, shrinks, true);
				}
			}
		}

		private static void Merge(NodeCollection nodes)
		{
			var dir = new DirectoryInfo(".");
			foreach (var file in dir.GetFiles("*.bin"))
			{
				if (file.Name != "data.bin")
				{
					Console.WriteLine();
					nodes.AddRange(Nodes.Load(file));
					Console.WriteLine("Merged {0}", file);
					file.Delete();
				}
			}
		}

		private static List<Node> LoadNodes(FileInfo file)
		{
			var nodes = new List<Node>();
			if (file.Exists)
			{
				nodes.AddRange(Nodes.Load(file));
			}
			nodes.Sort();
			return nodes;
		}

		private static bool Log(string[] args, IEnumerable<Node> nodes)
		{
			if (args.Length == 1 && args[0].ToUpperInvariant() == "LOG")
			{
				using (var writer = new StreamWriter("data.log"))
				{
					foreach (var node in nodes)
					{
						writer.WriteLine(node);
					}
				}
				return true;
			}
			return false;
		}

		private static void ClearNewStatus(IEnumerable<Node> nodes)
		{
			// We don't want this feature for the simulator.
			foreach (var node in nodes) { node.IsNew = false; }
		}

		private static void Write(NodeCollection nodes, Stopwatch sw, long runs, int shrinks, bool writeLine = false)
		{

			Console.Write("\r{0} {1:#,##0} ({2:#,##0.00}/s), {3} ({4})",
				sw.Elapsed,
				runs,
				runs / sw.Elapsed.TotalSeconds,
				nodes.Count,
				shrinks);

			if (writeLine)
			{
				Console.WriteLine();
			}
		}
	}
}
