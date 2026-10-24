using System;
using PlayerStatsSystem;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010EC RID: 4332
	public class HealthBoost : Scp2536AnomalousGiftBase
	{
		// Token: 0x170013FC RID: 5116
		// (get) Token: 0x0600606D RID: 24685 RVA: 0x00040288 File Offset: 0x0003E488
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Two;
			}
		}

		// Token: 0x0600606E RID: 24686 RVA: 0x00142288 File Offset: 0x00140488
		public override bool CanBeGranted(ReferenceHub hub)
		{
			HealthStat healthStat;
			return base.CanBeGranted(hub) && hub.playerStats.TryGetModule<HealthStat>(out healthStat) && healthStat.NormalizedValue <= 0.4f;
		}

		// Token: 0x0600606F RID: 24687 RVA: 0x001422C4 File Offset: 0x001404C4
		public override void ServerGrant(ReferenceHub hub)
		{
			base.ServerGrant(hub);
			HealthStat healthStat;
			if (!hub.playerStats.TryGetModule<HealthStat>(out healthStat))
			{
				return;
			}
			healthStat.ServerHeal(healthStat.MaxValue * 0.15f);
		}

		// Token: 0x04004636 RID: 17974
		private const float MaxHpHealingMultiplier = 0.15f;

		// Token: 0x04004637 RID: 17975
		private const float MaximumHealthPercentageAllowed = 0.4f;
	}
}
