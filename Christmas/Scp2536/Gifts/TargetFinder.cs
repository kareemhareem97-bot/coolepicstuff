using System;
using System.Collections.Generic;
using CustomPlayerEffects;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010F0 RID: 4336
	public class TargetFinder : Scp2536AnomalousGiftBase
	{
		// Token: 0x17001400 RID: 5120
		// (get) Token: 0x0600607C RID: 24700 RVA: 0x00034727 File Offset: 0x00032927
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.One;
			}
		}

		// Token: 0x0600607D RID: 24701 RVA: 0x001423EC File Offset: 0x001405EC
		public override bool CanBeGranted(ReferenceHub hub)
		{
			int num = RoundSummary.singleton.TargetCount + RoundSummary.singleton.ExtraTargets;
			return num <= 5 && num != 0 && base.CanBeGranted(hub) && !this._coroutineHandle.IsRunning;
		}

		// Token: 0x0600607E RID: 24702 RVA: 0x00142431 File Offset: 0x00140631
		public override void ServerGrant(ReferenceHub hub)
		{
			base.ServerGrant(hub);
			this._coroutineHandle = Timing.RunCoroutine(this.ApplyEffect().CancelWith(Scp2536Controller.Singleton.gameObject));
		}

		// Token: 0x0600607F RID: 24703 RVA: 0x0014245A File Offset: 0x0014065A
		private IEnumerator<float> ApplyEffect()
		{
			yield return float.NegativeInfinity;
			this._timeLeft = 15f;
			while (this._timeLeft > 0f)
			{
				this._timeLeft -= Time.deltaTime;
				foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
				{
					Team team = referenceHub.roleManager.CurrentRole.Team;
					if (team != Team.SCPs && team != Team.Dead)
					{
						AnomalousTarget effect = referenceHub.playerEffectsController.GetEffect<AnomalousTarget>();
						if (!effect.IsEnabled)
						{
							effect.ServerSetState(1, this._timeLeft, false);
						}
					}
				}
			}
			yield break;
		}

		// Token: 0x0400463E RID: 17982
		private const float EffectDuration = 15f;

		// Token: 0x0400463F RID: 17983
		private const int MaxTargetCount = 5;

		// Token: 0x04004640 RID: 17984
		private CoroutineHandle _coroutineHandle;

		// Token: 0x04004641 RID: 17985
		private float _timeLeft;
	}
}
