using System;
using System.Diagnostics.CodeAnalysis;

namespace AIGames.TexasHoldEm.ACDC
{
	public class Program
	{
		[ExcludeFromCodeCoverage]
		public static void Main(string[] args)
		{
			Communication.ConsolePlatform.Run(new ACDCBot());
		}
	}
}
