using System;
using System.Collections.Generic;
using System.Diagnostics;

// Token: 0x02000150 RID: 336
public class RateLimiter
{
	// Token: 0x17000101 RID: 257
	// (get) Token: 0x0600065C RID: 1628 RVA: 0x00017EC4 File Offset: 0x000160C4
	public bool InstantReady
	{
		get
		{
			return this._instantCooldown == 0f || !this._instInput.IsRunning || this._instInput.Elapsed.TotalSeconds >= (double)this._instantCooldown;
		}
	}

	// Token: 0x17000102 RID: 258
	// (get) Token: 0x0600065D RID: 1629 RVA: 0x00017F0C File Offset: 0x0001610C
	public bool RateReady
	{
		get
		{
			if (!this._useWindowLimiter)
			{
				return true;
			}
			int num = this._inputs.Count;
			if (num < this._windowLength)
			{
				return true;
			}
			while (num > 0 && this._inputs.Peek().Elapsed.TotalSeconds >= (double)this._windowCooldown)
			{
				num--;
				this._inputs.Dequeue();
			}
			return num < this._windowLength;
		}
	}

	// Token: 0x17000103 RID: 259
	// (get) Token: 0x0600065E RID: 1630 RVA: 0x00017F78 File Offset: 0x00016178
	public bool AllReady
	{
		get
		{
			return this.InstantReady && this.RateReady;
		}
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x00017F8A File Offset: 0x0001618A
	public RateLimiter(float instantCooldown, int maxInputs, float timeWindow)
	{
		this._instantCooldown = instantCooldown;
		this._windowCooldown = timeWindow;
		this._windowLength = maxInputs;
		this._inputs = new Queue<Stopwatch>();
		this._instInput = new Stopwatch();
		this._useWindowLimiter = true;
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x00017FC4 File Offset: 0x000161C4
	public RateLimiter(float instantCooldown)
	{
		this._instantCooldown = instantCooldown;
		this._instInput = new Stopwatch();
		this._useWindowLimiter = false;
	}

	// Token: 0x06000661 RID: 1633 RVA: 0x00017FE5 File Offset: 0x000161E5
	public void RegisterInput()
	{
		this._instInput.Restart();
		if (this._useWindowLimiter)
		{
			this._inputs.Enqueue(Stopwatch.StartNew());
		}
	}

	// Token: 0x040005DC RID: 1500
	private readonly float _instantCooldown;

	// Token: 0x040005DD RID: 1501
	private readonly float _windowCooldown;

	// Token: 0x040005DE RID: 1502
	private readonly Queue<Stopwatch> _inputs;

	// Token: 0x040005DF RID: 1503
	private readonly Stopwatch _instInput;

	// Token: 0x040005E0 RID: 1504
	private readonly int _windowLength;

	// Token: 0x040005E1 RID: 1505
	private readonly bool _useWindowLimiter;
}
