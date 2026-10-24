using System;
using System.Runtime.CompilerServices;
using Mirror;

namespace Hints
{
	// Token: 0x0200081E RID: 2078
	public class IntHintParameter : PrimitiveHintParameter<int>
	{
		// Token: 0x06002FDB RID: 12251 RVA: 0x000A0AC2 File Offset: 0x0009ECC2
		public static IntHintParameter FromNetwork(NetworkReader reader)
		{
			IntHintParameter intHintParameter = new IntHintParameter();
			intHintParameter.Deserialize(reader);
			return intHintParameter;
		}

		// Token: 0x06002FDC RID: 12252 RVA: 0x000A0AD0 File Offset: 0x0009ECD0
		protected IntHintParameter()
		{
			Func<NetworkReader, int> deserializer;
			if ((deserializer = IntHintParameter.<>O.<0>__ReadInt) == null)
			{
				deserializer = (IntHintParameter.<>O.<0>__ReadInt = new Func<NetworkReader, int>(NetworkReaderExtensions.ReadInt));
			}
			base..ctor(deserializer, delegate(NetworkWriter writer, int writerValue)
			{
				writer.WriteInt(writerValue);
			});
		}

		// Token: 0x06002FDD RID: 12253 RVA: 0x000A0B20 File Offset: 0x0009ED20
		public IntHintParameter(int value)
		{
			Func<NetworkReader, int> deserializer;
			if ((deserializer = IntHintParameter.<>O.<0>__ReadInt) == null)
			{
				deserializer = (IntHintParameter.<>O.<0>__ReadInt = new Func<NetworkReader, int>(NetworkReaderExtensions.ReadInt));
			}
			base..ctor(value, deserializer, delegate(NetworkWriter writer, int writerValue)
			{
				writer.WriteInt(writerValue);
			});
		}

		// Token: 0x0200081F RID: 2079
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x0400254B RID: 9547
			public static Func<NetworkReader, int> <0>__ReadInt;
		}
	}
}
