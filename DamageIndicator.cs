using System;
using UnityEngine;

// Token: 0x0200005F RID: 95
public static class DamageIndicator
{
	// Token: 0x06000236 RID: 566 RVA: 0x00002E9A File Offset: 0x0000109A
	public static void ReceiveDamageFromPosition(Vector3 srcPos, float damage)
	{
	}

	// Token: 0x06000237 RID: 567 RVA: 0x00002E9A File Offset: 0x0000109A
	public static void ReceiveDamageFromDirection(Vector3 dir, float damage)
	{
	}

	// Token: 0x040001CD RID: 461
	private const float PulseIntensity = 1.5f;

	// Token: 0x040001CE RID: 462
	private const float ScrapeIntensity = 0.02f;

	// Token: 0x040001CF RID: 463
	private const float EffectsIntensity = 0.025f;
}
