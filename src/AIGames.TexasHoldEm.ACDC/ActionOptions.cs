using System;
using System.Collections.Generic;
using McCulloch.Networks;
using Troschuetz.Random.Generators;

namespace AIGames.TexasHoldEm.ACDC
{
    public class ActionOptions: List<ActionOption>
    {
        public ActionOptions(NeuralNetwork<ActionOption> network, MT19937Generator rnd)
        {
            Network = network;
            Rnd = rnd;
        }

        public NeuralNetwork<ActionOption> Network { get; }
        public MT19937Generator Rnd { get; }
 
        public void Add(ActionState state, GameAction action)
        {
            var option = new ActionOption(state, action)
            {
                Rnd = Rnd.NextDouble(.1),
            };
            Add(option);
        }
        
        public ActionOption Select(ActionState state)
        {
            if (state.AmountToCall != 0)
            {
                Add(state, GameAction.Call);
                Add(state, GameAction.Fold);
            }
            else
            {
                Add(state, GameAction.Check);
            }

            AddRaises(state);
            Predict();
            Sort();
            return this[0];
        }

        private void AddRaises(ActionState state)
        {
            // We can't raise on the small blind.
            if (state.AmountToCall == state.SmallBlind && state.SubRound == SubRoundType.Pre) { return; }
            
            var step = 1 + ((state.MaximumRaise - state.BigBlind) >> 4);

            for (var raise = state.BigBlind; raise <= state.MaximumRaise; raise += step)
            {
                Add(state, GameAction.Raise(raise));
            }
        }

        private void Predict()
        {
            foreach (var option in this)
            {
                Network.Predict(option);
            }
        }
    }
}
