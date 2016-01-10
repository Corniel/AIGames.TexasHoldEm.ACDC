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
			bot.Actor.Nodes.Clear();

			foreach (var file in dir.GetFiles("*.log"))
			{
				using (var platform = new ConsolePlatformTester(file))
				{
					platform.DoRun(bot);
				}
			}
			var nodes = new List<Node>(bot.Actor.Nodes);
			nodes.Sort();
			nodes.Save(new FileInfo(@"C:\temp\data.bin"));
			using (var writer = new StreamWriter(@"C:\temp\data.log"))
			{
				foreach (var node in nodes)
				{
					writer.WriteLine(node);
				}
			}
		}
	}
}
