using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;

namespace Achievements.Handlers
{
	// Token: 0x02000AEC RID: 2796
	public class ArmyOfOneHandler : AchievementHandlerBase
	{
		// Token: 0x06003F91 RID: 16273 RVA: 0x000D3158 File Offset: 0x000D1358
		internal override void OnInitialize()
		{
			Action<ReferenceHub, DamageHandlerBase> value;
			if ((value = ArmyOfOneHandler.<>O.<0>__OnAnyPlayerDied) == null)
			{
				value = (ArmyOfOneHandler.<>O.<0>__OnAnyPlayerDied = new Action<ReferenceHub, DamageHandlerBase>(ArmyOfOneHandler.OnAnyPlayerDied));
			}
			PlayerStats.OnAnyPlayerDied += value;
			PlayerRoleManager.ServerRoleSet value2;
			if ((value2 = ArmyOfOneHandler.<>O.<1>__OnServerRoleSet) == null)
			{
				value2 = (ArmyOfOneHandler.<>O.<1>__OnServerRoleSet = new PlayerRoleManager.ServerRoleSet(ArmyOfOneHandler.OnServerRoleSet));
			}
			PlayerRoleManager.OnServerRoleSet += value2;
		}

		// Token: 0x06003F92 RID: 16274 RVA: 0x000D31A5 File Offset: 0x000D13A5
		internal override void OnRoundStarted()
		{
			ArmyOfOneHandler.Kills.Clear();
		}

		// Token: 0x06003F93 RID: 16275 RVA: 0x000D31B1 File Offset: 0x000D13B1
		private static void OnServerRoleSet(ReferenceHub hub, RoleTypeId roleTypeId, RoleChangeReason changeReason)
		{
			if (changeReason == RoleChangeReason.Escaped)
			{
				return;
			}
			ArmyOfOneHandler.Kills.Remove(hub);
		}

		// Token: 0x06003F94 RID: 16276 RVA: 0x000D31C4 File Offset: 0x000D13C4
		private static void OnAnyPlayerDied(ReferenceHub victim, DamageHandlerBase handler)
		{
			if (NetworkServer.active)
			{
				AttackerDamageHandler attackerDamageHandler = handler as AttackerDamageHandler;
				if (attackerDamageHandler != null)
				{
					ReferenceHub hub = attackerDamageHandler.Attacker.Hub;
					if (hub == null || !hub.IsHuman())
					{
						return;
					}
					if (!HitboxIdentity.IsEnemy(attackerDamageHandler.Attacker.Role, victim.GetRoleId()))
					{
						return;
					}
					ItemType itemType = ArmyOfOneHandler.GetItemType(handler);
					if (itemType == ItemType.None)
					{
						return;
					}
					List<ItemType> list;
					if (!ArmyOfOneHandler.Kills.TryGetValue(hub, out list))
					{
						ArmyOfOneHandler.Kills[hub] = new List<ItemType>
						{
							itemType
						};
						return;
					}
					if (list.Contains(itemType))
					{
						return;
					}
					list.Add(itemType);
					ArmyOfOneHandler.Kills[hub] = list;
					if (list.Count < 4)
					{
						return;
					}
					AchievementHandlerBase.ServerAchieve(hub.connectionToClient, AchievementName.ArmyOfOne);
					return;
				}
			}
		}

		// Token: 0x06003F95 RID: 16277 RVA: 0x000D3284 File Offset: 0x000D1484
		private static ItemType GetItemType(DamageHandlerBase handler)
		{
			FirearmDamageHandler firearmDamageHandler = handler as FirearmDamageHandler;
			if (firearmDamageHandler != null)
			{
				return firearmDamageHandler.WeaponType;
			}
			if (handler is DisruptorDamageHandler)
			{
				return ItemType.ParticleDisruptor;
			}
			if (handler is JailbirdDamageHandler)
			{
				return ItemType.Jailbird;
			}
			if (handler is MicroHidDamageHandler)
			{
				return ItemType.MicroHID;
			}
			ExplosionDamageHandler explosionDamageHandler = handler as ExplosionDamageHandler;
			if (explosionDamageHandler == null)
			{
				return ItemType.None;
			}
			return ArmyOfOneHandler.ExplosionToItemType(explosionDamageHandler.ExplosionType);
		}

		// Token: 0x06003F96 RID: 16278 RVA: 0x000D32DC File Offset: 0x000D14DC
		private static ItemType ExplosionToItemType(ExplosionType explosionType)
		{
			if (explosionType == ExplosionType.Grenade)
			{
				return ItemType.GrenadeHE;
			}
			if (explosionType == ExplosionType.Disruptor)
			{
				return ItemType.ParticleDisruptor;
			}
			if (explosionType != ExplosionType.Jailbird)
			{
				return ItemType.None;
			}
			return ItemType.Jailbird;
		}

		// Token: 0x04002E72 RID: 11890
		private const int KillsNeeded = 4;

		// Token: 0x04002E73 RID: 11891
		private static readonly Dictionary<ReferenceHub, List<ItemType>> Kills = new Dictionary<ReferenceHub, List<ItemType>>();

		// Token: 0x02000AED RID: 2797
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002E74 RID: 11892
			public static Action<ReferenceHub, DamageHandlerBase> <0>__OnAnyPlayerDied;

			// Token: 0x04002E75 RID: 11893
			public static PlayerRoleManager.ServerRoleSet <1>__OnServerRoleSet;
		}
	}
}
