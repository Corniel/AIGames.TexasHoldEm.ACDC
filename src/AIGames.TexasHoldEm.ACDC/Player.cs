using AIGames.TexasHoldEm.ACDC.Communication;
using System;

namespace AIGames.TexasHoldEm.ACDC
{
	public static class Player
	{
		public static PlayerName TryParse(string str)
		{
			PlayerName player;

			if (Enum.TryParse<PlayerName>(str, true, out player))
			{
				return player;
			}
			return PlayerName.None;
		}

		public static PlayerName Other(this PlayerName my)
		{
			switch (my)
			{
				case PlayerName.player1: return PlayerName.player2;
				case PlayerName.player2: return PlayerName.player1;
			}
			return PlayerName.None;
		}
	}
}
