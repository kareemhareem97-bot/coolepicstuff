using System;
using System.IO;
using Mirror;

// Token: 0x020001A6 RID: 422
public static class OffsetSerializer
{
	// Token: 0x06000851 RID: 2129 RVA: 0x0002126C File Offset: 0x0001F46C
	public unsafe static void WriteOffset(this NetworkWriter writer, Offset value)
	{
		writer.EnsureLength(sizeof(Offset));
		fixed (byte* ptr = &writer.buffer[writer.Position])
		{
			*(Offset*)ptr = value;
		}
		writer.Position += sizeof(Offset);
	}

	// Token: 0x06000852 RID: 2130 RVA: 0x000212B8 File Offset: 0x0001F4B8
	public unsafe static Offset ReadOffset(this NetworkReader reader)
	{
		if (reader.Position + sizeof(Offset) > reader.buffer.Count)
		{
			throw new EndOfStreamException("ReadByte out of range:" + ((reader != null) ? reader.ToString() : null));
		}
		Offset result;
		fixed (byte* ptr = &reader.buffer.Array[reader.buffer.Offset + reader.Position])
		{
			result = *(Offset*)ptr;
		}
		reader.Position += sizeof(Offset);
		return result;
	}
}
