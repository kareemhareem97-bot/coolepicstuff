using System;
using Mirror;
using UnityEngine;
using Utils.Networking;

namespace Hints
{
	// Token: 0x020007FB RID: 2043
	public abstract class CurveHintEffect : HintEffect
	{
		// Token: 0x170008D2 RID: 2258
		// (get) Token: 0x06002F65 RID: 12133 RVA: 0x0009FF85 File Offset: 0x0009E185
		// (set) Token: 0x06002F66 RID: 12134 RVA: 0x0009FF90 File Offset: 0x0009E190
		protected AnimationCurve Curve
		{
			get
			{
				return this._curve;
			}
			set
			{
				if (value != null && value.length > 0)
				{
					this.IterationScalar = value[value.length - 1].time - value[0].time;
					this.IterationScalar *= this.IterationScalar;
				}
				else
				{
					this.IterationScalar = 0f;
				}
				this._curve = value;
			}
		}

		// Token: 0x170008D3 RID: 2259
		// (get) Token: 0x06002F67 RID: 12135 RVA: 0x0009FFFC File Offset: 0x0009E1FC
		// (set) Token: 0x06002F68 RID: 12136 RVA: 0x000A0004 File Offset: 0x0009E204
		private protected float IterationScalar { protected get; private set; }

		// Token: 0x06002F69 RID: 12137 RVA: 0x000A000D File Offset: 0x0009E20D
		protected CurveHintEffect(float startScalar = 0f, float durationScalar = 1f) : base(startScalar, durationScalar)
		{
		}

		// Token: 0x06002F6A RID: 12138 RVA: 0x000A0017 File Offset: 0x0009E217
		protected CurveHintEffect(AnimationCurve curve, float startScalar = 0f, float durationScalar = 1f) : this(startScalar, durationScalar)
		{
			if (curve == null)
			{
				throw new ArgumentNullException("curve");
			}
			this.Curve = curve;
		}

		// Token: 0x06002F6B RID: 12139 RVA: 0x000A0037 File Offset: 0x0009E237
		public override void Deserialize(NetworkReader reader)
		{
			base.Deserialize(reader);
			this.Curve = reader.ReadAnimationCurve();
		}

		// Token: 0x06002F6C RID: 12140 RVA: 0x000A004C File Offset: 0x0009E24C
		public override void Serialize(NetworkWriter writer)
		{
			base.Serialize(writer);
			writer.WriteAnimationCurve(this.Curve);
		}

		// Token: 0x04002518 RID: 9496
		private AnimationCurve _curve;
	}
}
