using System;

// Token: 0x020001DA RID: 474
public static class EaseInUtils
{
	// Token: 0x060009FB RID: 2555 RVA: 0x0002772C File Offset: 0x0002592C
	public static float EaseInOutCubic(float t)
	{
		if (t < 0.5f)
		{
			float num = t * t * t;
			return 4f * num;
		}
		float num2 = -2f * t + 2f;
		float num3 = num2 * num2 * num2;
		return 1f - num3 / 2f;
	}

	// Token: 0x060009FC RID: 2556 RVA: 0x00027770 File Offset: 0x00025970
	public static float ChoppyLoading(double t, double chunkSize = 0.1)
	{
		return (float)(Math.Round(t * t * (3.0 - 2.0 * t) / chunkSize) * chunkSize);
	}
}
