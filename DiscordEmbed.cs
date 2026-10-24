using System;
using Utf8Json;

// Token: 0x02000203 RID: 515
public readonly struct DiscordEmbed : IEquatable<DiscordEmbed>, IJsonSerializable
{
	// Token: 0x06000AC8 RID: 2760 RVA: 0x0002A1FA File Offset: 0x000283FA
	[SerializationConstructor]
	public DiscordEmbed(string title, string type, string description, int color, DiscordEmbedField[] fields)
	{
		this.title = title;
		this.type = type;
		this.description = description;
		this.color = color;
		this.fields = fields;
	}

	// Token: 0x06000AC9 RID: 2761 RVA: 0x0002A224 File Offset: 0x00028424
	public bool Equals(DiscordEmbed other)
	{
		return this.title == other.title && this.type == other.type && this.description == other.description && this.color == other.color && object.Equals(this.fields, other.fields);
	}

	// Token: 0x06000ACA RID: 2762 RVA: 0x0002A28C File Offset: 0x0002848C
	public override bool Equals(object obj)
	{
		if (obj is DiscordEmbed)
		{
			DiscordEmbed other = (DiscordEmbed)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000ACB RID: 2763 RVA: 0x0002A2B4 File Offset: 0x000284B4
	public override int GetHashCode()
	{
		return (((((this.title != null) ? this.title.GetHashCode() : 0) * 397 ^ ((this.type != null) ? this.type.GetHashCode() : 0)) * 397 ^ ((this.description != null) ? this.description.GetHashCode() : 0)) * 397 ^ this.color) * 397 ^ ((this.fields != null) ? this.fields.GetHashCode() : 0);
	}

	// Token: 0x06000ACC RID: 2764 RVA: 0x0002A33B File Offset: 0x0002853B
	public static bool operator ==(DiscordEmbed left, DiscordEmbed right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000ACD RID: 2765 RVA: 0x0002A345 File Offset: 0x00028545
	public static bool operator !=(DiscordEmbed left, DiscordEmbed right)
	{
		return !left.Equals(right);
	}

	// Token: 0x04000943 RID: 2371
	public readonly string title;

	// Token: 0x04000944 RID: 2372
	public readonly string type;

	// Token: 0x04000945 RID: 2373
	public readonly string description;

	// Token: 0x04000946 RID: 2374
	public readonly int color;

	// Token: 0x04000947 RID: 2375
	public readonly DiscordEmbedField[] fields;
}
