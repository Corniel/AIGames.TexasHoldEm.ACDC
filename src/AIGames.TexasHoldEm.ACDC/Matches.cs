using AIGames.TexasHoldEm.ACDC.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGames.TexasHoldEm.ACDC
{
	public class Matches
	{
		private Dictionary<int, Match> dict = new Dictionary<int, Match>();

		public Match this[int round]
		{
			get
			{
				Match match;
				if (!dict.TryGetValue(round, out match))
				{
					match = new Match(round);
					dict[round] = match;
				}
				return match;
			}
			set
			{
				dict[round] = value;
			}
		}

		public Match Current { get { return this[Round]; } }

		public List<Match> Rounds { get { return dict.Values.ToList(); } }

		public int Round { get; set; }


		public bool Apply(IInstruction instruction)
		{
			if (Mapping.ContainsKey(instruction.GetType()))
			{
				Mapping[instruction.GetType()].Invoke(instruction, this);
				return true;
			}
			return false;
		}

		private static Dictionary<Type, Action<IInstruction, Matches>> Mapping = new Dictionary<Type, Action<IInstruction, Matches>>()
		{
			{ typeof(RoundInstruction), (instruction, matches) => { matches.Round = ((RoundInstruction)instruction).Round; }},

			{ typeof(SmallBlindInstruction), (instruction, matches) => { matches.Current.SmallBlind = ((SmallBlindInstruction)instruction).Value; }},
			{ typeof(BigBlindInstruction), (instruction, matches) => { matches.Current.BigBlind = ((BigBlindInstruction)instruction).Value; }},
			{ typeof(OnButtonInstruction), (instruction, matches) => { matches.Current.OnButton = ((OnButtonInstruction)instruction).Name; }},
			{ typeof(TableInstruction), (instruction, matches) => { matches.Current.Table = ((TableInstruction)instruction).Value; }},
			
			{ typeof(HandInstruction), (instruction, matches) =>{ matches.Current.SetHand((HandInstruction)instruction); }},
			{ typeof(StackInstruction), (instruction, matches) =>{ matches.Current.Stack((StackInstruction)instruction); }},
			{ typeof(PostInstruction), (instruction, matches) =>{ matches.Current.Post((PostInstruction)instruction); }},
			{ typeof(ActionInstruction), (instruction, matches) =>{ matches.Current.Act((ActionInstruction)instruction); }},
		};
	}
}
