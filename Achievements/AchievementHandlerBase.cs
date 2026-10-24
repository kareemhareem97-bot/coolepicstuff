using System;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using Mirror;

namespace Achievements
{
	// Token: 0x02000AE7 RID: 2791
	public abstract class AchievementHandlerBase
	{
		// Token: 0x17000E5F RID: 3679
		// (get) Token: 0x06003F85 RID: 16261 RVA: 0x000D2991 File Offset: 0x000D0B91
		private static bool AllowAchievements
		{
			get
			{
				return (ServerConfigSynchronizer.Singleton.MainBoolsSync & 2) == 0;
			}
		}

		// Token: 0x06003F86 RID: 16262 RVA: 0x00002E9A File Offset: 0x0000109A
		internal virtual void OnInitialize()
		{
		}

		// Token: 0x06003F87 RID: 16263 RVA: 0x00002E9A File Offset: 0x0000109A
		internal virtual void OnRoundStarted()
		{
		}

		// Token: 0x06003F88 RID: 16264 RVA: 0x000D29A4 File Offset: 0x000D0BA4
		public static void ClientAchieve(AchievementName targetAchievement)
		{
			Achievement achievement;
			if (!AchievementHandlerBase.AllowAchievements || !AchievementManager.Achievements.TryGetValue(targetAchievement, out achievement))
			{
				return;
			}
			achievement.Achieve();
		}

		// Token: 0x06003F89 RID: 16265 RVA: 0x000D29D0 File Offset: 0x000D0BD0
		public static void ServerAchieve(NetworkConnection conn, AchievementName targetAchievement)
		{
			if (!NetworkServer.active)
			{
				throw new InvalidOperationException("Method ServerAchieve can only be executed on the server.");
			}
			if (conn.identity.isLocalPlayer)
			{
				return;
			}
			if (!AchievementHandlerBase.AllowAchievements)
			{
				return;
			}
			conn.Send<AchievementManager.AchievementMessage>(new AchievementManager.AchievementMessage
			{
				AchievementId = (byte)targetAchievement
			}, 0);
			PlayerEvents.OnReceivedAchievement(new PlayerReceivedAchievementEventArgs(conn.identity, targetAchievement));
		}
	}
}
