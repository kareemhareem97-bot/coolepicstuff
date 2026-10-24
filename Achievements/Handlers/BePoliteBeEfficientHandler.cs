using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Footprinting;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;

namespace Achievements.Handlers
{
	// Token: 0x02000AEE RID: 2798
	public class BePoliteBeEfficientHandler : AchievementHandlerBase
	{
		// Token: 0x06003F99 RID: 16281 RVA: 0x000D3309 File Offset: 0x000D1509
		internal override void OnInitialize()
		{
			Action<ReferenceHub, DamageHandlerBase> value;
			if ((value = BePoliteBeEfficientHandler.<>O.<0>__HandleDeath) == null)
			{
				value = (BePoliteBeEfficientHandler.<>O.<0>__HandleDeath = new Action<ReferenceHub, DamageHandlerBase>(BePoliteBeEfficientHandler.HandleDeath));
			}
			PlayerStats.OnAnyPlayerDied += value;
		}

		// Token: 0x06003F9A RID: 16282 RVA: 0x000D332B File Offset: 0x000D152B
		internal override void OnRoundStarted()
		{
			BePoliteBeEfficientHandler.Kills.Clear();
			BePoliteBeEfficientHandler.Timers.Clear();
			BePoliteBeEfficientHandler.AlreadyAchieved.Clear();
		}

		// Token: 0x06003F9B RID: 16283 RVA: 0x000D334C File Offset: 0x000D154C
		private static void HandleDeath(ReferenceHub deadPlayer, DamageHandlerBase handler)
		{
			if (NetworkServer.active)
			{
				FirearmDamageHandler firearmDamageHandler = handler as FirearmDamageHandler;
				if (firearmDamageHandler != null)
				{
					Footprint attacker = firearmDamageHandler.Attacker;
					ReferenceHub hub = attacker.Hub;
					if (!HitboxIdentity.IsEnemy(attacker.Role, deadPlayer.GetRoleId()))
					{
						return;
					}
					if (BePoliteBeEfficientHandler.AlreadyAchieved.Contains(attacker.NetId) || hub == null)
					{
						return;
					}
					int num;
					if (!BePoliteBeEfficientHandler.Kills.TryGetValue(hub, out num))
					{
						num = 0;
					}
					num = (BePoliteBeEfficientHandler.Kills[hub] = num + 1);
					Stopwatch stopwatch;
					if (!BePoliteBeEfficientHandler.Timers.TryGetValue(hub, out stopwatch))
					{
						BePoliteBeEfficientHandler.Timers.Add(hub, Stopwatch.StartNew());
						return;
					}
					if (stopwatch.Elapsed.TotalSeconds > 30.0)
					{
						BePoliteBeEfficientHandler.Timers[hub].Restart();
						BePoliteBeEfficientHandler.Kills[hub] = 1;
						return;
					}
					if (num < 5)
					{
						return;
					}
					BePoliteBeEfficientHandler.AlreadyAchieved.Add(attacker.NetId);
					BePoliteBeEfficientHandler.Kills.Remove(attacker.Hub);
					BePoliteBeEfficientHandler.Timers.Remove(attacker.Hub);
					AchievementHandlerBase.ServerAchieve(attacker.Hub.networkIdentity.connectionToClient, AchievementName.BePoliteBeEfficient);
					return;
				}
			}
		}

		// Token: 0x04002E76 RID: 11894
		private const float TimeLimit = 30f;

		// Token: 0x04002E77 RID: 11895
		private const int KillsTarget = 5;

		// Token: 0x04002E78 RID: 11896
		private static readonly Dictionary<ReferenceHub, Stopwatch> Timers = new Dictionary<ReferenceHub, Stopwatch>();

		// Token: 0x04002E79 RID: 11897
		private static readonly Dictionary<ReferenceHub, int> Kills = new Dictionary<ReferenceHub, int>();

		// Token: 0x04002E7A RID: 11898
		private static readonly HashSet<uint> AlreadyAchieved = new HashSet<uint>();

		// Token: 0x02000AEF RID: 2799
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002E7B RID: 11899
			public static Action<ReferenceHub, DamageHandlerBase> <0>__HandleDeath;
		}
	}
}
