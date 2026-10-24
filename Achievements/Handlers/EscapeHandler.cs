using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CustomPlayerEffects;
using InventorySystem.Disarming;
using InventorySystem.Items;
using Mirror;
using PlayerRoles;

namespace Achievements.Handlers
{
	// Token: 0x02000AF4 RID: 2804
	public class EscapeHandler : AchievementHandlerBase
	{
		// Token: 0x06003FA6 RID: 16294 RVA: 0x000D35AC File Offset: 0x000D17AC
		internal override void OnInitialize()
		{
			Action<ReferenceHub> value;
			if ((value = EscapeHandler.<>O.<0>__OnEscaped) == null)
			{
				value = (EscapeHandler.<>O.<0>__OnEscaped = new Action<ReferenceHub>(EscapeHandler.OnEscaped));
			}
			Escape.OnServerPlayerEscape += value;
			PlayerRoleManager.ServerRoleSet value2;
			if ((value2 = EscapeHandler.<>O.<1>__OnRoleSet) == null)
			{
				value2 = (EscapeHandler.<>O.<1>__OnRoleSet = new PlayerRoleManager.ServerRoleSet(EscapeHandler.OnRoleSet));
			}
			PlayerRoleManager.OnServerRoleSet += value2;
		}

		// Token: 0x06003FA7 RID: 16295 RVA: 0x000D35F9 File Offset: 0x000D17F9
		private static void OnRoleSet(ReferenceHub userHub, RoleTypeId newId, RoleChangeReason reason)
		{
			if (!NetworkServer.active || EscapeHandler._escapeFired || reason != RoleChangeReason.Escaped)
			{
				return;
			}
			AchievementHandlerBase.ServerAchieve(userHub.networkIdentity.connectionToClient, AchievementName.EscapeArtist);
			EscapeHandler._escapeFired = true;
		}

		// Token: 0x06003FA8 RID: 16296 RVA: 0x000D3626 File Offset: 0x000D1826
		internal override void OnRoundStarted()
		{
			EscapeHandler._escapeFired = false;
		}

		// Token: 0x06003FA9 RID: 16297 RVA: 0x000D3630 File Offset: 0x000D1830
		private static void OnEscaped(ReferenceHub userHub)
		{
			NetworkConnectionToClient connectionToClient = userHub.networkIdentity.connectionToClient;
			PlayerRoleBase currentRole = userHub.roleManager.CurrentRole;
			if (userHub.playerEffectsController.GetEffect<Scp207>().IsEnabled)
			{
				AchievementHandlerBase.ServerAchieve(connectionToClient, AchievementName.Escape207);
			}
			if (currentRole.RoleTypeId == RoleTypeId.Scientist)
			{
				AchievementHandlerBase.ServerAchieve(connectionToClient, AchievementName.ForScience);
				return;
			}
			if (currentRole.RoleTypeId == RoleTypeId.ClassD)
			{
				AchievementHandlerBase.ServerAchieve(connectionToClient, AchievementName.ItsAlwaysLeft);
				int num = 0;
				if (!userHub.inventory.IsDisarmed())
				{
					using (Dictionary<ushort, ItemBase>.ValueCollection.Enumerator enumerator = userHub.inventory.UserInventory.Items.Values.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.Category == ItemCategory.SCPItem)
							{
								num++;
							}
						}
					}
				}
				if (num >= 2)
				{
					AchievementHandlerBase.ServerAchieve(connectionToClient, AchievementName.PropertyOfChaos);
				}
			}
		}

		// Token: 0x04002E7D RID: 11901
		private static bool _escapeFired;

		// Token: 0x02000AF5 RID: 2805
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002E7E RID: 11902
			public static Action<ReferenceHub> <0>__OnEscaped;

			// Token: 0x04002E7F RID: 11903
			public static PlayerRoleManager.ServerRoleSet <1>__OnRoleSet;
		}
	}
}
