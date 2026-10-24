using System;
using System.Globalization;
using Mirror;
using UnityEngine;
using Utils.Networking;

namespace Hints
{
	// Token: 0x02000812 RID: 2066
	public class AnimationCurveHintParameter : HintParameter
	{
		// Token: 0x06002FBB RID: 12219 RVA: 0x000A0698 File Offset: 0x0009E898
		public static AnimationCurveHintParameter FromNetwork(NetworkReader reader)
		{
			AnimationCurveHintParameter animationCurveHintParameter = new AnimationCurveHintParameter();
			animationCurveHintParameter.Deserialize(reader);
			return animationCurveHintParameter;
		}

		// Token: 0x06002FBC RID: 12220 RVA: 0x000A06A6 File Offset: 0x0009E8A6
		public AnimationCurveHintParameter(double offset, AnimationCurve curve, string format, bool integral)
		{
			this._offset = offset;
			this._curve = curve;
			this._format = format;
			this._integral = integral;
		}

		// Token: 0x06002FBD RID: 12221 RVA: 0x000A0569 File Offset: 0x0009E769
		protected AnimationCurveHintParameter()
		{
		}

		// Token: 0x06002FBE RID: 12222 RVA: 0x000A06CB File Offset: 0x0009E8CB
		public override void Serialize(NetworkWriter writer)
		{
			writer.WriteDouble(this._offset);
			writer.WriteString(this._format);
			writer.WriteBool(this._integral);
			writer.WriteAnimationCurve(this._curve);
		}

		// Token: 0x06002FBF RID: 12223 RVA: 0x000A06FD File Offset: 0x0009E8FD
		public override void Deserialize(NetworkReader reader)
		{
			this._offset = reader.ReadDouble();
			this._format = reader.ReadString();
			this._integral = reader.ReadBool();
			this._curve = reader.ReadAnimationCurve();
		}

		// Token: 0x06002FC0 RID: 12224 RVA: 0x000A0730 File Offset: 0x0009E930
		protected override string UpdateState(float progress)
		{
			double num = NetworkTime.time - this._offset;
			float f = this._curve.Evaluate((float)num);
			if (this._integral)
			{
				return Mathf.RoundToInt(f).ToString(this._format, CultureInfo.InvariantCulture);
			}
			return f.ToString(this._format, CultureInfo.InvariantCulture);
		}

		// Token: 0x0400253D RID: 9533
		private double _offset;

		// Token: 0x0400253E RID: 9534
		private string _format;

		// Token: 0x0400253F RID: 9535
		private bool _integral;

		// Token: 0x04002540 RID: 9536
		private AnimationCurve _curve;
	}
}
