using System;
using Mirror;

namespace Hints
{
	// Token: 0x0200081C RID: 2076
	public static class FloatHintParameterFunctions
	{
		// Token: 0x06002FD2 RID: 12242 RVA: 0x0009FF00 File Offset: 0x0009E100
		public static void Serialize(this NetworkWriter writer, FloatHintParameter value)
		{
			value.Serialize(writer);
		}

		// Token: 0x06002FD3 RID: 12243 RVA: 0x000A0A22 File Offset: 0x0009EC22
		public static FloatHintParameter Deserialize(this NetworkReader reader)
		{
			return FloatHintParameter.FromNetwork(reader);
		}
	}
}
