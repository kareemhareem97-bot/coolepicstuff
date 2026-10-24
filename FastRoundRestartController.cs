using System;
using Mirror;
using UnityEngine;

// Token: 0x0200019C RID: 412
public class FastRoundRestartController : NetworkBehaviour
{
	// Token: 0x06000836 RID: 2102 RVA: 0x00020E65 File Offset: 0x0001F065
	private void Start()
	{
		UnityEngine.Object.Destroy(this);
	}

	// Token: 0x06000838 RID: 2104 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}
}
