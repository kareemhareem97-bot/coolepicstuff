using System;
using UnityEngine;

// Token: 0x0200005A RID: 90
public static class AudioUtils
{
	// Token: 0x06000217 RID: 535 RVA: 0x00008B3A File Offset: 0x00006D3A
	public static void SetSpace(this AudioSource src, bool is3D)
	{
		if (is3D)
		{
			src.Set3D();
			return;
		}
		src.Set2D();
	}

	// Token: 0x06000218 RID: 536 RVA: 0x00008B4C File Offset: 0x00006D4C
	public static void Set3D(this AudioSource src)
	{
		src.spatialBlend = 1f;
		src.spread = 0f;
	}

	// Token: 0x06000219 RID: 537 RVA: 0x00008B64 File Offset: 0x00006D64
	public static void Replay(this AudioSource src)
	{
		src.Stop();
		src.Play();
	}

	// Token: 0x0600021A RID: 538 RVA: 0x00008B72 File Offset: 0x00006D72
	public static void UpdateFade(this AudioSource src, bool isEnabled, float speed, float maxVolume = 1f)
	{
		src.volume = Mathf.MoveTowards(src.volume, isEnabled ? maxVolume : 0f, speed * Time.deltaTime);
	}

	// Token: 0x0600021B RID: 539 RVA: 0x00008B97 File Offset: 0x00006D97
	public static void UpdateFadeIn(this AudioSource src, float speed, float targetVolume = 1f)
	{
		src.UpdateFade(true, speed, targetVolume);
	}

	// Token: 0x0600021C RID: 540 RVA: 0x00008BA2 File Offset: 0x00006DA2
	public static void UpdateFadeOut(this AudioSource src, float speed)
	{
		src.UpdateFade(false, speed, 1f);
	}

	// Token: 0x0600021D RID: 541 RVA: 0x00008BB1 File Offset: 0x00006DB1
	public static void Set2D(this AudioSource src)
	{
		src.spatialBlend = 0f;
		src.spread = 360f;
	}
}
