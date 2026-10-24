using System;
using System.Runtime.CompilerServices;
using PlayerRoles;

namespace Achievements.Handlers
{
	// Token: 0x02000AF1 RID: 2801
	public class CompleteTheMissionHandler : AchievementHandlerBase
	{
		// Token: 0x06003FA1 RID: 16289 RVA: 0x000D34DD File Offset: 0x000D16DD
		internal override void OnInitialize()
		{
			RoundSummary.RoundEnded value;
			if ((value = CompleteTheMissionHandler.<>O.<0>__OnRoundEnded) == null)
			{
				value = (CompleteTheMissionHandler.<>O.<0>__OnRoundEnded = new RoundSummary.RoundEnded(CompleteTheMissionHandler.OnRoundEnded));
			}
			RoundSummary.OnRoundEnded += value;
		}

		// Token: 0x06003FA2 RID: 16290 RVA: 0x000D3500 File Offset: 0x000D1700
		private static void OnRoundEnded(RoundSummary.LeadingTeam leadingTeam, RoundSummary.SumInfo_ClassList sumInfo)
		{
			ReferenceHub referenceHub = null;
			int num = 0;
			foreach (ReferenceHub referenceHub2 in ReferenceHub.AllHubs)
			{
				Team team = referenceHub2.GetTeam();
				if (team != Team.Dead && leadingTeam == CompleteTheMissionHandler.GetLeadingTeam(team))
				{
					num++;
					referenceHub = referenceHub2;
					if (num > 1)
					{
						break;
					}
				}
			}
			if (num == 1)
			{
				AchievementHandlerBase.ServerAchieve(referenceHub.connectionToClient, AchievementName.CompleteTheMission);
			}
		}

		// Token: 0x06003FA3 RID: 16291 RVA: 0x000D3584 File Offset: 0x000D1784
		private static RoundSummary.LeadingTeam GetLeadingTeam(Team team)
		{
			switch (team)
			{
			case Team.SCPs:
				return RoundSummary.LeadingTeam.Anomalies;
			case Team.FoundationForces:
			case Team.Scientists:
				return RoundSummary.LeadingTeam.FacilityForces;
			case Team.ChaosInsurgency:
			case Team.ClassD:
				return RoundSummary.LeadingTeam.ChaosInsurgency;
			default:
				return RoundSummary.LeadingTeam.Draw;
			}
		}

		// Token: 0x02000AF2 RID: 2802
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002E7C RID: 11900
			public static RoundSummary.RoundEnded <0>__OnRoundEnded;
		}
	}
}
