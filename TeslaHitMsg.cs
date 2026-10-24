using System;
using Mirror;

// Token: 0x02000171 RID: 369
public struct TeslaHitMsg : NetworkMessage
{
	// Token: 0x0600076E RID: 1902 RVA: 0x0001C581 File Offset: 0x0001A781
	public TeslaHitMsg(TeslaGate gate)
	{
		this.Gate = gate;
	}

	// Token: 0x040006A1 RID: 1697
	public readonly TeslaGate Gate;
}
