using System;
using InventorySystem;
using InventorySystem.Items;
using PlayerStatsSystem;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010E3 RID: 4323
	public class MedicalItems : Scp2536ItemGift
	{
		// Token: 0x170013EF RID: 5103
		// (get) Token: 0x06006033 RID: 24627 RVA: 0x0003F21F File Offset: 0x0003D41F
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Five;
			}
		}

		// Token: 0x170013F0 RID: 5104
		// (get) Token: 0x06006034 RID: 24628 RVA: 0x0014184C File Offset: 0x0013FA4C
		protected override Scp2536Reward[] Rewards
		{
			get
			{
				return new Scp2536Reward[]
				{
					new Scp2536Reward(ItemType.Medkit, 50f),
					new Scp2536Reward(ItemType.Painkillers, 40f),
					new Scp2536Reward(ItemType.AntiSCP207, 10f)
				};
			}
		}

		// Token: 0x06006035 RID: 24629 RVA: 0x00141898 File Offset: 0x0013FA98
		public override bool CanBeGranted(ReferenceHub hub)
		{
			HealthStat healthStat;
			return base.CanBeGranted(hub) && hub.playerStats.TryGetModule<HealthStat>(out healthStat) && healthStat.CurValue <= healthStat.MaxValue * 0.5f;
		}

		// Token: 0x06006036 RID: 24630 RVA: 0x001418D8 File Offset: 0x0013FAD8
		public override void ServerGrant(ReferenceHub hub)
		{
			ItemType type = base.GenerateRandomReward();
			hub.inventory.ServerAddItem(type, ItemAddReason.Scp2536, 0, null).GrantAmmoReward();
		}
	}
}
