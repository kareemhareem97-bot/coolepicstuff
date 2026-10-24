using System;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008F1 RID: 2289
	public interface IConflictableEffect
	{
		// Token: 0x06003480 RID: 13440
		bool CheckConflicts(StatusEffectBase other);

		// Token: 0x06003481 RID: 13441 RVA: 0x000ACE15 File Offset: 0x000AB015
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			StatusEffectBase.OnEnabled += delegate(StatusEffectBase newEffect)
			{
				foreach (StatusEffectBase statusEffectBase in newEffect.Hub.playerEffectsController.AllEffects)
				{
					if (statusEffectBase.IsEnabled)
					{
						IConflictableEffect conflictableEffect = statusEffectBase as IConflictableEffect;
						if (conflictableEffect != null && !(statusEffectBase == newEffect))
						{
							conflictableEffect.CheckConflicts(newEffect);
						}
					}
				}
			};
		}
	}
}
