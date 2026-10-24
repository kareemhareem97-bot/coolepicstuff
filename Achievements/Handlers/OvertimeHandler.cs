using System;
using System.Runtime.CompilerServices;
using PlayerRoles;

namespace Achievements.Handlers
{
	// Token: 0x02000B0A RID: 2826
	public class OvertimeHandler : AchievementHandlerBase
	{
		// Token: 0x06003FE5 RID: 16357 RVA: 0x000D42D8 File Offset: 0x000D24D8
		internal override void OnInitialize()
		{
			RoundSummary.RoundEnded value;
			if ((value = OvertimeHandler.<>O.<0>__OnRoundEnded) == null)
			{
				value = (OvertimeHandler.<>O.<0>__OnRoundEnded = new RoundSummary.RoundEnded(OvertimeHandler.OnRoundEnded));
			}
			RoundSummary.OnRoundEnded += value;
		}

		// Token: 0x06003FE6 RID: 16358 RVA: 0x000D42FC File Offset: 0x000D24FC
		private static void OnRoundEnded(RoundSummary.LeadingTeam leading, RoundSummary.SumInfo_ClassList sumInfo)
		{
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				PlayerRoleBase currentRole = referenceHub.roleManager.CurrentRole;
				if (currentRole == null || currentRole.RoleTypeId != RoleTypeId.FacilityGuard)
				{
					goto IL_40;
				}
				RoleChangeReason serverSpawnReason = currentRole.ServerSpawnReason;
				if (serverSpawnReason - RoleChangeReason.RoundStart > 1)
				{
					goto IL_40;
				}
				bool flag = true;
				IL_43:
				if (flag)
				{
					AchievementHandlerBase.ServerAchieve(referenceHub.connectionToClient, AchievementName.Overtime);
					continue;
				}
				continue;
				IL_40:
				flag = false;
				goto IL_43;
			}
		}

		// Token: 0x04002E92 RID: 11922
		private const RoleTypeId TargetRole = RoleTypeId.FacilityGuard;

		// Token: 0x02000B0B RID: 2827
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002E93 RID: 11923
			public static RoundSummary.RoundEnded <0>__OnRoundEnded;
		}
	}
}
