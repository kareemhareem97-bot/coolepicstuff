using System;
using UnityEngine;

namespace FacilitySoundtrack.ChaseThemes
{
	// Token: 0x02000885 RID: 2181
	[Serializable]
	public class StingerThresholdPair
	{
		// Token: 0x060031CA RID: 12746 RVA: 0x000A7334 File Offset: 0x000A5534
		public static StingerThresholdPair GetBest(StingerThresholdPair[] pairs, float val)
		{
			foreach (StingerThresholdPair stingerThresholdPair in pairs)
			{
				if (stingerThresholdPair.Threshold <= val)
				{
					return stingerThresholdPair;
				}
			}
			return null;
		}

		// Token: 0x040026E0 RID: 9952
		public AudioClip Clip;

		// Token: 0x040026E1 RID: 9953
		public float Threshold;
	}
}
