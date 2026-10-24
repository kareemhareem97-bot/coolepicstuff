using System;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;
using UnityEngine;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010EB RID: 4331
	public class FlyingCorpse : Scp2536AnomalousGiftBase
	{
		// Token: 0x170013FA RID: 5114
		// (get) Token: 0x06006067 RID: 24679 RVA: 0x001420BC File Offset: 0x001402BC
		private static string RagdollNickname
		{
			get
			{
				return "SCP-2536-" + UnityEngine.Random.Range(6, ReferenceHub.AllHubs.Count).ToString();
			}
		}

		// Token: 0x170013FB RID: 5115
		// (get) Token: 0x06006068 RID: 24680 RVA: 0x00054ECB File Offset: 0x000530CB
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Three;
			}
		}

		// Token: 0x06006069 RID: 24681 RVA: 0x001420EC File Offset: 0x001402EC
		public override bool CanBeGranted(ReferenceHub hub)
		{
			if (!base.CanBeGranted(hub))
			{
				return false;
			}
			Scp3114Role scp3114Role = hub.roleManager.CurrentRole as Scp3114Role;
			return scp3114Role != null && !scp3114Role.Disguised;
		}

		// Token: 0x0600606A RID: 24682 RVA: 0x00142124 File Offset: 0x00140324
		public override void ServerGrant(ReferenceHub hub)
		{
			base.ServerGrant(hub);
			Vector3 position = hub.PlayerCameraReference.position;
			Vector3 forward = hub.PlayerCameraReference.forward;
			float maxDistance = 6.05f;
			RaycastHit raycastHit;
			if (!Physics.Raycast(position, forward, out raycastHit, maxDistance, FlyingCorpse.RaycastLayer))
			{
				return;
			}
			Vector3 vector = raycastHit.point + Vector3.up * 0.5f;
			Quaternion rotation = Quaternion.LookRotation(vector - (position + Vector3.up * 5f));
			Vector3 value = Vector3.one * 0.75f;
			RoleTypeId role = FlyingCorpse.TargetCorpseTypes.RandomItem<RoleTypeId>();
			DamageHandlerBase handler = new UniversalDamageHandler(0f, DeathTranslations.Asphyxiated, null);
			DynamicRagdoll dynamicRagdoll = RagdollManager.ServerCreateRagdoll(role, vector, rotation, handler, hub.nicknameSync.DisplayName, new Vector3?(value), null, null) as DynamicRagdoll;
			if (dynamicRagdoll == null)
			{
				return;
			}
			Vector3 force = dynamicRagdoll.transform.forward * -7f;
			Rigidbody[] linkedRigidbodies = dynamicRagdoll.LinkedRigidbodies;
			for (int i = 0; i < linkedRigidbodies.Length; i++)
			{
				linkedRigidbodies[i].AddForce(force, ForceMode.Impulse);
			}
		}

		// Token: 0x0400462F RID: 17967
		private const float RagdollRotationOffset = 5f;

		// Token: 0x04004630 RID: 17968
		private const float MaxDistanceMultiplier = 2.5f;

		// Token: 0x04004631 RID: 17969
		private const float RagdollHeightOffset = 0.5f;

		// Token: 0x04004632 RID: 17970
		private const float RagdollScale = 0.75f;

		// Token: 0x04004633 RID: 17971
		private const float CorpseForce = -7f;

		// Token: 0x04004634 RID: 17972
		private static readonly RoleTypeId[] TargetCorpseTypes = new RoleTypeId[]
		{
			RoleTypeId.NtfPrivate,
			RoleTypeId.NtfSergeant,
			RoleTypeId.NtfCaptain,
			RoleTypeId.NtfSpecialist,
			RoleTypeId.ChaosMarauder,
			RoleTypeId.ChaosRepressor,
			RoleTypeId.ChaosRifleman,
			RoleTypeId.ChaosConscript
		};

		// Token: 0x04004635 RID: 17973
		private static readonly CachedLayerMask RaycastLayer = new CachedLayerMask(new string[]
		{
			"Default",
			"Door"
		});
	}
}
