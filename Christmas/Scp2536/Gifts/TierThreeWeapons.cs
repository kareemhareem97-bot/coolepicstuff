using System;
using InventorySystem;
using InventorySystem.Configs;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010FD RID: 4349
	public class TierThreeWeapons : Scp2536ItemGift, ITimeLimitedGift
	{
		// Token: 0x17001413 RID: 5139
		// (get) Token: 0x060060BA RID: 24762 RVA: 0x0003F21F File Offset: 0x0003D41F
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Five;
			}
		}

		// Token: 0x17001414 RID: 5140
		// (get) Token: 0x060060BB RID: 24763 RVA: 0x0014170F File Offset: 0x0013F90F
		public float SecondsUntilAvailable
		{
			get
			{
				return 360f;
			}
		}

		// Token: 0x17001415 RID: 5141
		// (get) Token: 0x060060BC RID: 24764 RVA: 0x00142AD5 File Offset: 0x00140CD5
		public float SecondsUntilUnavailable
		{
			get
			{
				return 600f;
			}
		}

		// Token: 0x17001416 RID: 5142
		// (get) Token: 0x060060BD RID: 24765 RVA: 0x00142CA8 File Offset: 0x00140EA8
		protected override Scp2536Reward[] Rewards
		{
			get
			{
				return new Scp2536Reward[]
				{
					new Scp2536Reward(ItemType.GunE11SR, 35f),
					new Scp2536Reward(ItemType.GunAK, 35f),
					new Scp2536Reward(ItemType.GunShotgun, 30f)
				};
			}
		}

		// Token: 0x060060BE RID: 24766 RVA: 0x00142CF4 File Offset: 0x00140EF4
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

		// Token: 0x060060BF RID: 24767 RVA: 0x00142D7C File Offset: 0x00140F7C
		public override void ServerGrant(ReferenceHub hub)
		{
			ItemType type = base.GenerateRandomReward();
			hub.inventory.ServerAddItem(type, ItemAddReason.Scp2536, 0, null).GrantAmmoReward();
		}
	}
}
