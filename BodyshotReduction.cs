using System;
using PlayerStatsSystem;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008A1 RID: 2209
	public class BodyshotReduction : StatusEffectBase, ISpectatorDataPlayerEffect, IDamageModifierEffect
	{
		// Token: 0x17000952 RID: 2386
		// (get) Token: 0x0600322E RID: 12846 RVA: 0x00040288 File Offset: 0x0003E488
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Positive;
			}
		}

		// Token: 0x17000953 RID: 2387
		// (get) Token: 0x0600322F RID: 12847 RVA: 0x00004070 File Offset: 0x00002270
		public bool DamageModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x17000954 RID: 2388
		// (get) Token: 0x06003230 RID: 12848 RVA: 0x000A851C File Offset: 0x000A671C
		private float CurrentMultiplier
		{
			get
			{
				return BodyshotReduction.Multipliers[Mathf.Min((int)base.Intensity, BodyshotReduction.Multipliers.Length - 1)];
			}
		}

		// Token: 0x06003231 RID: 12849 RVA: 0x000A8538 File Offset: 0x000A6738
		public bool GetSpectatorText(out string s)
		{
			s = string.Format("Damage Reduction (Body, -{0}%)", Mathf.Round((1f - this.CurrentMultiplier) * 1000f) / 10f);
			return base.IsEnabled;
		}

		// Token: 0x06003232 RID: 12850 RVA: 0x000A856E File Offset: 0x000A676E
		public float GetDamageModifier(float baseDamage, DamageHandlerBase handler, HitboxType hitboxType)
		{
			if (hitboxType == HitboxType.Body)
			{
				return this.CurrentMultiplier;
			}
			return 1f;
		}

		// Token: 0x0400272A RID: 10026
		private static readonly float[] Multipliers = new float[]
		{
			1f,
			0.95f,
			0.9f,
			0.875f,
			0.85f
		};
	}
}
