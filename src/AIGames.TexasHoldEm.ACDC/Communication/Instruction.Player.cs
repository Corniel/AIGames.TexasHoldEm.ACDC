namespace AIGames.TexasHoldEm.ACDC.Communication
{
	public static class PlayerInstruction
	{
		internal static IInstruction Parse(PlayerName name, string[] splitted)
		{
			if (splitted.Length != 3) { return null; }

			switch (splitted[1].ToUpperInvariant())
			{
				case "STACK": return StackInstruction.Parse(name, splitted);
				case "POST": return PostInstruction.Parse(name, splitted);
				case "HAND": return HandInstruction.Parse(name, splitted);
				case "FOLD": 
				case "CALL": 
				case "CHECK":
				case "RAISE": return ActionInstruction.Parse(name, splitted);
			}
			return null;
		}
	}
}
