using System;
using UnityEngine;

// Token: 0x020000A4 RID: 164
public class BackgroundScaleToFill : MonoBehaviour
{
	// Token: 0x0600036A RID: 874 RVA: 0x0000CDA4 File Offset: 0x0000AFA4
	private void Update()
	{
		Vector2Int vector2Int = new Vector2Int(Screen.width, Screen.height);
		if (vector2Int == this._prevResolution)
		{
			return;
		}
		this._prevResolution = new Vector2Int?(vector2Int);
		this.Rescale((float)vector2Int.x / (float)vector2Int.y);
	}

	// Token: 0x0600036B RID: 875 RVA: 0x0000CE0C File Offset: 0x0000B00C
	private void Rescale(float screenAspect)
	{
		float num = this._originalResolution.x / this._originalResolution.y;
		if (screenAspect > num)
		{
			float num2 = screenAspect / num;
			base.transform.localScale = new Vector3(this._scale.x, this._scale.y * num2, 1f);
			return;
		}
		float num3 = num / screenAspect;
		base.transform.localScale = new Vector3(this._scale.x * num3, this._scale.y, 1f);
	}

	// Token: 0x0600036C RID: 876 RVA: 0x0000CE98 File Offset: 0x0000B098
	private void Reset()
	{
		this._originalResolution = new Vector2(16f, 9f);
		this._scale = Vector2.one;
	}

	// Token: 0x04000301 RID: 769
	private Vector2Int? _prevResolution;

	// Token: 0x04000302 RID: 770
	[SerializeField]
	private Vector2 _originalResolution;

	// Token: 0x04000303 RID: 771
	[SerializeField]
	private Vector2 _scale;
}
