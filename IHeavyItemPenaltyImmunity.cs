using System;

namespace CustomPlayerEffects
{
	// Token: 0x020008FB RID: 2299
	public interface IHeavyItemPenaltyImmunity
	{
		// Token: 0x0600348F RID: 13455 RVA: 0x000ACE9C File Offset: 0x000AB09C
		public static bool IsImmune(ReferenceHub hub)
		{
			foreach (StatusEffectBase statusEffectBase in hub.playerEffectsController.AllEffects)
			{
				if (statusEffectBase is IHeavyItemPenaltyImmunity && statusEffectBase.IsEnabled)
				{
					return true;
				}
			}
			return false;
		}
	}
}
