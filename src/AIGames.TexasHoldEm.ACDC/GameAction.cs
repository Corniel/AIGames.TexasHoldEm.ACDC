using AIGames.TexasHoldEm.ACDC.Communication;
using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AIGames.TexasHoldEm.ACDC
{
	/// <summary>Represents an action.</summary>
	[Serializable]
	public struct GameAction : IInstruction, ISerializable, IXmlSerializable
	{
		/// <summary>Represents a check.</summary>
		public static readonly GameAction Check = new GameAction() { m_Value = (ushort)GameActionType.check };
		/// <summary>Represents a call.</summary>
		public static readonly GameAction Call = new GameAction() { m_Value = (ushort)GameActionType.call };
		/// <summary>Represents a fold.</summary>
		public static readonly GameAction Fold = new GameAction() { m_Value = (ushort)GameActionType.fold };

		/// <summary>Invalid game action.</summary>
		public static readonly GameAction Invalid = new GameAction() { m_Value = ushort.MaxValue };

		/// <summary>Creates a raise.</summary>
		public static GameAction Raise(int amount)
		{
			if (amount < 0) { throw new ArgumentOutOfRangeException("The amount should be at least 0.", "amount"); }
			return new GameAction() { m_Value = (ushort)((amount << 2) | (ushort)GameActionType.raise) };
		}

		public static GameAction CheckOrCall(bool noAmountToCall) { return noAmountToCall ? Check : Call; }

		#region (XML) (De)serialization

		/// <summary>Initializes a new instance of action based on the serialization info.</summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		private GameAction(SerializationInfo info, StreamingContext context)
		{
			if (info == null) { throw new ArgumentNullException("info"); }
			m_Value = info.GetUInt16("Value");
		}

		/// <summary>Adds the underlying property of action to the serialization info.</summary>
		/// <param name="info">The serialization info.</param>
		/// <param name="context">The streaming context.</param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null) { throw new ArgumentNullException("info"); }
			info.AddValue("Value", m_Value);
		}

		/// <summary>Gets the xml schema to (de) xml serialize an action.</summary>
		/// <remarks>
		/// Returns null as no schema is required.
		/// </remarks>
		XmlSchema IXmlSerializable.GetSchema() { return null; }

		/// <summary>Reads the action from an xml writer.</summary>
		/// <remarks>
		/// Uses the string parse function of action.
		/// </remarks>
		/// <param name="reader">An xml reader.</param>
		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			var s = reader.ReadElementString();
			var val = Parse(s);
			m_Value = val.m_Value;
		}

		/// <summary>Writes the action to an xml writer.</summary>
		/// <remarks>
		/// Uses the string representation of action.
		/// </remarks>
		/// <param name="writer">An xml writer.</param>
		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			writer.WriteString(ToString());
		}

		#endregion

		#region Properties

		/// <summary>Represents the internal value.</summary>
		private ushort m_Value;

		/// <summary>Gets the height of the action.</summary>
		public int Amount { get { return m_Value >> 2; } }

		/// <summary>Gets the suit of the action.</summary>
		public GameActionType ActionType { get { return (GameActionType)(m_Value & 3); } }

		#endregion

		#region Byte Array

		public ushort ToUInt16() { return m_Value; }

		public static GameAction FromUIint16(ushort val) { return new GameAction() { m_Value = val }; }

		#endregion

		#region Tostring

		/// <summary>Represents an action as a string.</summary>
		public override string ToString()
		{
			if (m_Value == Invalid.m_Value) { return "INVALID"; }
			return String.Format("{0} {1}", this.ActionType, this.Amount);
		}

		#endregion

		#region Equality

		/// <summary>Indicates whether this instance and a specified object are equal.</summary>
		/// <param name="obj">
		/// Another object to compare to.
		/// </param>
		/// <returns>
		/// true if obj and this action are the same type and represent the same value;
		/// otherwise, false.
		/// </returns>
		public override bool Equals(object obj) { return base.Equals(obj); }

		/// <summary>Returns the hash code for this action.</summary>
		public override int GetHashCode() { return (int)m_Value; }

		/// <summary>Returns true if left equals right, otherwise false.</summary>
		public static bool operator ==(GameAction l, GameAction r) { return l.Equals(r); }
		/// <summary>Returns false if left equals right, otherwise true.</summary>
		public static bool operator !=(GameAction l, GameAction r) { return !(l == r); }

		#endregion

		#region Factory methods

		/// <summary>Parses an action.</summary>
		/// <param name="str">
		/// The string representing an action.
		/// </param>
		public static GameAction Parse(string str)
		{
			GameAction action;
			if (TryParse(str, out action))
			{
				return action;
			}
			throw new ArgumentException("The input does not represent a valid action.", "str");
		}

		/// <summary>Tries to parse an action.</summary>
		/// <param name="str">
		/// The string representing an action.
		/// </param>
		/// <param name="action">
		/// The actual action.
		/// </param>
		/// <returns>
		/// True, if the parsing succeeded, otherwise false.
		/// </returns>
		public static bool TryParse(string str, out GameAction action)
		{
			action = GameAction.Check;

			if (String.IsNullOrEmpty(str)) { return true; }


			var splitted = str.Split(' ');

			GameActionType tp;

			if (splitted.Length < 3 && Enum.TryParse<GameActionType>(splitted[0], true, out tp))
			{
				if (tp != GameActionType.raise && (splitted.Length == 1 || splitted[1] == "0"))
				{
					action = new GameAction() { m_Value = (ushort)tp };
					return true;
				}
				int amount;
				if (Int32.TryParse(splitted[1], out amount))
				{
					action = Raise(amount < 0 ? 0 : amount);
					return true;
				}
			}
			return false;
		}

		#endregion
	}
}
