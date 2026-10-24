using System;
using System.Collections.Generic;
using NorthwoodLib.Pools;
using UnityEngine;

// Token: 0x02000065 RID: 101
public class RealisticLoadingBar
{
	// Token: 0x0600024A RID: 586 RVA: 0x000090F4 File Offset: 0x000072F4
	public RealisticLoadingBar(float targetTime, int numberOfSteps, float maxStepSizeVar, float maxTickVar)
	{
		this._lastProgress = 0f;
		this._queue = new Queue<Vector2>();
		List<Vector2> list = ListPool<Vector2>.Shared.Rent();
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < numberOfSteps; i++)
		{
			float num3 = UnityEngine.Random.Range(1f, maxTickVar);
			num += num3;
			float num4 = UnityEngine.Random.Range(1f, maxStepSizeVar);
			num2 += num4;
			list.Add(new Vector2(num, num4));
		}
		float num5 = num / targetTime;
		foreach (Vector2 vector in list)
		{
			this._queue.Enqueue(new Vector2(Time.realtimeSinceStartup + vector.x / num5, vector.y / num2));
		}
	}

	// Token: 0x17000082 RID: 130
	// (get) Token: 0x0600024B RID: 587 RVA: 0x000091E0 File Offset: 0x000073E0
	public float Progress
	{
		get
		{
			while (this._queue.Count > 0 && this._queue.Peek().x <= Time.realtimeSinceStartup)
			{
				this._lastProgress += this._queue.Dequeue().y;
			}
			if (this._queue.Count != 0)
			{
				return this._lastProgress;
			}
			return 1f;
		}
	}

	// Token: 0x040001E7 RID: 487
	private float _lastProgress;

	// Token: 0x040001E8 RID: 488
	private readonly Queue<Vector2> _queue;
}
