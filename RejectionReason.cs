using System;

// Token: 0x02000183 RID: 387
public enum RejectionReason : byte
{
	// Token: 0x040006DC RID: 1756
	NotSpecified,
	// Token: 0x040006DD RID: 1757
	ServerFull,
	// Token: 0x040006DE RID: 1758
	InvalidToken,
	// Token: 0x040006DF RID: 1759
	VersionMismatch,
	// Token: 0x040006E0 RID: 1760
	Error,
	// Token: 0x040006E1 RID: 1761
	AuthenticationRequired,
	// Token: 0x040006E2 RID: 1762
	Banned,
	// Token: 0x040006E3 RID: 1763
	NotWhitelisted,
	// Token: 0x040006E4 RID: 1764
	GloballyBanned,
	// Token: 0x040006E5 RID: 1765
	Geoblocked,
	// Token: 0x040006E6 RID: 1766
	Custom,
	// Token: 0x040006E7 RID: 1767
	ExpiredAuth,
	// Token: 0x040006E8 RID: 1768
	RateLimit,
	// Token: 0x040006E9 RID: 1769
	Challenge,
	// Token: 0x040006EA RID: 1770
	InvalidChallengeKey,
	// Token: 0x040006EB RID: 1771
	InvalidChallenge,
	// Token: 0x040006EC RID: 1772
	Redirect,
	// Token: 0x040006ED RID: 1773
	Delay,
	// Token: 0x040006EE RID: 1774
	VerificationAccepted,
	// Token: 0x040006EF RID: 1775
	VerificationRejected,
	// Token: 0x040006F0 RID: 1776
	CentralServerAuthRejected
}
