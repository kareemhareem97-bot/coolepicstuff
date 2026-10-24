using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace CustomPlayerEffects
{
	// Token: 0x020008A4 RID: 2212
	public class CardiacHealthIndicator : SubEffectBase
	{
		// Token: 0x1700095C RID: 2396
		// (get) Token: 0x06003247 RID: 12871 RVA: 0x000A8706 File Offset: 0x000A6906
		public override bool IsActive
		{
			get
			{
				return base.MainEffect.IsEnabled;
			}
		}

		// Token: 0x06003248 RID: 12872 RVA: 0x000A8713 File Offset: 0x000A6913
		public override void DisableEffect()
		{
			base.DisableEffect();
		}

		// Token: 0x06003249 RID: 12873 RVA: 0x000A871B File Offset: 0x000A691B
		internal override void UpdateEffect()
		{
			base.UpdateEffect();
		}

		// Token: 0x0600324A RID: 12874 RVA: 0x000A8723 File Offset: 0x000A6923
		internal override void Init(StatusEffectBase mainEffect)
		{
			base.Init(mainEffect);
		}

		// Token: 0x04002733 RID: 10035
		[SerializeField]
		private AnimationCurve _healthToWeight;

		// Token: 0x04002734 RID: 10036
		[SerializeField]
		private Volume _ppv;

		// Token: 0x04002735 RID: 10037
		[SerializeField]
		private float _speedMultiplier;
	}
}
