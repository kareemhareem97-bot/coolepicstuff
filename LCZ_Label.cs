using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000CB RID: 203
public class LCZ_Label : MonoBehaviour
{
	// Token: 0x060003F9 RID: 1017 RVA: 0x0000FA1D File Offset: 0x0000DC1D
	private void Awake()
	{
		LCZ_Label.AllLabels.Add(this);
	}

	// Token: 0x060003FA RID: 1018 RVA: 0x0000FA2B File Offset: 0x0000DC2B
	private void OnDestroy()
	{
		LCZ_Label.AllLabels.Remove(this);
	}

	// Token: 0x060003FB RID: 1019 RVA: 0x0000FA39 File Offset: 0x0000DC39
	public void Refresh(Material ch, Material num)
	{
		this.chRend.sharedMaterial = ch;
		this.numRend.sharedMaterial = num;
	}

	// Token: 0x04000382 RID: 898
	public static readonly HashSet<LCZ_Label> AllLabels = new HashSet<LCZ_Label>();

	// Token: 0x04000383 RID: 899
	public MeshRenderer chRend;

	// Token: 0x04000384 RID: 900
	public MeshRenderer numRend;
}
