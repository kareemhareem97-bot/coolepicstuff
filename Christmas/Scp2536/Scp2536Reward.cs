using System;

namespace Christmas.Scp2536
{
	// Token: 0x020010D5 RID: 4309
	public struct Scp2536Reward
	{
		// Token: 0x06005FF8 RID: 24568 RVA: 0x00140FC8 File Offset: 0x0013F1C8
		public Scp2536Reward(ItemType reward, float weight)
		{
			this.Reward = reward;
			this.Weight = weight;
		}

		// Token: 0x040045FF RID: 17919
		public readonly ItemType Reward;

		// Token: 0x04004600 RID: 17920
		public readonly float Weight;
	}
}
