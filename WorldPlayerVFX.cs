using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000080 RID: 128
public class WorldPlayerVFX : MonoBehaviour
{
	// Token: 0x060002BA RID: 698 RVA: 0x0000A527 File Offset: 0x00008727
	public void Assign(ReferenceHub hub)
	{
		this._hub = hub;
	}

	// Token: 0x060002BB RID: 699 RVA: 0x0000A530 File Offset: 0x00008730
	private void Start()
	{
		foreach (WorldPlayerVFX.WorldPlayerParticleSystem worldPlayerParticleSystem in this._particleSystems)
		{
			worldPlayerParticleSystem.ParticleSystem.main.startSpeed = worldPlayerParticleSystem.BaseVelocity + this._hub.PlayerCameraReference.transform.forward.z * worldPlayerParticleSystem.AddVelocity;
			worldPlayerParticleSystem.ParticleSystem.Play();
		}
		this._lastPos = this._hub.PlayerCameraReference.transform.position;
	}

	// Token: 0x060002BC RID: 700 RVA: 0x0000A5E4 File Offset: 0x000087E4
	private void Update()
	{
		this.AddVelocityToParticles();
		this._currentTime += Time.deltaTime;
		if (this._currentTime >= this._particleLifetime)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060002BD RID: 701 RVA: 0x0000A618 File Offset: 0x00008818
	private void AddVelocityToParticles()
	{
		Vector3 rhs = this._hub.PlayerCameraReference.transform.position - this._lastPos;
		this._lastPos = this._hub.PlayerCameraReference.transform.position;
		if (rhs.magnitude < 0.01f)
		{
			this._movingForwardDir = 0;
		}
		else
		{
			this._movingForwardDir = ((Vector3.Dot(this._hub.PlayerCameraReference.transform.forward, rhs) > 0f) ? 1 : -1);
		}
		foreach (WorldPlayerVFX.WorldPlayerParticleSystem worldPlayerParticleSystem in this._particleSystems)
		{
			worldPlayerParticleSystem.ParticleSystem.main.startSpeed = worldPlayerParticleSystem.BaseVelocity + (float)this._movingForwardDir * worldPlayerParticleSystem.AddVelocity;
		}
	}

	// Token: 0x04000247 RID: 583
	private const float Threshold = 0.01f;

	// Token: 0x04000248 RID: 584
	[SerializeField]
	private List<WorldPlayerVFX.WorldPlayerParticleSystem> _particleSystems;

	// Token: 0x04000249 RID: 585
	[SerializeField]
	[Tooltip("Once this much time in seconds passes, the particle gameObject will be destroyed.")]
	private float _particleLifetime = 8f;

	// Token: 0x0400024A RID: 586
	private ReferenceHub _hub;

	// Token: 0x0400024B RID: 587
	private int _movingForwardDir;

	// Token: 0x0400024C RID: 588
	private float _currentTime;

	// Token: 0x0400024D RID: 589
	private Vector3 _lastPos;

	// Token: 0x02000081 RID: 129
	[Serializable]
	private struct WorldPlayerParticleSystem
	{
		// Token: 0x0400024E RID: 590
		public ParticleSystem ParticleSystem;

		// Token: 0x0400024F RID: 591
		[Tooltip("This is how much velocity to apply to the particleEffect if the player is not moving on the Z axis.")]
		public float BaseVelocity;

		// Token: 0x04000250 RID: 592
		[Tooltip("This is how much velocity to add or substract from the baseVelocity if the player is moving on the Z axis.")]
		public float AddVelocity;
	}
}
