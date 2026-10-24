using System;
using System.Collections.Generic;
using System.Diagnostics;
using CustomPlayerEffects;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;

namespace Achievements.Handlers
{
	// Token: 0x02000AFC RID: 2812
	public class HatsOffToYouHandler : AchievementHandlerBase
	{
		// Token: 0x06003FB7 RID: 16311 RVA: 0x000D3B4C File Offset: 0x000D1D4C
		internal override void OnInitialize()
		{
			PlayerStats.OnAnyPlayerDied += this.HandleDeath;
			StatusEffectBase.OnDisabled += this.HandleEffectDisabled;
		}

		// Token: 0x06003FB8 RID: 16312 RVA: 0x000D3B70 File Offset: 0x000D1D70
		internal override void OnRoundStarted()
		{
			HatsOffToYouHandler.Timers.Clear();
		}

		// Token: 0x06003FB9 RID: 16313 RVA: 0x000D3B7C File Offset: 0x000D1D7C
		private void HandleEffectDisabled(StatusEffectBase effectBase)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			Invisible invisible = effectBase as Invisible;
			if (invisible == null)
			{
				return;
			}
			Stopwatch stopwatch;
			if (HatsOffToYouHandler.Timers.TryGetValue(invisible.Hub, out stopwatch))
			{
				if (stopwatch.Elapsed.TotalSeconds >= 5.0)
				{
					stopwatch.Restart();
					return;
				}
			}
			else
			{
				HatsOffToYouHandler.Timers[invisible.Hub] = Stopwatch.StartNew();
			}
		}

		// Token: 0x06003FBA RID: 16314 RVA: 0x000D3BE8 File Offset: 0x000D1DE8
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
			if (hub.IsSCP(true) || !HitboxIdentity.IsEnemy(hub, deadPlayer))
			{
				return;
			}
			Stopwatch stopwatch;
			if (!HatsOffToYouHandler.Timers.TryGetValue(hub, out stopwatch))
			{
				return;
			}
			if (stopwatch.Elapsed.TotalSeconds <= 5.0)
			{
				AchievementHandlerBase.ServerAchieve(hub.networkIdentity.connectionToClient, AchievementName.HatsOffToYou);
			}
			HatsOffToYouHandler.Timers.Remove(hub);
		}

		// Token: 0x04002E86 RID: 11910
		private const float KillWindowSeconds = 5f;

		// Token: 0x04002E87 RID: 11911
		private static readonly Dictionary<ReferenceHub, Stopwatch> Timers = new Dictionary<ReferenceHub, Stopwatch>();
	}
}
