using System;
using UnityEngine;

namespace Cassie
{
	// Token: 0x02000ABD RID: 2749
	public struct CassieStutterSettings
	{
		// Token: 0x17000E1A RID: 3610
		// (get) Token: 0x06003E07 RID: 15879 RVA: 0x000CCD8B File Offset: 0x000CAF8B
		public readonly double DurationCost
		{
			get
			{
				return (double)(this.RepeatedLength * (float)this.Repetitions);
			}
		}

		// Token: 0x06003E08 RID: 15880 RVA: 0x000CCD9C File Offset: 0x000CAF9C
		public CassieStutterSettings(float startNormalizedTime, float repeatedLength, int repetitions)
		{
			this.StartNormalizedTime = Mathf.Clamp01(startNormalizedTime);
			this.RepeatedLength = Mathf.Max(repeatedLength, 0f);
			this.Repetitions = Mathf.Max(repetitions, 1);
		}

		// Token: 0x04002D61 RID: 11617
		public float StartNormalizedTime;

		// Token: 0x04002D62 RID: 11618
		public float RepeatedLength;

		// Token: 0x04002D63 RID: 11619
		public int Repetitions;
	}
}
