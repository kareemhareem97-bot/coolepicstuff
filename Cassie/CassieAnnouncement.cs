using System;

namespace Cassie
{
	// Token: 0x02000AAE RID: 2734
	public class CassieAnnouncement
	{
		// Token: 0x17000E0B RID: 3595
		// (get) Token: 0x06003DB2 RID: 15794 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public virtual bool ReadyToPlay
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000E0C RID: 3596
		// (get) Token: 0x06003DB3 RID: 15795 RVA: 0x000CBA36 File Offset: 0x000C9C36
		public virtual float GlitchScale
		{
			get
			{
				return this._glitchScale * (float)(AlphaWarheadController.Detonated ? 2 : 1);
			}
		}

		// Token: 0x17000E0D RID: 3597
		// (get) Token: 0x06003DB4 RID: 15796 RVA: 0x000CBA4B File Offset: 0x000C9C4B
		public virtual float PostAnnouncementCooldown
		{
			get
			{
				if (!this.Payload.PlayBackground)
				{
					return 1f;
				}
				return 3.1f;
			}
		}

		// Token: 0x06003DB5 RID: 15797 RVA: 0x000CBA65 File Offset: 0x000C9C65
		public CassieAnnouncement(CassieTtsPayload payload, float priority = 0f, float glitchScale = 1f)
		{
			this.Payload = payload;
			this.Priority = priority;
			this._glitchScale = glitchScale;
		}

		// Token: 0x06003DB6 RID: 15798 RVA: 0x000CBA84 File Offset: 0x000C9C84
		public virtual void OnStartedPlaying()
		{
			string text = CassieGlitchifier.Glitchify(this.Payload.Content, this.GlitchScale);
			this.Payload = new CassieTtsPayload(this.Payload, text.ToUpperInvariant());
		}

		// Token: 0x06003DB7 RID: 15799 RVA: 0x00002E9A File Offset: 0x0000109A
		public virtual void UpdateQueued()
		{
		}

		// Token: 0x06003DB8 RID: 15800 RVA: 0x00002E9A File Offset: 0x0000109A
		public virtual void UpdatePlayed()
		{
		}

		// Token: 0x06003DB9 RID: 15801 RVA: 0x00002E9A File Offset: 0x0000109A
		public virtual void OnFinishedPlaying()
		{
		}

		// Token: 0x06003DBA RID: 15802 RVA: 0x00002E9A File Offset: 0x0000109A
		public virtual void OnCancelled()
		{
		}

		// Token: 0x04002D20 RID: 11552
		private readonly float _glitchScale;

		// Token: 0x04002D21 RID: 11553
		public CassieTtsPayload Payload;

		// Token: 0x04002D22 RID: 11554
		public float Priority;
	}
}
