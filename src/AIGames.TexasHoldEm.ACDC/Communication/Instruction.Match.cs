namespace AIGames.TexasHoldEm.ACDC.Communication
{
	public static class MatchInstruction
	{
		internal static IInstruction Parse(string[] splitted)
		{
			if (splitted.Length != 3) { return null; }

			switch (splitted[1].ToUpperInvariant())
			{
				case "ROUND": return RoundInstruction.Parse(splitted);
				case "SMALL_BLIND": return SmallBlindInstruction.Parse(splitted);
				case "BIG_BLIND": return BigBlindInstruction.Parse(splitted);
				case "ON_BUTTON": return OnButtonInstruction.Parse(splitted);
				case "TABLE": return TableInstruction.Parse(splitted);
			}
			return null;
		}
	}
	
}
