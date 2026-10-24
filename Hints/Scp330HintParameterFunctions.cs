using System;
using Mirror;

namespace Hints
{
	// Token: 0x0200080C RID: 2060
	public static class Scp330HintParameterFunctions
	{
		// Token: 0x06002FA5 RID: 12197 RVA: 0x0009FF00 File Offset: 0x0009E100
		public static void Serialize(this NetworkWriter writer, Scp330HintParameter value)
		{
			value.Serialize(writer);
		}

		// Token: 0x06002FA6 RID: 12198 RVA: 0x000A0550 File Offset: 0x0009E750
		public static Scp330HintParameter Deserialize(this NetworkReader reader)
		{
			return Scp330HintParameter.FromNetwork(reader);
		}
	}
}
