using System;
using Mirror;

namespace Hints
{
	// Token: 0x02000835 RID: 2101
	public static class ServerSettingKeybindHintParameterFunctions
	{
		// Token: 0x0600300E RID: 12302 RVA: 0x0009FF00 File Offset: 0x0009E100
		public static void Serialize(this NetworkWriter writer, SSKeybindHintParameter value)
		{
			value.Serialize(writer);
		}

		// Token: 0x0600300F RID: 12303 RVA: 0x000A10AC File Offset: 0x0009F2AC
		public static SSKeybindHintParameter Deserialize(this NetworkReader reader)
		{
			return SSKeybindHintParameter.FromNetwork(reader);
		}
	}
}
