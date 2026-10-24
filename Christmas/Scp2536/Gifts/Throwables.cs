using System;
using System.Collections.Generic;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.ThrowableProjectiles;
using Utils.NonAllocLINQ;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010F9 RID: 4345
	public class Throwables : Scp2536ItemGift
	{
		// Token: 0x17001409 RID: 5129
		// (get) Token: 0x060060A4 RID: 24740 RVA: 0x0003F21F File Offset: 0x0003D41F
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Five;
			}
		}

		// Token: 0x1700140A RID: 5130
		// (get) Token: 0x060060A5 RID: 24741 RVA: 0x001429E4 File Offset: 0x00140BE4
		protected override Scp2536Reward[] Rewards
		{
			get
			{
				return new Scp2536Reward[]
				{
					new Scp2536Reward(ItemType.GrenadeHE, 40f),
					new Scp2536Reward(ItemType.GrenadeFlash, 45f),
					new Scp2536Reward(ItemType.SCP018, 5f),
					new Scp2536Reward(ItemType.SCP2176, 10f)
				};
			}
		}

		// Token: 0x060060A6 RID: 24742 RVA: 0x00142A44 File Offset: 0x00140C44
		public override bool CanBeGranted(ReferenceHub hub)
		{
			if (base.CanBeGranted(hub))
			{
				return !hub.inventory.UserInventory.Items.Any((KeyValuePair<ushort, ItemBase> i) => i.Value is ThrowableItem);
			}
			return false;
		}

		// Token: 0x060060A7 RID: 24743 RVA: 0x00142A94 File Offset: 0x00140C94
		public override void ServerGrant(ReferenceHub hub)
		{
			ItemType type = base.GenerateRandomReward();
			hub.inventory.ServerAddItem(type, ItemAddReason.Scp2536, 0, null);
		}
	}
}
