using System;
using MapGeneration.Holidays;

// Token: 0x020001AB RID: 427
[Serializable]
public struct ClutterStruct
{
	// Token: 0x0400079B RID: 1947
	public string descriptor;

	// Token: 0x0400079C RID: 1948
	public Clutter clutterComponent;

	// Token: 0x0400079D RID: 1949
	public float chanceToSpawn;

	// Token: 0x0400079E RID: 1950
	public bool invertTimespan;

	// Token: 0x0400079F RID: 1951
	public HolidayType[] targetHolidays;
}
