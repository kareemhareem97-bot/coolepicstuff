using System;
using Mirror;

namespace Hints
{
	// Token: 0x02000825 RID: 2085
	public static class LongHintParameterFunctions
	{
		// Token: 0x06002FEB RID: 12267 RVA: 0x0009FF00 File Offset: 0x0009E100
		public static void Serialize(this NetworkWriter writer, LongHintParameter value)
		{
			value.Serialize(writer);
		}

		// Token: 0x06002FEC RID: 12268 RVA: 0x000A0C4F File Offset: 0x0009EE4F
		public static LongHintParameter Deserialize(this NetworkReader reader)
		{
			return LongHintParameter.FromNetwork(reader);
		}
	}
}
