using System;
using System.Collections.Generic;
using AudioPooling;
using PlayerRoles;
using UnityEngine;
using UserSettings;
using UserSettings.AudioSettings;

namespace FacilitySoundtrack.ChaseThemes
{
	// Token: 0x0200087E RID: 2174
	public class ChaseThemeSoundtrack : SoundtrackLayerBase
	{
		// Token: 0x17000935 RID: 2357
		// (get) Token: 0x0600319F RID: 12703 RVA: 0x000A6A15 File Offset: 0x000A4C15
		// (set) Token: 0x060031A0 RID: 12704 RVA: 0x000A6A1C File Offset: 0x000A4C1C
		public static double? SynchronizationDpsTime { get; private set; }

		// Token: 0x17000936 RID: 2358
		// (get) Token: 0x060031A1 RID: 12705 RVA: 0x000A6A24 File Offset: 0x000A4C24
		public override float Weight
		{
			get
			{
				return this._weight;
			}
		}

		// Token: 0x17000937 RID: 2359
		// (get) Token: 0x060031A2 RID: 12706 RVA: 0x00004094 File Offset: 0x00002294
		public override bool Additive
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000938 RID: 2360
		// (get) Token: 0x060031A3 RID: 12707 RVA: 0x00004094 File Offset: 0x00002294
		public override bool IsMuteable
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000939 RID: 2361
		// (get) Token: 0x060031A4 RID: 12708 RVA: 0x000A6A2C File Offset: 0x000A4C2C
		private float UserVolume
		{
			get
			{
				return this._volumeOverUserSetting.Evaluate(ChaseThemeSoundtrack.SettingCache.Value);
			}
		}

		// Token: 0x060031A5 RID: 12709 RVA: 0x000A6A44 File Offset: 0x000A4C44
		public override void UpdateVolume(float masterVolumeScale)
		{
			ReferenceHub hub;
			bool flag = ReferenceHub.TryGetLocalHub(out hub) && hub.IsHuman();
			if (flag)
			{
				this.FindNewChases();
			}
			float totalDominance;
			int highestPriority;
			this.UpdateActiveThemes(!flag, out totalDominance, out highestPriority);
			this.ResolveThemeConflicts(totalDominance, highestPriority);
			this.ApplyThemeMixing(masterVolumeScale * this.UserVolume);
		}

		// Token: 0x060031A6 RID: 12710 RVA: 0x000A6A94 File Offset: 0x000A4C94
		private void FindNewChases()
		{
			foreach (IChaseThemeProvider chaseThemeProvider in ChaseThemeProviders.Instances)
			{
				if (!ChaseThemeSoundtrack.ActiveChases.Contains(chaseThemeProvider) && chaseThemeProvider.TryStartChase())
				{
					AudioPoolSession? audioPoolSession = chaseThemeProvider.OnChaseStarted(this._alreadyPlayedStingers);
					if (audioPoolSession != null)
					{
						this._alreadyPlayedStingers.Add(audioPoolSession.Value);
					}
					ChaseThemeSoundtrack.ActiveChases.Add(chaseThemeProvider);
				}
			}
		}

		// Token: 0x060031A7 RID: 12711 RVA: 0x000A6B28 File Offset: 0x000A4D28
		private void UpdateActiveThemes(bool forceDisable, out float totalDominance, out int highestPriority)
		{
			totalDominance = 0f;
			highestPriority = int.MinValue;
			for (int i = ChaseThemeSoundtrack.ActiveChases.Count - 1; i >= 0; i--)
			{
				IChaseThemeProvider chaseThemeProvider = ChaseThemeSoundtrack.ActiveChases[i];
				if (forceDisable || !chaseThemeProvider.IsValid() || !chaseThemeProvider.UpdateChase())
				{
					ChaseThemeSoundtrack.ActiveChases[i].OnChaseStopped();
					ChaseThemeSoundtrack.ActiveChases.RemoveAt(i);
				}
				else
				{
					int priority = chaseThemeProvider.Priority;
					if (priority > highestPriority)
					{
						highestPriority = priority;
						totalDominance = chaseThemeProvider.DominanceScale;
					}
					else if (priority == highestPriority)
					{
						totalDominance += chaseThemeProvider.DominanceScale;
					}
				}
			}
		}

		// Token: 0x060031A8 RID: 12712 RVA: 0x000A6BC8 File Offset: 0x000A4DC8
		private void ResolveThemeConflicts(float totalDominance, int highestPriority)
		{
			foreach (ChaseThemeSoundtrack.ClipHandler clipHandler in this._clipHandlers)
			{
				clipHandler.ClearMix();
			}
			if (ChaseThemeSoundtrack.ActiveChases.Count == 0)
			{
				this._alreadyPlayedStingers.Clear();
				return;
			}
			foreach (IChaseThemeProvider chaseThemeProvider in ChaseThemeSoundtrack.ActiveChases)
			{
				if (chaseThemeProvider.Priority == highestPriority)
				{
					float dominanceScale = chaseThemeProvider.DominanceScale;
					if (dominanceScale != 0f)
					{
						foreach (DesiredChaseClip desiredChaseClip in chaseThemeProvider.ClipsToPlay)
						{
							ChaseThemeSoundtrack.ClipHandler orCreateHandler = this.GetOrCreateHandler(desiredChaseClip.Clip);
							float volumeScale = this._volumeScaleOverDominance.Evaluate(dominanceScale / totalDominance);
							orCreateHandler.AddToMix(desiredChaseClip, volumeScale);
						}
					}
				}
			}
		}

		// Token: 0x060031A9 RID: 12713 RVA: 0x000A6CE8 File Offset: 0x000A4EE8
		private void ApplyThemeMixing(float masterVolumeScale)
		{
			float num = 0f;
			bool flag = false;
			foreach (ChaseThemeSoundtrack.ClipHandler clipHandler in this._clipHandlers)
			{
				DesiredChaseClip desiredChaseClip;
				if (!clipHandler.TryGetMixed(out desiredChaseClip))
				{
					clipHandler.FadeAway(masterVolumeScale);
				}
				else
				{
					flag = true;
					if (!clipHandler.IsPlaying)
					{
						clipHandler.StartPlaying(desiredChaseClip.JoinOffset);
					}
					clipHandler.MoveVolume(desiredChaseClip.TrackVolume, desiredChaseClip.VolumeIncreaseSpeed, desiredChaseClip.VolumeDecreaseSpeed, masterVolumeScale);
				}
				num += clipHandler.UnscaledVolume;
			}
			if (!flag && num <= 0f)
			{
				ChaseThemeSoundtrack.SynchronizationDpsTime = null;
			}
			this._weight = Mathf.Clamp01(num * 4f * this.UserVolume);
		}

		// Token: 0x060031AA RID: 12714 RVA: 0x000A6DC0 File Offset: 0x000A4FC0
		private ChaseThemeSoundtrack.ClipHandler GetOrCreateHandler(AudioClip clip)
		{
			foreach (ChaseThemeSoundtrack.ClipHandler clipHandler in this._clipHandlers)
			{
				if (clipHandler.Clip == clip)
				{
					return clipHandler;
				}
			}
			ChaseThemeSoundtrack.ClipHandler clipHandler2 = new ChaseThemeSoundtrack.ClipHandler(this._chaseThemeSourceTemplate, clip);
			this._clipHandlers.Add(clipHandler2);
			return clipHandler2;
		}

		// Token: 0x040026C2 RID: 9922
		private const float FadeAwaySpeed = 0.17f;

		// Token: 0x040026C3 RID: 9923
		private const float WeightInfluence = 4f;

		// Token: 0x040026C4 RID: 9924
		private const float SchedulingDelayMargin = 0.1f;

		// Token: 0x040026C5 RID: 9925
		public static readonly List<IChaseThemeProvider> ActiveChases = new List<IChaseThemeProvider>();

		// Token: 0x040026C6 RID: 9926
		private static readonly CachedUserSetting<float> SettingCache = new CachedUserSetting<float>(MixerAudioSettings.VolumeSliderSetting.ChaseThemes);

		// Token: 0x040026C7 RID: 9927
		private readonly List<ChaseThemeSoundtrack.ClipHandler> _clipHandlers = new List<ChaseThemeSoundtrack.ClipHandler>();

		// Token: 0x040026C8 RID: 9928
		private readonly List<AudioPoolSession> _alreadyPlayedStingers = new List<AudioPoolSession>();

		// Token: 0x040026C9 RID: 9929
		private float _weight;

		// Token: 0x040026CA RID: 9930
		[SerializeField]
		private AudioSource _chaseThemeSourceTemplate;

		// Token: 0x040026CB RID: 9931
		[SerializeField]
		private AnimationCurve _volumeScaleOverDominance;

		// Token: 0x040026CC RID: 9932
		[SerializeField]
		private AnimationCurve _volumeOverUserSetting;

		// Token: 0x0200087F RID: 2175
		public class ClipHandler
		{
			// Token: 0x1700093A RID: 2362
			// (get) Token: 0x060031AD RID: 12717 RVA: 0x000A6E72 File Offset: 0x000A5072
			// (set) Token: 0x060031AE RID: 12718 RVA: 0x000A6E7A File Offset: 0x000A507A
			public float UnscaledVolume
			{
				get
				{
					return this._unscaledVolume;
				}
				set
				{
					this._unscaledVolume = Mathf.Clamp01(value);
				}
			}

			// Token: 0x1700093B RID: 2363
			// (get) Token: 0x060031AF RID: 12719 RVA: 0x000A6E88 File Offset: 0x000A5088
			public bool IsPlaying
			{
				get
				{
					return this._source.isPlaying || this._scheduled;
				}
			}

			// Token: 0x1700093C RID: 2364
			// (get) Token: 0x060031B0 RID: 12720 RVA: 0x000A6E9F File Offset: 0x000A509F
			public int TimeSamples
			{
				get
				{
					return this._source.timeSamples;
				}
			}

			// Token: 0x1700093D RID: 2365
			// (get) Token: 0x060031B1 RID: 12721 RVA: 0x000A6EAC File Offset: 0x000A50AC
			// (set) Token: 0x060031B2 RID: 12722 RVA: 0x000A6EB4 File Offset: 0x000A50B4
			public bool UsesCustomOffset { get; private set; }

			// Token: 0x060031B3 RID: 12723 RVA: 0x000A6EC0 File Offset: 0x000A50C0
			private void RecalcMix()
			{
				this._needsMixing = false;
				float num = 0f;
				float num2 = float.MinValue;
				float num3 = float.MinValue;
				DesiredChaseClip.Offset? offset = null;
				foreach (DesiredChaseClip desiredChaseClip in this._chaseThemes)
				{
					num += desiredChaseClip.TrackVolume;
					num2 = Mathf.Max(num2, desiredChaseClip.VolumeIncreaseSpeed);
					num3 = Mathf.Max(num3, desiredChaseClip.VolumeDecreaseSpeed);
					DesiredChaseClip.Offset? offset2 = offset;
					if (offset2 == null)
					{
						offset = desiredChaseClip.JoinOffset;
					}
				}
				this._mixed = new DesiredChaseClip(this.Clip, Mathf.Clamp01(num), num2, num3, offset);
			}

			// Token: 0x060031B4 RID: 12724 RVA: 0x000A6F84 File Offset: 0x000A5184
			public bool TryGetMixed(out DesiredChaseClip mixed)
			{
				if (this._needsMixing)
				{
					this.RecalcMix();
				}
				mixed = this._mixed;
				return this._chaseThemes.Count > 0;
			}

			// Token: 0x060031B5 RID: 12725 RVA: 0x000A6FAE File Offset: 0x000A51AE
			public void ClearMix()
			{
				this._chaseThemes.Clear();
				this._needsMixing = true;
			}

			// Token: 0x060031B6 RID: 12726 RVA: 0x000A6FC2 File Offset: 0x000A51C2
			public void AddToMix(DesiredChaseClip theme, float volumeScale)
			{
				theme = new DesiredChaseClip(theme.Clip, theme.TrackVolume * volumeScale, theme.VolumeIncreaseSpeed, theme.VolumeDecreaseSpeed, theme.JoinOffset);
				this._chaseThemes.Add(theme);
				this._needsMixing = true;
			}

			// Token: 0x060031B7 RID: 12727 RVA: 0x000A7000 File Offset: 0x000A5200
			public void FadeAway(float masterVolumeScale)
			{
				if (!this._source.isPlaying)
				{
					return;
				}
				this.UnscaledVolume -= Time.deltaTime * 0.17f;
				if (this.UnscaledVolume <= 0f)
				{
					this._scheduled = false;
					this._source.volume = 0f;
					this._source.Stop();
					return;
				}
				this._source.volume = this.UnscaledVolume * masterVolumeScale;
			}

			// Token: 0x060031B8 RID: 12728 RVA: 0x000A7078 File Offset: 0x000A5278
			public void StartPlaying(DesiredChaseClip.Offset? offset)
			{
				double dspTime = AudioSettings.dspTime;
				double? synchronizationDpsTime = ChaseThemeSoundtrack.SynchronizationDpsTime;
				double value = synchronizationDpsTime.GetValueOrDefault();
				if (synchronizationDpsTime == null)
				{
					value = dspTime;
					ChaseThemeSoundtrack.SynchronizationDpsTime = new double?(value);
				}
				int samplesElapsed = ChaseClipUtils.GetSamplesElapsed(this.Clip, ChaseThemeSoundtrack.SynchronizationDpsTime.Value, null);
				if (offset == null)
				{
					this.StartPlayingCustom(samplesElapsed, dspTime);
					return;
				}
				DesiredChaseClip.Offset value2 = offset.Value;
				if (value2.DontSyncWithOthers)
				{
					this.StartPlayingCustom(value2.StartSamples, dspTime + value2.SchedulingDelay);
					return;
				}
				this.StartPlayingCustom(samplesElapsed + value2.StartSamples, dspTime + value2.SchedulingDelay);
			}

			// Token: 0x060031B9 RID: 12729 RVA: 0x000A7121 File Offset: 0x000A5321
			public void StartPlayingCustom(int timeSamples, double dpsTime)
			{
				this._source.timeSamples = ChaseClipUtils.WrapSamples(this.Clip, timeSamples);
				this._source.PlayScheduled(dpsTime + 0.10000000149011612);
				this._scheduled = true;
			}

			// Token: 0x060031BA RID: 12730 RVA: 0x000A7158 File Offset: 0x000A5358
			public void MoveVolume(float targetVolume, float increaseRate, float decreaseRate, float masterVolumeScale)
			{
				float num = (targetVolume < this.UnscaledVolume) ? decreaseRate : increaseRate;
				this.UnscaledVolume = Mathf.MoveTowards(this.UnscaledVolume, targetVolume, num * Time.deltaTime);
				this._source.volume = this.UnscaledVolume * masterVolumeScale;
			}

			// Token: 0x060031BB RID: 12731 RVA: 0x000A71A0 File Offset: 0x000A53A0
			public ClipHandler(AudioSource template, AudioClip clip)
			{
				this.Clip = clip;
				this._source = UnityEngine.Object.Instantiate<AudioSource>(template, template.transform.parent);
				this._source.clip = this.Clip;
				this._chaseThemes = new List<DesiredChaseClip>();
			}

			// Token: 0x040026CE RID: 9934
			private readonly AudioSource _source;

			// Token: 0x040026CF RID: 9935
			private readonly List<DesiredChaseClip> _chaseThemes;

			// Token: 0x040026D0 RID: 9936
			private bool _needsMixing;

			// Token: 0x040026D1 RID: 9937
			private DesiredChaseClip _mixed;

			// Token: 0x040026D2 RID: 9938
			private float _unscaledVolume;

			// Token: 0x040026D3 RID: 9939
			private bool _scheduled;

			// Token: 0x040026D4 RID: 9940
			public readonly AudioClip Clip;
		}
	}
}
