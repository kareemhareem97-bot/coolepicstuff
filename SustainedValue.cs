using System;
using System.Diagnostics;

// Token: 0x0200007B RID: 123
public class SustainedValue
{
	// Token: 0x1700008A RID: 138
	// (get) Token: 0x060002A7 RID: 679 RVA: 0x0000A201 File Offset: 0x00008401
	// (set) Token: 0x060002A8 RID: 680 RVA: 0x0000A20C File Offset: 0x0000840C
	public float Value
	{
		get
		{
			return this._peak;
		}
		set
		{
			float num = this.ConditionalAbs(value);
			float num2 = this.ConditionalAbs(this._peak);
			bool flag = this._susTime > 0f;
			if ((this._inverse && num <= num2) || (!this._inverse && num >= num2))
			{
				this._peak = value;
				if (flag)
				{
					this._lastUpdateSw.Restart();
					return;
				}
			}
			else if (flag && this._lastUpdateSw.Elapsed.TotalSeconds >= (double)this._susTime)
			{
				this._peak = value;
			}
		}
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x0000A290 File Offset: 0x00008490
	public SustainedValue(float initialValue, float sustainTime, bool inverse, bool absolute)
	{
		this._peak = initialValue;
		this._susTime = sustainTime;
		this._inverse = inverse;
		this._abs = absolute;
		if (sustainTime > 0f)
		{
			this._lastUpdateSw = Stopwatch.StartNew();
		}
	}

	// Token: 0x060002AA RID: 682 RVA: 0x0000A2C8 File Offset: 0x000084C8
	private float ConditionalAbs(float val)
	{
		if (!this._abs || val >= 0f)
		{
			return val;
		}
		return -val;
	}

	// Token: 0x04000230 RID: 560
	private float _peak;

	// Token: 0x04000231 RID: 561
	private readonly float _susTime;

	// Token: 0x04000232 RID: 562
	private readonly bool _inverse;

	// Token: 0x04000233 RID: 563
	private readonly bool _abs;

	// Token: 0x04000234 RID: 564
	private readonly Stopwatch _lastUpdateSw;
}
