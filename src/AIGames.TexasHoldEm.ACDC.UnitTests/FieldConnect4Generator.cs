using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIGames.TexasHoldEm.ACDC.UnitTests
{
	public class FieldConnect4Generator
	{
		public FieldConnect4Generator()
		{
			Connect4 = new ulong[69];
			Connect2Out4 = new ulong[69 << 3];
			Connect3Out4 = new ulong[69 << 2];
			Connect4Threat = new ulong[69 << 2];

			SetConnect4();
			SetConnect2Out4();
			SetConnect3Out4();
		}

		public ulong[] Connect4;
		public ulong[] Connect2Out4;
		public ulong[] Connect3Out4;
		public ulong[] Connect4Threat;

		private void SetConnect4()
		{
			var items = new List<ulong>();

			// row scores.
			for (var col = 0; col < 4; col++)
			{
				for (var row = 0; row < 6; row++)
				{
					ulong line = 0x0F;
					line <<= col + (row << 3);
					items.Add(line);

				}
			}

			// column scores.
			for (var col = 0; col < 7; col++)
			{
				for (var row = 0; row < 3; row++)
				{
					ulong line = 0x01010101;
					line <<= col + (row << 3);
					items.Add(line);
				}
			}
			// diagonal scores.
			for (var col = 0; col < 4; col++)
			{
				for (var row = 0; row < 3; row++)
				{
					ulong dig0 = 0x08040201;
					ulong dig1 = 0x01020408;
					dig0 <<= col + (row << 3);
					dig1 <<= col + (row << 3);
					items.Add(dig0);
					items.Add(dig1);
				}
			}

			items.Sort();

			Connect4 = items.ToArray();

		}

		private void SetConnect3Out4()
		{
			var lookup = new ulong[69 << 2];

			for (var i = 0; i < 69; i++)
			{
				var mask = Connect4[i];

				var matches = GetMatch3(mask);
				for (var p = 0; p < 4; p++)
				{
					Connect3Out4[(i << 2) | p] = matches[p];
					Connect4Threat[(i << 2) | p] = mask - matches[p];
				}
			}
		}
		private void SetConnect2Out4()
		{
			var lookup = new ulong[69 << 3];

			for (var i = 0; i < 69; i++)
			{
				var mask = Connect4[i];

				var matches = GetMatch2(mask);
				for (var p = 0; p < 6; p++)
				{
					Connect2Out4[(i << 3) | p] = matches[p];
				}
			}
		}

		private static List<ulong> GetMatch3(ulong mask)
		{
			var matches = new List<ulong>(4);

			var pos = 0;
			var indexes = new int[4];

			for (var i = 0; i < 64; i++)
			{
				var flag = 1UL << i;

				if ((flag & mask) != 0)
				{
					indexes[pos++] = i;
					if (pos == 4) { break; }
				}
			}
			matches.Add((1UL << indexes[0]) | (1UL << indexes[1]) | (1UL << indexes[2]));
			matches.Add((1UL << indexes[0]) | (1UL << indexes[1]) | (1UL << indexes[3]));
			matches.Add((1UL << indexes[0]) | (1UL << indexes[2]) | (1UL << indexes[3]));
			matches.Add((1UL << indexes[1]) | (1UL << indexes[2]) | (1UL << indexes[3]));

			matches.Sort();
			return matches;
		}

		private static ulong[] GetMatch2(ulong mask)
		{
			var matches = new ulong[8];

			var pos = 0;
			var indexes = new int[4];

			for (var i = 0; i < 64; i++)
			{
				var flag = 1UL << i;

				if ((flag & mask) != 0)
				{
					indexes[pos++] = i;
					if (pos == 4) { break; }
				}
			}
			matches[0] = (1UL << indexes[0]) | (1UL << indexes[1]);
			matches[1] = (1UL << indexes[0]) | (1UL << indexes[2]);
			matches[2] = (1UL << indexes[0]) | (1UL << indexes[3]);
			matches[3] = (1UL << indexes[1]) | (1UL << indexes[2]);
			matches[4] = (1UL << indexes[1]) | (1UL << indexes[3]);
			matches[5] = (1UL << indexes[2]) | (1UL << indexes[3]);
			return matches;
		}

		public static string ToString(ulong[] patterns)
		{
			var sb = new StringBuilder();
			sb.Append("{");

			for (var i = 0; i < patterns.Length; i++)
			{
				sb.Append("0x").Append(patterns[i].ToString("X").ToLowerInvariant());
				if (i != patterns.Length - 1)
				{
					sb.Append(", ");
				}
			}
			sb.Append("};");

			return sb.ToString();
		}
		public static string ToString(int[] scores)
		{
			var sb = new StringBuilder();
			sb.Append("{");

			for (var i = 0; i < scores.Length; i++)
			{
				sb.Append(scores[i]);
				if (i != scores.Length - 1)
				{
					sb.Append(", ");
				}
			}
			sb.Append("};");

			return sb.ToString();
		}
	}
}
