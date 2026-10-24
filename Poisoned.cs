using System;
using Mirror;
using PlayerStatsSystem;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008C9 RID: 2249
	public class Poisoned : TickingEffectBase, IHealableEffect, IPulseEffect
	{
		// Token: 0x06003347 RID: 13127 RVA: 0x00002E9A File Offset: 0x0000109A
		public void ExecutePulse()
		{
		}

		// Token: 0x06003348 RID: 13128 RVA: 0x00004242 File Offset: 0x00002442
		public bool IsHealable(ItemType it)
		{
			return it == ItemType.SCP500;
		}

		// Token: 0x06003349 RID: 13129 RVA: 0x000AA484 File Offset: 0x000A8684
		protected override void OnTick()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			base.Hub.playerStats.DealDamage(new UniversalDamageHandler(this.damagePerTick, DeathTranslations.Poisoned, null));
			base.Hub.playerEffectsController.ServerSendPulse<Poisoned>();
			this.damagePerTick *= this.multPerTick;
			this.damagePerTick = Mathf.Clamp(this.damagePerTick, this.minDamage, this.maxDamage);
		}

		// Token: 0x0600334A RID: 13130 RVA: 0x000AA4FB File Offset: 0x000A86FB
		protected override void Enabled()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			this.damagePerTick = this.minDamage;
		}

		// Token: 0x0400279C RID: 10140
		private float damagePerTick = 2f;

		// Token: 0x0400279D RID: 10141
		private readonly float minDamage = 2f;

		// Token: 0x0400279E RID: 10142
		private readonly float maxDamage = 20f;

		// Token: 0x0400279F RID: 10143
		private readonly float multPerTick = 2f;
	}
}
