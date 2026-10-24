using System;
using InventorySystem;
using InventorySystem.Configs;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010FE RID: 4350
	public class TierTwoWeapons : Scp2536ItemGift, ITimeLimitedGift
	{
		// Token: 0x17001417 RID: 5143
		// (get) Token: 0x060060C1 RID: 24769 RVA: 0x0003F21F File Offset: 0x0003D41F
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Five;
			}
		}

		// Token: 0x17001418 RID: 5144
		// (get) Token: 0x060060C2 RID: 24770 RVA: 0x00142BC3 File Offset: 0x00140DC3
		public float SecondsUntilAvailable
		{
			get
			{
				return 240f;
			}
		}

		// Token: 0x17001419 RID: 5145
		// (get) Token: 0x060060C3 RID: 24771 RVA: 0x0014170F File Offset: 0x0013F90F
		public float SecondsUntilUnavailable
		{
			get
			{
				return 360f;
			}
		}

		// Token: 0x1700141A RID: 5146
		// (get) Token: 0x060060C4 RID: 24772 RVA: 0x00142DA4 File Offset: 0x00140FA4
		protected override Scp2536Reward[] Rewards
		{
			get
			{
				return new Scp2536Reward[]
				{
					new Scp2536Reward(ItemType.GunFSP9, 30f),
					new Scp2536Reward(ItemType.GunCrossvec, 45f),
					new Scp2536Reward(ItemType.GunA7, 10f),
					new Scp2536Reward(ItemType.GunRevolver, 15f)
				};
			}
		}

		// Token: 0x060060C5 RID: 24773 RVA: 0x00142E04 File Offset: 0x00141004
		public override bool CanBeGranted(ReferenceHub hub)
		{
			if (!base.CanBeGranted(hub))
			{
				return false;
			}
			int num = 0;
			foreach (ItemBase itemBase in hub.inventory.UserInventory.Items.Values)
			{
				if (itemBase.CountsTowardsCategoryLimit && itemBase is Firearm)
				{
					num++;
				}
			}
			return num < (int)InventoryLimits.GetCategoryLimit(ItemCategory.Firearm, hub);
		}

		// Token: 0x060060C6 RID: 24774 RVA: 0x00142E8C File Offset: 0x0014108C
		public override void ServerGrant(ReferenceHub hub)
		{
			ItemType type = base.GenerateRandomReward();
			hub.inventory.ServerAddItem(type, ItemAddReason.Scp2536, 0, null).GrantAmmoReward();
		}
	}
}
