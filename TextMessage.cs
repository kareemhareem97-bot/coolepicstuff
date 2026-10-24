using System;
using UnityEngine;

// Token: 0x02000173 RID: 371
public class TextMessage : MonoBehaviour
{
	// Token: 0x06000771 RID: 1905 RVA: 0x0001C5A5 File Offset: 0x0001A7A5
	private Vector3 GetPosition()
	{
		return new Vector3(this.xOffset, this.spacing * this.position, 0f);
	}

	// Token: 0x06000772 RID: 1906 RVA: 0x00002E9A File Offset: 0x0000109A
	private void Start()
	{
	}

	// Token: 0x06000773 RID: 1907 RVA: 0x0001C5C4 File Offset: 0x0001A7C4
	private void Update()
	{
		this.remainingLife -= Time.deltaTime;
	}

	// Token: 0x040006A2 RID: 1698
	public float spacing = 15.5f;

	// Token: 0x040006A3 RID: 1699
	public float xOffset = 3f;

	// Token: 0x040006A4 RID: 1700
	public float lerpSpeed = 3f;

	// Token: 0x040006A5 RID: 1701
	public float position;

	// Token: 0x040006A6 RID: 1702
	public float remainingLife;
}
