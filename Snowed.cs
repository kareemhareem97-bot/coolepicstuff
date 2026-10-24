using System;
using CustomPlayerEffects;
using MapGeneration.Holidays;

// Token: 0x02000032 RID: 50
public class Snowed : StatusEffectBase, IHolidayEffect
{
	// Token: 0x1700004C RID: 76
	// (get) Token: 0x06000117 RID: 279 RVA: 0x000058C0 File Offset: 0x00003AC0
	public HolidayType[] TargetHolidays { get; } = new HolidayType[]
	{
		HolidayType.Christmas,
		HolidayType.AprilFools
	};
}
