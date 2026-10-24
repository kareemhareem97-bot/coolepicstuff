using System;

namespace CustomPlayerEffects
{
	// Token: 0x020008DD RID: 2269
	public class SoundtrackMute : StatusEffectBase, ISoundtrackMutingEffect
	{
		// Token: 0x17000A06 RID: 2566
		// (get) Token: 0x060033EA RID: 13290 RVA: 0x00004094 File Offset: 0x00002294
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Technical;
			}
		}

		// Token: 0x17000A07 RID: 2567
		// (get) Token: 0x060033EB RID: 13291 RVA: 0x00004070 File Offset: 0x00002270
		public bool MuteSoundtrack
		{
			get
			{
				return base.IsEnabled;
			}
		}
	}
}
