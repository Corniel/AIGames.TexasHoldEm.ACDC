using AIGames.TexasHoldEm.ACDC.Communication;
using NUnit.Framework;
using System;
using System.IO;

namespace AIGames.TexasHoldEm.ACDC.UnitTests.Communication
{
	public class ConsolePlatformTester : ConsolePlatform
	{
		public ConsolePlatformTester(FileInfo file)
			: base(file.OpenText(), Console.Out, new StringWriter()) { }

		public ConsolePlatformTester(string path)
			: base(LoadInput(path), Console.Out, new StringWriter()) {}

		public override void DoRun(IBot bot)
		{
			base.DoRun(bot);

			var log = Logger.ToString();
			Console.Error.Write(log);
			if (log.Contains("Exception"))
			{
				Assert.Fail("The log output contains at least on exception.");
			}
		}
		
		public static TextReader LoadInput(string path)
		{
			return
				new StreamReader(
					typeof(ConsolePlatformTester)
					.Assembly
					.GetManifestResourceStream("AIGames.TexasHoldEm.ACDC.UnitTests.Communication." + path));
		}
	}
}