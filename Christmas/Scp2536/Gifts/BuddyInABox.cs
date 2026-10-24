using System;
using System.Collections.Generic;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Spectating;
using Respawning;
using Respawning.Waves;
using UnityEngine;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010D8 RID: 4312
	public class BuddyInABox : Scp2536ItemGift
	{
		// Token: 0x170013E5 RID: 5093
		// (get) Token: 0x06006003 RID: 24579 RVA: 0x00040288 File Offset: 0x0003E488
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Two;
			}
		}

		// Token: 0x170013E6 RID: 5094
		// (get) Token: 0x06006004 RID: 24580 RVA: 0x001410D9 File Offset: 0x0013F2D9
		protected override Scp2536Reward[] Rewards
		{
			get
			{
				return new Scp2536Reward[]
				{
					new Scp2536Reward(ItemType.Adrenaline, 100f)
				};
			}
		}

		// Token: 0x06006005 RID: 24581 RVA: 0x001410F4 File Offset: 0x0013F2F4
		public override bool CanBeGranted(ReferenceHub hub)
		{
			if (!base.CanBeGranted(hub))
			{
				return false;
			}
			Team team = hub.GetTeam();
			if (!BuddyInABox.TeamToRole.ContainsKey(team))
			{
				return false;
			}
			int num = 0;
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				if (!(referenceHub == hub))
				{
					if (team == referenceHub.GetTeam())
					{
						return false;
					}
					SpectatorRole spectatorRole = referenceHub.roleManager.CurrentRole as SpectatorRole;
					if (spectatorRole != null && spectatorRole.ReadyToRespawn)
					{
						num++;
					}
				}
			}
			return num < 2;
		}

		// Token: 0x06006006 RID: 24582 RVA: 0x001411A4 File Offset: 0x0013F3A4
		public override void ServerGrant(ReferenceHub hub)
		{
			IFpcRole fpcRole = hub.roleManager.CurrentRole as IFpcRole;
			if (fpcRole == null)
			{
				return;
			}
			Vector3 forward = hub.PlayerCameraReference.forward;
			Vector3 position = fpcRole.FpcModule.Position;
			Team team = hub.GetTeam();
			RoleTypeId newRole = BuddyInABox.TeamToRole[team];
			int num = 0;
			foreach (ReferenceHub referenceHub in WaveSpawner.GetAvailablePlayers(team, null))
			{
				if (num >= 2)
				{
					break;
				}
				referenceHub.roleManager.ServerSetRole(newRole, RoleChangeReason.ItemUsage, ~RoleSpawnFlags.UseSpawnpoint, null);
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

		// Token: 0x06006007 RID: 24583 RVA: 0x001412B4 File Offset: 0x0013F4B4
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			WaveManager.OnWaveSpawned += delegate(SpawnableWaveBase w, List<ReferenceHub> _)
			{
				BuddyInABox._waveSpawnedAlready = true;
			};
			CustomNetworkManager.OnClientReady += delegate()
			{
				BuddyInABox._waveSpawnedAlready = false;
			};
		}

		// Token: 0x06006009 RID: 24585 RVA: 0x00141311 File Offset: 0x0013F511
		// Note: this type is marked as 'beforefieldinit'.
		static BuddyInABox()
		{
			Dictionary<Team, RoleTypeId> dictionary = new Dictionary<Team, RoleTypeId>();
			dictionary[Team.ClassD] = RoleTypeId.ChaosFlamingo;
			dictionary[Team.ChaosInsurgency] = RoleTypeId.ChaosFlamingo;
			dictionary[Team.FoundationForces] = RoleTypeId.NtfFlamingo;
			dictionary[Team.Scientists] = RoleTypeId.NtfFlamingo;
			BuddyInABox.TeamToRole = dictionary;
		}

		// Token: 0x04004605 RID: 17925
		private const int MinimumSpectators = 2;

		// Token: 0x04004606 RID: 17926
		private const float MaxSpawnDistance = 3f;

		// Token: 0x04004607 RID: 17927
		private static readonly Dictionary<Team, RoleTypeId> TeamToRole;

		// Token: 0x04004608 RID: 17928
		private static bool _waveSpawnedAlready;
	}
}
