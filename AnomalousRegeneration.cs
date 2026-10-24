using System;
using Mirror;
using PlayerRoles.PlayableScps.HumeShield;

namespace CustomPlayerEffects
{
	// Token: 0x0200089A RID: 2202
	public class AnomalousRegeneration : StatusEffectBase, IHumeRegenModifier
	{
		// Token: 0x17000947 RID: 2375
		// (get) Token: 0x0600320A RID: 12810 RVA: 0x00040288 File Offset: 0x0003E488
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Positive;
			}
		}

		// Token: 0x17000948 RID: 2376
		// (get) Token: 0x0600320B RID: 12811 RVA: 0x00004094 File Offset: 0x00002294
		public bool HumeRegenCooldownEnabled
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600320C RID: 12812 RVA: 0x000A8118 File Offset: 0x000A6318
		protected override void Enabled()
		{
			base.Enabled();
			if (!NetworkServer.active)
			{
				return;
			}
			IHumeShieldedRole humeShieldedRole = base.Hub.roleManager.CurrentRole as IHumeShieldedRole;
			if (humeShieldedRole == null)
			{
				return;
			}
			humeShieldedRole.HumeShieldModule.HsCurrent = humeShieldedRole.HumeShieldModule.HsMax;
		}
	}
}
