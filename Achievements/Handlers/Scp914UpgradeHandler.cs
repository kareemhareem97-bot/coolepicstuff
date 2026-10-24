using System;
using System.Runtime.CompilerServices;
using Footprinting;
using InventorySystem.Items;
using InventorySystem.Items.Keycards;
using InventorySystem.Items.Pickups;
using MapGeneration;
using PlayerRoles;
using Scp914;

namespace Achievements.Handlers
{
	// Token: 0x02000B11 RID: 2833
	public class Scp914UpgradeHandler : AchievementHandlerBase
	{
		// Token: 0x06003FF2 RID: 16370 RVA: 0x000D44B4 File Offset: 0x000D26B4
		internal override void OnInitialize()
		{
			Delegate onInventoryItemUpgraded = Scp914Upgrader.OnInventoryItemUpgraded;
			Action<ItemBase, Scp914KnobSetting> b;
			if ((b = Scp914UpgradeHandler.<>O.<0>__ItemUpgraded) == null)
			{
				b = (Scp914UpgradeHandler.<>O.<0>__ItemUpgraded = new Action<ItemBase, Scp914KnobSetting>(Scp914UpgradeHandler.ItemUpgraded));
			}
			Scp914Upgrader.OnInventoryItemUpgraded = (Action<ItemBase, Scp914KnobSetting>)Delegate.Combine(onInventoryItemUpgraded, b);
			Delegate onPickupUpgraded = Scp914Upgrader.OnPickupUpgraded;
			Action<ItemPickupBase, Scp914KnobSetting> b2;
			if ((b2 = Scp914UpgradeHandler.<>O.<1>__PickupUpgraded) == null)
			{
				b2 = (Scp914UpgradeHandler.<>O.<1>__PickupUpgraded = new Action<ItemPickupBase, Scp914KnobSetting>(Scp914UpgradeHandler.PickupUpgraded));
			}
			Scp914Upgrader.OnPickupUpgraded = (Action<ItemPickupBase, Scp914KnobSetting>)Delegate.Combine(onPickupUpgraded, b2);
		}

		// Token: 0x06003FF3 RID: 16371 RVA: 0x000D451F File Offset: 0x000D271F
		private static void ItemUpgraded(ItemBase item, Scp914KnobSetting sett)
		{
			if (item.Owner.GetRoleId() == RoleTypeId.Scientist && item is KeycardItem && Scp914UpgradeHandler.AnyDClassIn914)
			{
				AchievementHandlerBase.ServerAchieve(item.OwnerInventory.connectionToClient, AchievementName.Friendship);
			}
		}

		// Token: 0x06003FF4 RID: 16372 RVA: 0x000D4550 File Offset: 0x000D2750
		private static void PickupUpgraded(ItemPickupBase ipb, Scp914KnobSetting sett)
		{
			Footprint previousOwner = ipb.PreviousOwner;
			ReferenceHub hub = previousOwner.Hub;
			if (!(ipb is KeycardPickup))
			{
				return;
			}
			if (previousOwner.Role != RoleTypeId.Scientist)
			{
				return;
			}
			if (hub == null)
			{
				return;
			}
			PlayerRoleBase currentRole = hub.roleManager.CurrentRole;
			if (previousOwner.LifeIdentifier != currentRole.UniqueLifeIdentifier)
			{
				return;
			}
			if (!Scp914UpgradeHandler.AnyDClassIn914)
			{
				return;
			}
			AchievementHandlerBase.ServerAchieve(ipb.PreviousOwner.Hub.networkIdentity.connectionToClient, AchievementName.Friendship);
		}

		// Token: 0x17000E61 RID: 3681
		// (get) Token: 0x06003FF5 RID: 16373 RVA: 0x000D45CC File Offset: 0x000D27CC
		private static bool AnyDClassIn914
		{
			get
			{
				foreach (ReferenceHub hub in ReferenceHub.AllHubs)
				{
					RoomIdentifier roomIdentifier;
					if (hub.GetRoleId() == RoleTypeId.ClassD && hub.TryGetCurrentRoom(out roomIdentifier) && roomIdentifier.Name == RoomName.Lcz914)
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x02000B12 RID: 2834
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002E99 RID: 11929
			public static Action<ItemBase, Scp914KnobSetting> <0>__ItemUpgraded;

			// Token: 0x04002E9A RID: 11930
			public static Action<ItemPickupBase, Scp914KnobSetting> <1>__PickupUpgraded;
		}
	}
}
