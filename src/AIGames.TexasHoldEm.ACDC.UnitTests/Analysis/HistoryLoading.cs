using AIGames.TexasHoldEm.ACDC.Analysis;
using AIGames.TexasHoldEm.ACDC.UnitTests.Communication;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace AIGames.TexasHoldEm.ACDC.UnitTests.Analysis
{
	[TestFixture, Category(Category.IntegrationTest)]
	public class HistoryLoading
	{
		[Test]
		public void LoadAndReplay()
		{
			var dir = new DirectoryInfo(@"C:\Code\AIGames.Challenger\games\texas-hold-em");

			var bot = new ACDCBot();
			bot.Actor.Records.Clear();

			foreach (var file in dir.GetFiles("*.log"))
			{
				using (var platform = new ConsolePlatformTester(file))
				{
					platform.DoRun(bot);
				}
			}
			var records = new List<Record>(bot.Actor.Records);
			records.Sort();
			records.Save(new FileInfo(@"C:\temp\data.bin"));
			using (var writer = new StreamWriter(@"C:\temp\data.log"))
			{
				foreach (var record in records)
				{
					writer.WriteLine(record);
				}
			}
		}
	}
}
