using System;
using UnityEngine;

// Token: 0x02000151 RID: 337
public class Remap
{
	// Token: 0x06000662 RID: 1634 RVA: 0x0001800A File Offset: 0x0001620A
	public float Get(float f)
	{
		return Remap.Evaluate(this._inMin, this._inMax, this._outMin, this._outMax, f, this._clamped);
	}

	// Token: 0x06000663 RID: 1635 RVA: 0x00018030 File Offset: 0x00016230
	public Remap(float inMin, float inMax, float outMin, float outMax, bool clamped = true)
	{
		this._inMin = inMin;
		this._inMax = inMax;
		this._outMin = outMin;
		this._outMax = outMax;
		this._clamped = clamped;
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x00018060 File Offset: 0x00016260
	public static float Evaluate(float inMin, float inMax, float outMin, float outMax, float inValue, bool clamped = true)
	{
		float num = (inMin != inMax) ? ((inValue - inMin) / (inMax - inMin)) : 0f;
		float num2 = outMin + (outMax - outMin) * num;
		if (!clamped)
		{
			return num2;
		}
		float min;
		float max;
		if (outMin < outMax)
		{
			min = outMin;
			max = outMax;
		}
		else
		{
			min = outMax;
			max = outMin;
		}
		return Mathf.Clamp(num2, min, max);
	}

	// Token: 0x040005E2 RID: 1506
	private readonly float _inMin;

	// Token: 0x040005E3 RID: 1507
	private readonly float _inMax;

	// Token: 0x040005E4 RID: 1508
	private readonly float _outMin;

	// Token: 0x040005E5 RID: 1509
	private readonly float _outMax;

	// Token: 0x040005E6 RID: 1510
	private readonly bool _clamped;
}
