using System;
using Mirror;

namespace Hints
{
	// Token: 0x0200082F RID: 2095
	public static class SByteHintParameterFunctions
	{
		// Token: 0x06003002 RID: 12290 RVA: 0x0009FF00 File Offset: 0x0009E100
		public static void Serialize(this NetworkWriter writer, SByteHintParameter value)
		{
			value.Serialize(writer);
		}

		// Token: 0x06003003 RID: 12291 RVA: 0x000A0EA7 File Offset: 0x0009F0A7
		public static SByteHintParameter Deserialize(this NetworkReader reader)
		{
			return SByteHintParameter.FromNetwork(reader);
		}
	}
}
