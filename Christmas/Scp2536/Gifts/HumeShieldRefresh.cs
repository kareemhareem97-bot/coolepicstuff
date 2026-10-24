using System;
using CustomPlayerEffects;
using PlayerStatsSystem;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010ED RID: 4333
	public class HumeShieldRefresh : Scp2536AnomalousGiftBase
	{
		// Token: 0x170013FD RID: 5117
		// (get) Token: 0x06006071 RID: 24689 RVA: 0x0003F71E File Offset: 0x0003D91E
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Four;
			}
		}

		// Token: 0x06006072 RID: 24690 RVA: 0x001422FC File Offset: 0x001404FC
		public override bool CanBeGranted(ReferenceHub hub)
		{
			HumeShieldStat humeShieldStat;
			return base.CanBeGranted(hub) && hub.playerStats.TimeSinceDamageTick <= 15.0 && hub.playerStats.TryGetModule<HumeShieldStat>(out humeShieldStat) && humeShieldStat.NormalizedValue <= 0.4f;
		}

		// Token: 0x06006073 RID: 24691 RVA: 0x0014234B File Offset: 0x0014054B
		public override void ServerGrant(ReferenceHub hub)
		{
			base.ServerGrant(hub);
			hub.playerEffectsController.EnableEffect<AnomalousRegeneration>(20f, false);
		}

		// Token: 0x04004638 RID: 17976
		private const float EffectDuration = 20f;

		// Token: 0x04004639 RID: 17977
		private const float MaximumHumePercentageAllowed = 0.4f;

		// Token: 0x0400463A RID: 17978
		private const float InCombatDuration = 15f;
	}
}
