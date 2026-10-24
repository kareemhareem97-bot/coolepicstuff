using System;
using GameCore;

namespace Christmas.Scp2536
{
	// Token: 0x020010C8 RID: 4296
	public interface ITimeLimitedGift
	{
		// Token: 0x170013D4 RID: 5076
		// (get) Token: 0x06005FAC RID: 24492
		float SecondsUntilAvailable { get; }

		// Token: 0x170013D5 RID: 5077
		// (get) Token: 0x06005FAD RID: 24493
		float SecondsUntilUnavailable { get; }

		// Token: 0x170013D6 RID: 5078
		// (get) Token: 0x06005FAE RID: 24494 RVA: 0x00140014 File Offset: 0x0013E214
		bool IsAvailable
		{
			get
			{
				double totalSeconds = RoundStart.RoundLength.TotalSeconds;
				return totalSeconds > (double)this.SecondsUntilAvailable && totalSeconds < (double)this.SecondsUntilUnavailable;
			}
		}
	}
}
