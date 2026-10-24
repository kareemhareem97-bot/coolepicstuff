using System;
using Utf8Json;

// Token: 0x0200020F RID: 527
public readonly struct ServerListSigned : IEquatable<ServerListSigned>, IJsonSerializable
{
	// Token: 0x06000B0E RID: 2830 RVA: 0x0002AF39 File Offset: 0x00029139
	[SerializationConstructor]
	public ServerListSigned(string payload, long timestamp, string signature, string nonce, string error)
	{
		this.payload = payload;
		this.timestamp = timestamp;
		this.signature = signature;
		this.nonce = nonce;
		this.error = error;
	}

	// Token: 0x06000B0F RID: 2831 RVA: 0x0002AF60 File Offset: 0x00029160
	public bool Equals(ServerListSigned other)
	{
		return this.payload == other.payload && this.timestamp == other.timestamp && this.signature == other.signature && this.nonce == other.nonce && this.error == other.error;
	}

	// Token: 0x06000B10 RID: 2832 RVA: 0x0002AFC8 File Offset: 0x000291C8
	public override bool Equals(object obj)
	{
		if (obj is ServerListSigned)
		{
			ServerListSigned other = (ServerListSigned)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000B11 RID: 2833 RVA: 0x0002AFF0 File Offset: 0x000291F0
	public override int GetHashCode()
	{
		return (((((this.payload != null) ? this.payload.GetHashCode() : 0) * 397 ^ this.timestamp.GetHashCode()) * 397 ^ ((this.signature != null) ? this.signature.GetHashCode() : 0)) * 397 ^ ((this.nonce != null) ? this.nonce.GetHashCode() : 0)) * 397 ^ ((this.error != null) ? this.error.GetHashCode() : 0);
	}

	// Token: 0x06000B12 RID: 2834 RVA: 0x0002B07F File Offset: 0x0002927F
	public static bool operator ==(ServerListSigned left, ServerListSigned right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000B13 RID: 2835 RVA: 0x0002B089 File Offset: 0x00029289
	public static bool operator !=(ServerListSigned left, ServerListSigned right)
	{
		return !left.Equals(right);
	}

	// Token: 0x0400097C RID: 2428
	public readonly string payload;

	// Token: 0x0400097D RID: 2429
	public readonly long timestamp;

	// Token: 0x0400097E RID: 2430
	public readonly string signature;

	// Token: 0x0400097F RID: 2431
	public readonly string nonce;

	// Token: 0x04000980 RID: 2432
	public readonly string error;
}
