using System;

namespace CustomPlayerEffects
{
	// Token: 0x020008BC RID: 2236
	public class HeavyFooted : StatusEffectBase, IJumpModifierEffect
	{
		// Token: 0x17000999 RID: 2457
		// (get) Token: 0x060032D0 RID: 13008 RVA: 0x00004094 File Offset: 0x00002294
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Technical;
			}
		}

		// Token: 0x1700099A RID: 2458
		// (get) Token: 0x060032D1 RID: 13009 RVA: 0x000A945A File Offset: 0x000A765A
		public float JumpStrengthMultplier
		{
			get
			{
				return Math.Max(1f - (float)base.Intensity * 0.01f, 0f);
			}
		}
	}
}
