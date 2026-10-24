using System;
using UnityEngine;

// Token: 0x020000F8 RID: 248
public class UserMainInterface : MonoBehaviour
{
	// Token: 0x060004AD RID: 1197 RVA: 0x00011BDD File Offset: 0x0000FDDD
	private void Awake()
	{
		UserMainInterface.singleton = this;
	}

	// Token: 0x060004AE RID: 1198 RVA: 0x00002E9A File Offset: 0x0000109A
	private void Start()
	{
	}

	// Token: 0x04000414 RID: 1044
	public static UserMainInterface singleton;

	// Token: 0x04000415 RID: 1045
	public float LerpSpeed = 4f;
}
