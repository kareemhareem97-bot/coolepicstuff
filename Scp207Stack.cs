using System;

namespace CustomPlayerEffects
{
	// Token: 0x020008AE RID: 2222
	[Serializable]
	public struct Scp207Stack : ICokeStack
	{
		// Token: 0x1700097A RID: 2426
		// (get) Token: 0x0600328D RID: 12941 RVA: 0x000A9006 File Offset: 0x000A7206
		// (set) Token: 0x0600328E RID: 12942 RVA: 0x000A900E File Offset: 0x000A720E
		public float PostProcessIntensity { readonly get; set; }

		// Token: 0x1700097B RID: 2427
		// (get) Token: 0x0600328F RID: 12943 RVA: 0x000A9017 File Offset: 0x000A7217
		// (set) Token: 0x06003290 RID: 12944 RVA: 0x000A901F File Offset: 0x000A721F
		public float SpeedMultiplier { readonly get; set; }

		// Token: 0x04002757 RID: 10071
		public float DamageAmount;
	}
}
