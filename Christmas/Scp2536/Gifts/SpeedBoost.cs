using System;
using CustomPlayerEffects;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010EF RID: 4335
	public class SpeedBoost : Scp2536AnomalousGiftBase
	{
		// Token: 0x170013FF RID: 5119
		// (get) Token: 0x06006079 RID: 24697 RVA: 0x00040964 File Offset: 0x0003EB64
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Six;
			}
		}

		// Token: 0x0600607A RID: 24698 RVA: 0x001423CD File Offset: 0x001405CD
		public override void ServerGrant(ReferenceHub hub)
		{
			base.ServerGrant(hub);
			hub.playerEffectsController.ChangeState<MovementBoost>(30, 25f, false);
		}

		// Token: 0x0400463C RID: 17980
		private const int EffectIntensity = 30;

		// Token: 0x0400463D RID: 17981
		private const int EffectDuration = 25;
	}
}
