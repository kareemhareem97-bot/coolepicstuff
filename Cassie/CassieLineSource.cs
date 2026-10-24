using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cassie
{
	// Token: 0x02000AB8 RID: 2744
	public class CassieLineSource : MonoBehaviour
	{
		// Token: 0x140000B1 RID: 177
		// (add) Token: 0x06003DE1 RID: 15841 RVA: 0x000CC41C File Offset: 0x000CA61C
		// (remove) Token: 0x06003DE2 RID: 15842 RVA: 0x000CC450 File Offset: 0x000CA650
		public static event Action<CassieLineSource> OnStartedPlaying;

		// Token: 0x17000E13 RID: 3603
		// (get) Token: 0x06003DE3 RID: 15843 RVA: 0x000CC483 File Offset: 0x000CA683
		// (set) Token: 0x06003DE4 RID: 15844 RVA: 0x000CC48B File Offset: 0x000CA68B
		public bool IsBusy { get; private set; }

		// Token: 0x17000E14 RID: 3604
		// (get) Token: 0x06003DE5 RID: 15845 RVA: 0x000CC494 File Offset: 0x000CA694
		// (set) Token: 0x06003DE6 RID: 15846 RVA: 0x000CC49C File Offset: 0x000CA69C
		public CassieLine Line { get; private set; }

		// Token: 0x17000E15 RID: 3605
		// (get) Token: 0x06003DE7 RID: 15847 RVA: 0x000CC4A5 File Offset: 0x000CA6A5
		// (set) Token: 0x06003DE8 RID: 15848 RVA: 0x000CC4AD File Offset: 0x000CA6AD
		public CassiePlaybackModifiers Modifiers { get; private set; }

		// Token: 0x17000E16 RID: 3606
		// (get) Token: 0x06003DE9 RID: 15849 RVA: 0x000CC4B6 File Offset: 0x000CA6B6
		// (set) Token: 0x06003DEA RID: 15850 RVA: 0x000CC4BE File Offset: 0x000CA6BE
		public double ScheduledStartTime { get; private set; }

		// Token: 0x17000E17 RID: 3607
		// (get) Token: 0x06003DEB RID: 15851 RVA: 0x000CC4C7 File Offset: 0x000CA6C7
		// (set) Token: 0x06003DEC RID: 15852 RVA: 0x000CC4CF File Offset: 0x000CA6CF
		public double ScheduledEndTime { get; private set; }

		// Token: 0x06003DED RID: 15853 RVA: 0x000CC4D8 File Offset: 0x000CA6D8
		public void Schedule(CassieLine cassieLine, double startDspTime, CassiePlaybackModifiers modifiers)
		{
			AudioClip clip = cassieLine.Clip;
			if (clip.loadState == AudioDataLoadState.Unloaded)
			{
				CassieLineSource.LoadedClips.Add(clip);
				clip.LoadAudioData();
			}
			startDspTime += (double)modifiers.Sleep.GetValueOrDefault();
			float num = (float)clip.frequency * modifiers.StartTime.GetValueOrDefault();
			num = Mathf.Clamp(num, 0f, (float)(clip.samples - 1));
			double num2 = (double)((modifiers.MaxDuration ?? clip.length) / (modifiers.Pitch ?? 1f));
			double num3 = (modifiers.Stutter != null) ? modifiers.Stutter.GetValueOrDefault().DurationCost : 0.0;
			double scheduledEndTime = startDspTime + num2 + num3;
			this._src.clip = clip;
			this._src.timeSamples = (int)num;
			this._src.volume = (modifiers.Volume ?? 1f);
			this._src.pitch = (modifiers.Pitch ?? 1f);
			this.Line = cassieLine;
			this.IsBusy = true;
			this.Modifiers = modifiers;
			this.ScheduledStartTime = startDspTime;
			this.ScheduledEndTime = scheduledEndTime;
			this._src.PlayScheduled(startDspTime);
			this._src.SetScheduledEndTime(scheduledEndTime);
			this._remainingStutters = ((modifiers.Stutter != null) ? modifiers.Stutter.GetValueOrDefault().Repetitions : 0);
		}

		// Token: 0x06003DEE RID: 15854 RVA: 0x000CC681 File Offset: 0x000CA881
		public void Clear()
		{
			this._src.Stop();
			this._remainingStutters = 0;
			this.IsBusy = false;
		}

		// Token: 0x06003DEF RID: 15855 RVA: 0x000CC69C File Offset: 0x000CA89C
		public void UpdateSpatialization(CassieLineSource template)
		{
			this._src.volume = template._src.volume * (this.Modifiers.Volume ?? 1f);
			this._src.spatialBlend = template._src.spatialBlend;
			this._src.spread = template._src.spread;
		}

		// Token: 0x06003DF0 RID: 15856 RVA: 0x000CC70F File Offset: 0x000CA90F
		private void Awake()
		{
			this._src = base.GetComponent<AudioSource>();
		}

		// Token: 0x06003DF1 RID: 15857 RVA: 0x000CC720 File Offset: 0x000CA920
		private void Update()
		{
			if (!this.IsBusy)
			{
				return;
			}
			this.UpdateStutter();
			double dspTime = AudioSettings.dspTime;
			if (this._lastUpdateDspTime < this.ScheduledStartTime && dspTime >= this.ScheduledStartTime)
			{
				Action<CassieLineSource> onStartedPlaying = CassieLineSource.OnStartedPlaying;
				if (onStartedPlaying != null)
				{
					onStartedPlaying(this);
				}
			}
			if (dspTime > this.ScheduledEndTime)
			{
				this.IsBusy = false;
			}
			this._lastUpdateDspTime = dspTime;
		}

		// Token: 0x06003DF2 RID: 15858 RVA: 0x000CC784 File Offset: 0x000CA984
		private void UpdateStutter()
		{
			if (this._remainingStutters <= 0)
			{
				return;
			}
			CassieStutterSettings value = this.Modifiers.Stutter.Value;
			float num = value.StartNormalizedTime * this.Line.Clip.length;
			float num2 = this._src.time;
			bool flag = false;
			while (num2 >= num + value.RepeatedLength)
			{
				this._remainingStutters--;
				num2 -= value.RepeatedLength;
				flag = true;
				if (this._remainingStutters <= 0)
				{
					break;
				}
			}
			if (flag)
			{
				this._src.time = Mathf.Max(0f, num2);
			}
		}

		// Token: 0x06003DF3 RID: 15859 RVA: 0x000CC820 File Offset: 0x000CAA20
		public static void UpdateUnusedClips()
		{
			for (int i = 0; i < 4; i++)
			{
				int num = CassieLineSource.LoadedClips.Count - i - 1;
				if (num < 0)
				{
					return;
				}
				CassieLineSource.LoadedClips[num].UnloadAudioData();
				CassieLineSource.LoadedClips.RemoveAt(num);
			}
		}

		// Token: 0x04002D47 RID: 11591
		private static readonly List<AudioClip> LoadedClips = new List<AudioClip>();

		// Token: 0x04002D48 RID: 11592
		private double _lastUpdateDspTime;

		// Token: 0x04002D49 RID: 11593
		private int _remainingStutters;

		// Token: 0x04002D4A RID: 11594
		private AudioSource _src;
	}
}
