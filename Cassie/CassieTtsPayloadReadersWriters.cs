using System;
using Mirror;

namespace Cassie
{
	// Token: 0x02000AC2 RID: 2754
	public static class CassieTtsPayloadReadersWriters
	{
		// Token: 0x06003E21 RID: 15905 RVA: 0x000CD532 File Offset: 0x000CB732
		public static void WriteCassieTtsPayload(this NetworkWriter writer, CassieTtsPayload value)
		{
			value.WriteSelf(writer);
		}

		// Token: 0x06003E22 RID: 15906 RVA: 0x000CD53C File Offset: 0x000CB73C
		public static CassieTtsPayload ReadCassieTtsPayload(this NetworkReader reader)
		{
			return new CassieTtsPayload(reader);
		}
	}
}
