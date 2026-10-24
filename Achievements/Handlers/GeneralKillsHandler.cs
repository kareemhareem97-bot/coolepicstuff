using System;
using System.Runtime.CompilerServices;
using CustomPlayerEffects;
using GameCore;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.MicroHID;
using Mirror;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp096;
using PlayerRoles.PlayableScps.Scp939;
using PlayerStatsSystem;
using UnityEngine;

namespace Achievements.Handlers
{
	// Token: 0x02000AF8 RID: 2808
	public class GeneralKillsHandler : AchievementHandlerBase
	{
		// Token: 0x06003FAE RID: 16302 RVA: 0x000D37EF File Offset: 0x000D19EF
		internal override void OnInitialize()
		{
			Action<ReferenceHub, DamageHandlerBase> value;
			if ((value = GeneralKillsHandler.<>O.<0>__HandleDeath) == null)
			{
				value = (GeneralKillsHandler.<>O.<0>__HandleDeath = new Action<ReferenceHub, DamageHandlerBase>(GeneralKillsHandler.HandleDeath));
			}
			PlayerStats.OnAnyPlayerDied += value;
		}

		// Token: 0x06003FAF RID: 16303 RVA: 0x000D3814 File Offset: 0x000D1A14
		private static void HandleDeath(ReferenceHub deadPlayer, DamageHandlerBase handler)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			AttackerDamageHandler attackerDamageHandler = handler as AttackerDamageHandler;
			if (attackerDamageHandler != null && attackerDamageHandler.Attacker.Hub != null)
			{
				GeneralKillsHandler.HandleAttackerKill(deadPlayer, attackerDamageHandler);
			}
			ExplosionDamageHandler explosionDamageHandler = handler as ExplosionDamageHandler;
			if (explosionDamageHandler == null)
			{
				Scp939DamageHandler scp939DamageHandler = handler as Scp939DamageHandler;
				if (scp939DamageHandler == null)
				{
					MicroHidDamageHandler microHidDamageHandler = handler as MicroHidDamageHandler;
					if (microHidDamageHandler == null)
					{
						UniversalDamageHandler universalDamageHandler = handler as UniversalDamageHandler;
						if (universalDamageHandler == null)
						{
							return;
						}
						if (universalDamageHandler.TranslationId == DeathTranslations.Tesla.Id && deadPlayer.inventory.CurInstance is MicroHIDItem)
						{
							GeneralKillsHandler.Send(deadPlayer, AchievementName.Overcurrent);
						}
					}
					else if (microHidDamageHandler.Attacker.Hub != null && deadPlayer.IsSCP(true))
					{
						GeneralKillsHandler.Send(microHidDamageHandler.Attacker.Hub, AchievementName.MicrowaveMeal);
						return;
					}
				}
				else if (scp939DamageHandler.Attacker.Hub != null)
				{
					GeneralKillsHandler.HandleScp939Kill(deadPlayer, scp939DamageHandler);
					return;
				}
			}
			else if (explosionDamageHandler.Attacker.Hub != null && explosionDamageHandler.Attacker.Hub != deadPlayer && explosionDamageHandler.ExplosionType == ExplosionType.Grenade)
			{
				GeneralKillsHandler.Send(explosionDamageHandler.Attacker.Hub, AchievementName.FireInTheHole);
				return;
			}
		}

		// Token: 0x06003FB0 RID: 16304 RVA: 0x000D3944 File Offset: 0x000D1B44
		private static void HandleScp939Kill(ReferenceHub deadPlayer, Scp939DamageHandler scp939DH)
		{
			if (!deadPlayer.IsHuman())
			{
				return;
			}
			if (scp939DH.Attacker.Role != RoleTypeId.Scp939)
			{
				return;
			}
			AmnesiaVision amnesiaVision;
			if (!deadPlayer.playerEffectsController.TryGetEffect<AmnesiaVision>(out amnesiaVision))
			{
				return;
			}
			if (amnesiaVision.IsEnabled && scp939DH.Scp939DamageType == Scp939DamageType.LungeTarget)
			{
				GeneralKillsHandler.Send(scp939DH.Attacker.Hub, AchievementName.AmnesticAmbush);
			}
		}

		// Token: 0x06003FB1 RID: 16305 RVA: 0x000D39A0 File Offset: 0x000D1BA0
		private static void HandleAttackerKill(ReferenceHub deadPlayer, AttackerDamageHandler aDH)
		{
			ReferenceHub hub = aDH.Attacker.Hub;
			RoleTypeId roleId = deadPlayer.GetRoleId();
			RoleTypeId roleTypeId = roleId;
			if (roleTypeId != RoleTypeId.Scp096)
			{
				if (roleTypeId == RoleTypeId.Scp0492 && hub.IsHuman() && aDH is JailbirdDamageHandler)
				{
					GeneralKillsHandler.Send(hub, AchievementName.UndeadSpaceProgram);
				}
			}
			else if ((deadPlayer.roleManager.CurrentRole as Scp096Role).IsRageState(Scp096RageState.Distressed))
			{
				GeneralKillsHandler.Send(hub, AchievementName.Pacified);
			}
			switch (aDH.Attacker.Role.GetTeam())
			{
			case Team.SCPs:
				if (RoundStart.RoundStartTimer.Elapsed.TotalSeconds < 60.0)
				{
					GeneralKillsHandler.Send(hub, AchievementName.AnomalouslyEfficient);
				}
				break;
			case Team.Scientists:
				if (deadPlayer.IsSCP(true))
				{
					GeneralKillsHandler.Send(hub, AchievementName.SomethingDoneRight);
				}
				break;
			case Team.ClassD:
				if (roleId == RoleTypeId.Scientist && deadPlayer.inventory.CurInstance is KeycardItem)
				{
					GeneralKillsHandler.Send(hub, AchievementName.AccessGranted);
				}
				break;
			}
			FirearmDamageHandler firearmDamageHandler = aDH as FirearmDamageHandler;
			if (firearmDamageHandler != null && firearmDamageHandler.WeaponType == ItemType.GunRevolver)
			{
				if (!deadPlayer.IsHuman() && deadPlayer.GetRoleId() != RoleTypeId.Scp0492)
				{
					return;
				}
				if (firearmDamageHandler.Hitbox != HitboxType.Headshot)
				{
					return;
				}
				if (Vector3.Distance(deadPlayer.transform.position, hub.transform.position) < 25f)
				{
					return;
				}
				GeneralKillsHandler.Send(hub, AchievementName.ArizonaRanger);
			}
		}

		// Token: 0x06003FB2 RID: 16306 RVA: 0x000D3AE8 File Offset: 0x000D1CE8
		private static void Send(ReferenceHub hub, AchievementName name)
		{
			if (hub != null)
			{
				AchievementHandlerBase.ServerAchieve(hub.networkIdentity.connectionToClient, name);
			}
		}

		// Token: 0x04002E82 RID: 11906
		private const int AnomalouslyEfficientTime = 60;

		// Token: 0x04002E83 RID: 11907
		private const int ArizonaRangerDistance = 25;

		// Token: 0x02000AF9 RID: 2809
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002E84 RID: 11908
			public static Action<ReferenceHub, DamageHandlerBase> <0>__HandleDeath;
		}
	}
}
