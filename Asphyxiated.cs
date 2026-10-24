using System;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x0200089C RID: 2204
	public class Asphyxiated : TickingEffectBase, IStaminaModifier
	{
		// Token: 0x17000949 RID: 2377
		// (get) Token: 0x06003215 RID: 12821 RVA: 0x00004070 File Offset: 0x00002270
		public bool StaminaModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x1700094A RID: 2378
		// (get) Token: 0x06003216 RID: 12822 RVA: 0x0000408D File Offset: 0x0000228D
		public float StaminaUsageMultiplier
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x1700094B RID: 2379
		// (get) Token: 0x06003217 RID: 12823 RVA: 0x0000548E File Offset: 0x0000368E
		public float StaminaRegenMultiplier
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x1700094C RID: 2380
		// (get) Token: 0x06003218 RID: 12824 RVA: 0x00004094 File Offset: 0x00002294
		public bool SprintingDisabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06003219 RID: 12825 RVA: 0x000A82E4 File Offset: 0x000A64E4
		protected override void Enabled()
		{
			base.Enabled();
			this._stamina = base.Hub.playerStats.GetModule<StaminaStat>();
		}

		// Token: 0x0600321A RID: 12826 RVA: 0x000A8304 File Offset: 0x000A6504
		protected override void OnTick()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			this._stamina.CurValue = Mathf.Clamp01(this._stamina.CurValue - this.staminaDrainPerTick * 0.01f);
			if (this._stamina.CurValue <= 0f)
			{
				float damage = this.healthDrainPerTick * RainbowTaste.CurrentMultiplier(base.Hub);
				base.Hub.playerStats.DealDamage(new UniversalDamageHandler(damage, DeathTranslations.Asphyxiated, null));
			}
		}

		// Token: 0x0400271F RID: 10015
		public float staminaDrainPerTick = 5f;

		// Token: 0x04002720 RID: 10016
		public float healthDrainPerTick = 2f;

		// Token: 0x04002721 RID: 10017
		private StaminaStat _stamina;
	}
}
