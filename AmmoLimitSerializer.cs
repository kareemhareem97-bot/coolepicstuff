using System;
using Mirror;

// Token: 0x0200009C RID: 156
public static class AmmoLimitSerializer
{
	// Token: 0x06000350 RID: 848 RVA: 0x0000CA1F File Offset: 0x0000AC1F
	public static void WriteAmmoLimit(this NetworkWriter writer, ServerConfigSynchronizer.AmmoLimit value)
	{
		writer.WriteByte((byte)value.AmmoType);
		writer.WriteUShort(value.Limit);
	}

	// Token: 0x06000351 RID: 849 RVA: 0x0000CA3C File Offset: 0x0000AC3C
	public static ServerConfigSynchronizer.AmmoLimit ReadAmmoLimit(this NetworkReader reader)
	{
		return new ServerConfigSynchronizer.AmmoLimit
		{
			AmmoType = (ItemType)reader.ReadByte(),
			Limit = reader.ReadUShort()
		};
	}
}
