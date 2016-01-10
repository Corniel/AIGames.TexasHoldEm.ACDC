using AIGames.TexasHoldEm.ACDC.Actors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AIGames.TexasHoldEm.ACDC.Analysis
{
	public static partial class Records
	{
		public static IList<Record> Get()
		{
			var bytes = Convert.FromBase64String(GetData());
			using (var stream = new MemoryStream(bytes))
			{
				return Load(stream);
			}
		}
		public static IList<Record> Load(FileInfo file)
		{
			using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
			{
				return Load(stream);
			}
		}
		public static  IList<Record> Load(Stream stream)
		{
			var list = new List<Record>();
			var reader = new BinaryReader(stream);
			while(true)
			{
				var bytes = reader.ReadBytes(Record.ByteSize);
				if (bytes.Length != Record.ByteSize) { break; }
				list.Add(Record.FromByteArray(bytes));
			}
			return list;
		}

		public static void Shrink(this List<Record> records)
		{
			records.Sort();

			var old = records
				.OrderBy(r => Math.Abs(0.5 - r.Odds))
				.ToList();

			var buffer = new List<Record>(records.Capacity);

			while (old.Count > 0)
			{
				var candidate = old[0];
				old.RemoveAt(0);

				var match = old
					.Where(item =>
						candidate.ByteOdds == item.ByteOdds &&
						candidate.SubRound == item.SubRound &&
						candidate.HasAmountToCall == item.HasAmountToCall &&
						candidate.Action.ActionType == item.Action.ActionType)
					.OrderByDescending(item => Matcher.Record(candidate, item)).FirstOrDefault();

				if (match != null)
				{
					var merged = Record.Merge(candidate, match);
					buffer.Add(merged);
					old.Remove(match);
				}
				else
				{
					buffer.Add(candidate);
				}
			}
			records.Clear();
			records.AddRange(buffer);
			records.Sort();
		}

		public static void Save(this IEnumerable<Record> items, FileInfo file)
		{
			using (var stream = new FileStream(file.FullName, FileMode.Create, FileAccess.Write))
			{
				Save(items, stream);
			}
		}
		public static void Save(this IEnumerable<Record> items, Stream stream)
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
