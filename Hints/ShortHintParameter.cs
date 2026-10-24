using System;
using System.Runtime.CompilerServices;
using Mirror;

namespace Hints
{
	// Token: 0x02000830 RID: 2096
	public class ShortHintParameter : PrimitiveHintParameter<short>
	{
		// Token: 0x06003004 RID: 12292 RVA: 0x000A0EAF File Offset: 0x0009F0AF
		public static ShortHintParameter FromNetwork(NetworkReader reader)
		{
			ShortHintParameter shortHintParameter = new ShortHintParameter();
			shortHintParameter.Deserialize(reader);
			return shortHintParameter;
		}

		// Token: 0x06003005 RID: 12293 RVA: 0x000A0EBD File Offset: 0x0009F0BD
		protected ShortHintParameter()
		{
			Func<NetworkReader, short> deserializer;
			if ((deserializer = ShortHintParameter.<>O.<0>__ReadShort) == null)
			{
				deserializer = (ShortHintParameter.<>O.<0>__ReadShort = new Func<NetworkReader, short>(NetworkReaderExtensions.ReadShort));
			}
			Action<NetworkWriter, short> serializer;
			if ((serializer = ShortHintParameter.<>O.<1>__WriteShort) == null)
			{
				serializer = (ShortHintParameter.<>O.<1>__WriteShort = new Action<NetworkWriter, short>(NetworkWriterExtensions.WriteShort));
			}
			base..ctor(deserializer, serializer);
		}

		// Token: 0x06003006 RID: 12294 RVA: 0x000A0EFB File Offset: 0x0009F0FB
		public ShortHintParameter(short value)
		{
			Func<NetworkReader, short> deserializer;
			if ((deserializer = ShortHintParameter.<>O.<0>__ReadShort) == null)
			{
				deserializer = (ShortHintParameter.<>O.<0>__ReadShort = new Func<NetworkReader, short>(NetworkReaderExtensions.ReadShort));
			}
			Action<NetworkWriter, short> serializer;
			if ((serializer = ShortHintParameter.<>O.<1>__WriteShort) == null)
			{
				serializer = (ShortHintParameter.<>O.<1>__WriteShort = new Action<NetworkWriter, short>(NetworkWriterExtensions.WriteShort));
			}
			base..ctor(value, deserializer, serializer);
		}

		// Token: 0x02000831 RID: 2097
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x0400255D RID: 9565
			public static Func<NetworkReader, short> <0>__ReadShort;

			// Token: 0x0400255E RID: 9566
			public static Action<NetworkWriter, short> <1>__WriteShort;
		}
	}
}
