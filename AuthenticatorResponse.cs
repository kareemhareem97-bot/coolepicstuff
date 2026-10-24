using System;
using Utf8Json;

// Token: 0x020001FD RID: 509
public readonly struct AuthenticatorResponse : IEquatable<AuthenticatorResponse>, IJsonSerializable
{
	// Token: 0x06000AA9 RID: 2729 RVA: 0x00029C2C File Offset: 0x00027E2C
	[SerializationConstructor]
	public AuthenticatorResponse(bool success, bool verified, string error, string token, string[] messages, string[] actions, string[] authAccepted, AuthenticatiorAuthReject[] authRejected, string verificationChallenge, string verificationResponse)
	{
		this.success = success;
		this.verified = verified;
		this.error = error;
		this.token = token;
		this.messages = messages;
		this.actions = actions;
		this.authAccepted = authAccepted;
		this.authRejected = authRejected;
		this.verificationChallenge = verificationChallenge;
		this.verificationResponse = verificationResponse;
	}

	// Token: 0x06000AAA RID: 2730 RVA: 0x00029C88 File Offset: 0x00027E88
	public bool Equals(AuthenticatorResponse other)
	{
		return this.success == other.success && this.verified == other.verified && string.Equals(this.error, other.error) && string.Equals(this.token, other.token) && this.messages == other.messages && this.actions == other.actions && this.authAccepted == other.authAccepted && this.authRejected == other.authRejected && this.verificationChallenge == other.verificationChallenge && this.verificationResponse == other.verificationResponse;
	}

	// Token: 0x06000AAB RID: 2731 RVA: 0x00029D3C File Offset: 0x00027F3C
	public override bool Equals(object obj)
	{
		if (obj is AuthenticatorResponse)
		{
			AuthenticatorResponse other = (AuthenticatorResponse)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x00029D64 File Offset: 0x00027F64
	public override int GetHashCode()
	{
		return ((((((((this.success.GetHashCode() * 397 ^ this.verified.GetHashCode()) * 397 ^ ((this.error != null) ? this.error.GetHashCode() : 0)) * 397 ^ ((this.token != null) ? this.token.GetHashCode() : 0)) * 397 ^ ((this.messages != null) ? this.messages.GetHashCode() : 0)) * 397 ^ ((this.actions != null) ? this.actions.GetHashCode() : 0)) * 397 ^ ((this.authAccepted != null) ? this.authAccepted.GetHashCode() : 0)) * 397 ^ ((this.authRejected != null) ? this.authRejected.GetHashCode() : 0)) * 397 ^ ((this.verificationChallenge != null) ? this.verificationChallenge.GetHashCode() : 0)) * 397 ^ ((this.verificationResponse != null) ? this.verificationResponse.GetHashCode() : 0);
	}

	// Token: 0x06000AAD RID: 2733 RVA: 0x00029E7C File Offset: 0x0002807C
	public static bool operator ==(AuthenticatorResponse left, AuthenticatorResponse right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000AAE RID: 2734 RVA: 0x00029E86 File Offset: 0x00028086
	public static bool operator !=(AuthenticatorResponse left, AuthenticatorResponse right)
	{
		return !left.Equals(right);
	}

	// Token: 0x04000925 RID: 2341
	public readonly bool success;

	// Token: 0x04000926 RID: 2342
	public readonly bool verified;

	// Token: 0x04000927 RID: 2343
	public readonly string error;

	// Token: 0x04000928 RID: 2344
	public readonly string token;

	// Token: 0x04000929 RID: 2345
	public readonly string[] messages;

	// Token: 0x0400092A RID: 2346
	public readonly string[] actions;

	// Token: 0x0400092B RID: 2347
	public readonly string[] authAccepted;

	// Token: 0x0400092C RID: 2348
	public readonly AuthenticatiorAuthReject[] authRejected;

	// Token: 0x0400092D RID: 2349
	public readonly string verificationChallenge;

	// Token: 0x0400092E RID: 2350
	public readonly string verificationResponse;
}
