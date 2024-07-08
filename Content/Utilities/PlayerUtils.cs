using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.Player;

namespace LukaiAddons
{
	public static partial class LukaiUtils
	{
		public static int GetBestPickPower(this Player player) /* Thanks, Calamity mod. */
		{
			int highestPickPower = 35; /* 35% if you have no pickaxes. */
			for (int item = 0; item < Main.InventorySlotsTotal; item++)
			{
				if (player.inventory[item].pick <= 0)
					continue;

				if (player.inventory[item].pick > highestPickPower)
					highestPickPower = player.inventory[item].pick;
			}

			return highestPickPower;
		}
	}
}
