using System;
using InventorySystem;
using InventorySystem.Items;
using UnityEngine;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010E6 RID: 4326
	public class NonStandardWeapons : Scp2536ItemGift
	{
		// Token: 0x170013F4 RID: 5108
		// (get) Token: 0x06006043 RID: 24643 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Exclusive;
			}
		}

		// Token: 0x170013F5 RID: 5109
		// (get) Token: 0x06006044 RID: 24644 RVA: 0x00002DA5 File Offset: 0x00000FA5
		internal override bool IgnoredByRandomness
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170013F6 RID: 5110
		// (get) Token: 0x06006045 RID: 24645 RVA: 0x00141B70 File Offset: 0x0013FD70
		protected override Scp2536Reward[] Rewards
		{
			get
			{
				return new Scp2536Reward[]
				{
					new Scp2536Reward(ItemType.ParticleDisruptor, 25f),
					new Scp2536Reward(ItemType.Jailbird, 25f),
					new Scp2536Reward(ItemType.GunCom45, 10f),
					new Scp2536Reward(ItemType.Scp021J, 40f)
				};
			}
		}

		// Token: 0x06006046 RID: 24646 RVA: 0x00141BD0 File Offset: 0x0013FDD0
		public override void ServerGrant(ReferenceHub hub)
		{
			ItemType type = base.GenerateRandomReward();
			hub.inventory.ServerAddItem(type, ItemAddReason.Scp2536, 0, null).GrantAmmoReward();
		}

		// Token: 0x06006047 RID: 24647 RVA: 0x00141BF8 File Offset: 0x0013FDF8
		public static bool CanOverrideGift(Scp2536GiftBase gift)
		{
			return UnityEngine.Random.Range(1, 101) <= 15 && (gift is TierOneWeapons || gift is TierTwoWeapons || gift is TierThreeWeapons || gift is TierFourWeapons);
		}

		// Token: 0x0400461E RID: 17950
		private const int OverrideWeaponGiftChance = 15;
	}
}
