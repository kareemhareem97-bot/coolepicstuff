using System;
using PlayerRoles.FirstPersonControl;

namespace CustomPlayerEffects
{
	// Token: 0x020008C3 RID: 2243
	public class MovementBoost : StatusEffectBase, IMovementSpeedModifier, ISpectatorDataPlayerEffect
	{
		// Token: 0x170009B3 RID: 2483
		// (get) Token: 0x0600330E RID: 13070 RVA: 0x00004070 File Offset: 0x00002270
		public bool MovementModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x170009B4 RID: 2484
		// (get) Token: 0x0600330F RID: 13071 RVA: 0x000A9C13 File Offset: 0x000A7E13
		public float MovementSpeedMultiplier
		{
			get
			{
				return (100f + (float)base.Intensity) / 100f;
			}
		}

		// Token: 0x170009B5 RID: 2485
		// (get) Token: 0x06003310 RID: 13072 RVA: 0x0000407F File Offset: 0x0000227F
		public float MovementSpeedLimit
		{
			get
			{
				return float.MaxValue;
			}
		}

		// Token: 0x170009B6 RID: 2486
		// (get) Token: 0x06003311 RID: 13073 RVA: 0x00040288 File Offset: 0x0003E488
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Positive;
			}
		}

		// Token: 0x06003312 RID: 13074 RVA: 0x000A9C28 File Offset: 0x000A7E28
		public bool GetSpectatorText(out string s)
		{
			s = string.Format("+{0}% Movement Boost", base.Intensity);
			return true;
		}
	}
}
