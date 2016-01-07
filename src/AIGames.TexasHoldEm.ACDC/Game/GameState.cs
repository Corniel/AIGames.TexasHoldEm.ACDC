using AIGames.TexasHoldEm.ACDC.Communication;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Troschuetz.Random.Generators;

namespace AIGames.TexasHoldEm.ACDC.Game
{
	/// <summary>Represents the game state.</summary>
	[Serializable, DebuggerDisplay("{DebuggerDisplay}")]
	public class GameState
	{
		/// <summary>Constructs a new game state.</summary>
		public GameState()
		{
			this.Player1 = new PlayerState();
			this.Player2 = new PlayerState();
			this.HandsPerLevel = 10;
			this.SmallBlind = 10;
		}

		/// <summary>Constructs a new game state based on settings..</summary>
		public GameState(Settings settings): this()
		{
			Update(settings);
		}
		/// <summary>The player of this bot.</summary>
		public PlayerType YourBot { get; set; }

		/// <summary>The number of the currently played hand, counting starts at 1.</summary>
		public int Round { get; set; }

		/// <summary>The number of hands per (blind) level.</summary>
		public int HandsPerLevel { get; set; }

		/// <summary>The card that are on the table.</summary>
		/// <remarks>
		/// This can be 0, 3, 4, or 5 cards.
		/// </remarks>
		public Cards Table { get; set; }

		/// <summary>The name of the bot that currently has the dealer button.</summary>
		public PlayerType OnButton { get; set; }

		/// <summary>The current size of the small blind.</summary>
		public int SmallBlind { get; set; }

		/// <summary>The current size of the big blind.</summary>
		public int BigBlind { get { return SmallBlind * 2; } }

		/// <summary>Get player based on the player type.</summary>
		public PlayerState this[PlayerType tp]
		{
			get
			{
				switch (tp)
				{
					case PlayerType.player1: return this.Player1;
					case PlayerType.player2:
					default: return this.Player2;
				}
			}
		}

		/// <summary>Gets own player.</summary>
		public PlayerState Own { get { return this[this.YourBot]; } }
		/// <summary>Gets other player.</summary>
		public PlayerState Opp { get { return this[this.YourBot.Other()]; } }

		/// <summary>Gets the player with the button (small blind).</summary>
		public PlayerState Button { get { return this[this.OnButton]; } }
		/// <summary>Gets the player with the big blind.</summary>
		public PlayerState Blind { get { return this[this.OnButton.Other()]; } }

		/// <summary>Represents player1 specific data.</summary>
		public PlayerState Player1 { get; set; }
		/// <summary>Represents player2 specific data.</summary>
		public PlayerState Player2 { get; set; }

		/// <summary>Gets the pot.</summary>
		public int Pot { get { return this.Player1.Pot + this.Player2.Pot; } }

		/// <summary>Gets the total of chips of the game.</summary>
		public int Chips { get { return this.Player1.Chips + this.Player2.Chips; } }

		/// <summary>The result of the round.</summary>
		public RoundResult Result { get; set; }

		/// <summary>The result of the match.</summary>
		public RoundResult FinalResult { get; set; }

		/// <summary>Represents the sub round (equals the size of the table).</summary>
		public int SubRound { get { return this.Table == null ? 0 : this.Table.Count; } }

		/// <summary>Total amount of chips currently in the pot (+ side pot).</summary>
		public int MaxWinPot { get; set; }

		/// <summary>Get the amount of chips the player has to put in to call.</summary>
		public int GetMaxWinPot()
		{
			return Player2.Pot + Player1.Pot;
		}

		/// <summary>The amount of chips your bot has to put in to call.</summary>
		public int AmountToCall { get; set; }

		/// <summary>Returns true if ther is no amount to call, otherwise false.</summary>
		public bool NoAmountToCall { get { return AmountToCall == 0; } }

		/// <summary>Get the amount of chips the player has to put in to call.</summary>
		public int GetAmountToCall(PlayerType player)
		{
			var amount = player == PlayerType.player1 ? Player2.Pot - Player1.Pot : Player1.Pot - Player2.Pot;

#if DEBUG
			if (amount < 0)
			{
				throw new InvalidStateException(string.Format("Amount to call should not be negative: {0}. Player1: {1}, Player2: {2}", amount, Player1.Pot, Player2.Pot));
			}
			return amount;
#else
				return amount < 0 ? 0 : amount;
#endif
		}

		/// <summary>Test the minimum amount to raise.</summary>
		public int MinimumRaise
		{
			get
			{
				var minStack = Math.Min(Own.Stack - AmountToCall, Opp.Stack);
				return minStack < BigBlind ? 0 : BigBlind;
			}
		}
		/// <summary>Test the maximum amount to raise.</summary>
		public int MaxinumRaise
		{
			get
			{
				// Don't raise on small blind.
				if (AmountToCall == SmallBlind) { return 0; }
				var minStack = Math.Min(Own.Stack - AmountToCall, Opp.Stack);
				if (minStack < BigBlind) { return 0; };
				return Math.Min(minStack, MaxWinPot + AmountToCall);
			}
		}

		/// <summary>Returns true if there is an option to raise, otherwise false.</summary>
		public bool CanRaise { get { return MaxinumRaise > 0; } }

		/// <summary>Returns true if pre flop (empty table), otherwise false.</summary>
		public bool IsPreFlop { get { return this.Table == null || this.Table.Count == 0; } }

		/// <summary>Updates the state based on the settings.</summary>
		public void Update(Settings settings)
		{
			this.YourBot = settings.YourBot;
			this.HandsPerLevel = settings.HandsPerLevel;
			this.SmallBlind = settings.SmallBlind;
			this.Player1.Update(settings);
			this.Player2.Update(settings);
		}

		/// <summary>Updates a player based on the game action.</summary>
		public void Update(PlayerType player, GameAction action)
		{
			if (action == GameAction.Call)
			{
				this[player].Call(AmountToCall);
			}
			else if (action.ActionType == GameActionType.raise)
			{
				this[player].Raise(action.Amount, AmountToCall);
			}
		}

		/// <summary>Updates the state based on the instruction.</summary>
		public void Update(Instruction instruction)
		{
			switch (instruction.InstructionType)
			{
				case InstructionType.Action: UpdateAction(instruction); break;
				case InstructionType.Match: UpdateMatch(instruction); break;
				case InstructionType.Player: UpdatePlayer(instruction); break;
				case InstructionType.Output: UpdateOutput(instruction); break;
				case InstructionType.Settings: break;
				case InstructionType.None:
				default: break;
			}
		}

		/// <summary>Updates the state based on an action instruction.</summary>
		private void UpdateAction(Instruction instruction)
		{
			switch (instruction.Action)
			{
				case "player1": this.Player1.TimeBank = TimeSpan.FromMilliseconds(instruction.Int32Value); break;
				case "player2": this.Player2.TimeBank = TimeSpan.FromMilliseconds(instruction.Int32Value); break;
			}
		}
		/// <summary>Updates the state based on a match instruction.</summary>
		private void UpdateMatch(Instruction instruction)
		{
			switch (instruction.Action)
			{
				// ignore, should be two times the small blind.
				case "bigBlind":
#if DEBUG
					if (instruction.Int32Value != this.SmallBlind * 2) { throw new InvalidStateException("Big blind is not two times the small blind."); }
#endif
					break;

				case "round": UpdateRound(instruction); break;
				case "smallBlind": this.SmallBlind = instruction.Int32Value; break;
				case "maxWinPot": this.MaxWinPot = instruction.Int32Value; break;
				case "amountToCall": this.AmountToCall = instruction.Int32Value; break;
				case "onButton": this.OnButton = instruction.PlayerTypeValue; break;
				case "table": this.Table = instruction.CardsValue;break;
			}
		}
		/// <summary>Updates the state based on a player instruction.</summary>
		private void UpdatePlayer(Instruction instruction)
		{
			var pt = instruction.Player;
			var player = this[pt];
			
			switch (instruction.Action)
			{
				// Nothing need to be done.
				case "fold":
				// Nothing need to be done.
				case "check": break;
				case "hand": player.Hand = instruction.CardsValue; break;
				case "call": player.Call(AmountToCall); break;
				case "post": player.Post(instruction.Player == this.OnButton ? this.SmallBlind : this.BigBlind); break;
				case "raise": player.Raise(instruction.Int32Value, AmountToCall); break;
				case "stack": player.SetStack(instruction.Int32Value); break;
				case "wins":
					if (this.Result == RoundResult.NoResult)
					{
						this.Result = pt == PlayerType.player1 ? RoundResult.Player1Wins : RoundResult.Player2Wins;
					}
					// if set, and another results follows, its a draw.
					else
					{
						this.Result = RoundResult.Draw;
					}
					break;
			}
		}
		/// <summary>Updates the state based on a output instruction.</summary>
		private void UpdateOutput(Instruction instruction)
		{
			switch (instruction.FinalResult)
			{
				case RoundResult.Player1Wins:
					this.Result = RoundResult.Player1Wins;
					this.FinalResult = RoundResult.Player1Wins;
					this.Player1.Stack = this.Chips;
					this.Player2.Stack = 0;
					break;

				case RoundResult.Player2Wins:
					this.Result = RoundResult.Player2Wins;
					this.FinalResult = RoundResult.Player2Wins;
					this.Player2.Stack = this.Chips;
					this.Player1.Stack = 0;
					break;

				case RoundResult.NoResult:
				case RoundResult.Draw:
				default: break;
			}
		}

		private void UpdateRound(Instruction instruction)
		{
#if DEBUG
			if (instruction.Int32Value != this.Round + 1) { throw new InvalidStateException("The round is not updated properly."); }
#endif
			this.Round = instruction.Int32Value;
			Reset();
		}

		/// <summary>Starts a new round.</summary>
		/// <param name="rnd">
		/// The randomizer to shuffle the deck.
		/// </param>
		public bool StartNewRound(MT19937Generator rnd)
		{
			this.Round++;
			var deck = Cards.GetShuffledDeck(rnd);
			this.Player1.Hand = Cards.Create(deck.Take(4));
			this.Player2.Hand = Cards.Create(deck.Skip(4).Take(4));
			this.Table = Cards.Create(deck.Skip(8).Take(5));
			this.OnButton = this.OnButton.Other();

			if (this.Round > 1 && this.Round % HandsPerLevel == 1)
			{
				this.SmallBlind += 5;
			}

			if (Player1.Stack < BigBlind || Player2.Stack < BigBlind) { return false; }

			Button.Post(this.SmallBlind);
			Blind.Post(this.BigBlind);

			return true;
		}

		/// <summary>Apply the round result.</summary>
		public int ApplyRoundResult(RoundResult result)
		{
			int compare = 0;
			if (result == RoundResult.NoResult)
			{
				var pokerhand1 = PokerHand.CreateFromHeadsUpOmaha(this.Table, this.Player1.Hand);
				var pokerhand2 = PokerHand.CreateFromHeadsUpOmaha(this.Table, this.Player2.Hand);

				compare = PokerHandComparer.Instance.Compare(pokerhand1, pokerhand2);
			}
			else
			{
				switch (result)
				{
					case RoundResult.Player1Wins: compare = 1; break;
					case RoundResult.Player2Wins: compare = -1; break;
				}
			}
			int pot = this.Pot;

			if (compare == 0)
			{
				 pot /= 2;
				this.Player1.Win(pot);
				this.Player2.Win(pot);
			}
			else if (compare > 0)
			{
				this.Player1.Win(pot);
				this.Player2.Win(0);
			}
			else
			{
				this.Player2.Win(pot);
				this.Player1.Win(0);
			}
			return pot;
		}

		/// <summary>Sets the final result.</summary>
		public bool SetFinalResult(RoundResult roundResult)
		{
			switch (roundResult)
			{
				case RoundResult.Player1Wins:
					this.Result = RoundResult.Player1Wins;
					this.FinalResult = RoundResult.Player1Wins;
					this.Player1.Stack = this.Chips;
					this.Player2.Stack = 0;
					return true;

				case RoundResult.Player2Wins:
					this.Result = RoundResult.Player2Wins;
					this.FinalResult = RoundResult.Player2Wins;
					this.Player2.Stack = this.Chips;
					this.Player1.Stack = 0;
					return true;

				case RoundResult.NoResult:
				case RoundResult.Draw:
				default: return false;
			}
		}

		/// <summary>Copies the game state.</summary>
		/// <remarks>
		/// Uses the XML serialization.
		/// </remarks>
		public GameState Copy()
		{
			using (var stream = new MemoryStream())
			{
				Save(stream);
				stream.Position = 0;
				return Load(stream);
			}
		}

		/// <summary>Makes a full copy of the settings for a specified player.</summary>
		public GameState Personalize(PlayerType player)
		{
			this.YourBot = player;
			return this;
		}

		/// <summary>Resets the state.</summary>
		/// <remarks>
		/// Tables, hands an result are being reset.
		/// </remarks>
		public void Reset()
		{
			this.Table = Cards.Empty;
			this.Player1.Reset();
			this.Player2.Reset();
			this.Result = RoundResult.NoResult;
		}

		#region I/O operations

		/// <summary>Saves the game to a file.</summary>
		/// <param name="fileName">
		/// The name of the file.
		/// </param>
		/// <param name="mode">
		/// The file mode.
		/// </param>
		public void Save(string fileName, FileMode mode = FileMode.Create) { Save(new FileInfo(fileName), mode); }

		/// <summary>Saves the game to a file.</summary>
		/// <param name="file">
		/// The file to save to.
		/// </param>
		/// <param name="mode">
		/// The file mode.
		/// </param>
		public void Save(FileInfo file, FileMode mode = FileMode.Create)
		{
			using (var stream = new FileStream(file.FullName, mode, FileAccess.Write))
			{
				Save(stream);
			}
		}

		/// <summary>Saves the game to a stream.</summary>
		/// <param name="stream">
		/// The stream to save to.
		/// </param>
		public void Save(Stream stream)
		{
			var serializer = new XmlSerializer(typeof(GameState));
			serializer.Serialize(stream, this);
		}

		/// <summary>Loads the game from a file.</summary>
		/// <param name="fileName">
		/// The name of the file.
		/// </param>
		public static GameState Load(string fileName) { return Load(new FileInfo(fileName)); }

		/// <summary>Loads the game from a file.</summary>
		/// <param name="file">
		/// The file to load from.
		/// </param>
		public static GameState Load(FileInfo file)
		{
			using (var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
			{
				return Load(stream);
			}
		}

		/// <summary>Loads the game from a stream.</summary>
		/// <param name="stream">
		/// The stream to load from.
		/// </param>
		public static GameState Load(Stream stream)
		{
			var serializer = new XmlSerializer(typeof(GameState));
			return (GameState)serializer.Deserialize(stream);
		}

		#endregion

		[ExcludeFromCodeCoverage, DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string DebuggerDisplay
		{
			get
			{
				var sb = new StringBuilder();
				sb.AppendFormat("State[{0}.{1}], P1: {2}, P2: {3} ({4}+{5}={6})",
					Round, SubRound,
					Player1.Stack, Player2.Stack,
					Player1.Pot, Player2.Pot, Pot);


				return sb.ToString();
			}
		}
	}
}
