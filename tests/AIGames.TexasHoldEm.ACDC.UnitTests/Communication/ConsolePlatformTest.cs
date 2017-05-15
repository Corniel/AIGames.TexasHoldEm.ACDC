using NUnit.Framework;

namespace AIGames.TexasHoldEm.ACDC.UnitTests.Communication
{
	[TestFixture, Category(Category.IntegrationTest)]
	public class ConsolePlatformTest
	{
		[Test]
		public void DoRun_Simple_NoExceptions()
		{
			using(var platform = new ConsolePlatformTester("input.simple.txt"))
			{
				platform.DoRun(new ACDCBot());
			}
		}
	}
}
