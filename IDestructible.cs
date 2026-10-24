using System;
using MapGeneration.StaticHelpers;
using PlayerStatsSystem;
using UnityEngine;

// Token: 0x02000063 RID: 99
public interface IDestructible : IBlockStaticBatching
{
	// Token: 0x17000080 RID: 128
	// (get) Token: 0x06000244 RID: 580
	uint NetworkId { get; }

	// Token: 0x17000081 RID: 129
	// (get) Token: 0x06000245 RID: 581
	Vector3 CenterOfMass { get; }

	// Token: 0x06000246 RID: 582
	bool Damage(float damage, DamageHandlerBase handler, Vector3 exactHitPos);
}
