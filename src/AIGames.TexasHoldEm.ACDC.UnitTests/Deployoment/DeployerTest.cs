using NUnit.Framework;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;

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
				bytes = stream.GetBuffer();
			}

			var csFile = new FileInfo(Path.Combine(Deployer.GetCollectDir().FullName, "Analysis/Records.Data.cs"));
			using (var writer = new StreamWriter(csFile.OpenWrite()))
			{
				writer.WriteLine("namespace AIGames.TexasHoldEm.ACDC.Analysis");
				writer.WriteLine("{");
				writer.WriteLine("\tpublic static partial class Records");
				writer.WriteLine("\t{");
				writer.Write("\t\tpublic static string GetData() { return ");
				writer.Write('"');
				writer.Write(Convert.ToBase64String(bytes));
				writer.Write('"');
				writer.WriteLine("};");
				writer.WriteLine("\t}");
				writer.WriteLine("}");
			}
		}
	}
}
