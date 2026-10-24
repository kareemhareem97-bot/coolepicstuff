using System;
using UnityEngine;

namespace FacilitySoundtrack.ChaseThemes
{
	// Token: 0x0200087D RID: 2173
	public struct ChaseClipUtils
	{
		// Token: 0x1700092F RID: 2351
		// (get) Token: 0x0600318E RID: 12686 RVA: 0x000A67FF File Offset: 0x000A49FF
		// (set) Token: 0x0600318F RID: 12687 RVA: 0x000A6807 File Offset: 0x000A4A07
		public int CurSample
		{
			readonly get
			{
				return this._curSample;
			}
			set
			{
				this._curSample = ChaseClipUtils.WrapSamples(this._clip, value);
			}
		}

		// Token: 0x17000930 RID: 2352
		// (get) Token: 0x06003190 RID: 12688 RVA: 0x000A681B File Offset: 0x000A4A1B
		// (set) Token: 0x06003191 RID: 12689 RVA: 0x000A682E File Offset: 0x000A4A2E
		public double CurSeconds
		{
			readonly get
			{
				return ChaseClipUtils.SamplesToSeconds(this._clip, this.CurSample);
			}
			set
			{
				this._curSample = ChaseClipUtils.SecondsToSamples(this._clip, value);
			}
		}

		// Token: 0x17000931 RID: 2353
		// (get) Token: 0x06003192 RID: 12690 RVA: 0x000A6842 File Offset: 0x000A4A42
		// (set) Token: 0x06003193 RID: 12691 RVA: 0x000A6853 File Offset: 0x000A4A53
		public int CurrentBar
		{
			readonly get
			{
				return (int)((double)this.CurSample / this.SamplesPerBar);
			}
			set
			{
				this.CurSample = (int)((double)value * this.SamplesPerBar + (double)this.SampleIntoBar);
			}
		}

		// Token: 0x17000932 RID: 2354
		// (get) Token: 0x06003194 RID: 12692 RVA: 0x000A686D File Offset: 0x000A4A6D
		// (set) Token: 0x06003195 RID: 12693 RVA: 0x000A687E File Offset: 0x000A4A7E
		public int SampleIntoBar
		{
			readonly get
			{
				return (int)((double)this.CurSample % this.SamplesPerBar);
			}
			set
			{
				this.CurSample = (int)((double)this.CurrentBar * this.SamplesPerBar + (double)value);
			}
		}

		// Token: 0x17000933 RID: 2355
		// (get) Token: 0x06003196 RID: 12694 RVA: 0x000A6898 File Offset: 0x000A4A98
		// (set) Token: 0x06003197 RID: 12695 RVA: 0x000A68A9 File Offset: 0x000A4AA9
		public int SamplesUntilNextBar
		{
			readonly get
			{
				return (int)(this.SamplesPerBar - (double)this.SampleIntoBar);
			}
			set
			{
				this.SampleIntoBar = (int)(this.SamplesPerBar - (double)value);
			}
		}

		// Token: 0x17000934 RID: 2356
		// (get) Token: 0x06003198 RID: 12696 RVA: 0x000A68BB File Offset: 0x000A4ABB
		// (set) Token: 0x06003199 RID: 12697 RVA: 0x000A68CE File Offset: 0x000A4ACE
		public double SecondsUntilNextBar
		{
			readonly get
			{
				return ChaseClipUtils.SamplesToSeconds(this._clip, this.SamplesUntilNextBar);
			}
			set
			{
				this.SamplesUntilNextBar = ChaseClipUtils.SecondsToSamples(this._clip, value);
			}
		}

		// Token: 0x0600319A RID: 12698 RVA: 0x000A68E4 File Offset: 0x000A4AE4
		public ChaseClipUtils(AudioClip sourceClip, int beatsPerBar = 4, int? curTimeSamples = null)
		{
			this._curSample = 0;
			this._clip = sourceClip;
			this.BeatsPerBar = beatsPerBar;
			this.SamplesPerBar = 0.42857142857142855 * (double)beatsPerBar * (double)sourceClip.frequency;
			if (curTimeSamples != null)
			{
				this.CurSample = curTimeSamples.Value;
				return;
			}
			double startingDpsTime = ChaseThemeSoundtrack.SynchronizationDpsTime ?? AudioSettings.dspTime;
			this.CurSample = ChaseClipUtils.GetSamplesElapsed(sourceClip, startingDpsTime, null);
		}

		// Token: 0x0600319B RID: 12699 RVA: 0x000A696C File Offset: 0x000A4B6C
		public static int GetSamplesElapsed(AudioClip clip, double startingDpsTime, double? dpsNow = null)
		{
			double value = dpsNow.GetValueOrDefault();
			if (dpsNow == null)
			{
				value = AudioSettings.dspTime;
				dpsNow = new double?(value);
			}
			double elapsed = dpsNow.Value - startingDpsTime;
			return ChaseClipUtils.SecondsToSamples(clip, elapsed);
		}

		// Token: 0x0600319C RID: 12700 RVA: 0x000A69AC File Offset: 0x000A4BAC
		public static int SecondsToSamples(AudioClip clip, double elapsed)
		{
			int frequency = clip.frequency;
			double num = (double)clip.samples / (double)frequency;
			int samples = (int)Math.Floor(elapsed % num * (double)frequency);
			return ChaseClipUtils.WrapSamples(clip, samples);
		}

		// Token: 0x0600319D RID: 12701 RVA: 0x000A69DF File Offset: 0x000A4BDF
		public static double SamplesToSeconds(AudioClip clip, int sample)
		{
			sample = ChaseClipUtils.WrapSamples(clip, sample);
			return (double)sample / (double)clip.frequency;
		}

		// Token: 0x0600319E RID: 12702 RVA: 0x000A69F4 File Offset: 0x000A4BF4
		public static int WrapSamples(AudioClip clip, int samples)
		{
			int samples2 = clip.samples;
			int num = samples % samples2;
			if (num >= 0)
			{
				return num;
			}
			return num + samples2;
		}

		// Token: 0x040026BC RID: 9916
		private readonly AudioClip _clip;

		// Token: 0x040026BD RID: 9917
		private int _curSample;

		// Token: 0x040026BE RID: 9918
		public const double ChaseBPM = 140.0;

		// Token: 0x040026BF RID: 9919
		public const double SecondsPerBeat = 0.42857142857142855;

		// Token: 0x040026C0 RID: 9920
		public readonly int BeatsPerBar;

		// Token: 0x040026C1 RID: 9921
		public readonly double SamplesPerBar;
	}
}
