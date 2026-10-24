using System;
using System.Runtime.CompilerServices;
using Mirror;

namespace Hints
{
	// Token: 0x02000814 RID: 2068
	public class ByteHintParameter : PrimitiveHintParameter<byte>
	{
		// Token: 0x06002FC3 RID: 12227 RVA: 0x000A0794 File Offset: 0x0009E994
		public static ByteHintParameter FromNetwork(NetworkReader reader)
		{
			ByteHintParameter byteHintParameter = new ByteHintParameter();
			byteHintParameter.Deserialize(reader);
			return byteHintParameter;
		}

		// Token: 0x06002FC4 RID: 12228 RVA: 0x000A07A2 File Offset: 0x0009E9A2
		protected ByteHintParameter()
		{
			Func<NetworkReader, byte> deserializer;
			if ((deserializer = ByteHintParameter.<>O.<0>__ReadByte) == null)
			{
				deserializer = (ByteHintParameter.<>O.<0>__ReadByte = new Func<NetworkReader, byte>(NetworkReaderExtensions.ReadByte));
			}
			Action<NetworkWriter, byte> serializer;
			if ((serializer = ByteHintParameter.<>O.<1>__WriteByte) == null)
			{
				serializer = (ByteHintParameter.<>O.<1>__WriteByte = new Action<NetworkWriter, byte>(NetworkWriterExtensions.WriteByte));
			}
			base..ctor(deserializer, serializer);
		}

		// Token: 0x06002FC5 RID: 12229 RVA: 0x000A07E0 File Offset: 0x0009E9E0
		public ByteHintParameter(byte value)
		{
			Func<NetworkReader, byte> deserializer;
			if ((deserializer = ByteHintParameter.<>O.<0>__ReadByte) == null)
			{
				deserializer = (ByteHintParameter.<>O.<0>__ReadByte = new Func<NetworkReader, byte>(NetworkReaderExtensions.ReadByte));
			}
			Action<NetworkWriter, byte> serializer;
			if ((serializer = ByteHintParameter.<>O.<1>__WriteByte) == null)
			{
				serializer = (ByteHintParameter.<>O.<1>__WriteByte = new Action<NetworkWriter, byte>(NetworkWriterExtensions.WriteByte));
			}
			base..ctor(value, deserializer, serializer);
		}

		// Token: 0x02000815 RID: 2069
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002541 RID: 9537
			public static Func<NetworkReader, byte> <0>__ReadByte;

			// Token: 0x04002542 RID: 9538
			public static Action<NetworkWriter, byte> <1>__WriteByte;
		}
	}
}
