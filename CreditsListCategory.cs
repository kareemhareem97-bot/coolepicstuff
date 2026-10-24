using System;
using Utf8Json;

// Token: 0x02000200 RID: 512
public readonly struct CreditsListCategory : IEquatable<CreditsListCategory>, IJsonSerializable
{
	// Token: 0x06000AB6 RID: 2742 RVA: 0x00029F6B File Offset: 0x0002816B
	[SerializationConstructor]
	public CreditsListCategory(string category, CreditsListMember[] members)
	{
		this.category = category;
		this.members = members;
	}

	// Token: 0x06000AB7 RID: 2743 RVA: 0x00029F7B File Offset: 0x0002817B
	public bool Equals(CreditsListCategory other)
	{
		return this.category == other.category && this.members == other.members;
	}

	// Token: 0x06000AB8 RID: 2744 RVA: 0x00029FA0 File Offset: 0x000281A0
	public override bool Equals(object obj)
	{
		if (obj is CreditsListCategory)
		{
			CreditsListCategory other = (CreditsListCategory)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000AB9 RID: 2745 RVA: 0x00029FC5 File Offset: 0x000281C5
	public override int GetHashCode()
	{
		if (this.category == null)
		{
			return 0;
		}
		return this.category.GetHashCode();
	}

	// Token: 0x06000ABA RID: 2746 RVA: 0x00029FDC File Offset: 0x000281DC
	public static bool operator ==(CreditsListCategory left, CreditsListCategory right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000ABB RID: 2747 RVA: 0x00029FE6 File Offset: 0x000281E6
	public static bool operator !=(CreditsListCategory left, CreditsListCategory right)
	{
		return !left.Equals(right);
	}

	// Token: 0x04000939 RID: 2361
	public readonly string category;

	// Token: 0x0400093A RID: 2362
	public readonly CreditsListMember[] members;
}
