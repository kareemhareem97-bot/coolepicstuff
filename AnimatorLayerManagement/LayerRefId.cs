using System;

namespace AnimatorLayerManagement
{
	// Token: 0x02000AD2 RID: 2770
	[Serializable]
	public struct LayerRefId
	{
		// Token: 0x06003E5D RID: 15965 RVA: 0x000CE79C File Offset: 0x000CC99C
		public LayerRefId(int refId)
		{
			this.Value = refId;
		}

		// Token: 0x04002DB5 RID: 11701
		public int Value;
	}
}
