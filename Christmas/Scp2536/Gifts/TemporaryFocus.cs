using System;
using CustomPlayerEffects;
using PlayerRoles.PlayableScps.Scp939;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010F2 RID: 4338
	public class TemporaryFocus : Scp2536AnomalousGiftBase
	{
		// Token: 0x17001403 RID: 5123
		// (get) Token: 0x06006087 RID: 24711 RVA: 0x00054ECB File Offset: 0x000530CB
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Three;
			}
		}

		// Token: 0x06006088 RID: 24712 RVA: 0x00142585 File Offset: 0x00140785
		public override bool CanBeGranted(ReferenceHub hub)
		{
			return base.CanBeGranted(hub) && hub.roleManager.CurrentRole is Scp939Role;
		}

		// Token: 0x06006089 RID: 24713 RVA: 0x001425A5 File Offset: 0x001407A5
		public override void ServerGrant(ReferenceHub hub)
		{
			base.ServerGrant(hub);
			hub.playerEffectsController.EnableEffect<FocusedVision>(40f, false);
		}

		// Token: 0x04004645 RID: 17989
		private const float EffectDuration = 40f;
	}
}
