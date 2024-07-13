using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using LukaiAddons.Content.Projectiles;

namespace LukaiAddons.Content.Items
{ 
	public class AdventurerDice : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 40;
			Item.height = 40;
			Item.scale = 0.5f;
			Item.useTime = 30;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.knockBack = 50f;
			Item.shoot = ModContent.ProjectileType<Dice>();
			Item.shootSpeed = 8f;
			Item.shopCustomPrice = Item.buyPrice(gold: 3, silver: 38);
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item40; /* Sniper sound */
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (player.BuyItem(Item.buyPrice(gold: 7, silver: 77)))
			{
            	int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: 2f); /* ai1 = 2f represents this dice */
			} else { Main.NewText("Not enough money to roll your [c/D2A0FF:Adventurer's Dice]!"); }
            return false;
        }

	}
}
