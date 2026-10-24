using System;
using PlayerRoles.PlayableScps.Scp096;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010EE RID: 4334
	public class NoPresent : Scp2536AnomalousGiftBase
	{
		// Token: 0x170013FE RID: 5118
		// (get) Token: 0x06006075 RID: 24693 RVA: 0x00054ECB File Offset: 0x000530CB
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Three;
			}
		}

		// Token: 0x06006076 RID: 24694 RVA: 0x00142366 File Offset: 0x00140566
		public override bool CanBeGranted(ReferenceHub hub)
		{
			return base.CanBeGranted(hub) && hub.roleManager.CurrentRole is Scp096Role;
		}

		// Token: 0x06006077 RID: 24695 RVA: 0x00142388 File Offset: 0x00140588
		public override void ServerGrant(ReferenceHub hub)
		{
			base.ServerGrant(hub);
			Scp096Role scp096Role = hub.roleManager.CurrentRole as Scp096Role;
			if (scp096Role == null)
			{
				return;
			}
			Scp096RageCycleAbility scp096RageCycleAbility;
			if (!scp096Role.SubroutineModule.TryGetSubroutine<Scp096RageCycleAbility>(out scp096RageCycleAbility))
			{
				return;
			}
			scp096RageCycleAbility.ServerTryEnableInput(15f);
		}

		// Token: 0x0400463B RID: 17979
		private const float EffectDuration = 15f;
	}
}
