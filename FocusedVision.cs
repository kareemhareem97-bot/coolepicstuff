using System;

namespace CustomPlayerEffects
{
	// Token: 0x020008B9 RID: 2233
	public class FocusedVision : StatusEffectBase
	{
		// Token: 0x17000994 RID: 2452
		// (get) Token: 0x060032C4 RID: 12996 RVA: 0x00040288 File Offset: 0x0003E488
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Positive;
			}
		}
	}
}
