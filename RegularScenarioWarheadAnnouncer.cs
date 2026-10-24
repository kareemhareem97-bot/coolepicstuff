using System;
using Mirror;
using Subtitles;
using UnityEngine;

// Token: 0x0200004B RID: 75
public class RegularScenarioWarheadAnnouncer : AlphaWarheadScenarioAnnouncer
{
	// Token: 0x17000076 RID: 118
	// (get) Token: 0x060001E5 RID: 485 RVA: 0x000081F2 File Offset: 0x000063F2
	public override float CurrentMusicOutput
	{
		get
		{
			return this._lastMusicOutput;
		}
	}

	// Token: 0x17000077 RID: 119
	// (get) Token: 0x060001E6 RID: 486 RVA: 0x000081FA File Offset: 0x000063FA
	public override SubtitleType Subtitle
	{
		get
		{
			return this._subtitleType;
		}
	}

	// Token: 0x060001E7 RID: 487 RVA: 0x00008204 File Offset: 0x00006404
	public override void UpdateActive()
	{
		AlphaWarheadController.DetonationScenario curScenario = AlphaWarheadScenarioAnnouncer.Controller.CurScenario;
		foreach (RegularScenarioWarheadAnnouncer.TimeVariant timeVariant in this._timeVariants)
		{
			if (curScenario.TimeToDetonate == timeVariant.Seconds)
			{
				double num = NetworkTime.time - AlphaWarheadScenarioAnnouncer.Controller.Info.StartTime;
				this.UpdateSources(timeVariant, (float)num, curScenario.AdditionalTime);
			}
		}
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x0000826A File Offset: 0x0000646A
	public override void OnCancelled()
	{
		this.Clear();
		this._announcementSource.PlayOneShot(this._cancelledClip);
	}

	// Token: 0x060001E9 RID: 489 RVA: 0x00008284 File Offset: 0x00006484
	public override void Clear()
	{
		this._mainSiren.Stop();
		this._finishingSiren.Stop();
		this._soundtrackSource.Stop();
		this._announcementSource.Stop();
		this._tminusSource.Stop();
		AnnouncementSource.SetVolumeScale(this._soundtrackSource, 1f);
	}

	// Token: 0x060001EA RID: 490 RVA: 0x000082D8 File Offset: 0x000064D8
	private void UpdateSources(RegularScenarioWarheadAnnouncer.TimeVariant variant, float elapsed, float leadTime)
	{
		AlphaWarheadScenarioAnnouncer.EnsureClip(this._announcementSource, this._announcementClip);
		AlphaWarheadScenarioAnnouncer.EnsureSync(this._announcementSource, elapsed);
		elapsed -= leadTime;
		AlphaWarheadScenarioAnnouncer.EnsureClip(this._tminusSource, variant.Clip);
		AlphaWarheadScenarioAnnouncer.EnsureSync(this._tminusSource, elapsed);
		float time = elapsed - variant.SirenTransitionTime;
		AnnouncementSource.SetVolumeScale(this._mainSiren, this._mainSirenVolumeFade.Evaluate(time));
		AnnouncementSource.SetVolumeScale(this._finishingSiren, this._finishingSirenVolumeFade.Evaluate(time));
		AlphaWarheadScenarioAnnouncer.EnsureSync(this._mainSiren, elapsed);
		AlphaWarheadScenarioAnnouncer.EnsureSync(this._finishingSiren, elapsed - variant.SirenTransitionTime);
		this._lastMusicOutput = variant.SoundtrackVolume.Evaluate(elapsed);
		AnnouncementSource.SetVolumeScale(this._soundtrackSource, this._lastMusicOutput);
		AlphaWarheadScenarioAnnouncer.EnsureClip(this._soundtrackSource, this._soundtrackClip);
		AlphaWarheadScenarioAnnouncer.EnsureSync(this._soundtrackSource, elapsed - (float)variant.Seconds + this._soundtrackExplosionTime);
	}

	// Token: 0x060001EB RID: 491 RVA: 0x000083C9 File Offset: 0x000065C9
	public override void OnStarted()
	{
		this._mainSiren.clip.LoadAudioData();
		this._finishingSiren.clip.LoadAudioData();
		this._soundtrackClip.LoadAudioData();
	}

	// Token: 0x0400018F RID: 399
	[SerializeField]
	private AudioSource _mainSiren;

	// Token: 0x04000190 RID: 400
	[SerializeField]
	private AnimationCurve _mainSirenVolumeFade;

	// Token: 0x04000191 RID: 401
	[SerializeField]
	private AudioSource _finishingSiren;

	// Token: 0x04000192 RID: 402
	[SerializeField]
	private AnimationCurve _finishingSirenVolumeFade;

	// Token: 0x04000193 RID: 403
	[SerializeField]
	private AudioSource _soundtrackSource;

	// Token: 0x04000194 RID: 404
	[SerializeField]
	private AudioClip _soundtrackClip;

	// Token: 0x04000195 RID: 405
	[SerializeField]
	private float _soundtrackExplosionTime;

	// Token: 0x04000196 RID: 406
	[SerializeField]
	private AudioClip _announcementClip;

	// Token: 0x04000197 RID: 407
	[SerializeField]
	private AudioClip _cancelledClip;

	// Token: 0x04000198 RID: 408
	[SerializeField]
	private AudioSource _announcementSource;

	// Token: 0x04000199 RID: 409
	[SerializeField]
	private AudioSource _tminusSource;

	// Token: 0x0400019A RID: 410
	[SerializeField]
	private RegularScenarioWarheadAnnouncer.TimeVariant[] _timeVariants;

	// Token: 0x0400019B RID: 411
	[SerializeField]
	private SubtitleType _subtitleType;

	// Token: 0x0400019C RID: 412
	private float _lastMusicOutput;

	// Token: 0x0200004C RID: 76
	[Serializable]
	private class TimeVariant
	{
		// Token: 0x0400019D RID: 413
		public int Seconds;

		// Token: 0x0400019E RID: 414
		public AudioClip Clip;

		// Token: 0x0400019F RID: 415
		public float SirenTransitionTime;

		// Token: 0x040001A0 RID: 416
		public AnimationCurve SoundtrackVolume;
	}
}
