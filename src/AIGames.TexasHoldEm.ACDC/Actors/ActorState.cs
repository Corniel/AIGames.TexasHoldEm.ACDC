using AIGames.TexasHoldEm.ACDC.Analysis;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Troschuetz.Random.Generators;

namespace AIGames.TexasHoldEm.ACDC.Actors
{
	[DebuggerDisplay("{DebuggerDisplay}")]
	public class ActorState
	{
		public int Round { get; set; }
		public SubRoundType SubRound { get; set; }
		public int Step { get; set; }

		public double Odds { get; set; }
		public int SmallBlind { get { return BigBlind >> 1; } }
		public int BigBlind { get; set; }
		public int OwnStack { get; set; }
		public int OwnPot { get; set; }
		public int OtherStack { get; set; }
		public int OtherPot { get; set; }
		
		public int Pot { get { return OwnPot + OtherPot; } }
		public int Gap { get { return OwnStack - OtherStack; } }

		/// <summary>Test the maximum amount to raise.</summary>
		public int MaximumRaise
		{
			get
			{
				// Don't raise on small blind.
				if (AmountToCall == SmallBlind) { return 0; }
				var minStack = Math.Min(OwnStack - AmountToCall, OtherStack);
				if (minStack < BigBlind) { return 0; };
				return Math.Min(minStack, Pot + AmountToCall);
			}
		}

		public int AmountToCall { get { return Math.Max(0, OtherPot - OwnPot); } }

		/// <summary>Returns true if there is no amount to call, otherwise false.</summary>
		public bool NoAmountToCall { get { return AmountToCall == 0; } }

		public Record ToRecord()
		{
			return new Record()
			{
				Odds = Odds,
				Round = (byte)Round,
				SubRound = SubRound,
				Step = (byte)Step,
				Gap = (short)Gap,
			};
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never), ExcludeFromCodeCoverage]
		private string DebuggerDisplay
		{
			get
			{
				return String.Format("{0:0.0%} Blind: {1}, Pot: {2}, Own: {3} ({4}), Other: {5} ({6}){7}",
					Odds, BigBlind,
					Pot,
					OwnPot, OwnStack,
					OtherPot, OtherStack,
					NoAmountToCall ? "": "Call: "+ AmountToCall);
			}
		}
	}
}
