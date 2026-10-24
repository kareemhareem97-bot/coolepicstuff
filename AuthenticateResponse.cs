using System;
using Utf8Json;

// Token: 0x020001FA RID: 506
public readonly struct AuthenticateResponse : IEquatable<AuthenticateResponse>, IJsonSerializable
{
	// Token: 0x06000A9C RID: 2716 RVA: 0x0002980C File Offset: 0x00027A0C
	[SerializationConstructor]
	public AuthenticateResponse(bool success, string error, string token, string id, string nonce, string country, byte flags, long expiration, string preauth, string globalBan, ushort lifetime, bool NoWatermarking)
	{
		this.success = success;
		this.error = error;
		this.token = token;
		this.id = id;
		this.nonce = nonce;
		this.country = country;
		this.flags = flags;
		this.expiration = expiration;
		this.preauth = preauth;
		this.globalBan = globalBan;
		this.lifetime = lifetime;
		this.NoWatermarking = NoWatermarking;
	}

	// Token: 0x06000A9D RID: 2717 RVA: 0x00029878 File Offset: 0x00027A78
	public bool Equals(AuthenticateResponse other)
	{
		return this.success == other.success && this.error == other.error && this.token == other.token && this.id == other.id && this.nonce == other.nonce && this.country == other.country && this.flags == other.flags && this.expiration == other.expiration && this.preauth == other.preauth && this.globalBan == other.globalBan && this.lifetime == other.lifetime && this.NoWatermarking == other.NoWatermarking;
	}

	// Token: 0x06000A9E RID: 2718 RVA: 0x00029960 File Offset: 0x00027B60
	public override bool Equals(object obj)
	{
		if (obj is AuthenticateResponse)
		{
			AuthenticateResponse other = (AuthenticateResponse)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000A9F RID: 2719 RVA: 0x00029988 File Offset: 0x00027B88
	public override int GetHashCode()
	{
		return ((((((((((this.success.GetHashCode() * 397 ^ ((this.error != null) ? this.error.GetHashCode() : 0)) * 397 ^ ((this.token != null) ? this.token.GetHashCode() : 0)) * 397 ^ ((this.id != null) ? this.id.GetHashCode() : 0)) * 397 ^ ((this.nonce != null) ? this.nonce.GetHashCode() : 0)) * 397 ^ ((this.country != null) ? this.country.GetHashCode() : 0)) * 397 ^ this.flags.GetHashCode()) * 397 ^ this.expiration.GetHashCode()) * 397 ^ ((this.preauth != null) ? this.preauth.GetHashCode() : 0)) * 397 ^ ((this.globalBan != null) ? this.globalBan.GetHashCode() : 0)) * 397 ^ this.lifetime.GetHashCode()) * 397 ^ this.NoWatermarking.GetHashCode();
	}

	// Token: 0x06000AA0 RID: 2720 RVA: 0x00029AC2 File Offset: 0x00027CC2
	public static bool operator ==(AuthenticateResponse left, AuthenticateResponse right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000AA1 RID: 2721 RVA: 0x00029ACC File Offset: 0x00027CCC
	public static bool operator !=(AuthenticateResponse left, AuthenticateResponse right)
	{
		return !left.Equals(right);
	}

	// Token: 0x04000907 RID: 2311
	public readonly bool success;

	// Token: 0x04000908 RID: 2312
	public readonly string error;

	// Token: 0x04000909 RID: 2313
	public readonly string token;

	// Token: 0x0400090A RID: 2314
	public readonly string id;

	// Token: 0x0400090B RID: 2315
	public readonly string nonce;

	// Token: 0x0400090C RID: 2316
	public readonly string country;

	// Token: 0x0400090D RID: 2317
	public readonly byte flags;

	// Token: 0x0400090E RID: 2318
	public readonly long expiration;

	// Token: 0x0400090F RID: 2319
	public readonly string preauth;

	// Token: 0x04000910 RID: 2320
	public readonly string globalBan;

	// Token: 0x04000911 RID: 2321
	public readonly ushort lifetime;

	// Token: 0x04000912 RID: 2322
	public readonly bool NoWatermarking;
}
