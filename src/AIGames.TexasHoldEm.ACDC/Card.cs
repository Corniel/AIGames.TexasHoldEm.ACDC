using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AIGames.TexasHoldEm.ACDC
{
	/// <summary>Represents a card.</summary>
	[Serializable, DebuggerDisplay("{DebuggerDisplay}")]
	public struct Card : ISerializable, IXmlSerializable, IComparable, IComparable<Card>, IFormattable
	{
		/// <summary>Represents an unset/empty card.</summary>
		public static readonly Card Empty = default(Card);
		
		#region (XML) (De)serialization

		/// <summary>Initializes a new instance of card based on the serialization info.</summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		private Card(SerializationInfo info, StreamingContext context)
		{
			if (info == null) { throw new ArgumentNullException("info"); }
			m_Value = info.GetByte("Value");
		}

		/// <summary>Adds the underlying property of card to the serialization info.</summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null) { throw new ArgumentNullException("info"); }
			info.AddValue("Value", m_Value);
		}

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
			var s = reader.ReadElementString();
			var val = Parse(s);
			m_Value = val.m_Value;
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

		#region Properties

		/// <summary>Represents the internal value.</summary>
		private byte m_Value;

		/// <summary>Gets the height of the card.</summary>
		public int Height { get { return 1 + (m_Value >> 2); } }

		/// <summary>Gets the suit of the card.</summary>
		public CardSuit Suit { get { return (CardSuit)(m_Value & 3); } }

		#endregion

		/// <summary>Returns true if card represents the empty value, otherwise false.</summary>
		public bool IsEmpty() { return m_Value == default(byte); }

		#region Tostring

		/// <summary>Represents a card as a formatted string.</summary>
		public string ToString(string format)
		{
			return ToString(format, CultureInfo.CurrentCulture);
		}

		/// <summary>Represents a card as a formatted string.</summary>
		/// <param name="format"></param>
		/// <param name="formatProvider"></param>
		/// <returns></returns>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			if (format == "f")
			{
				if (IsEmpty()) { return ""; }
				var number = Numbers[m_Value >> 2];
				var color = ColorAlternatives[m_Value & 3];

				var str = new string(new char[] { number, color });
				str = str.Replace("T", "10");
				return str;
			}
			return ToString();
		}

		/// <summary>Represents a card as a string.</summary>
		public override string ToString()
		{
			if (IsEmpty()) { return string.Empty; }
			var number = Numbers[m_Value >> 2];
			var color = Colors[m_Value & 3];
			return new string(new char[] { number, color });
		}

		/// <summary>Represents a card as a debug string.</summary>
		[DebuggerBrowsable(DebuggerBrowsableState.Never), ExcludeFromCodeCoverage]
		private string DebuggerDisplay
		{
			get
			{
				if (IsEmpty()) { return "{empty}"; }
				return ToString("f");
			}
		}

		#endregion

		#region Equality

		/// <summary>Indicates whether this instance and a specified object are equal.</summary>
		/// <param name="obj">
		/// Another object to compare to.
		/// </param>
		/// <returns>
		/// true if obj and this card are the same type and represent the same value;
		/// otherwise, false.
		/// </returns>
		public override bool Equals(object obj) { return base.Equals(obj); }

		/// <summary>Returns the hash code for this card.</summary>
		public override int GetHashCode() { return (int)m_Value; }

		/// <summary>Returns true if left equals right, otherwise false.</summary>
		public static bool operator ==(Card l, Card r) { return l.Equals(r); }
		/// <summary>Returns false if left equals right, otherwise true.</summary>
		public static bool operator !=(Card l, Card r) { return !(l == r); }

		#endregion

		#region IComparable

		/// <summary>Compares this instance with a specified System.Object and indicates whether
		/// this instance precedes, follows, or appears in the same position in the sort
		/// order as the specified System.Object.
		/// </summary>
		/// <param name="obj">
		/// An object that evaluates to a card.
		/// </param>
		/// <returns>
		/// A 32-bit signed integer that indicates whether this instance precedes, follows,
		/// or appears in the same position in the sort order as the value parameter.Value
		/// Condition Less than zero This instance precedes value. Zero This instance
		/// has the same position in the sort order as value. Greater than zero This
		/// instance follows value.-or- value is null.
		/// </returns>
		/// <exception cref="System.ArgumentException">
		/// value is not a card.
		/// </exception>
		public int CompareTo(object obj)
		{
			if (obj is Card)
			{
				return CompareTo((Card)obj);
			}
			throw new ArgumentException("Argument must be a card.", "obj");
		}

		/// <summary>Compares this instance with a specified card and indicates
		/// whether this instance precedes, follows, or appears in the same position
		/// in the sort order as the specified card.
		/// </summary>
		/// <param name="other">
		/// The card to compare with this instance.
		/// </param>
		/// <returns>
		/// A 32-bit signed integer that indicates whether this instance precedes, follows,
		/// or appears in the same position in the sort order as the value parameter.
		/// </returns>
		public int CompareTo(Card other) { return m_Value.CompareTo(other.m_Value); }

		#endregion

		#region Index

		/// <summary>Gets the index of the card.</summary>
		/// <remarks>
		/// these index can be in the range [0,51].
		/// </remarks>
		public int Index 
		{ 
			get
			{
				return (int)Suit * 13 + Height - 2;
			}
		}

		/// <summary>Creates a card based on its index.</summary>
		public static Card FromIndex(int index)
		{
			var height = 2 + index % 13;
			var suit = (CardSuit)(index / 13);
			return Create(height, suit);
		}

		#endregion

		#region Factory methods

		/// <summary>Creates a card.</summary>
		/// <param name="height">
		/// The height of the card.
		/// </param>
		/// <param name="suit">
		/// The suit of the card.
		/// </param>
		public static Card Create(int height, CardSuit suit)
		{
			if (height < 2 || height > 14) { throw new ArgumentOutOfRangeException("The height should be in the range [2, 14].", "height"); }

			var val = (byte)(((height - 1) << 2) + (int)suit);

			return new Card() { m_Value = val };
		}
	
		/// <summary>Parses a card.</summary>
		/// <param name="str">
		/// The string representing a card.
		/// </param>
		public static Card Parse(string str)
		{
			Card card;
			if (TryParse(str, out card))
			{
				return card;
			}
			throw new ArgumentException("The input does not represent a valid card.", "str");
		}

		/// <summary>Tries to parse a card.</summary>
		/// <param name="str">
		/// The string representing a card.
		/// </param>
		/// <param name="card">
		/// The actual card.
		/// </param>
		/// <returns>
		/// True, if the parsing succeeded, otherwise false.
		/// </returns>
		/// <remarks>
		/// A card is always represented by two characters:
		/// The first represents the card height and can be any number 2-9,
		/// T, J, Q, K, A are 10, Jack, Queen, King and Ace respectively.
		/// 
		/// The second character represents the suit and can be d, c, h or s.
		/// For Diamonds, Clubs, Hearts and Spades respectively.
		/// </remarks>
		public static bool TryParse(string str, out Card card)
		{
			card = Empty;

			if (str != null && str.Length == 2)
			{
				var number = Numbers.IndexOf(str[0]);
				var color = Colors.IndexOf(str[1]);

				if (number > -1 && color > -1)
				{
					card.m_Value = (byte)(color + (number << 2));
					return true;
				}
			}
			return false;
		}

		private const string Numbers = " 23456789TJQKA";
		private const string Colors = "dchs";
		private const string ColorAlternatives = "♦♣♥♠";

		#endregion
	}
}
