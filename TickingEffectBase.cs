using System;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x0200090A RID: 2314
	public abstract class TickingEffectBase : StatusEffectBase
	{
		// Token: 0x060034DF RID: 13535
		protected abstract void OnTick();

		// Token: 0x060034E0 RID: 13536 RVA: 0x000AD719 File Offset: 0x000AB919
		protected override void Enabled()
		{
			base.Enabled();
			this._timeTillTick = this.TimeBetweenTicks;
		}

		// Token: 0x060034E1 RID: 13537 RVA: 0x000AD730 File Offset: 0x000AB930
		protected override void OnEffectUpdate()
		{
			base.OnEffectUpdate();
			this._timeTillTick -= Time.deltaTime;
			if (this._timeTillTick > 0f)
			{
				return;
			}
			this._timeTillTick += this.TimeBetweenTicks;
			this.OnTick();
		}

		// Token: 0x04002855 RID: 10325
		[Tooltip("Used to track intervals/timers/etc without every effect needing to redefine a unique float.")]
		public float TimeBetweenTicks = 1f;

		// Token: 0x04002856 RID: 10326
		private float _timeTillTick;
	}
}
