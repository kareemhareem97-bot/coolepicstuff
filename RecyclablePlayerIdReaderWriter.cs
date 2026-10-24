using System;
using Mirror;

// Token: 0x02000095 RID: 149
public static class RecyclablePlayerIdReaderWriter
{
	// Token: 0x06000309 RID: 777 RVA: 0x0000B8C0 File Offset: 0x00009AC0
	public static void WriteRecyclablePlayerId(this NetworkWriter writer, RecyclablePlayerId val)
	{
		for (int i = val.Value; i >= 0; i -= 255)
		{
			writer.WriteByte((byte)Math.Min(i, 255));
		}
	}

	// Token: 0x0600030A RID: 778 RVA: 0x0000B8F4 File Offset: 0x00009AF4
	public static RecyclablePlayerId ReadRecyclablePlayerId(this NetworkReader reader)
	{
		int num = 0;
		byte b;
		do
		{
			b = reader.ReadByte();
			num += (int)b;
		}
		while (b == 255);
		return new RecyclablePlayerId(num);
	}
}
