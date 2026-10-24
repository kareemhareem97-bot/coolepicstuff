using System;
using Utf8Json;

// Token: 0x02000202 RID: 514
public readonly struct DiscordWebhook : IEquatable<DiscordWebhook>, IJsonSerializable
{
	// Token: 0x06000AC2 RID: 2754 RVA: 0x0002A09B File Offset: 0x0002829B
	[SerializationConstructor]
	public DiscordWebhook(string content, string username, string avatar_url, bool tts, DiscordEmbed[] embeds)
	{
		this.content = content;
		this.username = username;
		this.avatar_url = avatar_url;
		this.tts = tts;
		this.embeds = embeds;
	}

	// Token: 0x06000AC3 RID: 2755 RVA: 0x0002A0C4 File Offset: 0x000282C4
	public bool Equals(DiscordWebhook other)
	{
		return this.content == other.content && this.username == other.username && this.avatar_url == other.avatar_url && this.tts == other.tts && object.Equals(this.embeds, other.embeds);
	}

	// Token: 0x06000AC4 RID: 2756 RVA: 0x0002A12C File Offset: 0x0002832C
	public override bool Equals(object obj)
	{
		if (obj is DiscordWebhook)
		{
			DiscordWebhook other = (DiscordWebhook)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000AC5 RID: 2757 RVA: 0x0002A154 File Offset: 0x00028354
	public override int GetHashCode()
	{
		return (((((this.content != null) ? this.content.GetHashCode() : 0) * 397 ^ ((this.username != null) ? this.username.GetHashCode() : 0)) * 397 ^ ((this.avatar_url != null) ? this.avatar_url.GetHashCode() : 0)) * 397 ^ this.tts.GetHashCode()) * 397 ^ ((this.embeds != null) ? this.embeds.GetHashCode() : 0);
	}

	// Token: 0x06000AC6 RID: 2758 RVA: 0x0002A1E3 File Offset: 0x000283E3
	public static bool operator ==(DiscordWebhook left, DiscordWebhook right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000AC7 RID: 2759 RVA: 0x0002A1ED File Offset: 0x000283ED
	public static bool operator !=(DiscordWebhook left, DiscordWebhook right)
	{
		return !left.Equals(right);
	}

	// Token: 0x0400093E RID: 2366
	public readonly string content;

	// Token: 0x0400093F RID: 2367
	public readonly string username;

	// Token: 0x04000940 RID: 2368
	public readonly string avatar_url;

	// Token: 0x04000941 RID: 2369
	public readonly bool tts;

	// Token: 0x04000942 RID: 2370
	public readonly DiscordEmbed[] embeds;
}
