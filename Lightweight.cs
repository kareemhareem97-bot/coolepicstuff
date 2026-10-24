using System;

namespace CustomPlayerEffects
{
	// Token: 0x020008C1 RID: 2241
	public class Lightweight : StatusEffectBase, IJumpModifierEffect
	{
		// Token: 0x170009A8 RID: 2472
		// (get) Token: 0x060032EE RID: 13038 RVA: 0x00004094 File Offset: 0x00002294
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Technical;
			}
		}

		// Token: 0x170009A9 RID: 2473
		// (get) Token: 0x060032EF RID: 13039 RVA: 0x000A95F8 File Offset: 0x000A77F8
		public float JumpStrengthMultplier
		{
			get
			{
				return 1f + (float)base.Intensity * 0.01f;
			}
		}
	}
}
