using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Terraria;
using Terraria.Chat;
using Terraria.Localization;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Shaders;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using ReLogic.Content;
using LukaiAddons.Common.Players;

namespace LukaiAddons
{
	public class LukaiAddons : Mod
	{
		public override void Load()
		{
			if (Main.netMode != NetmodeID.Server)
			{
				Asset<Effect> bloodRealmFilter = this.Assets.Request<Effect>("Content/Effects/ShockwaveEffect");

				/* Terraria's blood moon */
				Filters.Scene["BloodRealmMoon"] = new Filter(new BloodMoonScreenShaderData("FilterBloodMoon").UseColor(2f, -0.8f, -0.6f), EffectPriority.Medium);
				Filters.Scene["BloodRealm"] = new Filter(new ScreenShaderData(bloodRealmFilter, "Shockwave"), EffectPriority.VeryHigh);
			}
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			byte msgType = reader.ReadByte();
			switch (msgType)
			{
				case 0: /* Gamble Chat */
					if (Main.netMode == NetmodeID.Server)
					{
						int toPlayer = reader.ReadInt32();
						int buyPrice = reader.ReadInt32();

						Logger.Info($"[server handle packet] gamble chat");

						ModPacket cfPacket = this.GetPacket();
						cfPacket.Write((byte)0);
						cfPacket.Write(whoAmI);
						cfPacket.Write(buyPrice);
						cfPacket.Send(toPlayer);
					}
					if (Main.netMode == NetmodeID.MultiplayerClient)
					{
						int fromWho = reader.ReadInt32();
						int buyPrice = reader.ReadInt32();

						Player player = Main.player[fromWho];
						Main.NewText($"{player.name} challenged you to a coinflip for {CoinFlipChallenge.FormatBuyPrice(buyPrice)}, type /cfa to accept!");
						Main.LocalPlayer.GetModPlayer<LukaiAddonsPlayer>().coinflipPending = true;
						Main.LocalPlayer.GetModPlayer<LukaiAddonsPlayer>().coinflipAmount = buyPrice;
						Main.LocalPlayer.GetModPlayer<LukaiAddonsPlayer>().coinflipFrom = fromWho;
					}
					break;

				case 1: /* Generic pass through message, send to server int for who to send it to and the string message */
					if (Main.netMode == NetmodeID.Server)
					{
						int toWho = reader.ReadInt32();
						string chatMsg = reader.ReadString();

						ModPacket cfPacket = this.GetPacket();
						cfPacket.Write((byte)1);
						cfPacket.Write(chatMsg);
						cfPacket.Send(toWho);
					}
					if (Main.netMode == NetmodeID.MultiplayerClient)
					{
						string chatMsg = reader.ReadString();
						Main.NewText(chatMsg);
					}
					break;

				case 2: /* Player purchase packet for weird sync issues */
					if (Main.netMode == NetmodeID.Server)
					{
						int toWho = reader.ReadInt32();
						int amount = reader.ReadInt32();

						ModPacket pPacket = this.GetPacket();
						pPacket.Write((byte)2);
						pPacket.Write(amount);
						pPacket.Send(toWho);
					}
					if (Main.netMode == NetmodeID.MultiplayerClient)
					{
						Main.NewText($"subtracting money, {Main.LocalPlayer.name} should be challenger");
						Logger.Info($"[multiplayer sub money] {Main.LocalPlayer.name} should be challenger");
					
						int amount = reader.ReadInt32();
						Main.LocalPlayer.BuyItem(amount);
					}
					break;

				default:
					Logger.Warn($"[handle packet] unknown message type {msgType}");
					break;
			}
		}
	}
}
