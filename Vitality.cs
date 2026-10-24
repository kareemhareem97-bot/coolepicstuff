using System;

namespace CustomPlayerEffects
{
	// Token: 0x020008EC RID: 2284
	public class Vitality : StatusEffectBase, ISpectatorDataPlayerEffect
	{
		// Token: 0x17000A42 RID: 2626
		// (get) Token: 0x06003467 RID: 13415 RVA: 0x00040288 File Offset: 0x0003E488
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Positive;
			}
		}

		// Token: 0x06003468 RID: 13416 RVA: 0x000AC8AE File Offset: 0x000AAAAE
		public bool GetSpectatorText(out string s)
		{
			s = "Vitality";
			return true;
		}

		// Token: 0x06003469 RID: 13417 RVA: 0x000AC8B8 File Offset: 0x000AAAB8
		public static bool CheckPlayer(ReferenceHub ply)
		{
			return ply != null && ply.playerEffectsController.GetEffect<Vitality>().IsEnabled;
		}
	}
}
