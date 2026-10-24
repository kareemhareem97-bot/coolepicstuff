using System;
using Utf8Json;

// Token: 0x02000204 RID: 516
public readonly struct DiscordEmbedField : IEquatable<DiscordEmbedField>, IJsonSerializable
{
	// Token: 0x06000ACE RID: 2766 RVA: 0x0002A352 File Offset: 0x00028552
	[SerializationConstructor]
	public DiscordEmbedField(string name, string value, bool inline)
	{
		this.name = name;
		this.value = value;
		this.inline = inline;
	}

	// Token: 0x06000ACF RID: 2767 RVA: 0x0002A369 File Offset: 0x00028569
	public bool Equals(DiscordEmbedField other)
	{
		return this.name == other.name && this.value == other.value && this.inline == other.inline;
	}

	// Token: 0x06000AD0 RID: 2768 RVA: 0x0002A3A4 File Offset: 0x000285A4
	public override bool Equals(object obj)
	{
		if (obj is DiscordEmbedField)
		{
			DiscordEmbedField other = (DiscordEmbedField)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000AD1 RID: 2769 RVA: 0x0002A3CC File Offset: 0x000285CC
	public override int GetHashCode()
	{
		return (((this.name != null) ? this.name.GetHashCode() : 0) * 397 ^ ((this.value != null) ? this.value.GetHashCode() : 0)) * 397 ^ this.inline.GetHashCode();
	}

	// Token: 0x06000AD2 RID: 2770 RVA: 0x0002A421 File Offset: 0x00028621
	public static bool operator ==(DiscordEmbedField left, DiscordEmbedField right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000AD3 RID: 2771 RVA: 0x0002A42B File Offset: 0x0002862B
	public static bool operator !=(DiscordEmbedField left, DiscordEmbedField right)
	{
		return !left.Equals(right);
	}

	// Token: 0x04000948 RID: 2376
	public readonly string name;

	// Token: 0x04000949 RID: 2377
	public readonly string value;

	// Token: 0x0400094A RID: 2378
	public readonly bool inline;
}
