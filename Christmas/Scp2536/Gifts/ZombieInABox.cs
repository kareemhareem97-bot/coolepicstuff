using System;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp049;
using PlayerRoles.Spectating;
using Respawning.Waves;
using UnityEngine;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010F4 RID: 4340
	public class ZombieInABox : Scp2536AnomalousGiftBase
	{
		// Token: 0x17001405 RID: 5125
		// (get) Token: 0x0600608F RID: 24719 RVA: 0x00054ECB File Offset: 0x000530CB
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Three;
			}
		}

		// Token: 0x06006090 RID: 24720 RVA: 0x0014263C File Offset: 0x0014083C
		public override bool CanBeGranted(ReferenceHub hub)
		{
			if (!base.CanBeGranted(hub))
			{
				return false;
			}
			if (!(hub.roleManager.CurrentRole is Scp049Role))
			{
				return false;
			}
			int num = 0;
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				RoleTypeId roleId = referenceHub.GetRoleId();
				if (roleId == RoleTypeId.Scp0492 || roleId == RoleTypeId.ZombieFlamingo)
				{
					return false;
				}
				SpectatorRole spectatorRole = referenceHub.roleManager.CurrentRole as SpectatorRole;
				if (spectatorRole != null && spectatorRole.ReadyToRespawn)
				{
					num++;
				}
			}
			return num >= 1;
		}

		// Token: 0x06006091 RID: 24721 RVA: 0x001426EC File Offset: 0x001408EC
		public override void ServerGrant(ReferenceHub hub)
		{
			base.ServerGrant(hub);
			IFpcRole fpcRole = hub.roleManager.CurrentRole as IFpcRole;
			if (fpcRole == null)
			{
				return;
			}
			Vector3 forward = hub.PlayerCameraReference.forward;
			Vector3 position = fpcRole.FpcModule.Position;
			Team team = hub.GetTeam();
			int num = 0;
			foreach (ReferenceHub referenceHub in WaveSpawner.GetAvailablePlayers(team, null))
			{
				if (num >= 1)
				{
					break;
				}
				referenceHub.roleManager.ServerSetRole(RoleTypeId.Scp0492, RoleChangeReason.ItemUsage, ~RoleSpawnFlags.UseSpawnpoint, null);
				IFpcRole fpcRole2 = referenceHub.roleManager.CurrentRole as IFpcRole;
				if (fpcRole2 != null)
				{
					Vector3 safePosition = SafeLocationFinder.GetSafePosition(position, forward, 3f, fpcRole2.FpcModule.CharController);
					Vector3 eulerAngles = Quaternion.LookRotation(position - safePosition).eulerAngles;
					fpcRole2.FpcModule.ServerOverridePosition(safePosition);
					fpcRole2.FpcModule.ServerOverrideRotation(eulerAngles);
					num++;
				}
			}
		}

		// Token: 0x04004647 RID: 17991
		private const int MinimumSpectators = 1;

		// Token: 0x04004648 RID: 17992
		private const float MaxSpawnDistance = 3f;

		// Token: 0x04004649 RID: 17993
		private const RoleTypeId RespawnRole = RoleTypeId.Scp0492;
	}
}
