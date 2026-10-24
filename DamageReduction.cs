using System;
using PlayerStatsSystem;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008B1 RID: 2225
	public class DamageReduction : StatusEffectBase, ISpectatorDataPlayerEffect, IDamageModifierEffect
	{
		// Token: 0x1700097F RID: 2431
		// (get) Token: 0x06003298 RID: 12952 RVA: 0x000A90C5 File Offset: 0x000A72C5
		private float CurrentMultiplier
		{
			get
			{
				return 1f - (float)base.Intensity * 0.005f;
			}
		}

		// Token: 0x17000980 RID: 2432
		// (get) Token: 0x06003299 RID: 12953 RVA: 0x00040288 File Offset: 0x0003E488
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Positive;
			}
		}

		// Token: 0x17000981 RID: 2433
		// (get) Token: 0x0600329A RID: 12954 RVA: 0x00004070 File Offset: 0x00002270
		public bool DamageModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x0600329B RID: 12955 RVA: 0x000A90DA File Offset: 0x000A72DA
		public bool GetSpectatorText(out string s)
		{
			s = string.Format("Damage Reduction (All, -{0}%)", Mathf.Round((1f - this.CurrentMultiplier) * 1000f) / 10f);
			return base.IsEnabled;
		}

		// Token: 0x0600329C RID: 12956 RVA: 0x000A9110 File Offset: 0x000A7310
		public float GetDamageModifier(float baseDamage, DamageHandlerBase handler, HitboxType hitboxType)
		{
			return this.CurrentMultiplier;
		}
	}
}
