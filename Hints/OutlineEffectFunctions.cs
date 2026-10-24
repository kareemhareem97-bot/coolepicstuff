using System;
using Mirror;

namespace Hints
{
	// Token: 0x020007FD RID: 2045
	public static class OutlineEffectFunctions
	{
		// Token: 0x06002F76 RID: 12150 RVA: 0x0009FF00 File Offset: 0x0009E100
		public static void Serialize(this NetworkWriter writer, OutlineEffect value)
		{
			value.Serialize(writer);
		}

		// Token: 0x06002F77 RID: 12151 RVA: 0x000A00EC File Offset: 0x0009E2EC
		public static OutlineEffect Deserialize(this NetworkReader reader)
		{
			return OutlineEffect.FromNetwork(reader);
		}
	}
}
