using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Shaders;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using ReLogic.Content;

namespace LukaiAddons
{
	public class LukaiAddons : Mod
	{
		public override void Load()
		{
			if (Main.netMode != NetmodeID.Server)
			{
				Asset<Effect> bloodRealmFilter = this.Assets.Request<Effect>("Content/Effects/ShockwaveEffect");

				/* Terraria's blood moon */
				Filters.Scene["BloodRealmMoon"] = new Filter(new BloodMoonScreenShaderData("FilterBloodMoon").UseColor(2f, -0.8f, -0.6f), EffectPriority.Medium);
				Filters.Scene["BloodRealm"] = new Filter(new ScreenShaderData(bloodRealmFilter, "Shockwave"), EffectPriority.VeryHigh);
			}
		}
	}
}
