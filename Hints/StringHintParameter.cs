using System;
using System.Runtime.CompilerServices;
using Mirror;

namespace Hints
{
	// Token: 0x02000836 RID: 2102
	public class StringHintParameter : PrimitiveHintParameter<string>
	{
		// Token: 0x06003010 RID: 12304 RVA: 0x000A10B4 File Offset: 0x0009F2B4
		public static StringHintParameter FromNetwork(NetworkReader reader)
		{
			StringHintParameter stringHintParameter = new StringHintParameter();
			stringHintParameter.Deserialize(reader);
			return stringHintParameter;
		}

		// Token: 0x06003011 RID: 12305 RVA: 0x000A10C2 File Offset: 0x0009F2C2
		protected StringHintParameter()
		{
			Func<NetworkReader, string> deserializer;
			if ((deserializer = StringHintParameter.<>O.<0>__ReadString) == null)
			{
				deserializer = (StringHintParameter.<>O.<0>__ReadString = new Func<NetworkReader, string>(NetworkReaderExtensions.ReadString));
			}
			Action<NetworkWriter, string> serializer;
			if ((serializer = StringHintParameter.<>O.<1>__WriteString) == null)
			{
				serializer = (StringHintParameter.<>O.<1>__WriteString = new Action<NetworkWriter, string>(NetworkWriterExtensions.WriteString));
			}
			base..ctor(deserializer, serializer);
		}

		// Token: 0x06003012 RID: 12306 RVA: 0x000A1100 File Offset: 0x0009F300
		public StringHintParameter(string value)
		{
			Func<NetworkReader, string> deserializer;
			if ((deserializer = StringHintParameter.<>O.<0>__ReadString) == null)
			{
				deserializer = (StringHintParameter.<>O.<0>__ReadString = new Func<NetworkReader, string>(NetworkReaderExtensions.ReadString));
			}
			Action<NetworkWriter, string> serializer;
			if ((serializer = StringHintParameter.<>O.<1>__WriteString) == null)
			{
				serializer = (StringHintParameter.<>O.<1>__WriteString = new Action<NetworkWriter, string>(NetworkWriterExtensions.WriteString));
			}
			base..ctor(value, deserializer, serializer);
		}

		// Token: 0x02000837 RID: 2103
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002565 RID: 9573
			public static Func<NetworkReader, string> <0>__ReadString;

			// Token: 0x04002566 RID: 9574
			public static Action<NetworkWriter, string> <1>__WriteString;
		}
	}
}
