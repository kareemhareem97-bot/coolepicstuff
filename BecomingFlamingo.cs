using System;
using MapGeneration.Holidays;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp1507;

namespace CustomPlayerEffects
{
	// Token: 0x0200089D RID: 2205
	public class BecomingFlamingo : StatusEffectBase, IHolidayEffect
	{
		// Token: 0x1700094D RID: 2381
		// (get) Token: 0x0600321C RID: 12828 RVA: 0x000A83A1 File Offset: 0x000A65A1
		public HolidayType[] TargetHolidays { get; } = new HolidayType[]
		{
			HolidayType.Christmas,
			HolidayType.AprilFools
		};

		// Token: 0x0600321D RID: 12829 RVA: 0x000A83A9 File Offset: 0x000A65A9
		internal override void OnRoleChanged(PlayerRoleBase previousRole, PlayerRoleBase newRole)
		{
			if (newRole is Scp1507Role)
			{
				base.OnRoleChanged(previousRole, newRole);
			}
		}

		// Token: 0x0600321E RID: 12830 RVA: 0x00002E9A File Offset: 0x0000109A
		internal override void OnDeath(PlayerRoleBase previousRole)
		{
		}
	}
}
