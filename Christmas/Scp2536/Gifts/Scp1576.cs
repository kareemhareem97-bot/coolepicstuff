using System;
using InventorySystem;
using InventorySystem.Items;
using UnityEngine;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010E7 RID: 4327
	public class Scp1576 : Scp2536ItemGift
	{
		// Token: 0x170013F7 RID: 5111
		// (get) Token: 0x06006049 RID: 24649 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Exclusive;
			}
		}

		// Token: 0x170013F8 RID: 5112
		// (get) Token: 0x0600604A RID: 24650 RVA: 0x00141C33 File Offset: 0x0013FE33
		protected override Scp2536Reward[] Rewards
		{
			get
			{
				return new Scp2536Reward[]
				{
					new Scp2536Reward(ItemType.SCP1576, 100f)
				};
			}
		}

		// Token: 0x0600604B RID: 24651 RVA: 0x00141C4E File Offset: 0x0013FE4E
		public override bool CanBeGranted(ReferenceHub hub)
		{
			return !Scp1576._hasBeenGranted && base.CanBeGranted(hub) && UnityEngine.Random.value <= 0.05f;
		}

		// Token: 0x0600604C RID: 24652 RVA: 0x00141C74 File Offset: 0x0013FE74
		public override void ServerGrant(ReferenceHub hub)
		{
			ItemType type = base.GenerateRandomReward();
			hub.inventory.ServerAddItem(type, ItemAddReason.Scp2536, 0, null).GrantAmmoReward();
			Scp1576._hasBeenGranted = true;
		}

		// Token: 0x0600604D RID: 24653 RVA: 0x00141CA2 File Offset: 0x0013FEA2
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			CustomNetworkManager.OnClientReady += delegate()
			{
				Scp1576._hasBeenGranted = false;
			};
		}

		// Token: 0x0400461F RID: 17951
		private static bool _hasBeenGranted;
	}
}
