using System;
using UnityEngine;

namespace Cassie
{
	// Token: 0x02000AB2 RID: 2738
	public class CassieBackgroundPlayer : MonoBehaviour
	{
		// Token: 0x06003DD2 RID: 15826 RVA: 0x000CBF90 File Offset: 0x000CA190
		public void Play(double startingTime, double endingTime)
		{
			this.Clear();
			double num = endingTime - startingTime;
			if (num < (double)this._minDuration)
			{
				num = (double)this._minDuration;
				endingTime = startingTime + (double)this._minDuration;
			}
			CassieBackgroundPlayer.PlayScheduled(this._startingSource, startingTime);
			CassieBackgroundPlayer.PlayScheduled(this._endingSource, endingTime - 29.100000381469727);
			AnnouncementSource.SetVolumeScale(this._startingSource, 1f);
			AnnouncementSource.SetVolumeScale(this._endingSource, 0f);
			this._transitionTime = new double?(startingTime + (double)this._transitionTimeOverDuration.Evaluate((float)num));
		}

		// Token: 0x06003DD3 RID: 15827 RVA: 0x000CC021 File Offset: 0x000CA221
		public void Clear()
		{
			this._transitionTime = null;
			this._startingSource.Stop();
			this._endingSource.Stop();
		}

		// Token: 0x06003DD4 RID: 15828 RVA: 0x000CC048 File Offset: 0x000CA248
		private void Update()
		{
			if (this._transitionTime == null)
			{
				return;
			}
			double dspTime = AudioSettings.dspTime;
			if (dspTime < this._transitionTime.Value)
			{
				return;
			}
			float num = (float)(dspTime - this._transitionTime.Value) / 0.8f;
			AnnouncementSource.SetVolumeScale(this._startingSource, this._startingSourceTransitionCurve.Evaluate(num));
			AnnouncementSource.SetVolumeScale(this._endingSource, this._endingSourceTransitionCurve.Evaluate(num));
			if (num < 1f)
			{
				return;
			}
			this._transitionTime = null;
		}

		// Token: 0x06003DD5 RID: 15829 RVA: 0x000CC0D0 File Offset: 0x000CA2D0
		private static void PlayScheduled(AudioSource src, double startingTime)
		{
			double dspTime = AudioSettings.dspTime;
			startingTime -= 0.15000000596046448;
			if (startingTime - dspTime >= 0.0)
			{
				src.timeSamples = 0;
				src.PlayScheduled(startingTime + 0.15000000596046448);
				return;
			}
			double num = (dspTime - startingTime) * (double)src.clip.frequency;
			if (num < 0.0 || num >= (double)src.clip.samples)
			{
				return;
			}
			src.timeSamples = (int)num;
			src.PlayScheduled(0.15000000596046448);
		}

		// Token: 0x04002D2E RID: 11566
		public const float AnnouncementStart = 2.5f;

		// Token: 0x04002D2F RID: 11567
		public const float AnnouncementEnd = 29.1f;

		// Token: 0x04002D30 RID: 11568
		public const float EndCooldown = 3.1f;

		// Token: 0x04002D31 RID: 11569
		private double? _transitionTime;

		// Token: 0x04002D32 RID: 11570
		[Header("Starting")]
		[SerializeField]
		private AudioSource _startingSource;

		// Token: 0x04002D33 RID: 11571
		[SerializeField]
		private AnimationCurve _startingSourceTransitionCurve;

		// Token: 0x04002D34 RID: 11572
		[Header("Ending")]
		[SerializeField]
		private AudioSource _endingSource;

		// Token: 0x04002D35 RID: 11573
		[SerializeField]
		private AnimationCurve _endingSourceTransitionCurve;

		// Token: 0x04002D36 RID: 11574
		[Header("Shared")]
		[SerializeField]
		private float _minDuration;

		// Token: 0x04002D37 RID: 11575
		[SerializeField]
		private AnimationCurve _transitionTimeOverDuration;

		// Token: 0x02000AB3 RID: 2739
		[Serializable]
		private struct SpecificTransitionPoint
		{
			// Token: 0x04002D38 RID: 11576
			public float MaxDuration;

			// Token: 0x04002D39 RID: 11577
			public float TransitionTime;
		}
	}
}
