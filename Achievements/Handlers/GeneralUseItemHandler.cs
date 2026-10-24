using System;
using System.Runtime.CompilerServices;
using InventorySystem.Items.Usables;
using Mirror;

namespace Achievements.Handlers
{
	// Token: 0x02000AFA RID: 2810
	public class GeneralUseItemHandler : AchievementHandlerBase
	{
		// Token: 0x06003FB4 RID: 16308 RVA: 0x000D3B04 File Offset: 0x000D1D04
		internal override void OnInitialize()
		{
			Action<ReferenceHub, UsableItem> value;
			if ((value = GeneralUseItemHandler.<>O.<0>__ServerOnUsingCompleted) == null)
			{
				value = (GeneralUseItemHandler.<>O.<0>__ServerOnUsingCompleted = new Action<ReferenceHub, UsableItem>(GeneralUseItemHandler.ServerOnUsingCompleted));
			}
			UsableItemsController.ServerOnUsingCompleted += value;
		}

		// Token: 0x06003FB5 RID: 16309 RVA: 0x000D3B26 File Offset: 0x000D1D26
		private static void ServerOnUsingCompleted(ReferenceHub hub, UsableItem item)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			if (item.ItemTypeId == ItemType.SCP1576)
			{
				AchievementHandlerBase.ServerAchieve(hub.networkIdentity.connectionToClient, AchievementName.AfterlifeCommunicator);
			}
		}

		// Token: 0x02000AFB RID: 2811
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002E85 RID: 11909
			public static Action<ReferenceHub, UsableItem> <0>__ServerOnUsingCompleted;
		}
	}
}
