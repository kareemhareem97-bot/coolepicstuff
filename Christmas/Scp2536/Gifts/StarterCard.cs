using System;
using System.Collections.Generic;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Keycards;
using Utils.NonAllocLINQ;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010F5 RID: 4341
	public class StarterCard : Scp2536ItemGift
	{
		// Token: 0x17001406 RID: 5126
		// (get) Token: 0x06006093 RID: 24723 RVA: 0x0003F21F File Offset: 0x0003D41F
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Five;
			}
		}

		// Token: 0x17001407 RID: 5127
		// (get) Token: 0x06006094 RID: 24724 RVA: 0x001427EE File Offset: 0x001409EE
		protected override Scp2536Reward[] Rewards
		{
			get
			{
				return new Scp2536Reward[]
				{
					new Scp2536Reward(ItemType.KeycardJanitor, 50f),
					new Scp2536Reward(ItemType.KeycardScientist, 50f)
				};
			}
		}

		// Token: 0x06006095 RID: 24725 RVA: 0x0014281C File Offset: 0x00140A1C
		public override bool CanBeGranted(ReferenceHub hub)
		{
			if (base.CanBeGranted(hub))
			{
				return !hub.inventory.UserInventory.Items.Any((KeyValuePair<ushort, ItemBase> i) => i.Value is KeycardItem);
			}
			return false;
		}

		// Token: 0x06006096 RID: 24726 RVA: 0x0014286C File Offset: 0x00140A6C
		public override void ServerGrant(ReferenceHub hub)
		{
			ItemType type = base.GenerateRandomReward();
			hub.inventory.ServerAddItem(type, ItemAddReason.Scp2536, 0, null).GrantAmmoReward();
		}
	}
}
