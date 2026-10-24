using System;
using System.Runtime.CompilerServices;
using Mirror;
using PlayerRoles;

namespace Achievements.Handlers
{
	// Token: 0x02000B1B RID: 2843
	public class TurnThemAllHandler : AchievementHandlerBase
	{
		// Token: 0x0600400B RID: 16395 RVA: 0x000D49AE File Offset: 0x000D2BAE
		internal override void OnInitialize()
		{
			PlayerRoleManager.ServerRoleSet value;
			if ((value = TurnThemAllHandler.<>O.<0>__OnRoleChanged) == null)
			{
				value = (TurnThemAllHandler.<>O.<0>__OnRoleChanged = new PlayerRoleManager.ServerRoleSet(TurnThemAllHandler.OnRoleChanged));
			}
			PlayerRoleManager.OnServerRoleSet += value;
		}

		// Token: 0x0600400C RID: 16396 RVA: 0x000D49D0 File Offset: 0x000D2BD0
		internal override void OnRoundStarted()
		{
			TurnThemAllHandler._healedPlayers = 0;
			TurnThemAllHandler._alreadyAchieved = false;
		}

		// Token: 0x0600400D RID: 16397 RVA: 0x000D49E0 File Offset: 0x000D2BE0
		private static void OnRoleChanged(ReferenceHub userHub, RoleTypeId newClass, RoleChangeReason reason)
		{
			if (!NetworkServer.active || TurnThemAllHandler._alreadyAchieved || newClass != RoleTypeId.Scp0492 || reason != RoleChangeReason.Revived)
			{
				return;
			}
			TurnThemAllHandler._healedPlayers++;
			if (TurnThemAllHandler._healedPlayers < 10)
			{
				return;
			}
			NetworkConnection conn = null;
			bool flag = false;
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				if (referenceHub.GetRoleId() == RoleTypeId.Scp049)
				{
					if (flag)
					{
						return;
					}
					conn = referenceHub.networkIdentity.connectionToClient;
					flag = true;
				}
			}
			if (!flag)
			{
				return;
			}
			AchievementHandlerBase.ServerAchieve(conn, AchievementName.TurnThemAll);
		}

		// Token: 0x04002EA2 RID: 11938
		private const int TargetCured = 10;

		// Token: 0x04002EA3 RID: 11939
		private static int _healedPlayers;

		// Token: 0x04002EA4 RID: 11940
		private static bool _alreadyAchieved;

		// Token: 0x02000B1C RID: 2844
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002EA5 RID: 11941
			public static PlayerRoleManager.ServerRoleSet <0>__OnRoleChanged;
		}
	}
}
