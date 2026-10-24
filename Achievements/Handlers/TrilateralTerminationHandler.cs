using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;

namespace Achievements.Handlers
{
	// Token: 0x02000B19 RID: 2841
	public class TrilateralTerminationHandler : AchievementHandlerBase
	{
		// Token: 0x06004005 RID: 16389 RVA: 0x000D488C File Offset: 0x000D2A8C
		internal override void OnInitialize()
		{
			Action<ReferenceHub, DamageHandlerBase> value;
			if ((value = TrilateralTerminationHandler.<>O.<0>__OnAnyPlayerDied) == null)
			{
				value = (TrilateralTerminationHandler.<>O.<0>__OnAnyPlayerDied = new Action<ReferenceHub, DamageHandlerBase>(TrilateralTerminationHandler.OnAnyPlayerDied));
			}
			PlayerStats.OnAnyPlayerDied += value;
			PlayerRoleManager.ServerRoleSet value2;
			if ((value2 = TrilateralTerminationHandler.<>O.<1>__OnServerRoleSet) == null)
			{
				value2 = (TrilateralTerminationHandler.<>O.<1>__OnServerRoleSet = new PlayerRoleManager.ServerRoleSet(TrilateralTerminationHandler.OnServerRoleSet));
			}
			PlayerRoleManager.OnServerRoleSet += value2;
		}

		// Token: 0x06004006 RID: 16390 RVA: 0x000D48D9 File Offset: 0x000D2AD9
		internal override void OnRoundStarted()
		{
			TrilateralTerminationHandler.Kills.Clear();
		}

		// Token: 0x06004007 RID: 16391 RVA: 0x000D48E5 File Offset: 0x000D2AE5
		private static void OnServerRoleSet(ReferenceHub hub, RoleTypeId roleTypeId, RoleChangeReason changeReason)
		{
			if (changeReason == RoleChangeReason.Escaped)
			{
				return;
			}
			TrilateralTerminationHandler.Kills.Remove(hub);
		}

		// Token: 0x06004008 RID: 16392 RVA: 0x000D48F8 File Offset: 0x000D2AF8
		private static void OnAnyPlayerDied(ReferenceHub victim, DamageHandlerBase handler)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			AttackerDamageHandler attackerDamageHandler = handler as AttackerDamageHandler;
			if (attackerDamageHandler == null)
			{
				return;
			}
			if (!(handler is DisruptorDamageHandler))
			{
				ExplosionDamageHandler explosionDamageHandler = handler as ExplosionDamageHandler;
				if (explosionDamageHandler == null || explosionDamageHandler.ExplosionType != ExplosionType.Disruptor)
				{
					return;
				}
			}
			ReferenceHub hub = attackerDamageHandler.Attacker.Hub;
			if (hub == null)
			{
				return;
			}
			if (!HitboxIdentity.IsEnemy(attackerDamageHandler.Attacker.Role, victim.GetRoleId()))
			{
				return;
			}
			int num;
			if (!TrilateralTerminationHandler.Kills.TryGetValue(hub, out num))
			{
				num = 0;
			}
			num = (TrilateralTerminationHandler.Kills[hub] = num + 1);
			if (num < 3)
			{
				return;
			}
			TrilateralTerminationHandler.Kills.Remove(hub);
			AchievementHandlerBase.ServerAchieve(hub.connectionToClient, AchievementName.TrilateralTermination);
		}

		// Token: 0x04002E9E RID: 11934
		private const int KillsNeeded = 3;

		// Token: 0x04002E9F RID: 11935
		private static readonly Dictionary<ReferenceHub, int> Kills = new Dictionary<ReferenceHub, int>();

		// Token: 0x02000B1A RID: 2842
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002EA0 RID: 11936
			public static Action<ReferenceHub, DamageHandlerBase> <0>__OnAnyPlayerDied;

			// Token: 0x04002EA1 RID: 11937
			public static PlayerRoleManager.ServerRoleSet <1>__OnServerRoleSet;
		}
	}
}
