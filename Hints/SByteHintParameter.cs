using System;
using System.Runtime.CompilerServices;
using Mirror;

namespace Hints
{
	// Token: 0x0200082D RID: 2093
	public class SByteHintParameter : PrimitiveHintParameter<sbyte>
	{
		// Token: 0x06002FFF RID: 12287 RVA: 0x000A0E1C File Offset: 0x0009F01C
		public static SByteHintParameter FromNetwork(NetworkReader reader)
		{
			SByteHintParameter sbyteHintParameter = new SByteHintParameter();
			sbyteHintParameter.Deserialize(reader);
			return sbyteHintParameter;
		}

		// Token: 0x06003000 RID: 12288 RVA: 0x000A0E2A File Offset: 0x0009F02A
		protected SByteHintParameter()
		{
			Func<NetworkReader, sbyte> deserializer;
			if ((deserializer = SByteHintParameter.<>O.<0>__ReadSByte) == null)
			{
				deserializer = (SByteHintParameter.<>O.<0>__ReadSByte = new Func<NetworkReader, sbyte>(NetworkReaderExtensions.ReadSByte));
			}
			Action<NetworkWriter, sbyte> serializer;
			if ((serializer = SByteHintParameter.<>O.<1>__WriteSByte) == null)
			{
				serializer = (SByteHintParameter.<>O.<1>__WriteSByte = new Action<NetworkWriter, sbyte>(NetworkWriterExtensions.WriteSByte));
			}
			base..ctor(deserializer, serializer);
		}

		// Token: 0x06003001 RID: 12289 RVA: 0x000A0E68 File Offset: 0x0009F068
		public SByteHintParameter(sbyte value)
		{
			Func<NetworkReader, sbyte> deserializer;
			if ((deserializer = SByteHintParameter.<>O.<0>__ReadSByte) == null)
			{
				deserializer = (SByteHintParameter.<>O.<0>__ReadSByte = new Func<NetworkReader, sbyte>(NetworkReaderExtensions.ReadSByte));
			}
			Action<NetworkWriter, sbyte> serializer;
			if ((serializer = SByteHintParameter.<>O.<1>__WriteSByte) == null)
			{
				serializer = (SByteHintParameter.<>O.<1>__WriteSByte = new Action<NetworkWriter, sbyte>(NetworkWriterExtensions.WriteSByte));
			}
			base..ctor(value, deserializer, serializer);
		}

		// Token: 0x0200082E RID: 2094
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x0400255B RID: 9563
			public static Func<NetworkReader, sbyte> <0>__ReadSByte;

			// Token: 0x0400255C RID: 9564
			public static Action<NetworkWriter, sbyte> <1>__WriteSByte;
		}
	}
}
