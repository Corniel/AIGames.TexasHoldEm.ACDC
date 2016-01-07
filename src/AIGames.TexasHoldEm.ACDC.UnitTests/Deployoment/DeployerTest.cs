using NUnit.Framework;
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
			var collectDir = new DirectoryInfo(@"C:\Code\AIGames.TexasHoldEm.ACDC\src\AIGames.TexasHoldEm.ACDC");
			var full = collectDir.FullName;
			var version = typeof(ACDCBot).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
			var nr = int.Parse(version.Split('.')[0]);
			Deployer.Run(collectDir, "TheDaltons", nr.ToString("0000"), false);
		}
	}
}
