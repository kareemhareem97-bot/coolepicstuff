using System;

namespace CustomPlayerEffects
{
	// Token: 0x020008AF RID: 2223
	public class Concussed : StatusEffectBase, IHealableEffect
	{
		// Token: 0x06003291 RID: 12945 RVA: 0x000A9028 File Offset: 0x000A7228
		public bool IsHealable(ItemType it)
		{
			return it == ItemType.SCP500 || it == ItemType.Adrenaline || it == ItemType.Painkillers;
		}
	}
}
