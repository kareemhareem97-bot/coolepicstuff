using System;
using UnityEngine;

// Token: 0x020000CE RID: 206
public class LoadingCircle : MonoBehaviour
{
	// Token: 0x06000402 RID: 1026 RVA: 0x0000FB89 File Offset: 0x0000DD89
	private void FixedUpdate()
	{
		this.i++;
		if (this.i > this.framesToNextRotation)
		{
			this.i = 0;
			base.transform.Rotate(Vector3.forward * -45f, Space.Self);
		}
	}

	// Token: 0x0400039F RID: 927
	private int i;

	// Token: 0x040003A0 RID: 928
	public int framesToNextRotation = 10;
}
