using System;
using UnityEngine;

// Token: 0x020000A7 RID: 167
public class BrowserLerp : MonoBehaviour
{
	// Token: 0x06000372 RID: 882 RVA: 0x0000CEDD File Offset: 0x0000B0DD
	private void Start()
	{
		this.rectTransform = base.GetComponent<RectTransform>();
	}

	// Token: 0x06000373 RID: 883 RVA: 0x0000CEEC File Offset: 0x0000B0EC
	private void LateUpdate()
	{
		this.targetPos += this.rectTransform.localPosition - this.prevPos;
		this.rectTransform.localPosition = this.prevPos;
		this.rectTransform.localPosition = Vector3.Lerp(this.rectTransform.localPosition, this.targetPos, Time.deltaTime * this.speed * 4f);
		this.prevPos = this.rectTransform.localPosition;
	}

	// Token: 0x04000306 RID: 774
	private Vector3 prevPos;

	// Token: 0x04000307 RID: 775
	private RectTransform rectTransform;

	// Token: 0x04000308 RID: 776
	private Vector3 targetPos;

	// Token: 0x04000309 RID: 777
	public float speed = 2f;
}
