using System;
using Mirror;

namespace Hints
{
	// Token: 0x02000819 RID: 2073
	public static class DoubleHintParameterFunctions
	{
		// Token: 0x06002FCC RID: 12236 RVA: 0x0009FF00 File Offset: 0x0009E100
		public static void Serialize(this NetworkWriter writer, DoubleHintParameter value)
		{
			value.Serialize(writer);
		}

		// Token: 0x06002FCD RID: 12237 RVA: 0x000A0920 File Offset: 0x0009EB20
		public static DoubleHintParameter Deserialize(this NetworkReader reader)
		{
			return DoubleHintParameter.FromNetwork(reader);
		}
	}
}
