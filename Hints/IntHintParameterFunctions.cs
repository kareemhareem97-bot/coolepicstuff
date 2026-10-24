using System;
using Mirror;

namespace Hints
{
	// Token: 0x02000821 RID: 2081
	public static class IntHintParameterFunctions
	{
		// Token: 0x06002FE2 RID: 12258 RVA: 0x0009FF00 File Offset: 0x0009E100
		public static void Serialize(this NetworkWriter writer, IntHintParameter value)
		{
			value.Serialize(writer);
		}

		// Token: 0x06002FE3 RID: 12259 RVA: 0x000A0B83 File Offset: 0x0009ED83
		public static IntHintParameter Deserialize(this NetworkReader reader)
		{
			return IntHintParameter.FromNetwork(reader);
		}
	}
}
