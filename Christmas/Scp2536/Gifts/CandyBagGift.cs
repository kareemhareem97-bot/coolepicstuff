using System;
using System.Collections.Generic;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Usables.Scp330;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010DA RID: 4314
	public class CandyBagGift : Scp2536GiftBase
	{
		// Token: 0x170013E7 RID: 5095
		// (get) Token: 0x0600600E RID: 24590 RVA: 0x0003F21F File Offset: 0x0003D41F
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Five;
			}
		}

		// Token: 0x0600600F RID: 24591 RVA: 0x00141360 File Offset: 0x0013F560
		public override bool CanBeGranted(ReferenceHub hub)
		{
			if (!base.CanBeGranted(hub))
			{
				return false;
			}
			return !hub.inventory.UserInventory.Items.Any((KeyValuePair<ushort, ItemBase> i) => i.Value is Scp330Bag);
		}

		// Token: 0x06006010 RID: 24592 RVA: 0x001413B0 File Offset: 0x0013F5B0
		public override void ServerGrant(ReferenceHub hub)
		{
			for (int i = 0; i < 6; i++)
			{
				CandyKindID candyKindID = (10f < UnityEngine.Random.Range(0f, 100f)) ? Scp330Candies.GetRandom(CandyKindID.None) : CandyKindID.Black;
				CandyKindID candyId = (5f < UnityEngine.Random.Range(0f, 100f)) ? Scp330Candies.GetRandom(CandyKindID.None) : CandyKindID.Pink;
				hub.GrantCandy(candyId, ItemAddReason.Scp2536);
			}
		}

		// Token: 0x0400460C RID: 17932
		private const float PinkCandyChances = 5f;

		// Token: 0x0400460D RID: 17933
		private const float BlackCandyChances = 10f;
	}
}
