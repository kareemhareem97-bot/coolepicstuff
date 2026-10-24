using System;
using MapGeneration.Holidays;

namespace CustomPlayerEffects
{
	// Token: 0x020008EB RID: 2283
	public class TraumatizedByEvil : StatusEffectBase, IHealableEffect, IHolidayEffect
	{
		// Token: 0x17000A41 RID: 2625
		// (get) Token: 0x06003464 RID: 13412 RVA: 0x000AC88E File Offset: 0x000AAA8E
		public HolidayType[] TargetHolidays { get; } = new HolidayType[]
		{
			HolidayType.Halloween
		};

		// Token: 0x06003465 RID: 13413 RVA: 0x00004242 File Offset: 0x00002442
		public bool IsHealable(ItemType item)
		{
			return item == ItemType.SCP500;
		}
	}
}
