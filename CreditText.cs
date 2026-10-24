using System;
using UnityEngine;

// Token: 0x020000B4 RID: 180
public class CreditText : MonoBehaviour
{
	// Token: 0x060003A2 RID: 930 RVA: 0x0000E937 File Offset: 0x0000CB37
	private void FixedUpdate()
	{
		if (this.move)
		{
			base.transform.Translate(Vector3.up * this.speed);
		}
	}

	// Token: 0x04000344 RID: 836
	public bool move;

	// Token: 0x04000345 RID: 837
	public float speed;
}
