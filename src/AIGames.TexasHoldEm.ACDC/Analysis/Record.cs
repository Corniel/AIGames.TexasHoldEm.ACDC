using System;
using System.Diagnostics;

namespace AIGames.TexasHoldEm.ACDC.Analysis
{
	[DebuggerDisplay("{DebuggerDisplay}")]
	public class Record
	{
		public double Odds { get; set; }
		public byte Round { get; set; }
		public SubRoundType SubRound { get; set; }
		public byte Step { get; set; }
		public short Gap { get; set; }
		public GameAction Action { get;  set; }
		public short Profit { get;  set; }

		public byte[] ToByteArray()
		{
			var act = BitConverter.GetBytes(Action.ToUInt16());
			var gap = BitConverter.GetBytes(Gap);
			var profit = BitConverter.GetBytes(Profit);

			var bytes = new byte[9];
			bytes[0] = Round;
			bytes[1] = (byte)((int)SubRound << 5 | Step);
			bytes[2] = ToByte(Odds);

			Array.Copy(gap, 0, bytes, 3, 2);
			Array.Copy(act, 0, bytes, 5, 2);
			Array.Copy(profit, 0, bytes, 7, 2);

			return bytes;
		}

		private static byte ToByte(double odds)
		{
			return (byte)Math.Round(odds * 255);
		}
		public static double ToOdds(byte b)
		{
			return b / 255;
		}

		private string DebuggerDisplay
		{
			get
			{
				return String.Format("{0:00.0%} {1:00}.{2,-5}({3}), Gap: {4}, Prof: {5}{6}, {7}",
					Odds,
					Round,
					SubRound,
					Step,
					Gap,
					Profit > 0 ? "+": "",
					Profit,
					Action);
			}
		}
	}
}
