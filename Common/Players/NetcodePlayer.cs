using System;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace LukaiAddons.Common.Players
{
	public partial class LukaiAddonsPlayer : ModPlayer
	{
		public bool coinflipPending = false;
		public int coinflipAmount = 0;
		public int coinflipFrom = -1;
	}
}
