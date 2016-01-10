using AIGames.TexasHoldEm.ACDC.Actors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AIGames.TexasHoldEm.ACDC.Analysis
{
	public static partial class Nodes
	{
		public static IList<Node> Get()
		{
			var bytes = Convert.FromBase64String(GetData());
			using (var stream = new MemoryStream(bytes))
			{
				return Load(stream);
			}
		}
		public static IList<Node> Load(FileInfo file)
		{
			using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
			{
				return Load(stream);
			}
		}
		public static  IList<Node> Load(Stream stream)
		{
			var list = new List<Node>();
			var reader = new BinaryReader(stream);
			while(true)
			{
				var bytes = reader.ReadBytes(Node.ByteSize);
				if (bytes.Length != Node.ByteSize) { break; }
				list.Add(Node.FromByteArray(bytes));
			}
			return list;
		}

		public static void Shrink(this List<Node> nodes)
		{
			nodes.Sort();

			var old = nodes
				.Where(node=> node.Action != GameAction.Check)
				.OrderBy(node => Math.Abs(0.5 - node.Odds))
				.ToList();

			var buffer = new List<Node>(nodes.Capacity);

			while (old.Count > 0)
			{
				var candidate = old[0];
				old.RemoveAt(0);

				// get these out.
				if (candidate.Round == 0) { continue; }

				var match = old
					.Where(item =>
						candidate.ByteOdds == item.ByteOdds &&
						candidate.SubRound == item.SubRound &&
						candidate.HasAmountToCall == item.HasAmountToCall &&
						candidate.Action.ActionType == item.Action.ActionType)
					.OrderByDescending(item => Matcher.Node(candidate, item)).FirstOrDefault();

				var m = match == null ? -1 : Matcher.Node(candidate, match);

				if (m > 0)
				{
					var merged = Node.Merge(candidate, match);
					buffer.Add(merged);
					old.Remove(match);
				}
				else
				{
					buffer.Add(candidate);
				}
			}
			nodes.Clear();
			nodes.AddRange(buffer);
			nodes.Sort();
		}

		public static void Save(this IEnumerable<Node> items, FileInfo file)
		{
			using (var stream = new FileStream(file.FullName, FileMode.Create, FileAccess.Write))
			{
				Save(items, stream);
			}
		}
		public static void Save(this IEnumerable<Node> items, Stream stream)
		{
			var writer = new BinaryWriter(stream);
			foreach (var item in items)
			{
				var bytes = item.ToByteArray();
				writer.Write(bytes);
			}
		}
	}
}
