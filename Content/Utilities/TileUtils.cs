using System;
using System.Collections.Generic;
using System.Linq;
/*using CalamityMod.Tiles;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.AstralDesert;
using CalamityMod.Tiles.AstralSnow;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.FurnitureAbyss;
using CalamityMod.Tiles.FurnitureAshen;
using CalamityMod.Tiles.FurnitureEutrophic;
using CalamityMod.Tiles.FurnitureOtherworldly;
using CalamityMod.Tiles.FurnitureProfaned;
using CalamityMod.Tiles.FurnitureVoid;
using CalamityMod.Tiles.Ores;
using CalamityMod.Tiles.SunkenSea;*/
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using static Terraria.ModLoader.ModContent;

namespace LukaiAddons
{
	public static partial class LukaiUtils
	{
		public static int GetRequiredPickPower(this Tile tile, int i, int j)
		{
			return 0;
			if (!ModLoader.TryGetMod("CalamityMod", out Mod cmod))
				return 0;

			int pickReq = 0;

			if (Main.tileNoFail[tile.TileType])
				return pickReq;

			ModTile moddedTile = TileLoader.GetTile(tile.TileType);

			//Getting the pickaxe requirement of a modded tile is shrimple.
			if (moddedTile != null)
			pickReq = moddedTile.MinPick;

			//Getting the pickaxe requirement of a vanilla tile is quite clamplicated
			//This was lifted from code in onyx excavator, which likely was lifted from vanilla. It might need 1.4 updating.
			else
			{
			switch (tile.TileType)
			{
			case TileID.Chlorophyte:
				pickReq = 200;
				break;
			case TileID.Ebonstone:
			case TileID.Crimstone:
			case TileID.Pearlstone:
			case TileID.Hellstone:
				pickReq = 65;
				break;
			case TileID.Obsidian:
				pickReq = 55;
				break;
			case TileID.Meteorite:
				pickReq = 50;
				break;
			case TileID.Demonite:
			case TileID.Crimtane:
				if (j > Main.worldSurface)
					pickReq = 55;
				break;
			case TileID.LihzahrdBrick:
			case TileID.LihzahrdAltar:
				pickReq = 210;
				break;
			case TileID.Cobalt:
			case TileID.Palladium:
				pickReq = 100;
				break;
			case TileID.Mythril:
			case TileID.Orichalcum:
				pickReq = 110;
				break;
			case TileID.Adamantite:
			case TileID.Titanium:
				pickReq = 150;
				break;
			default:
				break;
			}
			}

			if (Main.tileDungeon[tile.TileType] && j > Main.worldSurface)
				pickReq = 100;

			return pickReq;
		}
	}
}
