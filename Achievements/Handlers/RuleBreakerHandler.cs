using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using InventorySystem.Items.Usables;
using PlayerRoles;

namespace Achievements.Handlers
{
	// Token: 0x02000B0F RID: 2831
	public class RuleBreakerHandler : AchievementHandlerBase
	{
		// Token: 0x06003FEC RID: 16364 RVA: 0x000D43F8 File Offset: 0x000D25F8
		internal override void OnInitialize()
		{
			Action<ReferenceHub, UsableItem> value;
			if ((value = RuleBreakerHandler.<>O.<0>__OnUsedScp330) == null)
			{
				value = (RuleBreakerHandler.<>O.<0>__OnUsedScp330 = new Action<ReferenceHub, UsableItem>(RuleBreakerHandler.OnUsedScp330));
			}
			UsableItemsController.ServerOnUsingCompleted += value;
			PlayerRoleManager.ServerRoleSet value2;
			if ((value2 = RuleBreakerHandler.<>O.<1>__OnServerRoleSet) == null)
			{
				value2 = (RuleBreakerHandler.<>O.<1>__OnServerRoleSet = new PlayerRoleManager.ServerRoleSet(RuleBreakerHandler.OnServerRoleSet));
			}
			PlayerRoleManager.OnServerRoleSet += value2;
		}

		// Token: 0x06003FED RID: 16365 RVA: 0x000D4445 File Offset: 0x000D2645
		internal override void OnRoundStarted()
		{
			RuleBreakerHandler.CandiesEaten.Clear();
		}

		// Token: 0x06003FEE RID: 16366 RVA: 0x000D4451 File Offset: 0x000D2651
		private static void OnServerRoleSet(ReferenceHub hub, RoleTypeId roleTypeId, RoleChangeReason changeReason)
		{
			RuleBreakerHandler.CandiesEaten.Remove(hub);
		}

		// Token: 0x06003FEF RID: 16367 RVA: 0x000D4460 File Offset: 0x000D2660
		private static void OnUsedScp330(ReferenceHub hub, UsableItem item)
		{
			if (item.ItemTypeId != ItemType.SCP330)
			{
				return;
			}
			int valueOrDefault = RuleBreakerHandler.CandiesEaten.GetValueOrDefault(hub, 0);
			if ((RuleBreakerHandler.CandiesEaten[hub] = valueOrDefault + 1) < 3)
			{
				return;
			}
			AchievementHandlerBase.ServerAchieve(hub.connectionToClient, AchievementName.RuleBreaker);
		}

		// Token: 0x04002E95 RID: 11925
		private const int CandiesNeeded = 3;

		// Token: 0x04002E96 RID: 11926
		private static readonly Dictionary<ReferenceHub, int> CandiesEaten = new Dictionary<ReferenceHub, int>();

		// Token: 0x02000B10 RID: 2832
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002E97 RID: 11927
			public static Action<ReferenceHub, UsableItem> <0>__OnUsedScp330;

			// Token: 0x04002E98 RID: 11928
			public static PlayerRoleManager.ServerRoleSet <1>__OnServerRoleSet;
		}
	}
}
