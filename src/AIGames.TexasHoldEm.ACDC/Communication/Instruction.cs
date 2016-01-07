using System;
using System.Collections.Generic;
using System.IO;

namespace AIGames.TexasHoldEm.ACDC.Communication
{
	public static class Instruction
	{
		public static IInstruction Parse(string line)
		{
			var splited = line.Split(' ');

			switch (splited[0].ToUpperInvariant())
			{
				case "ACTION": return RequestMoveInstruction.Parse(splited);
				case "SETTINGS": return SettingsInstruction.Parse(splited);
				case "MATCH": return MatchInstruction.Parse(splited);
				
				case "PLAYER1":
				case "PLAYER2": 
					return PlayerInstruction.Parse(
						(PlayerName)Enum.Parse(typeof(PlayerName),
						splited[0], 
						true), splited);
			}
			return null;
		}

		/// <summary>Reads the instructions from the reader.</summary>
		public static IEnumerable<IInstruction> Read(TextReader reader)
		{
			if (reader == null) { throw new ArgumentNullException("reader"); }

			string line;
			while ((line = reader.ReadLine()) != null)
			{
				var instruction = Parse(line);
				if (instruction != null)
				{
					yield return instruction;
				}
			}
		}
	}
}
