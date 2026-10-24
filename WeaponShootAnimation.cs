using System;
using UnityEngine;

// Token: 0x020000FB RID: 251
public class WeaponShootAnimation : MonoBehaviour
{
	// Token: 0x060004B4 RID: 1204 RVA: 0x00011BF8 File Offset: 0x0000FDF8
	private void LateUpdate()
	{
		if ((double)this.curPosition > 0.03)
		{
			this.curPosition = Mathf.Lerp(this.curPosition, 0f, Time.deltaTime * this.backSpeed * this.curPosition);
		}
		else
		{
			this.curPosition -= Time.deltaTime * 0.1f;
		}
		if (this.curPosition < 0f)
		{
			this.curPosition = 0f;
		}
		this.yOverride = Mathf.Lerp(0f, this.yOverride, this.curPosition);
		this.curY = Mathf.Lerp(this.curY, this.yOverride, Time.deltaTime * this.backY_Speed * this.curPosition);
		base.transform.localPosition = Vector3.Lerp(Vector3.zero, this.maxRecoilPos, this.curPosition);
		base.transform.localRotation = Quaternion.Lerp(Quaternion.Euler(Vector3.zero), Quaternion.Euler(this.maxRecoilRot + Vector3.up * this.curY), this.curPosition);
	}

	// Token: 0x060004B5 RID: 1205 RVA: 0x00011D19 File Offset: 0x0000FF19
	public void Recoil(float f)
	{
		this.curPosition = Mathf.Clamp01(this.curPosition + f);
		this.yOverride = UnityEngine.Random.Range(-10f, 10f) * f;
	}

	// Token: 0x04000416 RID: 1046
	public float curPosition;

	// Token: 0x04000417 RID: 1047
	public Vector3 maxRecoilPos;

	// Token: 0x04000418 RID: 1048
	public Vector3 maxRecoilRot;

	// Token: 0x04000419 RID: 1049
	public float backSpeed;

	// Token: 0x0400041A RID: 1050
	public float backY_Speed;

	// Token: 0x0400041B RID: 1051
	private float yOverride;

	// Token: 0x0400041C RID: 1052
	private float curY;
}
