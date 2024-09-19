using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using LukaiAddons.Content.Dusts;
using LukaiAddons.Content.Buffs;
using LukaiAddons.Common.Players;

namespace LukaiAddons.Content.Items
{ 
	public class BloodWatch : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 40;
			Item.scale = 0.5f;
			Item.useTime = 30;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.noMelee = true;
			Item.value = 0;
			Item.rare = ItemRarityID.Red;
			Item.UseSound = SoundID.Item27;
			
			ItemID.Sets.ShimmerTransformToItem[Type] = Type; /* You can't decraft */
		}

		bool recallReady = false;
		bool used = false;
		float useTim = 0f;
		float prevVolume = 0f;
		Vector2 prevPos;

		private void recall(Player player)
		{
			player.Teleport(prevPos);

			used = false;
			recallReady = false;
			useTim = 0f;
			Filters.Scene["BloodRealmMoon"].Deactivate();
			Filters.Scene["BloodRealm"].Deactivate();
			player.ClearBuff(ModContent.BuffType<TimeBuff>());
			player.GetModPlayer<LukaiAddonsPlayer>().inBloodRealm = false;
			player.GetModPlayer<LukaiAddonsPlayer>().ApplyBloodDamage();
			Main.soundVolume = prevVolume;
			foreach (Dust d in Main.dust)
			{
				if (d.type == ModContent.DustType<TimeDust>()) d.active = false;
			}
			foreach(NPC n in Main.npc)
			{
				n.alpha = 0;
				n.HideStrikeDamage = false;
			}
			foreach(Projectile p in Main.projectile)
			{
				p.alpha = 0;
			}
		}

		public override void UpdateInventory(Player player)
		{
			if (used)
			{
				++useTim;
				Dust.NewDust(player.Center, 1, 1, ModContent.DustType<TimeDust>());
				Dust.NewDust(new Vector2(player.Center.X, player.Center.Y + 1), 1, 1, ModContent.DustType<TimeDust>());
				foreach(NPC n in Main.npc)
				{
					n.alpha = 255;
					n.HideStrikeDamage = true;
				}
				foreach(Projectile p in Main.projectile)
				{
					if (p.owner != Main.myPlayer) p.alpha = 255;
				}
			}
			if (useTim >= 1.5 * 60) recallReady = true;
			if (useTim >= 7 * 60) recall(player);
		}

        public override bool? UseItem(Player player)
        {
			if (recallReady)
			{
				recall(player);
				return true;
			}

			if (used)
			{
				recallReady = true;
				return true;
			}

			if (!used)
			{
				for (int k = 0; k < 200; ++k)
				{
					int dust = Dust.NewDust((player.position * 2 - player.Center + player.velocity), (int)(player.width * 2f), (int)(player.height * 2f), ModContent.DustType<TimeDust>());
					/*Main.dust[dust].scale = 2f;*/
				}
				prevPos = player.position;
				used = true;

				player.AddBuff(ModContent.BuffType<TimeBuff>(), 7 * 60);
				player.AddBuff(ModContent.BuffType<TimeCooldownBuff>(), (int)(1.5 * 60));
				player.GetModPlayer<LukaiAddonsPlayer>().inBloodRealm = true;
				prevVolume = Main.soundVolume;
				//Main.soundVolume = 0f;

				Filters.Scene.Activate("BloodRealmMoon");
				Filters.Scene.Activate("BloodRealm", player.Center).GetShader().UseColor(3, 5, 15).UseTargetPosition(player.Center);;
				return true;
			}
			return true;
        }

		public override void AddRecipes()
		{
			/* Magic Mirror recipe */
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.AddIngredient(ItemID.PlatinumWatch, 1);
			recipe.AddIngredient(ItemID.MagicMirror, 1);
			recipe.AddIngredient(ItemID.Diamond, 3);
			if (ModLoader.TryGetMod("CalamityMod", out Mod cmod) && cmod.TryFind("BloodOrb", out ModItem orb))
				recipe.AddIngredient(orb.Type, 10);
			recipe.Register();

			/* Ice Mirror recipe */
			Recipe recipe2 = CreateRecipe();
			recipe2.AddTile(TileID.MythrilAnvil);
			recipe2.AddIngredient(ItemID.PlatinumWatch, 1);
			recipe2.AddIngredient(ItemID.IceMirror, 1);
			recipe2.AddIngredient(ItemID.Diamond, 3);
			if (ModLoader.TryGetMod("CalamityMod", out Mod cmod2) && cmod2.TryFind("BloodOrb", out ModItem orb2))
				recipe2.AddIngredient(orb2.Type, 10);
			recipe2.Register();
		}
	}
}
