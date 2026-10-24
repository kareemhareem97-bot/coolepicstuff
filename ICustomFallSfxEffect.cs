using System;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008F3 RID: 2291
	public interface ICustomFallSfxEffect
	{
		// Token: 0x17000A49 RID: 2633
		// (get) Token: 0x06003485 RID: 13445
		AudioClip FallSfxClip { get; }

		// Token: 0x17000A4A RID: 2634
		// (get) Token: 0x06003486 RID: 13446 RVA: 0x0000408D File Offset: 0x0000228D
		float FallSfxVolume
		{
			get
			{
				return 1f;
			}
		}
	}
}
