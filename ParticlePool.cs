using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.VFX;

// Token: 0x0200006A RID: 106
public static class ParticlePool
{
	// Token: 0x06000254 RID: 596 RVA: 0x0000939F File Offset: 0x0000759F
	public static VisualEffect GetFromPool(this VisualEffect template, float minAge = 0.1f)
	{
		return ParticlePool.GetAny<VisualEffect>(template, ParticlePool.VfxPools, (VisualEffect x) => x.aliveParticleCount > 0, minAge);
	}

	// Token: 0x06000255 RID: 597 RVA: 0x000093CC File Offset: 0x000075CC
	public static ParticleSystem GetFromPool(this ParticleSystem template, float minAge = 0.1f)
	{
		return ParticlePool.GetAny<ParticleSystem>(template, ParticlePool.PsPools, (ParticleSystem x) => x.isPlaying, minAge);
	}

	// Token: 0x06000256 RID: 598 RVA: 0x000093FC File Offset: 0x000075FC
	private static T GetAny<T>(T template, Dictionary<T, Queue<ParticlePool.PooledSystem<T>>> pool, Predicate<T> isBusy, float minElapsed) where T : Component
	{
		Queue<ParticlePool.PooledSystem<T>> orAddNew = pool.GetOrAddNew(template);
		ParticlePool.PooledSystem<T> pooledSystem;
		while (orAddNew.TryPeek(out pooledSystem))
		{
			if (pooledSystem.System == null)
			{
				orAddNew.Dequeue();
			}
			else
			{
				if (pooledSystem.AgeSeconds >= (double)minElapsed && !isBusy(pooledSystem.System))
				{
					ParticlePool.PooledSystem<T> pooledSystem2 = orAddNew.Dequeue();
					pooledSystem2.LastUseTime.Restart();
					orAddNew.Enqueue(pooledSystem2);
					return pooledSystem2.System;
				}
				break;
			}
		}
		ParticlePool.PooledSystem<T> pooledSystem3 = new ParticlePool.PooledSystem<T>(template);
		orAddNew.Enqueue(pooledSystem3);
		return pooledSystem3.System;
	}

	// Token: 0x040001F3 RID: 499
	private static readonly Dictionary<VisualEffect, Queue<ParticlePool.PooledSystem<VisualEffect>>> VfxPools = new Dictionary<VisualEffect, Queue<ParticlePool.PooledSystem<VisualEffect>>>();

	// Token: 0x040001F4 RID: 500
	private static readonly Dictionary<ParticleSystem, Queue<ParticlePool.PooledSystem<ParticleSystem>>> PsPools = new Dictionary<ParticleSystem, Queue<ParticlePool.PooledSystem<ParticleSystem>>>();

	// Token: 0x0200006B RID: 107
	private readonly struct PooledSystem<T> where T : Component
	{
		// Token: 0x17000083 RID: 131
		// (get) Token: 0x06000258 RID: 600 RVA: 0x0000949C File Offset: 0x0000769C
		public double AgeSeconds
		{
			get
			{
				return this.LastUseTime.Elapsed.TotalSeconds;
			}
		}

		// Token: 0x06000259 RID: 601 RVA: 0x000094BC File Offset: 0x000076BC
		public PooledSystem(T template)
		{
			this.System = UnityEngine.Object.Instantiate<T>(template);
			this.LastUseTime = Stopwatch.StartNew();
		}

		// Token: 0x040001F5 RID: 501
		public readonly T System;

		// Token: 0x040001F6 RID: 502
		public readonly Stopwatch LastUseTime;
	}
}
