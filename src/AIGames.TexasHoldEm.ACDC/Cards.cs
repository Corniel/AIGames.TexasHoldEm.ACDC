using AIGames.TexasHoldEm.ACDC.Analysis;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Troschuetz.Random.Generators;

namespace AIGames.TexasHoldEm.ACDC
{
	/// <summary>Represents a list of cards.</summary>
	[Serializable, DebuggerDisplay("{DebugToString()}")]
	public class Cards : List<Card> , IXmlSerializable, IFormattable, IEquatable<Cards>
	{
		private static readonly Regex Pattern = new Regex(@"^\[(.+)\]$", RegexOptions.Compiled);

		/// <summary>Represents a full deck of cards.</summary>
		public static readonly ReadOnlyCollection<Card> Deck = new ReadOnlyCollection<Card>("2s,3s,4s,5s,6s,7s,8s,9s,Ts,Js,Qs,Ks,As,2h,3h,4h,5h,6h,7h,8h,9h,Th,Jh,Qh,Kh,Ah,2c,3c,4c,5c,6c,7c,8c,9c,Tc,Jc,Qc,Kc,Ac,2d,3d,4d,5d,6d,7d,8d,9d,Td,Jd,Qd,Kd,Ad".Split(',').Select(s => Card.Parse(s)).ToList());

		/// <summary>Represent an empty set of cards.</summary>
		public static Cards Empty { get { return new Cards(); } }

		#region (XML) (De)serialization

		/// <summary>Gets the xml schema to (de) xml serialize a card.</summary>
		/// <remarks>
		/// Returns null as no schema is required.
		/// </remarks>
		XmlSchema IXmlSerializable.GetSchema() { return null; }

		/// <summary>Reads the card from an xml writer.</summary>
		/// <remarks>
		/// Uses the string parse function of card.
		/// </remarks>
		/// <param name="reader">An xml reader.</param>
		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			Clear();
			var s = reader.ReadElementString();
			var val = Parse(s);
			AddRange(val);
		}

		/// <summary>Writes the card to an xml writer.</summary>
		/// <remarks>
		/// Uses the string representation of card.
		/// </remarks>
		/// <param name="writer">An xml writer.</param>
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteString(ToString());
		}

		#endregion

		#region Tostring

		/// <summary>Represents a card as a formatted string.</summary>
		public string ToString(string format)
		{
			return ToString(format, CultureInfo.CurrentCulture);
		}

		/// <summary>Represents a card as a formatted string.</summary>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (Count == 0) { return ""; }
			return String.Format("[{0}]", String.Join(",", this.Select(item => item.ToString(format, formatProvider))));
		}

		/// <summary>Represents a card as a string.</summary>
		public override string ToString()
		{
			return ToString("");
		}

		/// <summary>Represents a card as a debug string.</summary>
		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		private string DebugToString()
		{
			if (Count == 0) { return "{empty}"; }
			return ToString("f");
		}

		#endregion

		public ulong GetMask()
		{
			var str = String.Join(" ", this);
			return Hand.ParseHand(str);
		}

		/// <summary>Gets the best card of the cards.</summary>
		public Card Best { get { return this.Max(); } }

		/// <summary>Gets the nth-best card of the cards.</summary>
		/// <remarks>
		/// position 0 equals the best card.
		/// </remarks>
		public Card GetBest(int pos) 
		{
			return this.OrderByDescending(c => c).Skip(pos).FirstOrDefault();
		}

		/// <summary>Creates a new copy of the cards.</summary>
		public Cards Copy()
		{
			var copy = Cards.Empty;
			copy.AddRange(this);
			return copy;
		}

		/// <summary>Parses a set of cards.</summary>
		/// <param name="str">
		/// The string representing a the hand.
		/// </param>
		public static Cards Parse(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return Cards.Empty;	
			}

			str = Pattern.Match(str).Groups[1].Value;

			var cards = Cards.Empty;
			cards.AddRange(str.Split(',').Select(s => Card.Parse(s)));

			return cards;
		}

		/// <summary>Creates a set of cards.</summary>
		public static Cards Create(IEnumerable<Card> cards)
		{
			var set = Cards.Empty;
			set.AddRange(cards);
			return set;
		}

		/// <summary>Gets a new shuffled deck.</summary>
		public static Cards GetShuffledDeck(MT19937Generator rnd = null)
		{
			var deck = Cards.Empty;
			rnd = rnd ?? s_Rnd;
			deck.AddRange(Deck.OrderBy(c => rnd.Next()));
			return deck;
		}
		private static MT19937Generator s_Rnd = new MT19937Generator();

		public override bool Equals(object obj)
		{
			return Equals(obj as Cards);
		}

		public bool Equals(Cards other)
		{
			if (other == null || Count != other.Count) { return false; }

			for (var i = 0; i < Count; i++)
			{
				if (this[i] != other[i]) { return false; }
			}
			return true;
		}

		public override int GetHashCode()
		{
			var hash = 0;
			for (var i = 0; i < Count; i++)
			{
				hash ^= this[i].GetHashCode() << i;
			}
			return hash;
		}
	}
}
