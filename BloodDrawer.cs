using System;
using Mirror;
using UnityEngine;

// Token: 0x0200001D RID: 29
public class BloodDrawer : NetworkBehaviour
{
	// Token: 0x0600007A RID: 122 RVA: 0x00002E9A File Offset: 0x0000109A
	public void PlaceUnderneath(Vector3 pos, float amountMultiplier = 1f)
	{
	}

	// Token: 0x0600007C RID: 124 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}
}
