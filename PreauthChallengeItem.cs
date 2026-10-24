using System;

// Token: 0x02000188 RID: 392
public struct PreauthChallengeItem
{
	// Token: 0x17000139 RID: 313
	// (get) Token: 0x060007BA RID: 1978 RVA: 0x0001D65B File Offset: 0x0001B85B
	// (set) Token: 0x060007BB RID: 1979 RVA: 0x0001D663 File Offset: 0x0001B863
	public long Added { readonly get; private set; }

	// Token: 0x1700013A RID: 314
	// (get) Token: 0x060007BC RID: 1980 RVA: 0x0001D66C File Offset: 0x0001B86C
	// (set) Token: 0x060007BD RID: 1981 RVA: 0x0001D674 File Offset: 0x0001B874
	public ArraySegment<byte> ValidResponse { readonly get; private set; }

	// Token: 0x060007BE RID: 1982 RVA: 0x0001D680 File Offset: 0x0001B880
	public PreauthChallengeItem(ArraySegment<byte> response)
	{
		this.ValidResponse = response;
		this.Added = DateTime.Now.Ticks;
	}
}
