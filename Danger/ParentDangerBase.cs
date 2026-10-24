using System;
using System.Collections.Generic;

namespace CustomPlayerEffects.Danger
{
	// Token: 0x02000912 RID: 2322
	public abstract class ParentDangerBase : DangerStackBase
	{
		// Token: 0x17000A6D RID: 2669
		// (get) Token: 0x060034F8 RID: 13560 RVA: 0x000AD914 File Offset: 0x000ABB14
		// (set) Token: 0x060034F9 RID: 13561 RVA: 0x000AD91C File Offset: 0x000ABB1C
		public override float DangerValue { get; set; }

		// Token: 0x17000A6E RID: 2670
		// (get) Token: 0x060034FA RID: 13562 RVA: 0x000AD925 File Offset: 0x000ABB25
		// (set) Token: 0x060034FB RID: 13563 RVA: 0x000AD92D File Offset: 0x000ABB2D
		private protected List<DangerStackBase> ChildDangers { protected get; private set; } = new List<DangerStackBase>();

		// Token: 0x17000A6F RID: 2671
		// (get) Token: 0x060034FC RID: 13564 RVA: 0x000AD936 File Offset: 0x000ABB36
		// (set) Token: 0x060034FD RID: 13565 RVA: 0x000AD94C File Offset: 0x000ABB4C
		public override bool IsActive
		{
			get
			{
				this.ProcessChildren();
				return this.ChildDangers.Count > 0;
			}
			protected set
			{
				base.IsActive = value;
			}
		}

		// Token: 0x060034FE RID: 13566 RVA: 0x000AD958 File Offset: 0x000ABB58
		private void ProcessChildren()
		{
			this.DangerValue = 0f;
			for (int i = this.ChildDangers.Count - 1; i >= 0; i--)
			{
				if (!this.ChildDangers[i].IsActive)
				{
					this.ChildDangers.RemoveAt(i);
				}
				else
				{
					this.DangerValue += this.ChildDangers[i].DangerValue;
				}
			}
		}

		// Token: 0x060034FF RID: 13567 RVA: 0x000AD9C7 File Offset: 0x000ABBC7
		public override void Dispose()
		{
			base.Dispose();
			this.ChildDangers.Clear();
		}
	}
}
