using System;
using UnityEngine;

// Token: 0x0200007E RID: 126
public class ToggleableLight : MonoBehaviour
{
	// Token: 0x060002B4 RID: 692 RVA: 0x0000A408 File Offset: 0x00008608
	public void SetLights(bool b)
	{
		GameObject[] array = this.allLights;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(this.isAlarm ? b : (!b));
		}
	}

	// Token: 0x0400023F RID: 575
	public GameObject[] allLights;

	// Token: 0x04000240 RID: 576
	public bool isAlarm;
}
