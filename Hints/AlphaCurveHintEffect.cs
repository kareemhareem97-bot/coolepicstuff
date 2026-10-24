using System;
using Mirror;
using UnityEngine;

namespace Hints
{
	// Token: 0x020007F7 RID: 2039
	public class AlphaCurveHintEffect : CurveHintEffect
	{
		// Token: 0x06002F57 RID: 12119 RVA: 0x0009FED5 File Offset: 0x0009E0D5
		public static AlphaCurveHintEffect FromNetwork(NetworkReader reader)
		{
			AlphaCurveHintEffect alphaCurveHintEffect = new AlphaCurveHintEffect();
			alphaCurveHintEffect.Deserialize(reader);
			return alphaCurveHintEffect;
		}

		// Token: 0x06002F58 RID: 12120 RVA: 0x0009FEE3 File Offset: 0x0009E0E3
		private AlphaCurveHintEffect() : base(0f, 1f)
		{
		}

		// Token: 0x06002F59 RID: 12121 RVA: 0x0009FEF5 File Offset: 0x0009E0F5
		public AlphaCurveHintEffect(AnimationCurve curve, float startScalar = 0f, float durationScalar = 1f) : base(curve, startScalar, durationScalar)
		{
		}
	}
}
