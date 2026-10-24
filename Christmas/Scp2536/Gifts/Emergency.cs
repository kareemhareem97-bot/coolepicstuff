using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CustomPlayerEffects;
using MapGeneration;
using PlayerRoles.PlayableScps.Scp096;
using UnityEngine;

namespace Christmas.Scp2536.Gifts
{
	// Token: 0x020010DC RID: 4316
	public class Emergency : Scp2536ItemGift
	{
		// Token: 0x170013E8 RID: 5096
		// (get) Token: 0x06006015 RID: 24597 RVA: 0x00054ECB File Offset: 0x000530CB
		public override UrgencyLevel Urgency
		{
			get
			{
				return UrgencyLevel.Three;
			}
		}

		// Token: 0x170013E9 RID: 5097
		// (get) Token: 0x06006016 RID: 24598 RVA: 0x0014143A File Offset: 0x0013F63A
		protected override Scp2536Reward[] Rewards
		{
			get
			{
				return new Scp2536Reward[]
				{
					new Scp2536Reward(ItemType.SCP500, 100f),
					new Scp2536Reward(ItemType.SCP207, 100f)
				};
			}
		}

		// Token: 0x06006017 RID: 24599 RVA: 0x00141468 File Offset: 0x0013F668
		public override bool CanBeGranted(ReferenceHub hub)
		{
			if (!base.CanBeGranted(hub))
			{
				return false;
			}
			if (hub.playerEffectsController.GetEffect<CardiacArrest>().IsEnabled)
			{
				return true;
			}
			if (hub.playerEffectsController.GetEffect<Corroding>().IsEnabled)
			{
				return true;
			}
			int uniqueLifeIdentifier = hub.roleManager.CurrentRole.UniqueLifeIdentifier;
			float num;
			return (Emergency.TargetTracker.TryGetValue(uniqueLifeIdentifier, out num) && Time.time <= num) || (AlphaWarheadController.InProgress && hub.GetCurrentZone() != FacilityZone.Surface);
		}

		// Token: 0x06006018 RID: 24600 RVA: 0x001414E7 File Offset: 0x0013F6E7
		public override void ServerGrant(ReferenceHub hub)
		{
			base.GrantAllRewards(hub);
		}

		// Token: 0x06006019 RID: 24601 RVA: 0x001414F0 File Offset: 0x0013F6F0
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			Action<ReferenceHub, ReferenceHub> value;
			if ((value = Emergency.<>O.<0>__OnTargetAdded) == null)
			{
				value = (Emergency.<>O.<0>__OnTargetAdded = new Action<ReferenceHub, ReferenceHub>(Emergency.OnTargetAdded));
			}
			Scp096TargetsTracker.OnTargetAdded += value;
		}

		// Token: 0x0600601A RID: 24602 RVA: 0x00141514 File Offset: 0x0013F714
		private static void OnTargetAdded(ReferenceHub scp096, ReferenceHub target)
		{
			int uniqueLifeIdentifier = target.roleManager.CurrentRole.UniqueLifeIdentifier;
			Emergency.TargetTracker[uniqueLifeIdentifier] = Time.time + 30f;
		}

		// Token: 0x04004610 RID: 17936
		private static readonly Dictionary<int, float> TargetTracker = new Dictionary<int, float>();

		// Token: 0x04004611 RID: 17937
		private const float Scp096TargetPersistanceDuration = 30f;

		// Token: 0x020010DD RID: 4317
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04004612 RID: 17938
			public static Action<ReferenceHub, ReferenceHub> <0>__OnTargetAdded;
		}
	}
}
