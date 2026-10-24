using System;

namespace CustomPlayerEffects
{
	// Token: 0x020008F4 RID: 2292
	public interface ICustomHealableEffect : IHealableEffect
	{
		// Token: 0x06003487 RID: 13447
		void OnHeal(ItemType item);
	}
}
