using System;
using Mirror;

// Token: 0x0200019B RID: 411
internal static class EncryptedChannelFunctions
{
	// Token: 0x06000834 RID: 2100 RVA: 0x00020E38 File Offset: 0x0001F038
	internal static void SerializeEncryptedMessageOutside(this NetworkWriter writer, EncryptedChannelManager.EncryptedMessageOutside value)
	{
		writer.WriteByte((byte)value.Level);
		writer.WriteArray(value.Data);
	}

	// Token: 0x06000835 RID: 2101 RVA: 0x00020E52 File Offset: 0x0001F052
	internal static EncryptedChannelManager.EncryptedMessageOutside DeserializeEncryptedMessageOutside(this NetworkReader reader)
	{
		return new EncryptedChannelManager.EncryptedMessageOutside((EncryptedChannelManager.SecurityLevel)reader.ReadByte(), reader.ReadArray<byte>());
	}
}
