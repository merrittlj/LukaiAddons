using Terraria;
using Terraria.ID;
using Terraria.Audio;
using Terraria.ModLoader;

namespace LukaiAddons.Content.Buffs
{
	public class TimeBuff : ModBuff
	{
		float tickTim = 0f;
		float count = 0f;

		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true; /* Do not save buff after leaving the world. */
			count = 0f;
			tickTim = 0f;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			++tickTim;
			
			if (tickTim >= 60)
			{
				if (count >= 7) count = 0;
				++count;
				SoundEngine.PlaySound(SoundID.NPCHit3 with {
					Volume = 1.0f,
					Pitch = 0.3f + (count / 10)
				});
				tickTim = 0f;
			}
		}
	}
}
