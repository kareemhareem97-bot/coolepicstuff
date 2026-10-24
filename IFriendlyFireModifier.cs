using System;
using PlayerStatsSystem;

namespace CustomPlayerEffects
{
	// Token: 0x020008F9 RID: 2297
	public interface IFriendlyFireModifier
	{
		// Token: 0x0600348D RID: 13453
		bool AllowFriendlyFire(float baseDamage, AttackerDamageHandler handler, HitboxType hitboxType);
	}
}
