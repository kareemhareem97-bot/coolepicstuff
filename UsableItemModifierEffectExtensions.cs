using System;

namespace CustomPlayerEffects
{
	// Token: 0x02000903 RID: 2307
	public static class UsableItemModifierEffectExtensions
	{
		// Token: 0x06003498 RID: 13464 RVA: 0x000ACF10 File Offset: 0x000AB110
		public static float GetSpeedMultiplier(this ItemType type, ReferenceHub player)
		{
			float result;
			type.TryGetSpeedMultiplier(player, out result);
			return result;
		}

		// Token: 0x06003499 RID: 13465 RVA: 0x000ACF28 File Offset: 0x000AB128
		public static bool TryGetSpeedMultiplier(this ItemType type, ReferenceHub player, out float multiplier)
		{
			PlayerEffectsController playerEffectsController = player.playerEffectsController;
			bool result = false;
			multiplier = 1f;
			for (int i = 0; i < playerEffectsController.EffectsLength; i++)
			{
				StatusEffectBase statusEffectBase = playerEffectsController.AllEffects[i];
				if (statusEffectBase.IsEnabled)
				{
					IUsableItemModifierEffect usableItemModifierEffect = statusEffectBase as IUsableItemModifierEffect;
					float num;
					if (usableItemModifierEffect != null && usableItemModifierEffect.TryGetSpeed(type, out num))
					{
						multiplier *= num;
						result = true;
					}
				}
			}
			return result;
		}
	}
}
