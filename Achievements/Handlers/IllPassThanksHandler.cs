using System;
using InventorySystem.Items.MicroHID;
using InventorySystem.Items.MicroHID.Modules;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp106;
using PlayerStatsSystem;

namespace Achievements.Handlers
{
	// Token: 0x02000AFF RID: 2815
	public class IllPassThanksHandler : AchievementHandlerBase
	{
		// Token: 0x06003FC3 RID: 16323 RVA: 0x000D3D02 File Offset: 0x000D1F02
		internal override void OnInitialize()
		{
			PlayerStats.OnAnyPlayerDied += this.HandleDeath;
			Scp106Attack.OnPlayerTeleported += this.PlayerTeleported;
		}

		// Token: 0x06003FC4 RID: 16324 RVA: 0x000D3D26 File Offset: 0x000D1F26
		private void PlayerTeleported(ReferenceHub scp106, ReferenceHub hub)
		{
			if (!this.CheckConditionsForVictim(hub))
			{
				return;
			}
			AchievementHandlerBase.ServerAchieve(scp106.connectionToClient, AchievementName.IllPassThanks);
		}

		// Token: 0x06003FC5 RID: 16325 RVA: 0x000D3D40 File Offset: 0x000D1F40
		private void HandleDeath(ReferenceHub deadPlayer, DamageHandlerBase handler)
		{
			AttackerDamageHandler attackerDamageHandler = handler as AttackerDamageHandler;
			if (attackerDamageHandler == null)
			{
				return;
			}
			ReferenceHub hub = attackerDamageHandler.Attacker.Hub;
			if (hub == null || !hub.IsSCP(true) || !this.CheckConditionsForVictim(deadPlayer))
			{
				return;
			}
			AchievementHandlerBase.ServerAchieve(hub.connectionToClient, AchievementName.IllPassThanks);
		}

		// Token: 0x06003FC6 RID: 16326 RVA: 0x000D3D90 File Offset: 0x000D1F90
		private bool CheckConditionsForVictim(ReferenceHub hub)
		{
			MicroHIDItem microHIDItem = hub.inventory.CurInstance as MicroHIDItem;
			if (microHIDItem != null && microHIDItem != null)
			{
				MicroHidPhase phase = microHIDItem.CycleController.Phase;
				return phase == MicroHidPhase.WindingUp || phase == MicroHidPhase.Firing;
			}
			return false;
		}
	}
}
