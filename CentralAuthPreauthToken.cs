using System;

// Token: 0x02000178 RID: 376
public readonly struct CentralAuthPreauthToken
{
	// Token: 0x06000785 RID: 1925 RVA: 0x0001C815 File Offset: 0x0001AA15
	public CentralAuthPreauthToken(string userId, byte flags, string country, long expiration, string signature)
	{
		this.UserId = userId;
		this.Flags = flags;
		this.Country = country;
		this.Expiration = expiration;
		this.Signature = signature;
	}

	// Token: 0x040006B0 RID: 1712
	public readonly string UserId;

	// Token: 0x040006B1 RID: 1713
	public readonly byte Flags;

	// Token: 0x040006B2 RID: 1714
	public readonly string Country;

	// Token: 0x040006B3 RID: 1715
	public readonly long Expiration;

	// Token: 0x040006B4 RID: 1716
	public readonly string Signature;
}
