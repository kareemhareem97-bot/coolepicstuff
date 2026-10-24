using System;
using PlayerStatsSystem;

// Token: 0x020000C7 RID: 199
public interface IHitmarkerPreventer
{
	// Token: 0x060003E7 RID: 999
	bool TryPreventHitmarker(AttackerDamageHandler attacker);
}
