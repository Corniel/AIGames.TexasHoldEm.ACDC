using AIGames.TexasHoldEm.ACDC.Analysis;
using AIGames.TexasHoldEm.ACDC.Communication;
using System;
using System.Collections.Generic;
using System.Linq;
using Troschuetz.Random.Generators;

namespace AIGames.TexasHoldEm.ACDC.Simulator
{
	public class Simulator
	{
		public void Simulate(NodeCollection nodes, MT19937Generator rnd)
		{
			var bots = new Dictionary<PlayerName, ACDCBot>()
			{
				{ PlayerName.player1, GetBot(nodes, PlayerName.player1, rnd) },
				{ PlayerName.player2, GetBot(nodes, PlayerName.player2, rnd) },
			};

			var matches = new Matches();
			matches.Round = 1;
			var stack1 = 2000;
			var stack2 = 2000;

			var blind = 20;
			
			for (var round = 1; round < int.MaxValue; round++)
			{
				if (round % 10 == 0) { blind += 10; }
				var small = blind >> 1;

				matches.Round = round;
				matches.Current.Player1.Stack = stack1;
				matches.Current.Player2.Stack = stack2;

				var mirrored = (round & 1) == 0;

				var shuffled = Cards.GetShuffledDeck(rnd);
				var deck1 = Cards.Empty;
				var deck2 = Cards.Empty;
				var tabl0 = Cards.Empty;
				var tabl3 = Cards.Empty;
				var tabl4 = Cards.Empty;
				var tabl5 = Cards.Empty;

				deck1.AddRange(shuffled.Take(2));
				deck2.AddRange(shuffled.Skip(2).Take(2));
				tabl3.AddRange(shuffled.Skip(4).Take(3));
				tabl4.AddRange(shuffled.Skip(4).Take(4));
				tabl5.AddRange(shuffled.Skip(4).Take(5));

				var tables = new Cards[] { tabl0, tabl3, tabl4, tabl5 };

				matches.Current.BigBlind = blind;
				matches.Current.SmallBlind = small;
				matches.Current.Player1.Hand = deck1;
				matches.Current.Player2.Hand = deck2;

				SetPot(matches.Current, mirrored);

				var outcome = PlayerName.None;

				foreach (var table in tables)
				{
					matches.Current.Table = table;
					outcome = SimulateRound(matches, bots);
					if (outcome != PlayerName.None) { break; }
				}
				if (outcome == PlayerName.None)
				{
					outcome = PokerHandEvaluator.GetOutcome(deck1, deck2, tabl5);
				}
				if (outcome == PlayerName.None)
				{
					matches.Current.Player1.Stack += matches.Current.Pot >> 1;
					matches.Current.Player2.Stack += matches.Current.Pot >> 1;
				}
				else
				{
					matches.Current[outcome].Stack += matches.Current.Pot;
					var wins = new WinsInstruction(outcome, matches.Current.Pot);
					bots[PlayerName.player1].Update(wins);
					bots[PlayerName.player2].Update(wins);
				}
				matches.Current.Player1.Pot = 0;
				matches.Current.Player2.Pot = 0;

				stack1 = matches.Current.Player1.Stack;
				stack2 = matches.Current.Player2.Stack;

				if (stack1 < blind || stack2 < blind)
				{
					break;
				}
			}
		}

		private void SetPot(Match match, bool mirrored)
		{
			match.OnButton = mirrored ? PlayerName.player2 : PlayerName.player1;
			var p1 = mirrored ? match.Player2 : match.Player1;
			var p2 = mirrored ? match.Player1 : match.Player2;

			p2.Pot = match.SmallBlind;
			p2.Stack -= match.SmallBlind;

			p1.Pot = match.BigBlind;
			p1.Stack -= match.BigBlind;
		}

		private static ACDCBot GetBot(NodeCollection nodes, PlayerName name, MT19937Generator rnd)
		{
			var bot = new ACDCBot(nodes, rnd)
			{
				Settings = new Settings()
				{
					YourBot = name,
				},
			};
			return bot;
		}

		private PlayerName SimulateRound(Matches matches, Dictionary<PlayerName, ACDCBot> bots)
		{
			var action = GameAction.Invalid;

			var p1 = bots[matches.Current.OnButton.Other()];
			var p2 = bots[matches.Current.OnButton];

			var s = 0;

			while (true)
			{
				s++;
				p1.Update(matches);
				action = p1.GetResponse(TimeSpan.Zero).Action;
				if (action == GameAction.Fold) { return matches.Current.OnButton; }
				if (action == GameAction.Check) { if (s > 1) { return PlayerName.None; } }
				else
				{
					var toCall = matches.Current.Off.GetAmountToCall(matches.Current.On);
					matches.Current.Off.Call(toCall);
					matches.Current.Off.Act(action);
				}

				s++;
				p2.Update(matches);
				action = p2.GetResponse(TimeSpan.Zero).Action;
				if (action == GameAction.Fold) { return matches.Current.OnButton.Other(); }
				if (action == GameAction.Check) { return PlayerName.None; }

				else
				{
					var toCall = matches.Current.On.GetAmountToCall(matches.Current.Off);
					matches.Current.On.Call(toCall);
					matches.Current.On.Act(action);
				}
			}
		}
	}
}
