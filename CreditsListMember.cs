using System;
using Utf8Json;

// Token: 0x02000201 RID: 513
public readonly struct CreditsListMember : IEquatable<CreditsListMember>, IJsonSerializable
{
	// Token: 0x06000ABC RID: 2748 RVA: 0x00029FF3 File Offset: 0x000281F3
	[SerializationConstructor]
	public CreditsListMember(string name, string title, string color)
	{
		this.name = name;
		this.title = title;
		this.color = color;
	}

	// Token: 0x06000ABD RID: 2749 RVA: 0x0002A00A File Offset: 0x0002820A
	public bool Equals(CreditsListMember other)
	{
		return this.name == other.name && this.title == other.title && this.color == other.color;
	}

	// Token: 0x06000ABE RID: 2750 RVA: 0x0002A048 File Offset: 0x00028248
	public override bool Equals(object obj)
	{
		if (obj is CreditsListMember)
		{
			CreditsListMember other = (CreditsListMember)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000ABF RID: 2751 RVA: 0x0002A06D File Offset: 0x0002826D
	public override int GetHashCode()
	{
		if (this.name == null)
		{
			return 0;
		}
		return this.name.GetHashCode();
	}

	// Token: 0x06000AC0 RID: 2752 RVA: 0x0002A084 File Offset: 0x00028284
	public static bool operator ==(CreditsListMember left, CreditsListMember right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000AC1 RID: 2753 RVA: 0x0002A08E File Offset: 0x0002828E
	public static bool operator !=(CreditsListMember left, CreditsListMember right)
	{
		return !left.Equals(right);
	}

	// Token: 0x0400093B RID: 2363
	public readonly string name;

	// Token: 0x0400093C RID: 2364
	public readonly string title;

	// Token: 0x0400093D RID: 2365
	public readonly string color;
}
