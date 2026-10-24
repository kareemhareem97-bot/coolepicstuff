using System;
using Mirror;
using RemoteAdmin.Interfaces;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008D4 RID: 2260
	public class Scp1576 : StatusEffectBase, ICustomDisplayName
	{
		// Token: 0x170009E0 RID: 2528
		// (get) Token: 0x06003396 RID: 13206 RVA: 0x000AAE8B File Offset: 0x000A908B
		public string DisplayName
		{
			get
			{
				return "SCP-1576";
			}
		}

		// Token: 0x170009E1 RID: 2529
		// (get) Token: 0x06003397 RID: 13207 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public bool CanBeDisplayed
		{
			get
			{
				return true;
			}
		}

		// Token: 0x170009E2 RID: 2530
		// (get) Token: 0x06003398 RID: 13208 RVA: 0x00004094 File Offset: 0x00002294
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Technical;
			}
		}

		// Token: 0x06003399 RID: 13209 RVA: 0x000AAE92 File Offset: 0x000A9092
		protected override void OnEffectUpdate()
		{
			base.OnEffectUpdate();
			if (!NetworkServer.active)
			{
				return;
			}
			base.Intensity = (byte)Mathf.Clamp(base.TimeLeft, 0f, 255f);
		}
	}
}
