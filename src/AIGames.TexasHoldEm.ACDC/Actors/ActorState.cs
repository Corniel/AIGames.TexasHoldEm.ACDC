using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGames.TexasHoldEm.ACDC.Actors
{
	[DebuggerDisplay("{DebuggerDisplay}")]
	public class ActorState
	{
		public double Odds { get; set; }
		public int BigBlind { get; set; }
		public int OwnStack { get; set; }
		public int OwnPot { get; set; }
		public int OtherStack { get; set; }
		public int OtherPot { get; set; }
		
		public int Pot { get { return OwnPot + OtherPot; } }

		public int AmountToCall { get { return Math.Max(0, OtherPot - OwnPot); } }

		public Troschuetz.Random.Generators.MT19937Generator Rnd { get; set; }

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
					AmountToCall == 0 ? "": "Call: "+ AmountToCall);
			}
		}
	}
}
