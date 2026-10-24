using System;
using System.Runtime.CompilerServices;
using Mirror;

namespace Hints
{
	// Token: 0x02000826 RID: 2086
	public class PackedLongHintParameter : PrimitiveHintParameter<long>
	{
		// Token: 0x06002FED RID: 12269 RVA: 0x000A0C57 File Offset: 0x0009EE57
		public static PackedLongHintParameter FromNetwork(NetworkReader reader)
		{
			PackedLongHintParameter packedLongHintParameter = new PackedLongHintParameter();
			packedLongHintParameter.Deserialize(reader);
			return packedLongHintParameter;
		}

		// Token: 0x06002FEE RID: 12270 RVA: 0x000A0C65 File Offset: 0x0009EE65
		protected PackedLongHintParameter()
		{
			Func<NetworkReader, long> deserializer;
			if ((deserializer = PackedLongHintParameter.<>O.<0>__ReadLong) == null)
			{
				deserializer = (PackedLongHintParameter.<>O.<0>__ReadLong = new Func<NetworkReader, long>(NetworkReaderExtensions.ReadLong));
			}
			Action<NetworkWriter, long> serializer;
			if ((serializer = PackedLongHintParameter.<>O.<1>__WriteLong) == null)
			{
				serializer = (PackedLongHintParameter.<>O.<1>__WriteLong = new Action<NetworkWriter, long>(NetworkWriterExtensions.WriteLong));
			}
			base..ctor(deserializer, serializer);
		}

		// Token: 0x06002FEF RID: 12271 RVA: 0x000A0CA3 File Offset: 0x0009EEA3
		public PackedLongHintParameter(long value)
		{
			Func<NetworkReader, long> deserializer;
			if ((deserializer = PackedLongHintParameter.<>O.<0>__ReadLong) == null)
			{
				deserializer = (PackedLongHintParameter.<>O.<0>__ReadLong = new Func<NetworkReader, long>(NetworkReaderExtensions.ReadLong));
			}
			Action<NetworkWriter, long> serializer;
			if ((serializer = PackedLongHintParameter.<>O.<1>__WriteLong) == null)
			{
				serializer = (PackedLongHintParameter.<>O.<1>__WriteLong = new Action<NetworkWriter, long>(NetworkWriterExtensions.WriteLong));
			}
			base..ctor(value, deserializer, serializer);
		}

		// Token: 0x02000827 RID: 2087
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002553 RID: 9555
			public static Func<NetworkReader, long> <0>__ReadLong;

			// Token: 0x04002554 RID: 9556
			public static Action<NetworkWriter, long> <1>__WriteLong;
		}
	}
}
