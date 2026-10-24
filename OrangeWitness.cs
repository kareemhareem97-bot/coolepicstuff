using System;
using MapGeneration.Holidays;

namespace CustomPlayerEffects
{
	// Token: 0x020008C6 RID: 2246
	public class OrangeWitness : StatusEffectBase, IHolidayEffect
	{
		// Token: 0x170009BC RID: 2492
		// (get) Token: 0x06003327 RID: 13095 RVA: 0x000AA0D0 File Offset: 0x000A82D0
		public HolidayType[] TargetHolidays { get; } = new HolidayType[]
		{
			HolidayType.Halloween
		};
	}
}
