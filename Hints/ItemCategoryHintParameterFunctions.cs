using System;
using Mirror;

namespace Hints
{
	// Token: 0x0200080F RID: 2063
	public static class ItemCategoryHintParameterFunctions
	{
		// Token: 0x06002FB3 RID: 12211 RVA: 0x0009FF00 File Offset: 0x0009E100
		public static void Serialize(this NetworkWriter writer, ItemCategoryHintParameter value)
		{
			value.Serialize(writer);
		}

		// Token: 0x06002FB4 RID: 12212 RVA: 0x000A0615 File Offset: 0x0009E815
		public static ItemCategoryHintParameter Deserialize(this NetworkReader reader)
		{
			return ItemCategoryHintParameter.FromNetwork(reader);
		}
	}
}
