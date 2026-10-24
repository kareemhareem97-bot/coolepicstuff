using System;
using System.Collections.Generic;

namespace Christmas.Scp2536
{
	// Token: 0x020010CE RID: 4302
	public abstract class Scp2536GiftBase
	{
		// Token: 0x170013DD RID: 5085
		// (get) Token: 0x06005FD7 RID: 24535
		public abstract UrgencyLevel Urgency { get; }

		// Token: 0x170013DE RID: 5086
		// (get) Token: 0x06005FD8 RID: 24536 RVA: 0x00004094 File Offset: 0x00002294
		internal virtual bool IgnoredByRandomness
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06005FD9 RID: 24537 RVA: 0x001408C7 File Offset: 0x0013EAC7
		public virtual bool CanBeGranted(ReferenceHub hub)
		{
			return !this.ObtainedBy.Contains(hub);
		}

		// Token: 0x06005FDA RID: 24538
		public abstract void ServerGrant(ReferenceHub hub);

		// Token: 0x06005FDB RID: 24539 RVA: 0x001408D8 File Offset: 0x0013EAD8
		public virtual void Reset()
		{
			this.ObtainedBy.Clear();
		}

		// Token: 0x040045E7 RID: 17895
		public readonly HashSet<ReferenceHub> ObtainedBy = new HashSet<ReferenceHub>();
	}
}
