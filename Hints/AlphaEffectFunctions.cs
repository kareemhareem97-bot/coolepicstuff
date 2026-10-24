using System;
using Mirror;

namespace Hints
{
	// Token: 0x020007FA RID: 2042
	public static class AlphaEffectFunctions
	{
		// Token: 0x06002F63 RID: 12131 RVA: 0x0009FF00 File Offset: 0x0009E100
		public static void Serialize(this NetworkWriter writer, AlphaEffect value)
		{
			value.Serialize(writer);
		}

		// Token: 0x06002F64 RID: 12132 RVA: 0x0009FF7D File Offset: 0x0009E17D
		public static AlphaEffect Deserialize(this NetworkReader reader)
		{
			return AlphaEffect.FromNetwork(reader);
		}
	}
}
