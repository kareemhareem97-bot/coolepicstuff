using System;
using UnityEngine;

namespace Cassie
{
	// Token: 0x02000AAD RID: 2733
	public class Cassie079RecontainAnnouncement : CassieAnnouncement
	{
		// Token: 0x17000E0A RID: 3594
		// (get) Token: 0x06003DAE RID: 15790 RVA: 0x000CB9B9 File Offset: 0x000C9BB9
		public override bool ReadyToPlay
		{
			get
			{
				return base.ReadyToPlay && this._remainingDelay <= 0f;
			}
		}

		// Token: 0x06003DAF RID: 15791 RVA: 0x000CB9D5 File Offset: 0x000C9BD5
		public Cassie079RecontainAnnouncement(Action callback, bool delay, CassieTtsPayload payload) : base(payload, 0f, 1f)
		{
			this._callback = callback;
			this._remainingDelay = (delay ? 3f : 0f);
		}

		// Token: 0x06003DB0 RID: 15792 RVA: 0x000CBA04 File Offset: 0x000C9C04
		public override void UpdateQueued()
		{
			base.UpdateQueued();
			this._remainingDelay -= Time.deltaTime;
		}

		// Token: 0x06003DB1 RID: 15793 RVA: 0x000CBA1E File Offset: 0x000C9C1E
		public override void OnFinishedPlaying()
		{
			base.OnFinishedPlaying();
			Action callback = this._callback;
			if (callback == null)
			{
				return;
			}
			callback();
		}

		// Token: 0x04002D1D RID: 11549
		private const float MinDelay = 3f;

		// Token: 0x04002D1E RID: 11550
		private readonly Action _callback;

		// Token: 0x04002D1F RID: 11551
		private float _remainingDelay;
	}
}
