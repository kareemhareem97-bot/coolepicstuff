using System;

namespace CentralAuth
{
	// Token: 0x02000AAC RID: 2732
	public enum ClientInstanceMode : byte
	{
		// Token: 0x04002D18 RID: 11544
		Unverified,
		// Token: 0x04002D19 RID: 11545
		ReadyClient,
		// Token: 0x04002D1A RID: 11546
		Host,
		// Token: 0x04002D1B RID: 11547
		DedicatedServer,
		// Token: 0x04002D1C RID: 11548
		Dummy
	}
}
