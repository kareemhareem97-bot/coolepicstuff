using System;
using System.Collections.Generic;

namespace Christmas.Scp2536
{
	// Token: 0x020010C9 RID: 4297
	public class Scp2536AnomalousGiftBase : Scp2536GiftBase
	{
		// Token: 0x170013D7 RID: 5079
		// (get) Token: 0x06005FAF RID: 24495 RVA: 0x00140045 File Offset: 0x0013E245
		public override UrgencyLevel Urgency { get; }

		// Token: 0x06005FB0 RID: 24496 RVA: 0x0014004D File Offset: 0x0013E24D
		public override bool CanBeGranted(ReferenceHub hub)
		{
			return !Scp2536AnomalousGiftBase.GlobalObtainedBy.Contains(hub) && base.CanBeGranted(hub);
		}

		// Token: 0x06005FB1 RID: 24497 RVA: 0x00140065 File Offset: 0x0013E265
		public override void ServerGrant(ReferenceHub hub)
		{
			Scp2536AnomalousGiftBase.GlobalObtainedBy.Add(hub);
		}

		// Token: 0x06005FB2 RID: 24498 RVA: 0x00140072 File Offset: 0x0013E272
		public override void Reset()
		{
			base.Reset();
			Scp2536AnomalousGiftBase.GlobalObtainedBy.Clear();
		}

		// Token: 0x040045C5 RID: 17861
		public static readonly List<ReferenceHub> GlobalObtainedBy = new List<ReferenceHub>();
	}
}
