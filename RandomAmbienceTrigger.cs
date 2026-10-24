using System;
using UnityEngine;

// Token: 0x02000071 RID: 113
public class RandomAmbienceTrigger : MonoBehaviour
{
	// Token: 0x06000283 RID: 643 RVA: 0x00009BCD File Offset: 0x00007DCD
	private void Start()
	{
		this.Rerandomize();
	}

	// Token: 0x06000284 RID: 644 RVA: 0x00009BD8 File Offset: 0x00007DD8
	private void Update()
	{
		this._timeUntilAmbience -= Time.deltaTime;
		if (this._timeUntilAmbience > 0f)
		{
			return;
		}
		this._ambientSource.clip = this._ambientClips.RandomItem<AudioClip>();
		this._ambientSource.Play();
		this.Rerandomize();
	}

	// Token: 0x06000285 RID: 645 RVA: 0x00009C2C File Offset: 0x00007E2C
	private void Rerandomize()
	{
		this._timeUntilAmbience = UnityEngine.Random.Range(this._minAmbienceTime, this._maxAmbienceTime);
	}

	// Token: 0x04000208 RID: 520
	[SerializeField]
	private AudioClip[] _ambientClips;

	// Token: 0x04000209 RID: 521
	[SerializeField]
	private AudioSource _ambientSource;

	// Token: 0x0400020A RID: 522
	private float _timeUntilAmbience;

	// Token: 0x0400020B RID: 523
	[SerializeField]
	private float _minAmbienceTime;

	// Token: 0x0400020C RID: 524
	[SerializeField]
	private float _maxAmbienceTime;
}
