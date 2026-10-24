using System;
using InventorySystem;
using InventorySystem.Configs;
using InventorySystem.Items;
using InventorySystem.Items.Firearms;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010FB RID: 4347
	public class TierFourWeapons : Scp2536ItemGift, ITimeLimitedGift
	{
		// Token: 0x1700140B RID: 5131
		// (get) Token: 0x060060AC RID: 24748 RVA: 0x0003F21F File Offset: 0x0003D41F
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Five;
			}
		}

		// Token: 0x1700140C RID: 5132
		// (get) Token: 0x060060AD RID: 24749 RVA: 0x00142AD5 File Offset: 0x00140CD5
		public float SecondsUntilAvailable
		{
			get
			{
				return 600f;
			}
		}

		// Token: 0x1700140D RID: 5133
		// (get) Token: 0x060060AE RID: 24750 RVA: 0x0000548E File Offset: 0x0000368E
		public float SecondsUntilUnavailable
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x1700140E RID: 5134
		// (get) Token: 0x060060AF RID: 24751 RVA: 0x00142ADC File Offset: 0x00140CDC
		protected override Scp2536Reward[] Rewards
		{
			get
			{
				return new Scp2536Reward[]
				{
					new Scp2536Reward(ItemType.GunLogicer, 50f),
					new Scp2536Reward(ItemType.GunFRMG0, 50f)
				};
			}
		}

		// Token: 0x060060B0 RID: 24752 RVA: 0x00142B0C File Offset: 0x00140D0C
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

		// Token: 0x060060B1 RID: 24753 RVA: 0x00142B94 File Offset: 0x00140D94
		public override void ServerGrant(ReferenceHub hub)
		{
			ItemType type = base.GenerateRandomReward();
			hub.inventory.ServerAddItem(type, ItemAddReason.Scp2536, 0, null).GrantAmmoReward();
		}
	}
}
