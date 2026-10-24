using System;
using System.Collections.Generic;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Keycards;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010DE RID: 4318
	public class KeycardUpgrade : Scp2536GiftBase
	{
		// Token: 0x170013EA RID: 5098
		// (get) Token: 0x0600601D RID: 24605 RVA: 0x0003F21F File Offset: 0x0003D41F
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Five;
			}
		}

		// Token: 0x0600601E RID: 24606 RVA: 0x00141554 File Offset: 0x0013F754
		public override bool CanBeGranted(ReferenceHub hub)
		{
			if (base.CanBeGranted(hub))
			{
				return hub.inventory.UserInventory.Items.Any(delegate(KeyValuePair<ushort, ItemBase> i)
				{
					KeycardItem keycardItem = i.Value as KeycardItem;
					if (keycardItem == null)
					{
						return false;
					}
					ItemType itemTypeId = keycardItem.ItemTypeId;
					return itemTypeId - ItemType.KeycardMTFCaptain > 3;
				});
			}
			return false;
		}

		// Token: 0x0600601F RID: 24607 RVA: 0x001415A0 File Offset: 0x0013F7A0
		public override void ServerGrant(ReferenceHub hub)
		{
			KeycardItem keycardItem = null;
			bool flag = false;
			foreach (ItemBase itemBase in hub.inventory.UserInventory.Items.Values)
			{
				KeycardItem keycardItem2 = itemBase as KeycardItem;
				if (keycardItem2 != null)
				{
					flag = true;
					keycardItem = keycardItem2;
					break;
				}
			}
			if (!flag)
			{
				Debug.LogError("Attempted to grant KeycardUpgrade to a player with no keycards.");
				return;
			}
			ItemType type = this.UpgradeKeycard(keycardItem.ItemTypeId);
			hub.inventory.ServerRemoveItem(keycardItem.ItemSerial, null);
			hub.inventory.ServerAddItem(type, ItemAddReason.Scp2536, 0, null).GrantAmmoReward();
		}

		// Token: 0x06006020 RID: 24608 RVA: 0x00141654 File Offset: 0x0013F854
		private ItemType GenerateOutcome(params ItemType[] outcomes)
		{
			return outcomes[UnityEngine.Random.Range(0, outcomes.Length)];
		}

		// Token: 0x06006021 RID: 24609 RVA: 0x00141664 File Offset: 0x0013F864
		public ItemType UpgradeKeycard(ItemType keyId)
		{
			switch (keyId)
			{
			case ItemType.KeycardJanitor:
				return this.GenerateOutcome(new ItemType[]
				{
					ItemType.KeycardZoneManager,
					ItemType.KeycardScientist
				});
			case ItemType.KeycardScientist:
				return ItemType.KeycardResearchCoordinator;
			case ItemType.KeycardResearchCoordinator:
				return ItemType.KeycardFacilityManager;
			case ItemType.KeycardZoneManager:
				return this.GenerateOutcome(new ItemType[]
				{
					ItemType.KeycardFacilityManager,
					ItemType.KeycardMTFOperative
				});
			case ItemType.KeycardGuard:
			case ItemType.KeycardMTFPrivate:
				return ItemType.KeycardMTFOperative;
			case ItemType.KeycardContainmentEngineer:
				return ItemType.KeycardFacilityManager;
			case ItemType.KeycardMTFOperative:
				return ItemType.KeycardMTFCaptain;
			default:
				return ItemType.KeycardChaosInsurgency;
			}
		}
	}
}
