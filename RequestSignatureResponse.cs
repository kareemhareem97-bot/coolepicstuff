using System;
using Utf8Json;

// Token: 0x0200020C RID: 524
public readonly struct RequestSignatureResponse : IEquatable<RequestSignatureResponse>, IJsonSerializable
{
	// Token: 0x06000AFC RID: 2812 RVA: 0x0002AABB File Offset: 0x00028CBB
	[SerializationConstructor]
	public RequestSignatureResponse(bool success, string error, SignedToken authToken, SignedToken badgeToken, string nonce)
	{
		this.success = success;
		this.error = error;
		this.authToken = authToken;
		this.badgeToken = badgeToken;
		this.nonce = nonce;
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x0002AAE4 File Offset: 0x00028CE4
	public bool Equals(RequestSignatureResponse other)
	{
		return this.success == other.success && this.error == other.error && this.authToken == other.authToken && this.badgeToken == other.badgeToken && this.nonce == other.nonce;
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x0002AB44 File Offset: 0x00028D44
	public override bool Equals(object obj)
	{
		if (obj is RequestSignatureResponse)
		{
			RequestSignatureResponse other = (RequestSignatureResponse)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x0002AB6C File Offset: 0x00028D6C
	public override int GetHashCode()
	{
		return (((this.success.GetHashCode() * 397 ^ ((this.error != null) ? this.error.GetHashCode() : 0)) * 397 ^ ((this.authToken != null) ? this.authToken.GetHashCode() : 0)) * 397 ^ ((this.badgeToken != null) ? this.badgeToken.GetHashCode() : 0)) * 397 ^ ((this.nonce != null) ? this.nonce.GetHashCode() : 0);
	}

	// Token: 0x06000B00 RID: 2816 RVA: 0x0002ABFB File Offset: 0x00028DFB
	public static bool operator ==(RequestSignatureResponse left, RequestSignatureResponse right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000B01 RID: 2817 RVA: 0x0002AC05 File Offset: 0x00028E05
	public static bool operator !=(RequestSignatureResponse left, RequestSignatureResponse right)
	{
		return !left.Equals(right);
	}

	// Token: 0x04000969 RID: 2409
	public readonly bool success;

	// Token: 0x0400096A RID: 2410
	public readonly string error;

	// Token: 0x0400096B RID: 2411
	public readonly SignedToken authToken;

	// Token: 0x0400096C RID: 2412
	public readonly SignedToken badgeToken;

	// Token: 0x0400096D RID: 2413
	public readonly string nonce;
}
