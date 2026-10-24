using System;
using Mirror;

// Token: 0x02000172 RID: 370
public static class TeslaHitMsgSerializers
{
	// Token: 0x0600076F RID: 1903 RVA: 0x0001C58A File Offset: 0x0001A78A
	public static void Serialize(this NetworkWriter writer, TeslaHitMsg value)
	{
		writer.WriteNetworkBehaviour(value.Gate);
	}

	// Token: 0x06000770 RID: 1904 RVA: 0x0001C598 File Offset: 0x0001A798
	public static TeslaHitMsg Deserialize(this NetworkReader reader)
	{
		return new TeslaHitMsg(reader.ReadNetworkBehaviour<TeslaGate>());
	}
}
