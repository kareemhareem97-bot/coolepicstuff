using System;
using Mirror;

namespace Hints
{
	// Token: 0x02000838 RID: 2104
	public static class StringHintParameterFunctions
	{
		// Token: 0x06003013 RID: 12307 RVA: 0x0009FF00 File Offset: 0x0009E100
		public static void Serialize(this NetworkWriter writer, StringHintParameter value)
		{
			value.Serialize(writer);
		}

		// Token: 0x06003014 RID: 12308 RVA: 0x000A113F File Offset: 0x0009F33F
		public static StringHintParameter Deserialize(this NetworkReader reader)
		{
			return StringHintParameter.FromNetwork(reader);
		}
	}
}
