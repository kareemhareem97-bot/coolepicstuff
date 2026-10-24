using System;

namespace CustomPlayerEffects.Danger
{
	// Token: 0x02000915 RID: 2325
	public class CachedEncounterDanger : ExpiringDanger
	{
		// Token: 0x17000A74 RID: 2676
		// (get) Token: 0x0600350E RID: 13582 RVA: 0x000ADC24 File Offset: 0x000ABE24
		// (set) Token: 0x0600350F RID: 13583 RVA: 0x000ADC2C File Offset: 0x000ABE2C
		public ReferenceHub EncounteredHub { get; private set; }

		// Token: 0x06003510 RID: 13584 RVA: 0x000ADC35 File Offset: 0x000ABE35
		public CachedEncounterDanger(float dangerValue, ReferenceHub owner, ReferenceHub target) : base(dangerValue, owner)
		{
			this.EncounteredHub = target;
		}
	}
}
