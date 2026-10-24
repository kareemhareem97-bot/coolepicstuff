using System;

namespace AnimatorLayerManagement
{
	// Token: 0x02000AD3 RID: 2771
	[Serializable]
	public struct RefIdIndexPair
	{
		// Token: 0x06003E5E RID: 15966 RVA: 0x000CE7A5 File Offset: 0x000CC9A5
		public RefIdIndexPair(LayerRefId refId, int layerIndex)
		{
			this.RefId = refId;
			this.LayerIndex = layerIndex;
		}

		// Token: 0x04002DB6 RID: 11702
		public LayerRefId RefId;

		// Token: 0x04002DB7 RID: 11703
		public int LayerIndex;
	}
}
