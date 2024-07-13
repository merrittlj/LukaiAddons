using Terraria;
using Terraria.ID;
using Terraria.GameContent;
using LukaiAddons.Content.Items;

using Terraria.ModLoader;

public class LukaiGlobalNPC : GlobalNPC
{
	public override void ModifyShop(NPCShop shop)
	{
		int type = shop.NpcType;

		if (type == NPCID.GoblinTinkerer)
		{
			shop.Add(ModContent.ItemType<CrappyDice>());
		}
	}
}
