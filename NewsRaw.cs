using System;
using Utf8Json;

// Token: 0x02000208 RID: 520
public readonly struct NewsRaw : IEquatable<NewsRaw>, IJsonSerializable
{
	// Token: 0x06000AE4 RID: 2788 RVA: 0x0002A691 File Offset: 0x00028891
	[SerializationConstructor]
	public NewsRaw(NewsList appnews)
	{
		this.appnews = appnews;
	}

	// Token: 0x06000AE5 RID: 2789 RVA: 0x0002A69A File Offset: 0x0002889A
	public bool Equals(NewsRaw other)
	{
		return this.appnews == other.appnews;
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x0002A6B0 File Offset: 0x000288B0
	public override bool Equals(object obj)
	{
		if (obj is NewsRaw)
		{
			NewsRaw other = (NewsRaw)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000AE7 RID: 2791 RVA: 0x0002A6D5 File Offset: 0x000288D5
	public override int GetHashCode()
	{
		return this.appnews.GetHashCode();
	}

	// Token: 0x06000AE8 RID: 2792 RVA: 0x0002A6E8 File Offset: 0x000288E8
	public static bool operator ==(NewsRaw left, NewsRaw right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000AE9 RID: 2793 RVA: 0x0002A6F2 File Offset: 0x000288F2
	public static bool operator !=(NewsRaw left, NewsRaw right)
	{
		return !left.Equals(right);
	}

	// Token: 0x0400095A RID: 2394
	public readonly NewsList appnews;
}
