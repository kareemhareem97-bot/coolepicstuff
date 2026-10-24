using System;
using Mirror;
using RelativePositioning;
using UnityEngine;

namespace Hazards
{
	// Token: 0x02000853 RID: 2131
	[RequireComponent(typeof(TransformWaypointFollower))]
	public abstract class TemporaryHazard : EnvironmentalHazard
	{
		// Token: 0x170008F6 RID: 2294
		// (get) Token: 0x060030B2 RID: 12466 RVA: 0x000A23C1 File Offset: 0x000A05C1
		// (set) Token: 0x060030B3 RID: 12467 RVA: 0x000A23D3 File Offset: 0x000A05D3
		public override bool IsActive
		{
			get
			{
				return !this._destroyed && this._active;
			}
			set
			{
				this._active = value;
			}
		}

		// Token: 0x170008F7 RID: 2295
		// (get) Token: 0x060030B4 RID: 12468
		// (set) Token: 0x060030B5 RID: 12469
		public abstract float HazardDuration { get; set; }

		// Token: 0x170008F8 RID: 2296
		// (get) Token: 0x060030B6 RID: 12470 RVA: 0x000A23DC File Offset: 0x000A05DC
		// (set) Token: 0x060030B7 RID: 12471 RVA: 0x000A23E4 File Offset: 0x000A05E4
		public virtual float DecaySpeed { get; set; } = 1f;

		// Token: 0x060030B8 RID: 12472 RVA: 0x000A23F0 File Offset: 0x000A05F0
		[Server]
		public virtual void ServerDestroy()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void Hazards.TemporaryHazard::ServerDestroy()' called when server was not active");
				return;
			}
			this._destroyed = true;
			this.IsActive = false;
			for (int i = base.AffectedPlayers.Count - 1; i >= 0; i--)
			{
				this.OnExit(base.AffectedPlayers[i]);
			}
		}

		// Token: 0x060030B9 RID: 12473 RVA: 0x000A244B File Offset: 0x000A064B
		protected override void Start()
		{
			base.Start();
			this.IsActive = true;
		}

		// Token: 0x060030BA RID: 12474 RVA: 0x000A245C File Offset: 0x000A065C
		protected override void Update()
		{
			base.Update();
			if (!NetworkServer.active || !this.IsActive)
			{
				return;
			}
			if (this.Elapsed > this.HazardDuration)
			{
				this.ServerDestroy();
				return;
			}
			this.Elapsed += this.DecaySpeed * Time.deltaTime;
		}

		// Token: 0x060030BC RID: 12476 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Weaved()
		{
			return true;
		}

		// Token: 0x0400259E RID: 9630
		public float Elapsed;

		// Token: 0x0400259F RID: 9631
		private bool _active;

		// Token: 0x040025A0 RID: 9632
		private bool _destroyed;
	}
}
