using System;
using Mirror;
using PlayerStatsSystem;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x0200089E RID: 2206
	public class Bleeding : TickingEffectBase, IPulseEffect, IHealableEffect
	{
		// Token: 0x06003220 RID: 12832 RVA: 0x00002E9A File Offset: 0x0000109A
		public void ExecutePulse()
		{
		}

		// Token: 0x06003221 RID: 12833 RVA: 0x000A83D8 File Offset: 0x000A65D8
		protected override void OnTick()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			float damage = this.damagePerTick * RainbowTaste.CurrentMultiplier(base.Hub);
			base.Hub.playerStats.DealDamage(new UniversalDamageHandler(damage, DeathTranslations.Bleeding, null));
			base.Hub.playerEffectsController.ServerSendPulse<Bleeding>();
			this.damagePerTick *= this.multPerTick;
			this.damagePerTick = Mathf.Clamp(this.damagePerTick, this.minDamage, this.maxDamage);
		}

		// Token: 0x06003222 RID: 12834 RVA: 0x000A845D File Offset: 0x000A665D
		protected override void Enabled()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			this.damagePerTick = this.maxDamage;
		}

		// Token: 0x06003223 RID: 12835 RVA: 0x000A8473 File Offset: 0x000A6673
		public bool IsHealable(ItemType it)
		{
			if (it == ItemType.SCP500)
			{
				this.damagePerTick = this.minDamage;
			}
			return it == ItemType.Medkit;
		}

		// Token: 0x04002723 RID: 10019
		public float minDamage = 2f;

		// Token: 0x04002724 RID: 10020
		public float maxDamage = 20f;

		// Token: 0x04002725 RID: 10021
		public float multPerTick = 0.5f;

		// Token: 0x04002726 RID: 10022
		public float damagePerTick = 20f;
	}
}
