namespace AIGames.TexasHoldEm.ACDC.Communication
{
	public static class SettingsInstruction
	{
		internal static IInstruction Parse(string[] splitted)
		{
			if (splitted.Length != 3) { return null; }

			switch (splitted[1].ToUpperInvariant())
			{
				case "HANDS_PER_LEVEL": return HandsPerLevelInstruction.Parse(splitted);
				case "STARTING_STACK": return StartingStackInstruction.Parse(splitted);
				case "TIMEBANK": return TimeBankInstruction.Parse(splitted);
				case "TIME_PER_MOVE": return TimePerMoveInstruction.Parse(splitted);
				case "YOUR_BOT": return YourBotInstruction.Parse(splitted);
			}
			return null;
		}
	}
	
}
