using System;

namespace CustomPlayerEffects.Danger
{
	// Token: 0x0200091B RID: 2331
	public class WarheadDanger : DangerStackBase
	{
		// Token: 0x17000A7B RID: 2683
		// (get) Token: 0x0600352D RID: 13613 RVA: 0x000AE10A File Offset: 0x000AC30A
		// (set) Token: 0x0600352E RID: 13614 RVA: 0x000AE112 File Offset: 0x000AC312
		public override float DangerValue { get; set; } = 1f;

		// Token: 0x0600352F RID: 13615 RVA: 0x000AE11B File Offset: 0x000AC31B
		public override void Initialize(ReferenceHub target)
		{
			base.Initialize(target);
			this.UpdateState(AlphaWarheadController.InProgress);
			AlphaWarheadController.OnProgressChanged += this.UpdateState;
			AlphaWarheadController.OnDetonated += this.OnDetonated;
		}

		// Token: 0x06003530 RID: 13616 RVA: 0x000AE151 File Offset: 0x000AC351
		public override void Dispose()
		{
			base.Dispose();
			AlphaWarheadController.OnProgressChanged -= this.UpdateState;
			AlphaWarheadController.OnDetonated -= this.OnDetonated;
		}

		// Token: 0x06003531 RID: 13617 RVA: 0x000AE17B File Offset: 0x000AC37B
		private void UpdateState(bool warheadActive)
		{
			this.IsActive = (warheadActive && !AlphaWarheadController.Detonated);
		}

		// Token: 0x06003532 RID: 13618 RVA: 0x000AE191 File Offset: 0x000AC391
		private void OnDetonated()
		{
			this.UpdateState(AlphaWarheadController.InProgress);
		}
	}
}
