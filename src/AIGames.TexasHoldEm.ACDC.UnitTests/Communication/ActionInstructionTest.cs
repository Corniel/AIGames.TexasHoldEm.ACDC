using AIGames.TexasHoldEm.ACDC.Communication;
using NUnit.Framework;

namespace AIGames.TexasHoldEm.ACDC.UnitTests.Communication
{
	[TestFixture]
	public class ActionInstructionTest
	{
		[Test]
		public void Ctor_Player1Call_Player1Call0()
		{
			var instruction = new ActionInstruction(PlayerName.player1, GameAction.Call);
			var act = instruction.ToString();
			var exp = "player1 call 0";

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Ctor_Player2Check_Playe2Check0()
		{
			var instruction = new ActionInstruction(PlayerName.player2, GameAction.Check);
			var act = instruction.ToString();
			var exp = "player2 check 0";

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Ctor_Player1Fold_Player1Fold0()
		{
			var instruction = new ActionInstruction(PlayerName.player1, GameAction.Fold);
			var act = instruction.ToString();
			var exp = "player1 fold 0";

			Assert.AreEqual(exp, act);
		}
		[Test]
		public void Ctor_Player1Raises123_Player1Raise123()
		{
			var instruction = new ActionInstruction(PlayerName.player1, GameAction.Raise(123));
			var act = instruction.ToString();
			var exp = "player1 raise 123";

			Assert.AreEqual(exp, act);
		}
	}
}
