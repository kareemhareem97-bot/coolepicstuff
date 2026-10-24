using System;
using InventorySystem;
using InventorySystem.Configs;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010FC RID: 4348
	public class TierOneWeapons : Scp2536ItemGift, ITimeLimitedGift
	{
		// Token: 0x1700140F RID: 5135
		// (get) Token: 0x060060B3 RID: 24755 RVA: 0x0003F21F File Offset: 0x0003D41F
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Five;
			}
		}

		// Token: 0x17001410 RID: 5136
		// (get) Token: 0x060060B4 RID: 24756 RVA: 0x00142BBC File Offset: 0x00140DBC
		public float SecondsUntilAvailable
		{
			get
			{
				return 120f;
			}
		}

		// Token: 0x17001411 RID: 5137
		// (get) Token: 0x060060B5 RID: 24757 RVA: 0x00142BC3 File Offset: 0x00140DC3
		public float SecondsUntilUnavailable
		{
			get
			{
				return 240f;
			}
		}

		// Token: 0x17001412 RID: 5138
		// (get) Token: 0x060060B6 RID: 24758 RVA: 0x00142BCA File Offset: 0x00140DCA
		protected override Scp2536Reward[] Rewards
		{
			get
			{
				return new Scp2536Reward[]
				{
					new Scp2536Reward(ItemType.GunCOM15, 50f),
					new Scp2536Reward(ItemType.GunCOM18, 50f)
				};
			}
		}

		// Token: 0x060060B7 RID: 24759 RVA: 0x00142BF8 File Offset: 0x00140DF8
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

		// Token: 0x060060B8 RID: 24760 RVA: 0x00142C80 File Offset: 0x00140E80
		public override void ServerGrant(ReferenceHub hub)
		{
			ItemType type = base.GenerateRandomReward();
			hub.inventory.ServerAddItem(type, ItemAddReason.Scp2536, 0, null).GrantAmmoReward();
		}
	}
}
