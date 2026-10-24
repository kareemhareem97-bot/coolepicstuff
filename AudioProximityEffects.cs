using System;
using CustomPlayerEffects;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

// Token: 0x02000059 RID: 89
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioReverbFilter))]
[RequireComponent(typeof(AudioLowPassFilter))]
public class AudioProximityEffects : MonoBehaviour
{
	// Token: 0x1700007B RID: 123
	// (get) Token: 0x06000213 RID: 531 RVA: 0x00008A20 File Offset: 0x00006C20
	private float ProximityLevel
	{
		get
		{
			ReferenceHub referenceHub;
			if (!ReferenceHub.TryGetLocalHub(out referenceHub))
			{
				return 0f;
			}
			IFpcRole fpcRole = referenceHub.roleManager.CurrentRole as IFpcRole;
			if (fpcRole == null)
			{
				return 0f;
			}
			return Vector3.Distance(base.transform.position, fpcRole.FpcModule.Position) / this._audioSource.maxDistance * this._audioSource.spatialBlend;
		}
	}

	// Token: 0x06000214 RID: 532 RVA: 0x00008A89 File Offset: 0x00006C89
	private void Awake()
	{
		this._audioSource = base.GetComponent<AudioSource>();
		this._reverbFilter = base.GetComponent<AudioReverbFilter>();
		this._lowPassFilter = base.GetComponent<AudioLowPassFilter>();
		this._audioEchoFilter = base.GetComponent<AudioEchoFilter>();
	}

	// Token: 0x06000215 RID: 533 RVA: 0x00008ABC File Offset: 0x00006CBC
	private void Update()
	{
		float proximityLevel = this.ProximityLevel;
		this._reverbFilter.dryLevel = this._reverbDryOverDistance.Evaluate(proximityLevel);
		this._reverbFilter.room = this._reverbSizeOverDistance.Evaluate(proximityLevel);
		this._lowPassFilter.cutoffFrequency = this._lowpassOverDistance.Evaluate(proximityLevel);
		ReferenceHub referenceHub;
		if (!ReferenceHub.TryGetLocalHub(out referenceHub))
		{
			return;
		}
		this._audioEchoFilter.enabled = referenceHub.playerEffectsController.GetEffect<Ghostly>().IsEnabled;
	}

	// Token: 0x040001BB RID: 443
	[SerializeField]
	private AnimationCurve _reverbSizeOverDistance;

	// Token: 0x040001BC RID: 444
	[SerializeField]
	private AnimationCurve _reverbDryOverDistance;

	// Token: 0x040001BD RID: 445
	[SerializeField]
	private AnimationCurve _lowpassOverDistance;

	// Token: 0x040001BE RID: 446
	private AudioSource _audioSource;

	// Token: 0x040001BF RID: 447
	private AudioReverbFilter _reverbFilter;

	// Token: 0x040001C0 RID: 448
	private AudioLowPassFilter _lowPassFilter;

	// Token: 0x040001C1 RID: 449
	private AudioEchoFilter _audioEchoFilter;
}
