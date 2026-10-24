using System;
using UnityEngine;

// Token: 0x02000017 RID: 23
public class HeadlessBehaviour : MonoBehaviour
{
	// Token: 0x0600006A RID: 106 RVA: 0x0000317E File Offset: 0x0000137E
	public void NullifyCamera(Camera camera)
	{
		camera.enabled = false;
	}
}
