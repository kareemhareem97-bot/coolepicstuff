using System;
using UnityEngine;

// Token: 0x02000181 RID: 385
public class ConnInfoCloser : ConnInfoButton
{
	// Token: 0x060007AF RID: 1967 RVA: 0x0001D520 File Offset: 0x0001B720
	public override void UseButton()
	{
		GameObject[] array = this.objToClose;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetActive(false);
		}
		base.UseButton();
	}

	// Token: 0x040006DA RID: 1754
	public GameObject[] objToClose;
}
