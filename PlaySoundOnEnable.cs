using System;
using UnityEngine;

// Token: 0x02000038 RID: 56
public class PlaySoundOnEnable : MonoBehaviour
{
	// Token: 0x06000141 RID: 321 RVA: 0x00006111 File Offset: 0x00004311
	private void Awake()
	{
		this.audio = base.GetComponent<AudioSource>();
	}

	// Token: 0x06000142 RID: 322 RVA: 0x0000611F File Offset: 0x0000431F
	private void OnEnable()
	{
		if (this.audio != null && !this.audio.isPlaying)
		{
			this.audio.Play();
		}
	}

	// Token: 0x0400011C RID: 284
	private AudioSource audio;
}
