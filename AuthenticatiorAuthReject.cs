using System;
using Utf8Json;

// Token: 0x020001FC RID: 508
public readonly struct AuthenticatiorAuthReject : IEquatable<AuthenticatiorAuthReject>, IJsonSerializable
{
	// Token: 0x06000AA3 RID: 2723 RVA: 0x00029B7E File Offset: 0x00027D7E
	[SerializationConstructor]
	public AuthenticatiorAuthReject(string id, string reason)
	{
		this.Id = id;
		this.Reason = reason;
	}

	// Token: 0x06000AA4 RID: 2724 RVA: 0x00029B8E File Offset: 0x00027D8E
	public bool Equals(AuthenticatiorAuthReject other)
	{
		return string.Equals(this.Id, other.Id) && string.Equals(this.Reason, other.Reason);
	}

	// Token: 0x06000AA5 RID: 2725 RVA: 0x00029BB8 File Offset: 0x00027DB8
	public override bool Equals(object obj)
	{
		if (obj is AuthenticatiorAuthReject)
		{
			AuthenticatiorAuthReject other = (AuthenticatiorAuthReject)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000AA6 RID: 2726 RVA: 0x00029BDD File Offset: 0x00027DDD
	public override int GetHashCode()
	{
		return ((this.Id != null) ? this.Id.GetHashCode() : 0) * 397 ^ ((this.Reason != null) ? this.Reason.GetHashCode() : 0);
	}

	// Token: 0x06000AA7 RID: 2727 RVA: 0x00029C12 File Offset: 0x00027E12
	public static bool operator ==(AuthenticatiorAuthReject left, AuthenticatiorAuthReject right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000AA8 RID: 2728 RVA: 0x00029C1C File Offset: 0x00027E1C
	public static bool operator !=(AuthenticatiorAuthReject left, AuthenticatiorAuthReject right)
	{
		return !left.Equals(right);
	}

	// Token: 0x04000923 RID: 2339
	public readonly string Id;

	// Token: 0x04000924 RID: 2340
	public readonly string Reason;
}
