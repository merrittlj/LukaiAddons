using LukaiAddons.Content.Items.Mounts;
using Terraria;
using Terraria.ModLoader;

namespace LukaiAddons.Content.Buffs.Mounts
{
	public class BoulderBusterBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.buffNoTimeDisplay[Type] = true; /* Do not display the time left(as it is infinite). */
			Main.buffNoSave[Type] = true; /* Do not save buff after leaving the world. */
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.mount.SetMount(ModContent.MountType<BoulderBuster>(), player);
			player.buffTime[buffIndex] = 10;
		}
	}
}
