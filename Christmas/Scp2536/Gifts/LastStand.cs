using System;
using System.Linq;
using InventorySystem;
using InventorySystem.Items;
using PlayerRoles;
using UnityEngine;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010E0 RID: 4320
	public class LastStand : Scp2536ItemGift, ITimeLimitedGift
	{
		// Token: 0x170013EB RID: 5099
		// (get) Token: 0x06006026 RID: 24614 RVA: 0x0003F71E File Offset: 0x0003D91E
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Four;
			}
		}

		// Token: 0x170013EC RID: 5100
		// (get) Token: 0x06006027 RID: 24615 RVA: 0x0014170F File Offset: 0x0013F90F
		public float SecondsUntilAvailable
		{
			get
			{
				return 360f;
			}
		}

		// Token: 0x170013ED RID: 5101
		// (get) Token: 0x06006028 RID: 24616 RVA: 0x0000548E File Offset: 0x0000368E
		public float SecondsUntilUnavailable
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x170013EE RID: 5102
		// (get) Token: 0x06006029 RID: 24617 RVA: 0x00141718 File Offset: 0x0013F918
		protected override Scp2536Reward[] Rewards
		{
			get
			{
				return new Scp2536Reward[]
				{
					new Scp2536Reward(ItemType.GunLogicer, 100f),
					new Scp2536Reward(ItemType.SCP500, 100f),
					new Scp2536Reward(ItemType.SCP1853, 100f)
				};
			}
		}

		// Token: 0x0600602A RID: 24618 RVA: 0x00141764 File Offset: 0x0013F964
		public override bool CanBeGranted(ReferenceHub hub)
		{
			if (LastStand._hasBeenGranted || !base.CanBeGranted(hub))
			{
				return false;
			}
			Faction localFaction = hub.GetFaction();
			return ReferenceHub.AllHubs.Count((ReferenceHub x) => x.GetFaction() == localFaction) <= 3;
		}

		// Token: 0x0600602B RID: 24619 RVA: 0x001417B4 File Offset: 0x0013F9B4
		public override void ServerGrant(ReferenceHub hub)
		{
			LastStand._hasBeenGranted = true;
			ItemType type = base.GenerateRandomReward();
			hub.inventory.ServerAddItem(type, ItemAddReason.Scp2536, 0, null).GrantAmmoReward();
			if (UnityEngine.Random.Range(1, 101) > 40)
			{
				hub.inventory.ServerAddItem(ItemType.Scp021J, ItemAddReason.Scp2536, 0, null);
			}
		}

		// Token: 0x0600602C RID: 24620 RVA: 0x001417FF File Offset: 0x0013F9FF
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			CustomNetworkManager.OnClientReady += delegate()
			{
				LastStand._hasBeenGranted = false;
			};
		}

		// Token: 0x04004615 RID: 17941
		private const int EvilWaterBottleChance = 40;

		// Token: 0x04004616 RID: 17942
		private const int MaxTeammatesRequirement = 3;

		// Token: 0x04004617 RID: 17943
		private static bool _hasBeenGranted;
	}
}
