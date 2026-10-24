using System;
using UnityEngine;

// Token: 0x02000072 RID: 114
public class Rotate : MonoBehaviour
{
	// Token: 0x06000287 RID: 647 RVA: 0x00009C45 File Offset: 0x00007E45
	private void Update()
	{
		base.transform.Rotate(this.speed * Time.deltaTime);
	}

	// Token: 0x0400020D RID: 525
	private Vector3 speed;
}
