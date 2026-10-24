using System;
using System.Runtime.CompilerServices;
using Mirror;

namespace Hints
{
	// Token: 0x02000822 RID: 2082
	public class LongHintParameter : PrimitiveHintParameter<long>
	{
		// Token: 0x06002FE4 RID: 12260 RVA: 0x000A0B8B File Offset: 0x0009ED8B
		public static LongHintParameter FromNetwork(NetworkReader reader)
		{
			LongHintParameter longHintParameter = new LongHintParameter();
			longHintParameter.Deserialize(reader);
			return longHintParameter;
		}

		// Token: 0x06002FE5 RID: 12261 RVA: 0x000A0B9C File Offset: 0x0009ED9C
		protected LongHintParameter()
		{
			Func<NetworkReader, long> deserializer;
			if ((deserializer = LongHintParameter.<>O.<0>__ReadLong) == null)
			{
				deserializer = (LongHintParameter.<>O.<0>__ReadLong = new Func<NetworkReader, long>(NetworkReaderExtensions.ReadLong));
			}
			base..ctor(deserializer, delegate(NetworkWriter writer, long writerValue)
			{
				writer.WriteLong(writerValue);
			});
		}

		// Token: 0x06002FE6 RID: 12262 RVA: 0x000A0BEC File Offset: 0x0009EDEC
		public LongHintParameter(long value)
		{
			Func<NetworkReader, long> deserializer;
			if ((deserializer = LongHintParameter.<>O.<0>__ReadLong) == null)
			{
				deserializer = (LongHintParameter.<>O.<0>__ReadLong = new Func<NetworkReader, long>(NetworkReaderExtensions.ReadLong));
			}
			base..ctor(value, deserializer, delegate(NetworkWriter writer, long writerValue)
			{
				writer.WriteLong(writerValue);
			});
		}

		// Token: 0x02000823 RID: 2083
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x0400254F RID: 9551
			public static Func<NetworkReader, long> <0>__ReadLong;
		}
	}
}
