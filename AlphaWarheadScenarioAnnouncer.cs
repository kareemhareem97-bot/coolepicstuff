using System;
using Subtitles;
using UnityEngine;

// Token: 0x02000042 RID: 66
public abstract class AlphaWarheadScenarioAnnouncer : MonoBehaviour
{
	// Token: 0x1700006B RID: 107
	// (get) Token: 0x060001AF RID: 431 RVA: 0x000079F4 File Offset: 0x00005BF4
	protected static AlphaWarheadController Controller
	{
		get
		{
			return AlphaWarheadController.Singleton;
		}
	}

	// Token: 0x1700006C RID: 108
	// (get) Token: 0x060001B0 RID: 432
	public abstract SubtitleType Subtitle { get; }

	// Token: 0x1700006D RID: 109
	// (get) Token: 0x060001B1 RID: 433
	public abstract float CurrentMusicOutput { get; }

	// Token: 0x060001B2 RID: 434
	public abstract void UpdateActive();

	// Token: 0x060001B3 RID: 435
	public abstract void OnStarted();

	// Token: 0x060001B4 RID: 436
	public abstract void OnCancelled();

	// Token: 0x060001B5 RID: 437
	public abstract void Clear();

	// Token: 0x060001B6 RID: 438 RVA: 0x000079FB File Offset: 0x00005BFB
	protected static void EnsureClip(AudioSource src, AudioClip clip)
	{
		if (src.clip == clip)
		{
			return;
		}
		src.Stop();
		src.clip = clip;
		clip.LoadAudioData();
	}

	// Token: 0x060001B7 RID: 439 RVA: 0x00007A1C File Offset: 0x00005C1C
	protected static void EnsureSync(AudioSource src, float elapsed)
	{
		AudioClip clip = src.clip;
		if (elapsed < 0f || elapsed > clip.length || clip.loadState != AudioDataLoadState.Loaded)
		{
			src.Stop();
			return;
		}
		if (!src.isPlaying)
		{
			src.Play();
			src.time = elapsed;
			return;
		}
		if (Mathf.Abs(src.time - elapsed) > 0.5f)
		{
			src.time = elapsed;
		}
	}

	// Token: 0x0400016D RID: 365
	private const float MaxDesync = 0.5f;
}
