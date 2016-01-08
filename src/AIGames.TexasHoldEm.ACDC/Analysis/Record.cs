using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace AIGames.TexasHoldEm.ACDC.Analysis
{
	[DebuggerDisplay("{DebuggerDisplay}")]
	public class Record : IComparable, IComparable<Record>
	{
		public double Odds { get; set; }
		public byte Round { get; set; }
		public SubRoundType SubRound { get; set; }
		public byte Step { get; set; }
		public short Gap { get; set; }
		public GameAction Action { get;  set; }
		public short Profit { get;  set; }
		/// <summary>Returns true is the record is new.</summary>
		/// <remarks>
		/// Is not serialized.
		/// </remarks>
		public bool IsNew { get; set; }

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

		public static Record FromByteArray(byte[] bytes)
		{
			var record = new Record()
			{
				Round = bytes[0],
				SubRound = (SubRoundType)(bytes[1] >> 5),
				Step = (byte)(bytes[1] & 0x1F),
				Odds = ToOdds(bytes[2]),
				Gap = BitConverter.ToInt16(bytes, 3),
				Action = GameAction.FromUIint16(BitConverter.ToUInt16(bytes, 5)),
				Profit = BitConverter.ToInt16(bytes, 7),
			};
			return record;
		}

		private static byte ToByte(double odds)
		{
			return (byte)Math.Round(odds * 255);
		}
		public static double ToOdds(byte b)
		{
			return b / 255.0;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never), ExcludeFromCodeCoverage]
		private string DebuggerDisplay
		{
			get
			{
				return String.Format("{0:00.0%} {7} {1:00}.{2,-5}({3}), Gap: {4}, Prof: {5}{6}",
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

		public int CompareTo(object obj) { return CompareTo(obj as Record); }

		public int CompareTo(Record other)
		{
			var compare = other.Odds.CompareTo(this.Odds);

			if (compare == 0)
			{
				compare = other.Action.Amount.CompareTo(this.Action.Amount);
			}
			if (compare == 0)
			{
				compare = other.Action.ActionType.CompareTo(this.Action.ActionType);
			}
			return compare;
		}
	}
}
