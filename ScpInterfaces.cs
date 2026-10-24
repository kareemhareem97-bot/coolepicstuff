using System;
using UnityEngine;

// Token: 0x02000160 RID: 352
public class ScpInterfaces : MonoBehaviour
{
	// Token: 0x060006C6 RID: 1734 RVA: 0x000196C4 File Offset: 0x000178C4
	private void Awake()
	{
		ScpInterfaces.singleton = this;
	}

	// Token: 0x04000628 RID: 1576
	public static ScpInterfaces singleton;

	// Token: 0x04000629 RID: 1577
	public GameObject Scp106_eq;

	// Token: 0x0400062A RID: 1578
	public GameObject Scp049_eq;

	// Token: 0x0400062B RID: 1579
	public GameObject Scp096_eq;

	// Token: 0x0400062C RID: 1580
	public GameObject Scp173InterfaceObj;

	// Token: 0x0400062D RID: 1581
	public static int remTargs;
}
