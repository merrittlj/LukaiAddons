using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace LukaiAddons.Content.Projectiles
{
	public class Dice : ModProjectile
	{
		int bounces = 3;
		bool rolled = false;
		float rolled_tim = 0f; /* How long until the dice disappears */

		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.friendly = true;
			Projectile.scale = 0.5f;
			Projectile.timeLeft = 600;
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
			
			/* TODO: roll dice/reward */
		}
	}
}
