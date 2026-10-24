using System;
using UnityEngine;

// Token: 0x020000A6 RID: 166
public class BottomPickerItem : MonoBehaviour
{
	// Token: 0x0600036F RID: 879 RVA: 0x0000CEBA File Offset: 0x0000B0BA
	public void SetupButton(string k, int i)
	{
		this.key = k;
		this.id = i;
	}

	// Token: 0x06000370 RID: 880 RVA: 0x0000CECA File Offset: 0x0000B0CA
	public void Submit()
	{
		PlayerPrefsSl.Set(this.key, this.id);
	}

	// Token: 0x04000304 RID: 772
	private string key;

	// Token: 0x04000305 RID: 773
	private int id;
}
