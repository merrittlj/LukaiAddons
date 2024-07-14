using Terraria;
using Terraria.ModLoader;

namespace LukaiAddons.Content.Buffs
{
	public class TimeCooldownBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoSave[Type] = true; /* Do not save buff after leaving the world. */
		}
	}
}
