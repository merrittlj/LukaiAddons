using System;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using LukaiAddons;

namespace LukaiAddons.Common.Players
{
	public class CoinFlipChallenge : ModCommand
	{
		public override CommandType Type
			=> CommandType.Chat;

		public override string Command
			=> "cf";

		public override string Usage
			=> "/cf \"playerName\" [#p #g #s #c], ex: /cf \"Bob\" 50g 30c for 50 gold 30 copper bet";

		public override string Description
			=> "Challenge a player to a coin flip";

		public static string FormatBuyPrice(int buyPrice)
		{
			string humanTotal = "";
			int iter = 0;
			for (int p = buyPrice; p != 0; p /= 100)
			{
				string humanLevel = "";
				int humanAmount = p % 100;
				if (iter == 0) humanLevel = "copper";
				if (iter == 1) humanLevel = "silver";
				if (iter == 2) humanLevel = "gold";
				if (iter == 3) humanLevel = "platinum";

				if (humanAmount > 0) humanTotal = ($"{humanAmount} {humanLevel}, {humanTotal}");
				++iter;
			}
			humanTotal = humanTotal.Substring(0, humanTotal.Length - 2);
			return humanTotal;
		}

		public int ArgsToPrice(string[] args)
		{
			int buyPrice = 0;
			for (int i = 1; i < args.Length; ++i)
			{
				Regex r = new Regex(args[i], RegexOptions.IgnoreCase);
				Match m = Regex.Match(args[i], @"^(\d+)([a-zA-Z])");
				char coinLevel = m.Groups[2].Value[0];
				int coinAmount = int.Parse(m.Groups[1].Value);

				if (coinLevel == 'p') buyPrice += Item.buyPrice(platinum: coinAmount);
				if (coinLevel == 'g') buyPrice += Item.buyPrice(gold: coinAmount);
				if (coinLevel == 's') buyPrice += Item.buyPrice(silver: coinAmount);
				if (coinLevel == 'c') buyPrice += Item.buyPrice(copper: coinAmount);
			}
			return buyPrice;
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				Main.NewText("[c/FF00000:You must be in multiplayer!]");
			}

			int moneyStart = 1;
			string playerName = "";
			if (!args[0].StartsWith("\"")) playerName = args[0];
			else if (args[0].StartsWith("\"") && args[0].EndsWith("\""))
			{
				playerName = args[0].Substring(1, args[0].Length - 1); 
			} else {
				for (int i = 0; i < args.Length; ++i)
				{
					if (i == 0) playerName = args[i].Substring(1);
					else if (args[i].EndsWith("\""))
					{
						playerName += $" {args[i].Substring(0, args[i].Length - 1)}";
						moneyStart = i;
						break;
					} else { playerName += $" {args[i]}"; }
				}
			}

			if (args.Length < moneyStart || args[args.Length - 1].EndsWith("\""))
			{
				throw new UsageException($"Usage: {Usage}");
				return;
			}
			if (playerName == Main.LocalPlayer.name)
			{
				Main.NewText("[c/FF00000:You cannot challenge yourself!]");
				return;
			}

			bool found = false;
			for (int player = 0; player < Main.CurrentFrameFlags.ActivePlayersCount; ++player)
			{
				if (!found && Main.player[player].active && Main.player[player].name.Equals(playerName))
				{
					found = true;

					int buyPrice = ArgsToPrice(args);

					ModPacket cfPacket = Mod.GetPacket();
					cfPacket.Write((byte)0); /* Identifier */
					cfPacket.Write(player); /* To player */

					cfPacket.Write(buyPrice);
					cfPacket.Send();
					caller.Reply($"[c/55FF55:Sent \"{playerName}\" a coin flip challenge for {FormatBuyPrice(buyPrice)}.]");
				}
				if (!found && player == Main.CurrentFrameFlags.ActivePlayersCount - 1) Main.NewText($"[c/FF0000:Could not find the player \"{playerName}\"!]");
			}
		}
	}

	public class CoinFlipAccept : ModCommand
	{
		public override CommandType Type
			=> CommandType.Chat;

		public override string Command
			=> "cfa";

		public override string Usage
			=> "/cfa";

		public override string Description
			=> "Accept a player's coin flip";

		private static readonly Random rnd = new Random();

		public static void GiveMoney(Player player, int copperAmount)
		{
			int iter = 0;
			for (int p = copperAmount; p != 0; p /= 100)
			{
				int lvlID = -1;
				int amount = p % 100;
				if (iter == 0) lvlID = ItemID.CopperCoin;
				if (iter == 1) lvlID = ItemID.SilverCoin;
				if (iter == 2) lvlID = ItemID.GoldCoin;
				if (iter == 3) lvlID = ItemID.PlatinumCoin;

				player.QuickSpawnItem(null, lvlID, amount);
				++iter;
			}
		}

		public override void Action(CommandCaller caller, string input, string[] args)
		{
			Player player = Main.LocalPlayer;		
			bool pending = player.GetModPlayer<LukaiAddonsPlayer>().coinflipPending;
			int amount = player.GetModPlayer<LukaiAddonsPlayer>().coinflipAmount;
			int fromWho = player.GetModPlayer<LukaiAddonsPlayer>().coinflipFrom;
			player.GetModPlayer<LukaiAddonsPlayer>().coinflipPending = false;

			if (!pending)
			{
				Main.NewText("[c/FF0000:There are not any pending coinflips!]");
				return;
			}

			if (player.BuyItem(amount))
			{
				Player fromPlayer = Main.player[fromWho];
				bool fromHasMoney = fromPlayer.BuyItem(amount); /* Doesn't really remove due to sync? */
				if (fromHasMoney)
				{
					ModPacket pPacket = Mod.GetPacket();
					pPacket.Write((byte)2); /* Player purchase */
					pPacket.Write(fromWho); /* To player */
					pPacket.Write(amount);
					pPacket.Send();
				} else {
					Main.NewText($"[c/FF0000:{fromPlayer.name} does not have enough money!]");
					CoinFlipAccept.GiveMoney(player, amount);

					ModPacket cfPacket = Mod.GetPacket();
					cfPacket.Write((byte)1); /* Generic pass through */
					cfPacket.Write(fromWho); /* To player */
					cfPacket.Write($"[c/FF0000:{player.name} cannot accept your coinflip! You don't have enough money to coinflip!]");
					cfPacket.Send();
					return;
				}

				Main.NewText($"[c/55FF55:Accepted the coinflip from {fromPlayer.name}!]");
				string headTail = (rnd.Next(0, 2) == 0 ? "heads" : "tails");
				string opposite = (headTail.Equals("heads") ? "tails" : "heads");
				Main.NewText($"You choose {headTail}, {fromPlayer.name} chooses {opposite}.");
				string formatPrice = CoinFlipChallenge.FormatBuyPrice(amount);

				string opponentDisplay = $"[c/55FF55:{player.name} accepted your coinflip for {formatPrice}.]\nYou choose {opposite}, {player.name} chooses {headTail}.";
				if (rnd.Next(0, 2) == 0)
				{
					Main.NewText($"[c/55FF55:The coin is {headTail}! You gained {formatPrice} from the coinflip!]");
					CoinFlipAccept.GiveMoney(player, amount * 2);

					ModPacket cfResultPacket = Mod.GetPacket();
					cfResultPacket.Write((byte)1); /* Generic pass through */
					cfResultPacket.Write(fromWho); /* To player */
					cfResultPacket.Write($"{opponentDisplay}\n[c/FF0000:The coin is {headTail}.. You lost {formatPrice} from the coinflip..]");
					cfResultPacket.Send();
				} else {
					Main.NewText($"[c/FF0000:The coin is {opposite}.. You lost {formatPrice} from the coinflip..]");
					CoinFlipAccept.GiveMoney(fromPlayer, amount * 2);

					ModPacket cfResultPacket = Mod.GetPacket();
					cfResultPacket.Write((byte)1); /* Generic pass through */
					cfResultPacket.Write(fromWho); /* To player */
					cfResultPacket.Write($"{opponentDisplay}\n[c/55FF55:The coin is {opposite}! You gained {formatPrice} from the coinflip!]");
					cfResultPacket.Send();
				}
			} else {
				Main.NewText("[c/FF0000:You do not have enough money!]");
				player.GetModPlayer<LukaiAddonsPlayer>().coinflipPending = false;

				ModPacket cfPacket = Mod.GetPacket();
				cfPacket.Write((byte)1); /* Generic pass through */
				cfPacket.Write(fromWho); /* To player */
				cfPacket.Write($"[c/FF0000:{player.name} cannot accept your coinflip! They don't have enough money to coinflip!]");
				cfPacket.Send();

				return;
			}
		}
	}
}
