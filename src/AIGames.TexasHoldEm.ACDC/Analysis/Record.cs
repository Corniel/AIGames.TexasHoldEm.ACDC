using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace AIGames.TexasHoldEm.ACDC.Analysis
{
	public class Record : IComparable, IComparable<Record>
	{
		public const int ByteSize = 13;

		public double Odds { get; set; }
		public byte ByteOdds { get { return ToByte(Odds); } }
		public byte Round { get; set; }
		public SubRoundType SubRound { get; set; }
		public byte Step { get; set; }
		public short Pot { get; set; }
		public short Gap { get; set; }
		public short AmountToCall { get; set; }
		public bool HasAmountToCall { get { return AmountToCall != 0; } }
		public GameAction Action { get;  set; }
		public short Profit { get;  set; }
		/// <summary>Returns true is the record is new.</summary>
		/// <remarks>
		/// Is not serialized.
		/// </remarks>
		public bool IsNew { get; set; }

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
			if (compare == 0)
			{
				compare = other.SubRound.CompareTo(this.SubRound);
			}
			if (compare == 0)
			{
				compare = this.Pot.CompareTo(other.Pot);
			}
			if (compare == 0)
			{
				compare = this.Gap.CompareTo(other.Gap);
			}
			if (compare == 0)
			{
				compare = this.Round.CompareTo(other.Round);
			}
			if (compare == 0)
			{
				compare = this.Step.CompareTo(other.Step);
			}
			if (compare == 0)
			{
				compare = other.Profit.CompareTo(this.Profit);
			}
			return compare;
		}

		public byte[] ToByteArray()
		{
			var act = BitConverter.GetBytes(Action.ToUInt16());
			var gap = BitConverter.GetBytes(Gap);
			var profit = BitConverter.GetBytes(Profit);
			var pot = BitConverter.GetBytes(Pot);
			var call = BitConverter.GetBytes(AmountToCall);

			var bytes = new byte[ByteSize];
			bytes[0] = Round;
			bytes[1] = (byte)((int)SubRound << 5 | Step);
			bytes[2] = ToByte(Odds);

			Array.Copy(gap, 0, bytes, 3, 2);
			Array.Copy(act, 0, bytes, 5, 2);
			Array.Copy(profit, 0, bytes, 7, 2);
			Array.Copy(pot, 0, bytes, 9, 2);
			Array.Copy(call, 0, bytes, 11, 2);

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
				Pot = BitConverter.ToInt16(bytes, 9),
				AmountToCall = BitConverter.ToInt16(bytes, 11),
			};
			return record;
		}

		public static Record Merge(Record left, Record right)
		{
			var merged = new Record()
			{
				Odds = left.Odds,
				SubRound = left.SubRound,
				Action = left.Action,
				AmountToCall = (short)((left.AmountToCall + right.AmountToCall) >> 1),
				Gap = (short)((left.Gap + right.Gap) >> 1),
				Pot = (short)((left.Pot + right.Pot) >> 1),
				Profit = (short)((left.Profit + right.Profit) >> 1),
				Round = (byte)((left.Round + right.Round) >> 1),
				Step = (byte)((left.Step + right.Step) >> 1),
			};
			if (left.Action.ActionType == GameActionType.raise)
			{
				merged.Action = GameAction.Raise((left.Action.Amount + right.Action.Amount) >> 1);
			}
			return merged;
		}

		public static byte ToByte(double odds)
		{
			return (byte)Math.Round(odds * 255);
		}
		public static double ToOdds(byte b)
		{
			return b / 255.0;
		}

		public override string ToString()
		{
			return String.Format(
				CultureInfo.InvariantCulture,
				"{0:00.0%} {7} {1:00}.{2,-5}({3}), {7} Pot: {8}, Call: {9}, Gap: {4}, Profit: {5}{6}",
				Odds,
				Round,
				SubRound,
				Step,
				Gap,
				Profit > 0 ? "+": "",
				Profit,
				Action,
				Pot,
				AmountToCall);
		}
	}
}
