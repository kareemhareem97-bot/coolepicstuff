using System;
using Utf8Json;

// Token: 0x0200020B RID: 523
public readonly struct RenewResponse : IEquatable<RenewResponse>, IJsonSerializable
{
	// Token: 0x06000AF6 RID: 2806 RVA: 0x0002A858 File Offset: 0x00028A58
	[SerializationConstructor]
	public RenewResponse(bool success, string error, string id, string nonce, string country, byte flags, long expiration, string preauth, string globalBan, ushort lifetime)
	{
		this.success = success;
		this.error = error;
		this.id = id;
		this.country = country;
		this.nonce = nonce;
		this.flags = flags;
		this.expiration = expiration;
		this.preauth = preauth;
		this.globalBan = globalBan;
		this.lifetime = lifetime;
	}

	// Token: 0x06000AF7 RID: 2807 RVA: 0x0002A8B4 File Offset: 0x00028AB4
	public bool Equals(RenewResponse other)
	{
		return this.success == other.success && this.error == other.error && this.id == other.id && this.nonce == other.nonce && this.country == other.country && this.flags == other.flags && this.expiration == other.expiration && this.preauth == other.preauth && this.globalBan == other.globalBan && this.lifetime == other.lifetime;
	}

	// Token: 0x06000AF8 RID: 2808 RVA: 0x0002A974 File Offset: 0x00028B74
	public override bool Equals(object obj)
	{
		if (obj is RenewResponse)
		{
			RenewResponse other = (RenewResponse)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000AF9 RID: 2809 RVA: 0x0002A99C File Offset: 0x00028B9C
	public override int GetHashCode()
	{
		return ((((((((this.success.GetHashCode() * 397 ^ ((this.error != null) ? this.error.GetHashCode() : 0)) * 397 ^ ((this.id != null) ? this.id.GetHashCode() : 0)) * 397 ^ ((this.nonce != null) ? this.nonce.GetHashCode() : 0)) * 397 ^ ((this.country != null) ? this.country.GetHashCode() : 0)) * 397 ^ this.flags.GetHashCode()) * 397 ^ this.expiration.GetHashCode()) * 397 ^ ((this.preauth != null) ? this.preauth.GetHashCode() : 0)) * 397 ^ ((this.globalBan != null) ? this.globalBan.GetHashCode() : 0)) * 397 ^ this.lifetime.GetHashCode();
	}

	// Token: 0x06000AFA RID: 2810 RVA: 0x0002AAA4 File Offset: 0x00028CA4
	public static bool operator ==(RenewResponse left, RenewResponse right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000AFB RID: 2811 RVA: 0x0002AAAE File Offset: 0x00028CAE
	public static bool operator !=(RenewResponse left, RenewResponse right)
	{
		return !left.Equals(right);
	}

	// Token: 0x0400095F RID: 2399
	public readonly bool success;

	// Token: 0x04000960 RID: 2400
	public readonly string error;

	// Token: 0x04000961 RID: 2401
	public readonly string id;

	// Token: 0x04000962 RID: 2402
	public readonly string nonce;

	// Token: 0x04000963 RID: 2403
	public readonly string country;

	// Token: 0x04000964 RID: 2404
	public readonly byte flags;

	// Token: 0x04000965 RID: 2405
	public readonly long expiration;

	// Token: 0x04000966 RID: 2406
	public readonly string preauth;

	// Token: 0x04000967 RID: 2407
	public readonly string globalBan;

	// Token: 0x04000968 RID: 2408
	public readonly ushort lifetime;
}
