using System;
using Mirror;

namespace Hints
{
	// Token: 0x0200082B RID: 2091
	public static class PackedULongHintParameterFunctions
	{
		// Token: 0x06002FF5 RID: 12277 RVA: 0x0009FF00 File Offset: 0x0009E100
		public static void Serialize(this NetworkWriter writer, PackedULongHintParameter value)
		{
			value.Serialize(writer);
		}

		// Token: 0x06002FF6 RID: 12278 RVA: 0x000A0D75 File Offset: 0x0009EF75
		public static PackedULongHintParameter Deserialize(this NetworkReader reader)
		{
			return PackedULongHintParameter.FromNetwork(reader);
		}
	}
}
