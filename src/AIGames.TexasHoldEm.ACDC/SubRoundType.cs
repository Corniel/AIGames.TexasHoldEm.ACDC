namespace AIGames.TexasHoldEm.ACDC
{
	public enum SubRoundType
	{
		/// <summary>Pre flop: No table cards revealed.</summary>
		Pre = 0,
		/// <summary>Flop: Three table cards revealed.</summary>
		Flop = 3,
		/// <summary>Turn: Four table cards revealed.</summary>
		Turn = 4,
		/// <summary>River: All five table cards revealed.</summary>
		River = 5,
	}

	public static class SubRoundTypes
	{
		public static readonly SubRoundType[] All = new SubRoundType[] { SubRoundType.Pre, SubRoundType.Flop, SubRoundType.Turn, SubRoundType.River };
	}
}
