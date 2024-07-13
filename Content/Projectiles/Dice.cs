using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LukaiAddons.Content.Projectiles
{
	public class Dice : ModProjectile
	{
		int bounces = 3;
		bool rolled = false;
		float rolled_tim = 0f; /* How long until the dice disappears */
		private static readonly Random rnd = new Random();
		bool loaded = false;

		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.friendly = true;
			Projectile.scale = 0.5f;
			Projectile.timeLeft = 600;
			Projectile.ignoreWater = true;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D tex = TextureAssets.Projectile[Projectile.type].Value; /* Default, shouldn't be used */
			if (Projectile.ai[1] == 1)
				tex = ModContent.Request<Texture2D>("LukaiAddons/Content/Items/PoorManDice").Value;
			if (Projectile.ai[1] == 2)
				tex = ModContent.Request<Texture2D>("LukaiAddons/Content/Items/AdventurerDice").Value;
			if (Projectile.ai[1] == 3)
				tex = ModContent.Request<Texture2D>("LukaiAddons/Content/Items/HighRollerDice").Value;
			Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}

		public override void AI()
		{
			if (rolled) ++rolled_tim;

			Projectile.spriteDirection = Projectile.direction;
			if (!rolled) Projectile.rotation += 0.4f * (float)Projectile.direction;

			Projectile.ai[0] += 1f; /* Add ticks to a timer/count */
			if (Projectile.ai[0] >= 15f)
			{
				Projectile.ai[0] = 15f; /* No need to let it continue */
				Projectile.velocity.Y = Projectile.velocity.Y + 0.1f; /* After 15 ticks pass, add velocity vertically */
			}
			if (Projectile.velocity.Y > 16f)
			{
				Projectile.velocity.Y = 16f;
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (rolled_tim >= 30)
			{
				Projectile.Kill();
				return false;
			}

			if (bounces <= 0) rolled = true;
			else { --bounces; }

			if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
			if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;

			Projectile.velocity *= 0.55f;
			if (!rolled) SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

			return false;
		}

		public override void OnKill(int timeLeft)
		{
			for (int k = 0; k < 5; ++k)
			{
				Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.RedTorch, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
			}

			SoundEngine.PlaySound(SoundID.Item25, Projectile.position);
			
			Player player = Main.player[Projectile.owner];
			if (Main.myPlayer != player.whoAmI) return;
			
			if (Projectile.ai[1] == 1)
			{
				if (rnd.Next(1, 25) == 1) /* 6 is actually a 1/24 chance */
				{
					Main.NewText("Your [c/9696FF:Poor Man's Dice] rolled a [c/55FF55:6! Nice!]");
					Main.LocalPlayer.QuickSpawnItem(null, ItemID.GoldCoin, 15);
				} else {
					Main.NewText($"Your [c/9696FF:Poor Man's Dice] rolled a [c/FF0000:{rnd.Next(1, 6)}..]");
				}
			}
			if (Projectile.ai[1] == 2)
			{
				if (rnd.Next(1, 20) == 1) /* 6 is actually a 1/20 chance */
				{
					Main.NewText("Your [c/FF9696:Adventurer's Dice] rolled a [c/55FF55:6! Nice!]");
					Main.LocalPlayer.QuickSpawnItem(null, ItemID.PlatinumCoin, 1);
				} else {
					Main.NewText($"Your [c/FF9696:Adventurer's Dice] rolled a [c/FF0000:{rnd.Next(1, 6)}..]");
				}
			}
			if (Projectile.ai[1] == 3)
			{
				if (rnd.Next(1, 20) == 1) /* 6 is actually a 1/20 chance */
				{
					Main.NewText("Your [c/B428FF:High Roller's Dice] rolled a [c/55FF55:6! Nice!]");
					Main.LocalPlayer.QuickSpawnItem(null, ItemID.PlatinumCoin, 100);
				} else {
					Main.NewText($"Your [c/B428FF:High Roller's Dice] rolled a [c/FF0000:{rnd.Next(1, 6)}..]");
				}
			}
		}
	}
}
