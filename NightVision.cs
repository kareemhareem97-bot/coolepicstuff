using System;

namespace CustomPlayerEffects
{
	// Token: 0x020008C4 RID: 2244
	public class NightVision : StatusEffectBase, IAmbientBoostingEffect
	{
		// Token: 0x170009B7 RID: 2487
		// (get) Token: 0x06003314 RID: 13076 RVA: 0x00004094 File Offset: 0x00002294
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Technical;
			}
		}

		// Token: 0x170009B8 RID: 2488
		// (get) Token: 0x06003315 RID: 13077 RVA: 0x000A9C42 File Offset: 0x000A7E42
		public float AmbientBoostAddition
		{
			get
			{
				return (float)base.Intensity * 0.01f;
			}
		}
	}
}
