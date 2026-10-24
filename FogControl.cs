using System;
using CustomRendering;

namespace CustomPlayerEffects
{
	// Token: 0x020008BA RID: 2234
	public class FogControl : StatusEffectBase
	{
		// Token: 0x17000995 RID: 2453
		// (get) Token: 0x060032C6 RID: 12998 RVA: 0x000A9383 File Offset: 0x000A7583
		public override byte MaxIntensity { get; } = (byte)Enum.GetValues(typeof(FogType)).Length;

		// Token: 0x17000996 RID: 2454
		// (get) Token: 0x060032C7 RID: 12999 RVA: 0x00004094 File Offset: 0x00002294
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Technical;
			}
		}

		// Token: 0x060032C8 RID: 13000 RVA: 0x000A938B File Offset: 0x000A758B
		public void SetFogType(FogType fogType)
		{
			base.Intensity = (byte)(fogType + 1);
		}
	}
}
