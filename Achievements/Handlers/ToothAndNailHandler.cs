using System;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Modules;
using InventorySystem.Items.Firearms.Modules.Scp127;

namespace Achievements.Handlers
{
	// Token: 0x02000B18 RID: 2840
	public class ToothAndNailHandler : AchievementHandlerBase
	{
		// Token: 0x06004002 RID: 16386 RVA: 0x000D4838 File Offset: 0x000D2A38
		internal override void OnInitialize()
		{
			Scp127TierManagerModule.ServerOnLevelledUp += this.OnLevelUp;
		}

		// Token: 0x06004003 RID: 16387 RVA: 0x000D484C File Offset: 0x000D2A4C
		private void OnLevelUp(Firearm firearm)
		{
			if (!firearm.HasOwner)
			{
				return;
			}
			Scp127TierManagerModule scp127TierManagerModule;
			if (!firearm.TryGetModule(out scp127TierManagerModule, true))
			{
				return;
			}
			if (scp127TierManagerModule.CurTier < Scp127Tier.Tier3)
			{
				return;
			}
			AchievementHandlerBase.ServerAchieve(firearm.Owner.connectionToClient, AchievementName.ToothAndNail);
		}

		// Token: 0x04002E9D RID: 11933
		private const Scp127Tier TierLimit = Scp127Tier.Tier3;
	}
}
