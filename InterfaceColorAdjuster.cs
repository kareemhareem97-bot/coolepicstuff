using System;
using UnityEngine;

// Token: 0x020000C8 RID: 200
public class InterfaceColorAdjuster : MonoBehaviour
{
	// Token: 0x060003E8 RID: 1000 RVA: 0x0000F7CC File Offset: 0x0000D9CC
	private void Awake()
	{
		PlayerList.ica = this;
	}
}
