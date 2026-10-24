using System;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008CE RID: 2254
	public class RainbowTaste : StatusEffectBase, ISpectatorDataPlayerEffect
	{
		// Token: 0x170009D2 RID: 2514
		// (get) Token: 0x06003362 RID: 13154 RVA: 0x00040288 File Offset: 0x0003E488
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Positive;
			}
		}

		// Token: 0x06003363 RID: 13155 RVA: 0x000AA8C2 File Offset: 0x000A8AC2
		public bool GetSpectatorText(out string s)
		{
			s = "Rainbow Taste";
			return base.IsEnabled;
		}

		// Token: 0x06003364 RID: 13156 RVA: 0x000AA8D4 File Offset: 0x000A8AD4
		public static float CurrentMultiplier(ReferenceHub ply)
		{
			byte intensity = ply.playerEffectsController.GetEffect<RainbowTaste>().Intensity;
			return RainbowTaste.Multipliers[Mathf.Clamp((int)intensity, 0, RainbowTaste.Multipliers.Length - 1)];
		}

		// Token: 0x06003365 RID: 13157 RVA: 0x000AA908 File Offset: 0x000A8B08
		public static bool CheckPlayer(ReferenceHub ply)
		{
			return ply.playerEffectsController.GetEffect<RainbowTaste>().Intensity > 0;
		}

		// Token: 0x040027B3 RID: 10163
		private static readonly float[] Multipliers = new float[]
		{
			1f,
			0.6f,
			0.4f,
			0.35f
		};
	}
}
