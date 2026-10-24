using System;
using CustomRendering;
using UnityEngine;

// Token: 0x0200007F RID: 127
public class WaysideFogEffect : MonoBehaviour
{
	// Token: 0x060002B6 RID: 694 RVA: 0x0000A444 File Offset: 0x00008644
	private bool GetDesiredFogState()
	{
		Vector3 lastPosition = MainCameraController.LastPosition;
		float y = lastPosition.y;
		if (y > this._position.y)
		{
			return false;
		}
		if (y < this._position.y - this._depth)
		{
			return false;
		}
		Vector2 vector = new Vector2(lastPosition.x - this._position.x, lastPosition.z - this._position.z);
		return vector.sqrMagnitude < this._radiusSqr;
	}

	// Token: 0x060002B7 RID: 695 RVA: 0x0000A4BF File Offset: 0x000086BF
	private void Start()
	{
		this._position = base.transform.position;
		this._radiusSqr = this._radius * this._radius;
	}

	// Token: 0x060002B8 RID: 696 RVA: 0x0000A4E8 File Offset: 0x000086E8
	private void Update()
	{
		bool desiredFogState = this.GetDesiredFogState();
		if (desiredFogState != this._prevActive)
		{
			this._prevActive = desiredFogState;
			if (desiredFogState)
			{
				FogController.EnableFogType(FogType.Nuke, this._fogEnableDuration);
				return;
			}
			FogController.DisableFogType(FogType.Nuke, 0f);
		}
	}

	// Token: 0x04000241 RID: 577
	private bool _prevActive;

	// Token: 0x04000242 RID: 578
	private Vector3 _position;

	// Token: 0x04000243 RID: 579
	private float _radiusSqr;

	// Token: 0x04000244 RID: 580
	[SerializeField]
	private float _radius;

	// Token: 0x04000245 RID: 581
	[SerializeField]
	private float _depth;

	// Token: 0x04000246 RID: 582
	[SerializeField]
	private float _fogEnableDuration;
}
