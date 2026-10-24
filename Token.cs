using System;

// Token: 0x02000211 RID: 529
public abstract class Token
{
	// Token: 0x06000B18 RID: 2840 RVA: 0x0002B2AC File Offset: 0x000294AC
	protected Token(string userId, string nickname, DateTimeOffset issuanceTime, DateTimeOffset expirationTime, string usage, string issuedBy, string serial, bool testSignature, int tokenVersion)
	{
		this.UserId = userId;
		this.Nickname = nickname;
		this.IssuanceTime = issuanceTime;
		this.ExpirationTime = expirationTime;
		this.Usage = usage;
		this.IssuedBy = issuedBy;
		this.Serial = serial;
		this.TestSignature = testSignature;
		this.TokenVersion = tokenVersion;
	}

	// Token: 0x04000983 RID: 2435
	public readonly string UserId;

	// Token: 0x04000984 RID: 2436
	public readonly string Nickname;

	// Token: 0x04000985 RID: 2437
	public readonly DateTimeOffset IssuanceTime;

	// Token: 0x04000986 RID: 2438
	public readonly DateTimeOffset ExpirationTime;

	// Token: 0x04000987 RID: 2439
	public readonly string Usage;

	// Token: 0x04000988 RID: 2440
	public readonly string IssuedBy;

	// Token: 0x04000989 RID: 2441
	public readonly string Serial;

	// Token: 0x0400098A RID: 2442
	public readonly bool TestSignature;

	// Token: 0x0400098B RID: 2443
	public readonly int TokenVersion;
}
