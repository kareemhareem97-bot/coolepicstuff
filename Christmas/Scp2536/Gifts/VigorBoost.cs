using System;
using PlayerRoles.PlayableScps.Scp106;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010F3 RID: 4339
	public class VigorBoost : Scp2536AnomalousGiftBase
	{
		// Token: 0x17001404 RID: 5124
		// (get) Token: 0x0600608B RID: 24715 RVA: 0x00054ECB File Offset: 0x000530CB
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Three;
			}
		}

		// Token: 0x0600608C RID: 24716 RVA: 0x001425C0 File Offset: 0x001407C0
		public override bool CanBeGranted(ReferenceHub hub)
		{
			VigorStat vigorStat;
			return base.CanBeGranted(hub) && hub.playerStats.TryGetModule<VigorStat>(out vigorStat) && vigorStat.NormalizedValue <= 0.4f && hub.roleManager.CurrentRole is Scp106Role;
		}

		// Token: 0x0600608D RID: 24717 RVA: 0x0014260C File Offset: 0x0014080C
		public override void ServerGrant(ReferenceHub hub)
		{
			base.ServerGrant(hub);
			VigorStat vigorStat;
			if (!hub.playerStats.TryGetModule<VigorStat>(out vigorStat))
			{
				return;
			}
			vigorStat.CurValue = vigorStat.MaxValue;
		}

		// Token: 0x04004646 RID: 17990
		private const float MaximumVigorPercentageAllowed = 0.4f;
	}
}
