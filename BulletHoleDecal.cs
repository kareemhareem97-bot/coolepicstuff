using System;
using AudioPooling;
using Decals;
using UnityEngine;

// Token: 0x02000039 RID: 57
public class BulletHoleDecal : Decal
{
	// Token: 0x06000144 RID: 324 RVA: 0x00006148 File Offset: 0x00004348
	public override void AttachToSurface(RaycastHit hitSurface)
	{
		base.AttachToSurface(hitSurface);
		ParticleSystem fromPool = this._impactParticlesTemplate.GetFromPool(0.1f);
		fromPool.Play(true);
		Transform transform = fromPool.transform;
		transform.SetParent(this._impactParticlesTargetPosition);
		transform.ResetLocalPose();
		AudioSourcePoolManager.PlayOnTransform(((base.CachedTransform.parent.gameObject.layer == 27) ? this._doorImpactSounds : this._wallImpactSounds).RandomItem<AudioClip>(), base.CachedTransform, this._impactSoundRange, this._impactSoundVolume, FalloffType.Exponential, MixerChannel.DefaultSfx, 1f).Source.pitch += UnityEngine.Random.Range(-this._impactSoundRandomization, this._impactSoundRandomization);
	}

	// Token: 0x0400011D RID: 285
	[SerializeField]
	private ParticleSystem _impactParticlesTemplate;

	// Token: 0x0400011E RID: 286
	[SerializeField]
	private Transform _impactParticlesTargetPosition;

	// Token: 0x0400011F RID: 287
	[SerializeField]
	private AudioClip[] _wallImpactSounds;

	// Token: 0x04000120 RID: 288
	[SerializeField]
	private AudioClip[] _doorImpactSounds;

	// Token: 0x04000121 RID: 289
	[SerializeField]
	private float _impactSoundRange = 10f;

	// Token: 0x04000122 RID: 290
	[SerializeField]
	private float _impactSoundVolume = 1f;

	// Token: 0x04000123 RID: 291
	[SerializeField]
	private float _impactSoundRandomization;

	// Token: 0x04000124 RID: 292
	private const int DoorLayer = 27;
}
