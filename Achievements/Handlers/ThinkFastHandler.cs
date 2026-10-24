using System;
using System.Runtime.CompilerServices;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;

namespace Achievements.Handlers
{
	// Token: 0x02000B16 RID: 2838
	public class ThinkFastHandler : AchievementHandlerBase
	{
		// Token: 0x06003FFF RID: 16383 RVA: 0x000D479C File Offset: 0x000D299C
		internal override void OnInitialize()
		{
			Action<ReferenceHub, DamageHandlerBase> value;
			if ((value = ThinkFastHandler.<>O.<0>__OnAnyPlayerDied) == null)
			{
				value = (ThinkFastHandler.<>O.<0>__OnAnyPlayerDied = new Action<ReferenceHub, DamageHandlerBase>(ThinkFastHandler.OnAnyPlayerDied));
			}
			PlayerStats.OnAnyPlayerDied += value;
		}

		// Token: 0x06004000 RID: 16384 RVA: 0x000D47C0 File Offset: 0x000D29C0
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
			if (!(handler is Scp018DamageHandler))
			{
				ExplosionDamageHandler explosionDamageHandler = handler as ExplosionDamageHandler;
				if (explosionDamageHandler == null || explosionDamageHandler.ExplosionType != ExplosionType.SCP018)
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
			AchievementHandlerBase.ServerAchieve(hub.connectionToClient, AchievementName.ThinkFast);
		}

		// Token: 0x02000B17 RID: 2839
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002E9C RID: 11932
			public static Action<ReferenceHub, DamageHandlerBase> <0>__OnAnyPlayerDied;
		}
	}
}
