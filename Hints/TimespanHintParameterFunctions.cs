using System;
using Mirror;

namespace Hints
{
	// Token: 0x0200083A RID: 2106
	public static class TimespanHintParameterFunctions
	{
		// Token: 0x06003021 RID: 12321 RVA: 0x0009FF00 File Offset: 0x0009E100
		public static void Serialize(this NetworkWriter writer, TimespanHintParameter value)
		{
			value.Serialize(writer);
		}

		// Token: 0x06003022 RID: 12322 RVA: 0x000A1249 File Offset: 0x0009F449
		public static TimespanHintParameter Deserialize(this NetworkReader reader)
		{
			return TimespanHintParameter.FromNetwork(reader);
		}
	}
}
