using AIGames.TexasHoldEm.ACDC.Analysis;
using NUnit.Framework;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Linq;
using AIGames.TexasHoldEm.ACDC.UnitTests.Analysis;

namespace AIGames.TexasHoldEm.ACDC.UnitTests.Deployoment
{
	[TestFixture, Category(Category.Deployment)]
	public class DeployerTest
	{
		[Test]
		public void Deploy_Bot_CompileAndZip()
		{
			var version = typeof(ACDCBot).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
			var nr = int.Parse(version.Split('.')[0]);
			Deployer.Run(Deployer.GetCollectDir(), "ACDC", nr.ToString("0000"), false);
		}
		[Test]
		public void WriteBase64Data()
		{
			var file = new FileInfo(ConfigurationManager.AppSettings["Data.File"]);

			byte[] bytes = null;
			using (var stream = new MemoryStream())
			{
				file.OpenRead().CopyTo(stream);
				bytes = stream.ToArray();
			}

			var csFile = new FileInfo(Path.Combine(Deployer.GetCollectDir().FullName, "Analysis/Nodes.Data.cs"));
			using (var writer = new StreamWriter(csFile.FullName))
			{
				writer.WriteLine("namespace AIGames.TexasHoldEm.ACDC.Analysis");
				writer.WriteLine("{");
				writer.WriteLine("\tpublic static partial class Nodes");
				writer.WriteLine("\t{");
				writer.Write("\t\tpublic static string GetData() { return ");
				writer.Write('"');
				writer.Write(Convert.ToBase64String(bytes));
				writer.Write('"');
				writer.WriteLine(";}");
				writer.WriteLine("\t}");
				writer.WriteLine("}");
			}
		}

		[Test]
		public void AddPreFlopCallsAndChecks()
		{
			var file = new FileInfo(ConfigurationManager.AppSettings["Data.File"]);
			var nodes = Nodes.Load(file);

			nodes = nodes.Where(r => r.ByteOdds > 0).ToList();

			foreach (var odds in NodeStats.GetPreFlopOdds())
			{
				for (short call = 10; call < 100; call += 10)
				{
					var round = (byte)(call / 10 + 1);
					var pot = (short)(3 * call);
					var profitCall = odds * pot - (1 - odds) * call;
					var profitCheck = odds * pot;

					var nodeCall = new Node()
					{
						Odds = odds,
						Action = GameAction.Call,
						AmountToCall = call,
						SubRound = SubRoundType.Pre,
						Step = 1,
						Round = round,
						Pot = pot,
						Profit = (short)profitCall,
					};
					var nodeCheck = new Node()
					{
						Odds = odds,
						Action = GameAction.Check,
						SubRound = SubRoundType.Pre,
						Step = 1,
						Round = round,
						Pot = (short)(call * 4),
						Profit = (short)profitCall,
					};
					nodes.Add(nodeCall);
					nodes.Add(nodeCheck);
				}
			}
			nodes.Save(file);
		}
	}
}
