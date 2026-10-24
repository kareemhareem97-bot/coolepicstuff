using System;
using CustomPlayerEffects;
using UnityEngine;

namespace FacilitySoundtrack
{
	// Token: 0x02000878 RID: 2168
	public abstract class SoundtrackLayerBase : MonoBehaviour
	{
		// Token: 0x17000923 RID: 2339
		// (get) Token: 0x06003177 RID: 12663
		public abstract float Weight { get; }

		// Token: 0x17000924 RID: 2340
		// (get) Token: 0x06003178 RID: 12664
		public abstract bool Additive { get; }

		// Token: 0x17000925 RID: 2341
		// (get) Token: 0x06003179 RID: 12665
		public abstract bool IsMuteable { get; }

		// Token: 0x17000926 RID: 2342
		// (get) Token: 0x0600317A RID: 12666 RVA: 0x000A65B4 File Offset: 0x000A47B4
		protected bool IsPovMuted
		{
			get
			{
				ReferenceHub hub;
				return ReferenceHub.TryGetPovHub(out hub) && this.IsMutedForPlayer(hub);
			}
		}

		// Token: 0x0600317B RID: 12667
		public abstract void UpdateVolume(float volumeScale);

		// Token: 0x0600317C RID: 12668 RVA: 0x000A65D4 File Offset: 0x000A47D4
		protected bool IsMutedForPlayer(ReferenceHub hub)
		{
			if (!this.IsMuteable)
			{
				return false;
			}
			StatusEffectBase[] allEffects = hub.playerEffectsController.AllEffects;
			for (int i = 0; i < allEffects.Length; i++)
			{
				ISoundtrackMutingEffect soundtrackMutingEffect = allEffects[i] as ISoundtrackMutingEffect;
				if (soundtrackMutingEffect != null && soundtrackMutingEffect.MuteSoundtrack)
				{
					return true;
				}
			}
			return false;
		}
	}
}
