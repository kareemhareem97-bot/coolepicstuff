using System;
using UnityEngine;

namespace FacilitySoundtrack.ChaseThemes
{
	// Token: 0x02000880 RID: 2176
	public readonly struct DesiredChaseClip
	{
		// Token: 0x060031BC RID: 12732 RVA: 0x000A71ED File Offset: 0x000A53ED
		public DesiredChaseClip(AudioClip clip, float trackVolume = 1f, float volumeIncSpeed = 10f, float volumeDecSpeed = 10f, DesiredChaseClip.Offset? offset = null)
		{
			this.Clip = clip;
			this.TrackVolume = trackVolume;
			this.VolumeIncreaseSpeed = volumeIncSpeed;
			this.VolumeDecreaseSpeed = volumeDecSpeed;
			this.JoinOffset = offset;
		}

		// Token: 0x040026D6 RID: 9942
		public readonly AudioClip Clip;

		// Token: 0x040026D7 RID: 9943
		public readonly float TrackVolume;

		// Token: 0x040026D8 RID: 9944
		public readonly float VolumeIncreaseSpeed;

		// Token: 0x040026D9 RID: 9945
		public readonly float VolumeDecreaseSpeed;

		// Token: 0x040026DA RID: 9946
		public readonly DesiredChaseClip.Offset? JoinOffset;

		// Token: 0x02000881 RID: 2177
		public readonly struct Offset
		{
			// Token: 0x060031BD RID: 12733 RVA: 0x000A7214 File Offset: 0x000A5414
			public Offset(double schedulingDelaySeconds, int startSamples, bool dontSyncWithOthers = false)
			{
				if (schedulingDelaySeconds < 0.0)
				{
					schedulingDelaySeconds = 0.0;
					Debug.LogError("Attempting to schedule playback in the past.");
				}
				this.SchedulingDelay = schedulingDelaySeconds;
				this.StartSamples = startSamples;
				this.DontSyncWithOthers = dontSyncWithOthers;
			}

			// Token: 0x040026DB RID: 9947
			public readonly double SchedulingDelay;

			// Token: 0x040026DC RID: 9948
			public readonly int StartSamples;

			// Token: 0x040026DD RID: 9949
			public readonly bool DontSyncWithOthers;
		}
	}
}
