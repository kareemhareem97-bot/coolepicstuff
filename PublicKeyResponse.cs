using System;
using Utf8Json;

// Token: 0x0200020A RID: 522
public readonly struct PublicKeyResponse : IEquatable<PublicKeyResponse>, IJsonSerializable
{
	// Token: 0x06000AF0 RID: 2800 RVA: 0x0002A76B File Offset: 0x0002896B
	[SerializationConstructor]
	public PublicKeyResponse(string key, string signature, string credits)
	{
		this.key = key;
		this.signature = signature;
		this.credits = credits;
	}

	// Token: 0x06000AF1 RID: 2801 RVA: 0x0002A782 File Offset: 0x00028982
	public bool Equals(PublicKeyResponse other)
	{
		return this.key == other.key && this.signature == other.signature && this.credits == other.credits;
	}

	// Token: 0x06000AF2 RID: 2802 RVA: 0x0002A7C0 File Offset: 0x000289C0
	public override bool Equals(object obj)
	{
		if (obj is PublicKeyResponse)
		{
			PublicKeyResponse other = (PublicKeyResponse)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000AF3 RID: 2803 RVA: 0x0002A7E8 File Offset: 0x000289E8
	public override int GetHashCode()
	{
		return ((this.key != null) ? this.key.GetHashCode() : 0) * 397 ^ ((this.signature != null) ? this.signature.GetHashCode() : 0) ^ ((this.credits != null) ? this.credits.GetHashCode() : 0);
	}

	// Token: 0x06000AF4 RID: 2804 RVA: 0x0002A83F File Offset: 0x00028A3F
	public static bool operator ==(PublicKeyResponse left, PublicKeyResponse right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000AF5 RID: 2805 RVA: 0x0002A849 File Offset: 0x00028A49
	public static bool operator !=(PublicKeyResponse left, PublicKeyResponse right)
	{
		return !left.Equals(right);
	}

	// Token: 0x0400095C RID: 2396
	public readonly string key;

	// Token: 0x0400095D RID: 2397
	public readonly string signature;

	// Token: 0x0400095E RID: 2398
	public readonly string credits;
}
