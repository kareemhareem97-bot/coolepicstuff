using System;
using System.Collections.Generic;
using InventorySystem.Disarming;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;

namespace Achievements.Handlers
{
	// Token: 0x02000B03 RID: 2819
	public class JustResourcesHandler : AchievementHandlerBase
	{
		// Token: 0x06003FCE RID: 16334 RVA: 0x000D3E7D File Offset: 0x000D207D
		internal override void OnInitialize()
		{
			PlayerStats.OnAnyPlayerDied += this.HandleDeath;
			DisarmingHandlers.OnPlayerDisarmed += this.HandleDisarmed;
		}

		// Token: 0x06003FCF RID: 16335 RVA: 0x000D3EA1 File Offset: 0x000D20A1
		internal override void OnRoundStarted()
		{
			this._interactedPlayers.Clear();
		}

		// Token: 0x06003FD0 RID: 16336 RVA: 0x000D3EB0 File Offset: 0x000D20B0
		private void HandleDisarmed(ReferenceHub disarmerHub, ReferenceHub targetHub)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			if (disarmerHub == null || targetHub == null)
			{
				return;
			}
			if (disarmerHub.GetTeam() != Team.Scientists)
			{
				return;
			}
			if (targetHub.GetTeam() != Team.ClassD)
			{
				return;
			}
			if (this._interactedPlayers.Contains(targetHub))
			{
				return;
			}
			this._interactedPlayers.Add(targetHub);
			AchievementHandlerBase.ServerAchieve(disarmerHub.networkIdentity.connectionToClient, AchievementName.JustResources);
		}

		// Token: 0x06003FD1 RID: 16337 RVA: 0x000D3F1C File Offset: 0x000D211C
		private void HandleDeath(ReferenceHub deadPlayer, DamageHandlerBase handler)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			AttackerDamageHandler attackerDamageHandler = handler as AttackerDamageHandler;
			if (attackerDamageHandler == null || attackerDamageHandler.Attacker.Hub == null)
			{
				return;
			}
			ReferenceHub hub = attackerDamageHandler.Attacker.Hub;
			if (hub.GetTeam() != Team.Scientists)
			{
				return;
			}
			if (deadPlayer.GetTeam() != Team.ClassD)
			{
				return;
			}
			if (this._interactedPlayers.Contains(deadPlayer))
			{
				return;
			}
			this._interactedPlayers.Add(hub);
			AchievementHandlerBase.ServerAchieve(hub.networkIdentity.connectionToClient, AchievementName.JustResources);
		}

		// Token: 0x04002E8A RID: 11914
		private List<ReferenceHub> _interactedPlayers = new List<ReferenceHub>();
	}
}
