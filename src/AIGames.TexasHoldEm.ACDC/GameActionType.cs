namespace AIGames.TexasHoldEm.ACDC
{
	/// <summary>Represents the type of the game action.</summary>
	public enum GameActionType
	{
		/// <summary>Check.</summary>
		/// <remarks>
		/// This is also the default action on no response.
		/// </remarks>
		check = 0,

		/// <summary>Call.</summary>
		/// <remarks>
		/// Automatically with the correct amount.
		/// </remarks>
		call = 1,

		/// <summary>Raise.</summary>
		/// <remarks>
		/// Raises with the given amount of chips.
		/// </remarks>
		raise = 2,

		/// <summary>Fold.</summary>
		/// <remarks>
		/// This is also the default action on wrong outputs.
		/// </remarks>
		fold = 3,
	}
}
