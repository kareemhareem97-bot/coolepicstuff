using System;
using PlayerRoles.Spectating;
using UnityEngine;

// Token: 0x02000061 RID: 97
public class ExplosionCameraShake : MonoBehaviour
{
	// Token: 0x0600023B RID: 571 RVA: 0x00008F17 File Offset: 0x00007117
	private void Update()
	{
		this.force -= Time.deltaTime / this.deductSpeed;
		this.force = Mathf.Clamp01(this.force);
	}

	// Token: 0x0600023C RID: 572 RVA: 0x00008F43 File Offset: 0x00007143
	private void Awake()
	{
		ExplosionCameraShake.singleton = this;
		SpectatorTargetTracker.OnTargetChanged += this.StopShake;
	}

	// Token: 0x0600023D RID: 573 RVA: 0x00008F5C File Offset: 0x0000715C
	private void OnDestroy()
	{
		SpectatorTargetTracker.OnTargetChanged -= this.StopShake;
	}

	// Token: 0x0600023E RID: 574 RVA: 0x00008F6F File Offset: 0x0000716F
	private void StopShake()
	{
		this.force = 0f;
	}

	// Token: 0x0600023F RID: 575 RVA: 0x00008F7C File Offset: 0x0000717C
	public void Shake(float explosionForce)
	{
		if (explosionForce > this.force)
		{
			this.force = explosionForce;
		}
	}

	// Token: 0x040001D2 RID: 466
	public float force;

	// Token: 0x040001D3 RID: 467
	public float deductSpeed = 2f;

	// Token: 0x040001D4 RID: 468
	public static ExplosionCameraShake singleton;
}
