using System;
using Mirror;

namespace Hints
{
	// Token: 0x0200080A RID: 2058
	public static class AmmoHintParameterFunctions
	{
		// Token: 0x06002F9F RID: 12191 RVA: 0x0009FF00 File Offset: 0x0009E100
		public static void Serialize(this NetworkWriter writer, AmmoHintParameter value)
		{
			value.Serialize(writer);
		}

		// Token: 0x06002FA0 RID: 12192 RVA: 0x000A0520 File Offset: 0x0009E720
		public static AmmoHintParameter Deserialize(this NetworkReader reader)
		{
			return AmmoHintParameter.FromNetwork(reader);
		}
	}
}
