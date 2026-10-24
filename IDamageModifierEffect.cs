using System;
using PlayerStatsSystem;

namespace CustomPlayerEffects
{
	// Token: 0x020008F6 RID: 2294
	public interface IDamageModifierEffect
	{
		// Token: 0x06003489 RID: 13449
		float GetDamageModifier(float baseDamage, DamageHandlerBase handler, HitboxType hitboxType);

		// Token: 0x17000A4C RID: 2636
		// (get) Token: 0x0600348A RID: 13450
		bool DamageModifierActive { get; }
	}
}
