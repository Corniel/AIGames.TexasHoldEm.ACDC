﻿using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace AIGames.TexasHoldEm.ACDC.Actors
{
	[DebuggerDisplay("{DebuggerDisplay}")]
	public class ActionOption : IComparable, IComparable<ActionOption>
	{
		public ActionOption(GameAction action, double profit, double weight = 0.5)
		{
			Action = action;
			m_Profit = profit;
			m_Weight = weight;
		}

		private double m_Profit;
		private double m_Weight;

		public GameAction Action { get; private set; }
		public double Profit { get { return m_Weight == 0 ? 0 : m_Profit / m_Weight; } }
		public GameActionType ActionType { get { return Action.ActionType; } }

		public void Update(double profit, double weight)
		{
			m_Profit+= profit * weight;
			m_Weight+= weight;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never), ExcludeFromCodeCoverage]
		private string DebuggerDisplay
		{
			get
			{
				return String.Format("{0} {1}{2:#,##0.0} ({3:0.0})",
					Action,
					Profit > 0 ? "+" : "",
					Profit,
					m_Weight);
			}
		}

		public int CompareTo(object obj) { return CompareTo(obj as ActionOption); }

		public int CompareTo(ActionOption other)
		{
			return other.Profit.CompareTo(this.Profit);
		}
	}
}
