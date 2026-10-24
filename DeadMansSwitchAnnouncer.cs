using System;
using Mirror;
using Subtitles;
using UnityEngine;

// Token: 0x02000046 RID: 70
public class DeadMansSwitchAnnouncer : AlphaWarheadScenarioAnnouncer
{
	// Token: 0x1700006F RID: 111
	// (get) Token: 0x060001C7 RID: 455 RVA: 0x00007CA9 File Offset: 0x00005EA9
	public override float CurrentMusicOutput
	{
		get
		{
			return Mathf.Clamp01(this._lastElapsed - this._musicIntenseTime - this._musicOffset);
		}
	}

	// Token: 0x17000070 RID: 112
	// (get) Token: 0x060001C8 RID: 456 RVA: 0x00007CC4 File Offset: 0x00005EC4
	public override SubtitleType Subtitle
	{
		get
		{
			return SubtitleType.DeadMansSwitch;
		}
	}

	// Token: 0x060001C9 RID: 457 RVA: 0x00007CC8 File Offset: 0x00005EC8
	public override void UpdateActive()
	{
		this._lastElapsed = (float)(NetworkTime.time - AlphaWarheadScenarioAnnouncer.Controller.Info.StartTime);
		AlphaWarheadScenarioAnnouncer.EnsureClip(this._announcementSource, this._announcementClip);
		AlphaWarheadScenarioAnnouncer.EnsureSync(this._announcementSource, this._lastElapsed);
		AlphaWarheadScenarioAnnouncer.EnsureClip(this._musicSource, this._musicClip);
		AlphaWarheadScenarioAnnouncer.EnsureSync(this._musicSource, this._lastElapsed - this._musicOffset);
	}

	// Token: 0x060001CA RID: 458 RVA: 0x00007D3C File Offset: 0x00005F3C
	public override void OnCancelled()
	{
		this.Clear();
	}

	// Token: 0x060001CB RID: 459 RVA: 0x00007D44 File Offset: 0x00005F44
	public override void Clear()
	{
		this._announcementSource.Stop();
		this._musicSource.Stop();
	}

	// Token: 0x060001CC RID: 460 RVA: 0x00007D5C File Offset: 0x00005F5C
	public override void OnStarted()
	{
		this._musicClip.LoadAudioData();
	}

	// Token: 0x04000176 RID: 374
	private float _lastElapsed;

	// Token: 0x04000177 RID: 375
	[SerializeField]
	private AudioSource _announcementSource;

	// Token: 0x04000178 RID: 376
	[SerializeField]
	private AudioClip _announcementClip;

	// Token: 0x04000179 RID: 377
	[SerializeField]
	private AudioSource _musicSource;

	// Token: 0x0400017A RID: 378
	[SerializeField]
	private AudioClip _musicClip;

	// Token: 0x0400017B RID: 379
	[SerializeField]
	private float _musicOffset;

	// Token: 0x0400017C RID: 380
	[SerializeField]
	private float _musicIntenseTime;
}
