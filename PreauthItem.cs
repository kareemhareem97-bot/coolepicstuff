using System;

// Token: 0x02000187 RID: 391
public struct PreauthItem
{
	// Token: 0x17000137 RID: 311
	// (get) Token: 0x060007B4 RID: 1972 RVA: 0x0001D5E8 File Offset: 0x0001B7E8
	// (set) Token: 0x060007B5 RID: 1973 RVA: 0x0001D5F0 File Offset: 0x0001B7F0
	public string UserId { readonly get; private set; }

	// Token: 0x17000138 RID: 312
	// (get) Token: 0x060007B6 RID: 1974 RVA: 0x0001D5F9 File Offset: 0x0001B7F9
	// (set) Token: 0x060007B7 RID: 1975 RVA: 0x0001D601 File Offset: 0x0001B801
	public long Added { readonly get; private set; }

	// Token: 0x060007B8 RID: 1976 RVA: 0x0001D60C File Offset: 0x0001B80C
	public PreauthItem(string userId)
	{
		this.UserId = userId;
		this.Added = DateTime.Now.Ticks;
	}

	// Token: 0x060007B9 RID: 1977 RVA: 0x0001D634 File Offset: 0x0001B834
	public void SetUserId(string userId)
	{
		this.UserId = userId;
		this.Added = DateTime.Now.Ticks;
	}
}
