using System;
using System.Runtime.CompilerServices;
using Mirror;
using PlayerRoles;

namespace Achievements.Handlers
{
	// Token: 0x02000B0D RID: 2829
	public class RespawnHandler : AchievementHandlerBase
	{
		// Token: 0x06003FE9 RID: 16361 RVA: 0x000D4388 File Offset: 0x000D2588
		internal override void OnInitialize()
		{
			PlayerRoleManager.ServerRoleSet value;
			if ((value = RespawnHandler.<>O.<0>__OnRespawned) == null)
			{
				value = (RespawnHandler.<>O.<0>__OnRespawned = new PlayerRoleManager.ServerRoleSet(RespawnHandler.OnRespawned));
			}
			PlayerRoleManager.OnServerRoleSet += value;
		}

		// Token: 0x06003FEA RID: 16362 RVA: 0x000D43AC File Offset: 0x000D25AC
		private static void OnRespawned(ReferenceHub userHub, RoleTypeId newRole, RoleChangeReason reason)
		{
			PlayerRoleBase playerRoleBase;
			if ((reason != RoleChangeReason.Respawn && reason != RoleChangeReason.RespawnMiniwave) || !newRole.TryGetRoleTemplate(out playerRoleBase))
			{
				return;
			}
			NetworkConnection connectionToClient = userHub.connectionToClient;
			Team team = playerRoleBase.Team;
			if (team == Team.FoundationForces)
			{
				AchievementHandlerBase.ServerAchieve(connectionToClient, AchievementName.LightsOut);
				return;
			}
			if (team == Team.ChaosInsurgency)
			{
				AchievementHandlerBase.ServerAchieve(connectionToClient, AchievementName.DeltaCommand);
			}
		}

		// Token: 0x02000B0E RID: 2830
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002E94 RID: 11924
			public static PlayerRoleManager.ServerRoleSet <0>__OnRespawned;
		}
	}
}
