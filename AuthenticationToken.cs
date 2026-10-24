using System;
using Utf8Json;

// Token: 0x020001FB RID: 507
public class AuthenticationToken : Token, IJsonSerializable
{
	// Token: 0x06000AA2 RID: 2722 RVA: 0x00029ADC File Offset: 0x00027CDC
	[SerializationConstructor]
	public AuthenticationToken(string requestIp, int asn, string globalBan, string vacSession, bool doNotTrack, bool skipIpCheck, bool bypassBans, bool bypassGeoRestrictions, bool bypassWhitelists, bool globalBadge, bool privateBetaOwnership, bool syncHashed, string publicKey, string challenge, int playerId, string gameVersion, string userId, string nickname, DateTimeOffset issuanceTime, DateTimeOffset expirationTime, string usage, string issuedBy, string serial, bool testSignature, int tokenVersion) : base(userId, nickname, issuanceTime, expirationTime, usage, issuedBy, serial, testSignature, tokenVersion)
	{
		this.RequestIp = requestIp;
		this.Asn = asn;
		this.GlobalBan = globalBan;
		this.VacSession = vacSession;
		this.DoNotTrack = doNotTrack;
		this.SkipIpCheck = skipIpCheck;
		this.BypassBans = bypassBans;
		this.BypassGeoRestrictions = bypassGeoRestrictions;
		this.BypassWhitelists = bypassWhitelists;
		this.GlobalBadge = globalBadge;
		this.PrivateBetaOwnership = privateBetaOwnership;
		this.SyncHashed = syncHashed;
		this.PublicKey = publicKey;
		this.Challenge = challenge;
		this.PlayerId = playerId;
		this.GameVersion = gameVersion;
	}

	// Token: 0x04000913 RID: 2323
	public readonly string RequestIp;

	// Token: 0x04000914 RID: 2324
	public readonly int Asn;

	// Token: 0x04000915 RID: 2325
	public readonly string GlobalBan;

	// Token: 0x04000916 RID: 2326
	public readonly string VacSession;

	// Token: 0x04000917 RID: 2327
	public readonly bool DoNotTrack;

	// Token: 0x04000918 RID: 2328
	public readonly bool SkipIpCheck;

	// Token: 0x04000919 RID: 2329
	public readonly bool BypassBans;

	// Token: 0x0400091A RID: 2330
	public readonly bool BypassGeoRestrictions;

	// Token: 0x0400091B RID: 2331
	public readonly bool BypassWhitelists;

	// Token: 0x0400091C RID: 2332
	public readonly bool GlobalBadge;

	// Token: 0x0400091D RID: 2333
	public readonly bool PrivateBetaOwnership;

	// Token: 0x0400091E RID: 2334
	public readonly bool SyncHashed;

	// Token: 0x0400091F RID: 2335
	public readonly string PublicKey;

	// Token: 0x04000920 RID: 2336
	public readonly string Challenge;

	// Token: 0x04000921 RID: 2337
	public readonly int PlayerId;

	// Token: 0x04000922 RID: 2338
	public readonly string GameVersion;
}
