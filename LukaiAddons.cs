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

namespace LukaiAddons
{
	public class LukaiAddons : Mod
	{
		public override void Load()
		{
			if (Main.netMode != NetmodeID.Server)
			{
				Filters.Scene["BloodRealmMoon"] = new Filter(new BloodMoonScreenShaderData("FilterBloodMoon").UseColor(2f, -0.8f, -0.6f), EffectPriority.Medium);
			}
		}
	}
}
