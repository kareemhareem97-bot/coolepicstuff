using System;
using System.Diagnostics;

namespace CustomPlayerEffects.Danger
{
	// Token: 0x02000910 RID: 2320
	public abstract class DangerStackBase
	{
		// Token: 0x17000A67 RID: 2663
		// (get) Token: 0x060034E9 RID: 13545
		// (set) Token: 0x060034EA RID: 13546
		public abstract float DangerValue { get; set; }

		// Token: 0x17000A68 RID: 2664
		// (get) Token: 0x060034EB RID: 13547 RVA: 0x000AD797 File Offset: 0x000AB997
		public virtual float Duration { get; } = 20f;

		// Token: 0x17000A69 RID: 2665
		// (get) Token: 0x060034EC RID: 13548 RVA: 0x000AD7A0 File Offset: 0x000AB9A0
		// (set) Token: 0x060034ED RID: 13549 RVA: 0x000AD7FB File Offset: 0x000AB9FB
		public virtual bool IsActive
		{
			get
			{
				bool flag = this.TimeTracker.IsRunning && this.TimeTracker.Elapsed.TotalSeconds <= (double)this.Duration;
				if (!this._isActive && !flag)
				{
					this.TimeTracker.Stop();
				}
				return this._isActive || flag;
			}
			protected set
			{
				if (value == this._isActive)
				{
					return;
				}
				this._isActive = value;
				if (!this._isActive)
				{
					this.TimeTracker.Restart();
				}
			}
		}

		// Token: 0x17000A6A RID: 2666
		// (get) Token: 0x060034EE RID: 13550 RVA: 0x000AD821 File Offset: 0x000ABA21
		// (set) Token: 0x060034EF RID: 13551 RVA: 0x000AD829 File Offset: 0x000ABA29
		protected ReferenceHub Owner { get; set; }

		// Token: 0x060034F0 RID: 13552 RVA: 0x000AD832 File Offset: 0x000ABA32
		public virtual void Initialize(ReferenceHub target)
		{
			this.Owner = target;
		}

		// Token: 0x060034F1 RID: 13553 RVA: 0x00002E9A File Offset: 0x0000109A
		public virtual void Dispose()
		{
		}

		// Token: 0x04002859 RID: 10329
		public readonly Stopwatch TimeTracker = new Stopwatch();

		// Token: 0x0400285A RID: 10330
		private bool _isActive;
	}
}
