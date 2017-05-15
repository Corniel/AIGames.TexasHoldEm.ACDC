using McCulloch.Modeling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGames.TexasHoldEm.ACDC
{
    public class ActionOption: IComparable<ActionOption>
    {
        public ActionOption(ActionState state, GameAction action)
        {
            State = state;
            Action = action;
            Result = CategoryBool.False;
        }

        public ActionState State { get; }

        public GameAction Action { get; }

        [InputProperty]
        public double Odds => State.Odds;

        [InputProperty]
        public GameActionType ActionType => Action.ActionType;

        [InputProperty]
        public double Amount => Action.Amount / (double)State.BigBlind;

        [OutputProperty]
        public CategoryBool Result { get; set; }

        public double Score => Result[true];


        internal double Rnd { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private double Sort => Score + Rnd;

        public int CompareTo(ActionOption other) => other.Sort.CompareTo(Sort);

        public override string ToString()
        {
            return $"{Result[true].ToString("0.00%")} {Action}";
        }
    }
}
