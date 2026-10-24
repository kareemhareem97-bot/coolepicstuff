using System;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;

namespace CustomPlayerEffects
{
	// Token: 0x020008B0 RID: 2224
	public class Corroding : TickingEffectBase, IStaminaModifier
	{
		// Token: 0x1700097C RID: 2428
		// (get) Token: 0x06003293 RID: 12947 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool AllowEnabling
		{
			get
			{
				return true;
			}
		}

		// Token: 0x1700097D RID: 2429
		// (get) Token: 0x06003294 RID: 12948 RVA: 0x00004070 File Offset: 0x00002270
		public bool StaminaModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x1700097E RID: 2430
		// (get) Token: 0x06003295 RID: 12949 RVA: 0x0000548E File Offset: 0x0000368E
		public float StaminaRegenMultiplier
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x06003296 RID: 12950 RVA: 0x000A903C File Offset: 0x000A723C
		protected override void OnTick()
		{
			if (!NetworkServer.active || this.AttackerHub == null || Vitality.CheckPlayer(base.Hub))
			{
				return;
			}
			base.Hub.playerStats.DealDamage(new ScpDamageHandler(this.AttackerHub, 2.1f, DeathTranslations.PocketDecay));
			if (Corroding.StaminaDrainPercentage > 0f)
			{
				base.Hub.playerStats.GetModule<StaminaStat>().CurValue -= Corroding.StaminaDrainPercentage * 0.01f;
			}
		}

		// Token: 0x04002758 RID: 10072
		private const float DamagePerTick = 2.1f;

		// Token: 0x04002759 RID: 10073
		private static readonly float StaminaDrainPercentage;

		// Token: 0x0400275A RID: 10074
		public ReferenceHub AttackerHub;
	}
}
