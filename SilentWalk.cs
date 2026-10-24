using System;
using PlayerRoles.FirstPersonControl.Thirdperson;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008D9 RID: 2265
	public class SilentWalk : StatusEffectBase, IFootstepEffect
	{
		// Token: 0x170009F2 RID: 2546
		// (get) Token: 0x060033C6 RID: 13254 RVA: 0x00040288 File Offset: 0x0003E488
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Positive;
			}
		}

		// Token: 0x060033C7 RID: 13255 RVA: 0x000AB47F File Offset: 0x000A967F
		public float ProcessFootstepOverrides(AnimatedCharacterModel _, float dis)
		{
			return Mathf.Max(1f - 0.1f * (float)base.Intensity, 0f) * dis;
		}

		// Token: 0x040027E2 RID: 10210
		private const float VolumePerStack = 0.1f;
	}
}
