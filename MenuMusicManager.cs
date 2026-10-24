using System;
using UnityEngine;

// Token: 0x02000068 RID: 104
public class MenuMusicManager : MonoBehaviour
{
	// Token: 0x06000251 RID: 593 RVA: 0x000092E4 File Offset: 0x000074E4
	private void Update()
	{
		this.curState = Mathf.Lerp(this.curState, this.creditsHolder.activeSelf ? 1f : 0f, this.lerpSpeed * Time.deltaTime);
		this.mainSource.mute = ((double)this.curState > 0.5);
		this.creditsSource.volume = this.curState;
		if (this.creditsChanged != this.creditsHolder.activeSelf)
		{
			this.creditsChanged = this.creditsHolder.activeSelf;
			if (this.creditsChanged)
			{
				this.creditsSource.Play();
			}
		}
	}

	// Token: 0x040001ED RID: 493
	private float curState;

	// Token: 0x040001EE RID: 494
	public float lerpSpeed = 1f;

	// Token: 0x040001EF RID: 495
	private bool creditsChanged;

	// Token: 0x040001F0 RID: 496
	[Space(15f)]
	public AudioSource mainSource;

	// Token: 0x040001F1 RID: 497
	public AudioSource creditsSource;

	// Token: 0x040001F2 RID: 498
	[Space(8f)]
	public GameObject creditsHolder;
}
