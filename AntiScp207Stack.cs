using System;

namespace CustomPlayerEffects
{
	// Token: 0x020008A9 RID: 2217
	[Serializable]
	public struct AntiScp207Stack : ICokeStack
	{
		// Token: 0x17000966 RID: 2406
		// (get) Token: 0x06003264 RID: 12900 RVA: 0x000A8CE2 File Offset: 0x000A6EE2
		// (set) Token: 0x06003265 RID: 12901 RVA: 0x000A8CEA File Offset: 0x000A6EEA
		public float PostProcessIntensity { readonly get; set; }

		// Token: 0x17000967 RID: 2407
		// (get) Token: 0x06003266 RID: 12902 RVA: 0x000A8CF3 File Offset: 0x000A6EF3
		// (set) Token: 0x06003267 RID: 12903 RVA: 0x000A8CFB File Offset: 0x000A6EFB
		public float SpeedMultiplier { readonly get; set; }

		// Token: 0x0400274E RID: 10062
		public float HealAmount;
	}
}
