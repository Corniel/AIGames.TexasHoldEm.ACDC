using System;
using System.Globalization;

namespace AIGames.TexasHoldEm.ACDC.Actors
{
	public class ActionOption : IComparable, IComparable<ActionOption>
	{
		public ActionOption(GameAction action) : this(action, 0, 0) { }
		public ActionOption(GameAction action, double profit, double weight = 0.0000001)
		{
			Action = action;
			m_Profit = profit * weight;
			Weight = weight;
		}

		public GameAction Action { get; private set; }
		public double Profit { get { return Weight == 0 ? 0 : m_Profit / Weight; } }

		public double WeightedProfit { get { return Action == GameAction.Check ? 16 * (150 + Profit) : 100 + Profit; } }

		/// <summary>Returns true if there is expected profit.</summary>
		public bool IsProfitable { get { return ActionType != GameActionType.fold && Profit >= 0; } }

		private double m_Profit;
		public double Weight { get; private set; }
		public GameActionType ActionType { get { return Action.ActionType; } }

		public bool IsUncertain { get { return Action != GameAction.Check && Action != GameAction.Fold; } }

		public void Update(double profit, double weight)
		{
			m_Profit+= profit * weight;
			Weight+= weight;
		}

		public override string ToString()
		{
			return String.Format(
				CultureInfo.InvariantCulture,
				"{0} {1}{2:#,##0.0} ({3:0.000}{4})",
				Action,
				Profit > 0 ? "+" : "",
				Profit,
				Weight,
				Weight == 0 ? "*": "");
			
		}

		public int CompareTo(object obj) { return CompareTo(obj as ActionOption); }

		public int CompareTo(ActionOption other)
		{
			return other.Profit.CompareTo(this.Profit);
		}
	}
}
