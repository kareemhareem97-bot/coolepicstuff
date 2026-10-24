using System;
using Interactables.Interobjects.DoorUtils;
using MapGeneration.Holidays;

namespace CustomPlayerEffects
{
	// Token: 0x020008E7 RID: 2279
	public class TemporaryBypass : StatusEffectBase, IHolidayEffect, IDoorPermissionProvider
	{
		// Token: 0x17000A3C RID: 2620
		// (get) Token: 0x06003453 RID: 13395 RVA: 0x000AC734 File Offset: 0x000AA934
		public HolidayType[] TargetHolidays { get; } = new HolidayType[]
		{
			HolidayType.Halloween
		};

		// Token: 0x17000A3D RID: 2621
		// (get) Token: 0x06003454 RID: 13396 RVA: 0x000AC73C File Offset: 0x000AA93C
		public bool IsFullBypass
		{
			get
			{
				return base.Intensity > 1;
			}
		}

		// Token: 0x17000A3E RID: 2622
		// (get) Token: 0x06003455 RID: 13397 RVA: 0x000AC747 File Offset: 0x000AA947
		public bool IsScpBypass
		{
			get
			{
				return base.Intensity == 1;
			}
		}

		// Token: 0x17000A3F RID: 2623
		// (get) Token: 0x06003456 RID: 13398 RVA: 0x000042EB File Offset: 0x000024EB
		public PermissionUsed PermissionsUsedCallback
		{
			get
			{
				return null;
			}
		}

		// Token: 0x06003457 RID: 13399 RVA: 0x000AC752 File Offset: 0x000AA952
		public DoorPermissionFlags GetPermissions(IDoorPermissionRequester requester)
		{
			if (this.IsFullBypass)
			{
				return DoorPermissionFlags.All;
			}
			if (this.IsScpBypass)
			{
				return DoorPermissionFlags.ScpOverride;
			}
			return DoorPermissionFlags.None;
		}

		// Token: 0x04002825 RID: 10277
		public const byte ScpBypass = 1;

		// Token: 0x04002826 RID: 10278
		public const byte FullBypass = 2;
	}
}
