using System;
using PlayerRoles.FirstPersonControl;

namespace CustomPlayerEffects
{
	// Token: 0x020008BF RID: 2239
	public class Invigorated : StatusEffectBase, IStaminaModifier
	{
		// Token: 0x170009A0 RID: 2464
		// (get) Token: 0x060032E2 RID: 13026 RVA: 0x00040288 File Offset: 0x0003E488
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Positive;
			}
		}

		// Token: 0x170009A1 RID: 2465
		// (get) Token: 0x060032E3 RID: 13027 RVA: 0x00004070 File Offset: 0x00002270
		public bool StaminaModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x170009A2 RID: 2466
		// (get) Token: 0x060032E4 RID: 13028 RVA: 0x0000548E File Offset: 0x0000368E
		public float StaminaUsageMultiplier
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x170009A3 RID: 2467
		// (get) Token: 0x060032E5 RID: 13029 RVA: 0x0000408D File Offset: 0x0000228D
		public float StaminaRegenMultiplier
		{
			get
			{
				return 1f;
			}
		}

		// Token: 0x170009A4 RID: 2468
		// (get) Token: 0x060032E6 RID: 13030 RVA: 0x00004094 File Offset: 0x00002294
		public bool SprintingDisabled
		{
			get
			{
				return false;
			}
		}
	}
}
