using System;
using Mirror;

namespace Hints
{
	// Token: 0x02000828 RID: 2088
	public static class PackedLongHintParameterFunctions
	{
		// Token: 0x06002FF0 RID: 12272 RVA: 0x0009FF00 File Offset: 0x0009E100
		public static void Serialize(this NetworkWriter writer, PackedLongHintParameter value)
		{
			value.Serialize(writer);
		}

		// Token: 0x06002FF1 RID: 12273 RVA: 0x000A0CE2 File Offset: 0x0009EEE2
		public static PackedLongHintParameter Deserialize(this NetworkReader reader)
		{
			return PackedLongHintParameter.FromNetwork(reader);
		}
	}
}
