using AIGames.TexasHoldEm.ACDC.Actors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AIGames.TexasHoldEm.ACDC.Analysis
{
	public static partial class Records
	{
		public static ActionOption Select(this IEnumerable<Record> items,  Record test, ActionOptions options)
		{
			if (options.Count == 1) { return options.FirstOrDefault(); }

			foreach (var item in items)
			{
				foreach (var option in options)
				{
					var type = option.ActionType;
					if (type == item.Action.ActionType)
					{
						// To match amounts.
						test.Action = option.Action;
						var match = Matcher.Record(test, item);
						if (match > 0)
						{
							if (item.IsNew)
							{
								match *= 10;
							}
							option.Update(item.Profit, match);
						}
					}
				}
			}
			options.Sort();
			var best = options.FirstOrDefault();
			test.Action = best.Action;
			return best;
		}

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
