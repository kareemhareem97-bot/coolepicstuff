using System;
using Mirror;

namespace Hints
{
	// Token: 0x02000832 RID: 2098
	public static class ShortHintParameterFunctions
	{
		// Token: 0x06003007 RID: 12295 RVA: 0x0009FF00 File Offset: 0x0009E100
		public static void Serialize(this NetworkWriter writer, ShortHintParameter value)
		{
			value.Serialize(writer);
		}

		// Token: 0x06003008 RID: 12296 RVA: 0x000A0F3A File Offset: 0x0009F13A
		public static ShortHintParameter Deserialize(this NetworkReader reader)
		{
			return ShortHintParameter.FromNetwork(reader);
		}
	}
}
