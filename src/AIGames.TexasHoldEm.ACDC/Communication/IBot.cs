using System;

namespace AIGames.TexasHoldEm.ACDC.Communication
{
	public interface IBot
	{
		void ApplySettings(Settings settings);
		void Update(Matches matches);
		BotResponse GetResponse(TimeSpan time);
	}
}
