using System;
using GameCore;
using MapGeneration.Holidays;

namespace CustomPlayerEffects
{
	// Token: 0x020008FC RID: 2300
	public interface IHolidayEffect
	{
		// Token: 0x17000A4D RID: 2637
		// (get) Token: 0x06003490 RID: 13456 RVA: 0x000ACEDC File Offset: 0x000AB0DC
		bool IsAvailable
		{
			get
			{
				if (this.TargetHolidays.IsAnyHolidayActive(true))
				{
					return true;
				}
				GameCore.Version.VersionType buildType = GameCore.Version.BuildType;
				return buildType == GameCore.Version.VersionType.Development || buildType == GameCore.Version.VersionType.Nightly;
			}
		}

		// Token: 0x17000A4E RID: 2638
		// (get) Token: 0x06003491 RID: 13457
		HolidayType[] TargetHolidays { get; }
	}
}
