using System;
using Mirror;

namespace Hints
{
	// Token: 0x02000848 RID: 2120
	public readonly struct HintMessage : NetworkMessage
	{
		// Token: 0x06003044 RID: 12356 RVA: 0x000A152F File Offset: 0x0009F72F
		public HintMessage(Hint content)
		{
			this.Content = content;
		}

		// Token: 0x04002575 RID: 9589
		public readonly Hint Content;
	}
}
