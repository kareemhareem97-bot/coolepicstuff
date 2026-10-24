using System;
using Mirror;

namespace Hints
{
	// Token: 0x02000811 RID: 2065
	public static class ItemHintParameterFunctions
	{
		// Token: 0x06002FB9 RID: 12217 RVA: 0x0009FF00 File Offset: 0x0009E100
		public static void Serialize(this NetworkWriter writer, ItemHintParameter value)
		{
			value.Serialize(writer);
		}

		// Token: 0x06002FBA RID: 12218 RVA: 0x000A0690 File Offset: 0x0009E890
		public static ItemHintParameter Deserialize(this NetworkReader reader)
		{
			return ItemHintParameter.FromNetwork(reader);
		}
	}
}
