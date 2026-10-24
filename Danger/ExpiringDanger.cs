using System;

namespace CustomPlayerEffects.Danger
{
	// Token: 0x02000916 RID: 2326
	public class ExpiringDanger : DangerStackBase
	{
		// Token: 0x17000A75 RID: 2677
		// (get) Token: 0x06003511 RID: 13585 RVA: 0x000ADC46 File Offset: 0x000ABE46
		// (set) Token: 0x06003512 RID: 13586 RVA: 0x000ADC4E File Offset: 0x000ABE4E
		public override float DangerValue { get; set; }

		// Token: 0x06003513 RID: 13587 RVA: 0x000ADC57 File Offset: 0x000ABE57
		public ExpiringDanger(float dangerValue, ReferenceHub owner)
		{
			this.DangerValue = dangerValue;
			base.Owner = owner;
			this.Initialize(base.Owner);
		}

		// Token: 0x06003514 RID: 13588 RVA: 0x000ADC79 File Offset: 0x000ABE79
		public override void Initialize(ReferenceHub target)
		{
			this.TimeTracker.Start();
		}
	}
}
