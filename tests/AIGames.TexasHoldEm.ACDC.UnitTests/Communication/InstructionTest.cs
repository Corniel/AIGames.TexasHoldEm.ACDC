using AIGames.TexasHoldEm.ACDC.Communication;
using NUnit.Framework;
using System;
using System.Linq;

namespace AIGames.TexasHoldEm.ACDC.UnitTests.Communication
{
	[TestFixture]
	public class InstructionTest
	{
		[Test]
		public void Parse_All()
		{
			var reader = ConsolePlatformTester.LoadInput("input.simple.txt");
			var actual = Instruction.Read(reader).ToArray();

			var expected = new IInstruction[]
			{
				// Settings
				new HandsPerLevelInstruction(10),
				new StartingStackInstruction(2000),
				new YourBotInstruction(PlayerName.player1),
				new TimeBankInstruction(TimeSpan.FromSeconds(10)),
				new TimePerMoveInstruction(TimeSpan.FromMilliseconds(500)),
				// Match
				new RoundInstruction(1),
				new SmallBlindInstruction(10),
				new BigBlindInstruction(20),
				// Player
				new OnButtonInstruction(PlayerName.player2),
				new StackInstruction(PlayerName.player1, 2000),
				new StackInstruction(PlayerName.player2, 2000),
				new PostInstruction(PlayerName.player2, 10),
				new PostInstruction(PlayerName.player1, 20),
				new HandInstruction(PlayerName.player1, Cards.Parse("[9c,7h]")),
				new ActionInstruction(PlayerName.player2, GameAction.Call),
			};

			CollectionAssert.AreEqual(expected, actual.Take(expected.Length).ToArray());
			Assert.AreEqual(0, actual.Length);
		}

		#region Action

		[Test]
		public void Parse_RequestMoveInstruction_12345ms()
		{
			var act = Instruction.Parse("Action player1 12345");
			var exp = new RequestMoveInstruction(PlayerName.player1, TimeSpan.FromMilliseconds(12345));

			Assert.AreEqual(exp, act);
		}

		#endregion

		#region Settings

		[Test]
		public void Parse_YourBotInstruction_Player2()
		{
			var act = Instruction.Parse("Settings your_bot player2");
			var exp = new YourBotInstruction(PlayerName.player2);

			Assert.AreEqual(exp, act);
		}

		#endregion
	}
}
