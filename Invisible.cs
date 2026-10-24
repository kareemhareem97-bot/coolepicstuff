using System;
using PlayerStatsSystem;
using RemoteAdmin.Interfaces;

namespace CustomPlayerEffects
{
	// Token: 0x020008C0 RID: 2240
	public class Invisible : StatusEffectBase, ISpectatorDataPlayerEffect, ICustomDisplayName, IHitmarkerPreventer
	{
		// Token: 0x170009A5 RID: 2469
		// (get) Token: 0x060032E8 RID: 13032 RVA: 0x00040288 File Offset: 0x0003E488
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Positive;
			}
		}

		// Token: 0x170009A6 RID: 2470
		// (get) Token: 0x060032E9 RID: 13033 RVA: 0x000A95E7 File Offset: 0x000A77E7
		public string DisplayName
		{
			get
			{
				return "Invisibility";
			}
		}

		// Token: 0x170009A7 RID: 2471
		// (get) Token: 0x060032EA RID: 13034 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public bool CanBeDisplayed
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060032EB RID: 13035 RVA: 0x000A95EE File Offset: 0x000A77EE
		public bool GetSpectatorText(out string s)
		{
			s = "SCP-268";
			return true;
		}

		// Token: 0x060032EC RID: 13036 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public bool TryPreventHitmarker(AttackerDamageHandler attacker)
		{
			return true;
		}
	}
}
