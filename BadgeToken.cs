using System;
using Utf8Json;

// Token: 0x020001FE RID: 510
public class BadgeToken : Token, IJsonSerializable
{
	// Token: 0x06000AAF RID: 2735 RVA: 0x00029E94 File Offset: 0x00028094
	[SerializationConstructor]
	public BadgeToken(string badgeText, string badgeColor, int badgeType, bool staff, bool remoteAdmin, bool management, bool overwatchMode, bool globalBanning, ulong raPermissions, string userId, string nickname, DateTimeOffset issuanceTime, DateTimeOffset expirationTime, string usage, string issuedBy, string serial, bool testSignature, int tokenVersion) : base(userId, nickname, issuanceTime, expirationTime, usage, issuedBy, serial, testSignature, tokenVersion)
	{
		this.BadgeText = badgeText;
		this.BadgeColor = badgeColor;
		this.BadgeType = badgeType;
		this.Staff = staff;
		this.RemoteAdmin = remoteAdmin;
		this.Management = management;
		this.OverwatchMode = overwatchMode;
		this.GlobalBanning = globalBanning;
		this.RaPermissions = raPermissions;
	}

	// Token: 0x0400092F RID: 2351
	public readonly string BadgeText;

	// Token: 0x04000930 RID: 2352
	public readonly string BadgeColor;

	// Token: 0x04000931 RID: 2353
	public readonly int BadgeType;

	// Token: 0x04000932 RID: 2354
	public readonly bool Staff;

	// Token: 0x04000933 RID: 2355
	public readonly bool RemoteAdmin;

	// Token: 0x04000934 RID: 2356
	public readonly bool Management;

	// Token: 0x04000935 RID: 2357
	public readonly bool OverwatchMode;

	// Token: 0x04000936 RID: 2358
	public readonly bool GlobalBanning;

	// Token: 0x04000937 RID: 2359
	public readonly ulong RaPermissions;
}
