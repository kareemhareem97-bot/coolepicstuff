using System;

namespace CustomPlayerEffects
{
	// Token: 0x02000902 RID: 2306
	public interface IUsableItemModifierEffect
	{
		// Token: 0x06003497 RID: 13463
		bool TryGetSpeed(ItemType item, out float speed);
	}
}
