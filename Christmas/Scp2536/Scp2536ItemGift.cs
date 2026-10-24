using System;
using System.Collections.Generic;
using System.Linq;
using InventorySystem;
using InventorySystem.Items;
using UnityEngine;

namespace Christmas.Scp2536
{
	// Token: 0x020010D3 RID: 4307
	public abstract class Scp2536ItemGift : Scp2536GiftBase
	{
		// Token: 0x06005FF1 RID: 24561 RVA: 0x00140E9E File Offset: 0x0013F09E
		protected Scp2536ItemGift()
		{
			this._chanceSum = this.Rewards.Sum((Scp2536Reward r) => r.Weight);
		}

		// Token: 0x170013E2 RID: 5090
		// (get) Token: 0x06005FF2 RID: 24562
		protected abstract Scp2536Reward[] Rewards { get; }

		// Token: 0x06005FF3 RID: 24563 RVA: 0x00140ED8 File Offset: 0x0013F0D8
		protected void GrantAllRewards(ReferenceHub hub)
		{
			Scp2536Reward[] rewards = this.Rewards;
			for (int i = 0; i < rewards.Length; i++)
			{
				ItemType reward = rewards[i].Reward;
				Dictionary<ushort, ItemBase> items = hub.inventory.UserInventory.Items;
				if (items.Count >= 8)
				{
					hub.inventory.ServerDropItem(items.ElementAt(items.Count - 1).Key);
				}
				hub.inventory.ServerAddItem(reward, ItemAddReason.Scp2536, 0, null).GrantAmmoReward();
			}
		}

		// Token: 0x06005FF4 RID: 24564 RVA: 0x00140F58 File Offset: 0x0013F158
		protected ItemType GenerateRandomReward()
		{
			float num = UnityEngine.Random.Range(0f, this._chanceSum);
			ItemType result = ItemType.Coin;
			foreach (Scp2536Reward scp2536Reward in this.Rewards)
			{
				result = scp2536Reward.Reward;
				num -= scp2536Reward.Weight;
				if (num <= 0f)
				{
					return result;
				}
			}
			return result;
		}

		// Token: 0x040045FC RID: 17916
		private readonly float _chanceSum;
	}
}
