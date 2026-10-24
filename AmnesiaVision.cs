using System;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x02000899 RID: 2201
	public class AmnesiaVision : StatusEffectBase, ISoundtrackMutingEffect
	{
		// Token: 0x17000945 RID: 2373
		// (get) Token: 0x06003206 RID: 12806 RVA: 0x00004094 File Offset: 0x00002294
		public bool MuteSoundtrack
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000946 RID: 2374
		// (get) Token: 0x06003207 RID: 12807 RVA: 0x000A80F5 File Offset: 0x000A62F5
		public float LastActive
		{
			get
			{
				return Time.timeSinceLevelLoad - this._lastTime;
			}
		}

		// Token: 0x06003208 RID: 12808 RVA: 0x000A8103 File Offset: 0x000A6303
		protected override void Enabled()
		{
			base.Enabled();
			this._lastTime = Time.timeSinceLevelLoad;
		}

		// Token: 0x0400271B RID: 10011
		private float _lastTime;
	}
}
