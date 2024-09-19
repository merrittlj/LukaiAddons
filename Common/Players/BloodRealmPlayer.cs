using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace LukaiAddons.Common.Players
{
	public partial class LukaiAddonsPlayer : ModPlayer
	{
		public bool inBloodRealm = false;
		private bool prevInBloodRealm = false;
		private struct NPCDamageInfo
		{
			public int npcIndex { get; }
			public int damageTaken { get; set; }

			public NPCDamageInfo(int npcIndex, int damageTaken)
			{
				this.npcIndex = npcIndex;
				this.damageTaken = damageTaken;
			}
		}
		private List<NPCDamageInfo> npcDamageList = new List<NPCDamageInfo>();

		public override void ModifyHurt(ref Player.HurtModifiers modifiers)
		{
			if (!inBloodRealm) return;
			modifiers.FinalDamage *= 1.5f;
		}

		public void ApplyBloodDamage()
		{
			foreach(NPCDamageInfo dinfo in npcDamageList)
			{
				Main.npc[dinfo.npcIndex].SimpleStrikeNPC(dinfo.damageTaken, 1);
			}
			npcDamageList.Clear();
		}

		private void BloodRealmHit(int damage, int crit, NPC target, ref NPC.HitModifiers modifiers)
		{
			if (inBloodRealm)
			{
				modifiers.FinalDamage *= 1.5f; /* Multiplicative 50% damage increase */
				int targetDamage = modifiers.GetDamage(damage, Main.rand.Next(101) <= crit);

				int entryIndex = npcDamageList.FindIndex(entry => entry.npcIndex == target.whoAmI);
				if (entryIndex == -1)
					npcDamageList.Add(new NPCDamageInfo(target.whoAmI, targetDamage));
				else { npcDamageList[entryIndex] = new NPCDamageInfo(target.whoAmI, npcDamageList[entryIndex].damageTaken + targetDamage); }

				modifiers.FinalDamage *= 0;
				if (target.life < target.lifeMax)
				{
					++target.life; /* Attack should deal 1 damage */
					/*target.HealEffect(1);*/ /* Spawns heal text */
					target.netUpdate = true;
				}
			}
		}

		public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
		{
			/* TODO: does not capture shield bash damage */
			BloodRealmHit(item.damage, Player.GetWeaponCrit(item), target, ref modifiers);
		}

		public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
		{
			BloodRealmHit(proj.damage, proj.CritChance, target, ref modifiers); /* TODO: is this correct? */
		}
	}
}
