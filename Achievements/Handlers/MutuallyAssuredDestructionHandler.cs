using System;
using System.Runtime.CompilerServices;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;

namespace Achievements.Handlers
{
	// Token: 0x02000B06 RID: 2822
	public class MutuallyAssuredDestructionHandler : AchievementHandlerBase
	{
		// Token: 0x06003FDE RID: 16350 RVA: 0x000D41B7 File Offset: 0x000D23B7
		internal override void OnInitialize()
		{
			Action<ReferenceHub, DamageHandlerBase> value;
			if ((value = MutuallyAssuredDestructionHandler.<>O.<0>__OnAnyPlayerDied) == null)
			{
				value = (MutuallyAssuredDestructionHandler.<>O.<0>__OnAnyPlayerDied = new Action<ReferenceHub, DamageHandlerBase>(MutuallyAssuredDestructionHandler.OnAnyPlayerDied));
			}
			PlayerStats.OnAnyPlayerDied += value;
		}

		// Token: 0x06003FDF RID: 16351 RVA: 0x000D41DC File Offset: 0x000D23DC
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
			if (!MutuallyAssuredDestructionHandler.ValidDamage(attackerDamageHandler))
			{
				return;
			}
			ReferenceHub hub = attackerDamageHandler.Attacker.Hub;
			if (hub == null || hub.IsAlive())
			{
				return;
			}
			if (!HitboxIdentity.IsEnemy(attackerDamageHandler.Attacker.Role, victim.GetRoleId()))
			{
				return;
			}
			AchievementHandlerBase.ServerAchieve(hub.connectionToClient, AchievementName.MutuallyAssuredDestruction);
		}

		// Token: 0x06003FE0 RID: 16352 RVA: 0x000D424C File Offset: 0x000D244C
		private static bool ValidDamage(AttackerDamageHandler attackerHandler)
		{
			if (attackerHandler is Scp018DamageHandler)
			{
				return true;
			}
			ExplosionDamageHandler explosionDamageHandler = attackerHandler as ExplosionDamageHandler;
			if (explosionDamageHandler == null)
			{
				return false;
			}
			ExplosionType explosionType = explosionDamageHandler.ExplosionType;
			return explosionType == ExplosionType.SCP018 || explosionType == ExplosionType.Grenade;
		}

		// Token: 0x02000B07 RID: 2823
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002E90 RID: 11920
			public static Action<ReferenceHub, DamageHandlerBase> <0>__OnAnyPlayerDied;
		}
	}
}
