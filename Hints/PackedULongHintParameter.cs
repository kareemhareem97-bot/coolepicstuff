using System;
using System.Runtime.CompilerServices;
using Mirror;

namespace Hints
{
	// Token: 0x02000829 RID: 2089
	public class PackedULongHintParameter : PrimitiveHintParameter<ulong>
	{
		// Token: 0x06002FF2 RID: 12274 RVA: 0x000A0CEA File Offset: 0x0009EEEA
		public static PackedULongHintParameter FromNetwork(NetworkReader reader)
		{
			PackedULongHintParameter packedULongHintParameter = new PackedULongHintParameter();
			packedULongHintParameter.Deserialize(reader);
			return packedULongHintParameter;
		}

		// Token: 0x06002FF3 RID: 12275 RVA: 0x000A0CF8 File Offset: 0x0009EEF8
		protected PackedULongHintParameter()
		{
			Func<NetworkReader, ulong> deserializer;
			if ((deserializer = PackedULongHintParameter.<>O.<0>__ReadULong) == null)
			{
				deserializer = (PackedULongHintParameter.<>O.<0>__ReadULong = new Func<NetworkReader, ulong>(NetworkReaderExtensions.ReadULong));
			}
			Action<NetworkWriter, ulong> serializer;
			if ((serializer = PackedULongHintParameter.<>O.<1>__WriteULong) == null)
			{
				serializer = (PackedULongHintParameter.<>O.<1>__WriteULong = new Action<NetworkWriter, ulong>(NetworkWriterExtensions.WriteULong));
			}
			base..ctor(deserializer, serializer);
		}

		// Token: 0x06002FF4 RID: 12276 RVA: 0x000A0D36 File Offset: 0x0009EF36
		public PackedULongHintParameter(ulong value)
		{
			Func<NetworkReader, ulong> deserializer;
			if ((deserializer = PackedULongHintParameter.<>O.<0>__ReadULong) == null)
			{
				deserializer = (PackedULongHintParameter.<>O.<0>__ReadULong = new Func<NetworkReader, ulong>(NetworkReaderExtensions.ReadULong));
			}
			Action<NetworkWriter, ulong> serializer;
			if ((serializer = PackedULongHintParameter.<>O.<1>__WriteULong) == null)
			{
				serializer = (PackedULongHintParameter.<>O.<1>__WriteULong = new Action<NetworkWriter, ulong>(NetworkWriterExtensions.WriteULong));
			}
			base..ctor(value, deserializer, serializer);
		}

		// Token: 0x0200082A RID: 2090
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002555 RID: 9557
			public static Func<NetworkReader, ulong> <0>__ReadULong;

			// Token: 0x04002556 RID: 9558
			public static Action<NetworkWriter, ulong> <1>__WriteULong;
		}
	}
}
