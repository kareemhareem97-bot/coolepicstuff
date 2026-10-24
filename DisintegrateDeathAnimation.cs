using System;
using DeathAnimations;

// Token: 0x02000052 RID: 82
public class DisintegrateDeathAnimation : DeathAnimation
{
	// Token: 0x02000053 RID: 83
	public interface IDisintegrateDamageHandler
	{
		// Token: 0x17000078 RID: 120
		// (get) Token: 0x060001F9 RID: 505
		bool Disintegrate { get; }
	}
}
