using System;
using System.Collections.Generic;
using Mirror;
using PlayerRoles.PlayableScps.Scp106;

namespace Achievements.Handlers
{
	// Token: 0x02000B05 RID: 2821
	public class MelancholyOfDecayHandler : AchievementHandlerBase
	{
		// Token: 0x06003FD8 RID: 16344 RVA: 0x000D40F1 File Offset: 0x000D22F1
		internal override void OnInitialize()
		{
			Scp106SinkholeController.OnSubmergeStateChange += this.OnSubmergeStateChange;
			Scp106Attack.OnPlayerTeleported += this.OnPlayerTeleported;
		}

		// Token: 0x06003FD9 RID: 16345 RVA: 0x000D4115 File Offset: 0x000D2315
		internal override void OnRoundStarted()
		{
			MelancholyOfDecayHandler.TimeframesPerNetId.Clear();
		}

		// Token: 0x06003FDA RID: 16346 RVA: 0x000D4124 File Offset: 0x000D2324
		private void OnPlayerTeleported(ReferenceHub scp106, ReferenceHub hub)
		{
			double num;
			if (!MelancholyOfDecayHandler.TimeframesPerNetId.TryGetValue(scp106.netId, out num))
			{
				return;
			}
			if (NetworkTime.time > num)
			{
				return;
			}
			AchievementHandlerBase.ServerAchieve(scp106.connectionToClient, AchievementName.MelancholyOfDecay);
		}

		// Token: 0x06003FDB RID: 16347 RVA: 0x000D415C File Offset: 0x000D235C
		private void OnSubmergeStateChange(Scp106Role scp106, bool newTargetSubmerged)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			ReferenceHub referenceHub;
			if (newTargetSubmerged || !scp106.TryGetOwner(out referenceHub))
			{
				return;
			}
			MelancholyOfDecayHandler.TimeframesPerNetId[referenceHub.netId] = NetworkTime.time + (double)scp106.Sinkhole.TargetTransitionDuration + 5.0;
		}

		// Token: 0x04002E8E RID: 11918
		private const int TimeToCapture = 5;

		// Token: 0x04002E8F RID: 11919
		private static readonly Dictionary<uint, double> TimeframesPerNetId = new Dictionary<uint, double>();
	}
}
