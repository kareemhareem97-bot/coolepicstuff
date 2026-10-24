using System;
using System.IO;
using Mirror;

// Token: 0x02000216 RID: 534
public static class LowPrecisionQuaternionSerializer
{
	// Token: 0x06000B2D RID: 2861 RVA: 0x0002B64C File Offset: 0x0002984C
	public unsafe static void WriteLowPrecisionQuaternion(this NetworkWriter writer, LowPrecisionQuaternion value)
	{
		writer.EnsureLength(sizeof(LowPrecisionQuaternion));
		fixed (byte* ptr = &writer.buffer[writer.Position])
		{
			*(LowPrecisionQuaternion*)ptr = value;
		}
		writer.Position += sizeof(LowPrecisionQuaternion);
	}

	// Token: 0x06000B2E RID: 2862 RVA: 0x0002B698 File Offset: 0x00029898
	public unsafe static LowPrecisionQuaternion ReadLowPrecisionQuaternion(this NetworkReader reader)
	{
		if (reader.Position + sizeof(LowPrecisionQuaternion) > reader.buffer.Count)
		{
			throw new EndOfStreamException("ReadByte out of range:" + ((reader != null) ? reader.ToString() : null));
		}
		LowPrecisionQuaternion result;
		fixed (byte* ptr = &reader.buffer.Array[reader.buffer.Offset + reader.Position])
		{
			result = *(LowPrecisionQuaternion*)ptr;
		}
		reader.Position += sizeof(LowPrecisionQuaternion);
		return result;
	}
}
