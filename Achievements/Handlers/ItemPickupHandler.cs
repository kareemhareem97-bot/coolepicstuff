using System;
using System.Runtime.CompilerServices;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.Pickups;
using Mirror;
using PlayerRoles;

namespace Achievements.Handlers
{
	// Token: 0x02000B01 RID: 2817
	public class ItemPickupHandler : AchievementHandlerBase
	{
		// Token: 0x06003FCB RID: 16331 RVA: 0x000D3E05 File Offset: 0x000D2005
		internal override void OnInitialize()
		{
			Action<ReferenceHub, ItemBase, ItemPickupBase> value;
			if ((value = ItemPickupHandler.<>O.<0>__OnItemAdded) == null)
			{
				value = (ItemPickupHandler.<>O.<0>__OnItemAdded = new Action<ReferenceHub, ItemBase, ItemPickupBase>(ItemPickupHandler.OnItemAdded));
			}
			InventoryExtensions.OnItemAdded += value;
		}

		// Token: 0x06003FCC RID: 16332 RVA: 0x000D3E28 File Offset: 0x000D2028
		private static void OnItemAdded(ReferenceHub ply, ItemBase ib, ItemPickupBase pickup)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			KeycardItem keycardItem = ib as KeycardItem;
			if (keycardItem != null && keycardItem.ItemTypeId == ItemType.KeycardO5)
			{
				AchievementHandlerBase.ServerAchieve(ply.connectionToClient, AchievementName.ExecutiveAccess);
				return;
			}
			if (ply.GetRoleId() == RoleTypeId.ClassD && ib is Firearm)
			{
				AchievementHandlerBase.ServerAchieve(ply.connectionToClient, AchievementName.ThatCanBeUseful);
			}
		}

		// Token: 0x04002E88 RID: 11912
		private const ItemType ExecutiveAccessCardId = ItemType.KeycardO5;

		// Token: 0x02000B02 RID: 2818
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002E89 RID: 11913
			public static Action<ReferenceHub, ItemBase, ItemPickupBase> <0>__OnItemAdded;
		}
	}
}
