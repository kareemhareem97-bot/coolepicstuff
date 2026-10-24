using System;
using InventorySystem.Disarming;
using PlayerRoles;

namespace Achievements.Handlers
{
	// Token: 0x02000AF0 RID: 2800
	public class ChangeInCommandHandler : AchievementHandlerBase
	{
		// Token: 0x06003F9E RID: 16286 RVA: 0x000D3491 File Offset: 0x000D1691
		internal override void OnInitialize()
		{
			DisarmingHandlers.OnPlayerDisarmed += this.OnPlayerDisarmed;
		}

		// Token: 0x06003F9F RID: 16287 RVA: 0x000D34A4 File Offset: 0x000D16A4
		private void OnPlayerDisarmed(ReferenceHub disarmerHub, ReferenceHub targetHub)
		{
			if (disarmerHub == null || targetHub == null)
			{
				return;
			}
			if (targetHub.GetTeam() == Team.FoundationForces && disarmerHub.GetRoleId() == RoleTypeId.ClassD)
			{
				AchievementHandlerBase.ServerAchieve(disarmerHub.networkIdentity.connectionToClient, AchievementName.ChangeInCommand);
			}
		}
	}
}
