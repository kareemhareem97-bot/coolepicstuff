using System;
using UnityEngine;

// Token: 0x0200001B RID: 27
public class LiquidWobble : MonoBehaviour
{
	// Token: 0x06000076 RID: 118 RVA: 0x000033E8 File Offset: 0x000015E8
	private void Start()
	{
		this._renderer = base.GetComponent<Renderer>();
	}

	// Token: 0x06000077 RID: 119 RVA: 0x000033F8 File Offset: 0x000015F8
	private void Update()
	{
		this._time += Time.deltaTime;
		this._wobbleAmountToAddX = Mathf.Lerp(this._wobbleAmountToAddX, 0f, Time.deltaTime * this._recovery);
		this._wobbleAmountToAddZ = Mathf.Lerp(this._wobbleAmountToAddZ, 0f, Time.deltaTime * this._recovery);
		this._pulse = 6.2831855f * this._wobbleSpeed;
		this._wobbleAmountX = this._wobbleAmountToAddX * Mathf.Sin(this._pulse * this._time);
		this._wobbleAmountZ = this._wobbleAmountToAddZ * Mathf.Sin(this._pulse * this._time);
		this._renderer.material.SetFloat("_WobbleZ", this._wobbleAmountX);
		this._renderer.material.SetFloat("_WobbleX", this._wobbleAmountZ);
		this._velocity = (this._lastPosition - base.transform.position) / Time.deltaTime;
		this._angularVelocity = base.transform.rotation.eulerAngles - this._lastRotation;
		this._wobbleAmountToAddX += Mathf.Clamp((this._velocity.x + this._angularVelocity.z * 0.2f) * this._maxWobble, -this._maxWobble, this._maxWobble);
		this._wobbleAmountToAddZ += Mathf.Clamp((this._velocity.z + this._angularVelocity.x * 0.2f) * this._maxWobble, -this._maxWobble, this._maxWobble);
		this._lastPosition = base.transform.position;
		this._lastRotation = base.transform.rotation.eulerAngles;
	}

	// Token: 0x0400003B RID: 59
	[SerializeField]
	private float _maxWobble = 0.03f;

	// Token: 0x0400003C RID: 60
	[SerializeField]
	private float _wobbleSpeed = 1f;

	// Token: 0x0400003D RID: 61
	[SerializeField]
	private float _recovery = 1f;

	// Token: 0x0400003E RID: 62
	private Renderer _renderer;

	// Token: 0x0400003F RID: 63
	private Vector3 _lastPosition;

	// Token: 0x04000040 RID: 64
	private Vector3 _velocity;

	// Token: 0x04000041 RID: 65
	private Vector3 _lastRotation;

	// Token: 0x04000042 RID: 66
	private Vector3 _angularVelocity;

	// Token: 0x04000043 RID: 67
	private float _wobbleAmountX;

	// Token: 0x04000044 RID: 68
	private float _wobbleAmountZ;

	// Token: 0x04000045 RID: 69
	private float _wobbleAmountToAddX;

	// Token: 0x04000046 RID: 70
	private float _wobbleAmountToAddZ;

	// Token: 0x04000047 RID: 71
	private float _pulse;

	// Token: 0x04000048 RID: 72
	private float _time = 0.5f;
}
