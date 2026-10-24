using System;
using Mirror;

namespace Hints
{
	// Token: 0x02000816 RID: 2070
	public static class ByteHintParameterFunctions
	{
		// Token: 0x06002FC6 RID: 12230 RVA: 0x0009FF00 File Offset: 0x0009E100
		public static void Serialize(this NetworkWriter writer, ByteHintParameter value)
		{
			value.Serialize(writer);
		}

		// Token: 0x06002FC7 RID: 12231 RVA: 0x000A081F File Offset: 0x0009EA1F
		public static ByteHintParameter Deserialize(this NetworkReader reader)
		{
			return ByteHintParameter.FromNetwork(reader);
		}
	}
}
