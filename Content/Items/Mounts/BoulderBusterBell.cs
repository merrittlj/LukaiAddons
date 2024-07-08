using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace LukaiAddons.Content.Items.Mounts
{
	public class BoulderBusterBell : ModItem, ILocalizedModType
	{
		public new string LocalizationCategory => "Items.Mounts";
		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.holdStyle = ItemHoldStyleID.HoldHeavy;
			Item.rare = ItemRarityID.Yellow;
			Item.value = Item.buyPrice(copper: 69);
			Item.UseSound = SoundID.Item35; /* Bell sound */
			Item.noMelee = true;
			Item.mountType = ModContent.MountType<BoulderBuster>();

			if (ModLoader.TryGetMod("CalamityMod", out Mod cmod) && cmod.TryFind("OnyxExcavatorKey", out ModItem onyx)) {
				ItemID.Sets.ShimmerTransformToItem[Type] = onyx.Type; /* Transform bell into onyx */
				ItemID.Sets.ShimmerTransformToItem[onyx.Type] = Type; /* Transform onyx into bell */
			}
		}

		public override void AddRecipes()
		{
			if (ModLoader.TryGetMod("CalamityMod", out Mod cmod))
				return;

			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.WorkBenches);
			recipe.AddIngredient(ItemID.StoneBlock, 10);
			recipe.AddCondition(Condition.NearShimmer);
			recipe.Register();
		}
	}
}
