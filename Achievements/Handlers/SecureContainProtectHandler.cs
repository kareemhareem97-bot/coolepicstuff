using System;
using PlayerRoles;
using PlayerStatsSystem;
using Utils.NonAllocLINQ;

namespace Achievements.Handlers
{
	// Token: 0x02000B13 RID: 2835
	public class SecureContainProtectHandler : AchievementHandlerBase
	{
		// Token: 0x06003FF7 RID: 16375 RVA: 0x000D463C File Offset: 0x000D283C
		internal override void OnInitialize()
		{
			PlayerStats.OnAnyPlayerDied += this.PlayerDied;
		}

		// Token: 0x06003FF8 RID: 16376 RVA: 0x000D4650 File Offset: 0x000D2850
		private void PlayerDied(ReferenceHub plr, DamageHandlerBase handler)
		{
			if (plr.GetTeam() == Team.SCPs)
			{
				AttackerDamageHandler attackerDamageHandler = handler as AttackerDamageHandler;
				if (attackerDamageHandler != null && !(attackerDamageHandler.Attacker.Hub == null) && attackerDamageHandler.Attacker.Hub.GetTeam() == Team.FoundationForces)
				{
					if (!ReferenceHub.AllHubs.Any((ReferenceHub hub) => hub.GetTeam() == Team.SCPs && hub.GetRoleId() != RoleTypeId.Scp079 && hub.GetRoleId() != RoleTypeId.Scp0492 && hub != plr))
					{
						AchievementHandlerBase.ServerAchieve(attackerDamageHandler.Attacker.Hub.connectionToClient, AchievementName.SecureContainProtect);
					}
					return;
				}
			}
		}
	}
}
