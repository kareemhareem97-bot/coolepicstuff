using System;

namespace Hints
{
	// Token: 0x0200084B RID: 2123
	public abstract class HintParameter : NetworkObject<SharedHintData>
	{
		// Token: 0x170008DF RID: 2271
		// (get) Token: 0x06003047 RID: 12359 RVA: 0x000A15F8 File Offset: 0x0009F7F8
		// (set) Token: 0x06003048 RID: 12360 RVA: 0x000A1600 File Offset: 0x0009F800
		public string Formatted { get; private set; }

		// Token: 0x06003049 RID: 12361
		protected abstract string UpdateState(float progress);

		// Token: 0x0600304A RID: 12362 RVA: 0x000A160C File Offset: 0x0009F80C
		public bool Update(float progress)
		{
			string text = this.UpdateState(progress);
			if (text != null)
			{
				this.Formatted = text;
				return true;
			}
			return false;
		}
	}
}
