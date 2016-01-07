using System;
using System.Collections.Generic;

namespace AIGames.TexasHoldEm.ACDC.Communication
{
	public class Settings
	{
		public Settings()
		{
			HandsPerLevel = 10;
			StartingPost = 2000;
			TimeBank = TimeSpan.FromSeconds(10);
			TimePerMove = TimeSpan.FromMilliseconds(500);
		}
		public int HandsPerLevel { get; set; }
		public int StartingPost { get; set; }

		public TimeSpan TimeBank { get; set; }
		public TimeSpan TimePerMove { get; set; }

		public PlayerName YourBot { get; set; }
		public PlayerName OppoBot
		{
			get
			{
				switch (YourBot)
				{
					case PlayerName.player1: return PlayerName.player2;
					case PlayerName.player2: return PlayerName.player1;
					case PlayerName.None:
					default: return PlayerName.None;
				}
			}
		}

		public bool Apply(IInstruction instruction)
		{
			if (Mapping.ContainsKey(instruction.GetType()))
			{
				Mapping[instruction.GetType()].Invoke(instruction, this);
				return true;
			}
			return false;
		}

		private static Dictionary<Type, Action<IInstruction, Settings>> Mapping = new Dictionary<Type, Action<IInstruction, Settings>>()
		{
			{ typeof(HandsPerLevelInstruction), (instruction, settings) =>{ settings.HandsPerLevel = ((HandsPerLevelInstruction)instruction).Hands; }},
			{ typeof(StartingStackInstruction), (instruction, settings) =>{ settings.StartingPost = ((StartingStackInstruction)instruction).Stack; }},
			{ typeof(YourBotInstruction), (instruction, settings) =>{ settings.YourBot = ((YourBotInstruction)instruction).Name; }},
			{ typeof(TimeBankInstruction), (instruction, settings) =>{ settings.TimeBank = ((TimeBankInstruction)instruction).Duration; }},
			{ typeof(TimePerMoveInstruction), (instruction, settings) =>{ settings.TimePerMove = ((TimePerMoveInstruction)instruction).Duration; }},
		};
	}
}
