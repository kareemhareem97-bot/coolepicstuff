using System;
using Utf8Json;

// Token: 0x02000206 RID: 518
public readonly struct NewsList : IEquatable<NewsList>, IJsonSerializable
{
	// Token: 0x06000AD8 RID: 2776 RVA: 0x0002A4A4 File Offset: 0x000286A4
	[SerializationConstructor]
	public NewsList(int appid, NewsListItem[] newsitems, int count)
	{
		this.appid = appid;
		this.newsitems = newsitems;
		this.count = count;
	}

	// Token: 0x06000AD9 RID: 2777 RVA: 0x0002A4BB File Offset: 0x000286BB
	public bool Equals(NewsList other)
	{
		return this.appid == other.appid;
	}

	// Token: 0x06000ADA RID: 2778 RVA: 0x0002A4CC File Offset: 0x000286CC
	public override bool Equals(object obj)
	{
		if (obj is NewsList)
		{
			NewsList other = (NewsList)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000ADB RID: 2779 RVA: 0x0002A4F4 File Offset: 0x000286F4
	public override int GetHashCode()
	{
		return this.appid.GetHashCode();
	}

	// Token: 0x06000ADC RID: 2780 RVA: 0x0002A50F File Offset: 0x0002870F
	public static bool operator ==(NewsList left, NewsList right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000ADD RID: 2781 RVA: 0x0002A519 File Offset: 0x00028719
	public static bool operator !=(NewsList left, NewsList right)
	{
		return !left.Equals(right);
	}

	// Token: 0x0400094D RID: 2381
	public readonly int appid;

	// Token: 0x0400094E RID: 2382
	public readonly NewsListItem[] newsitems;

	// Token: 0x0400094F RID: 2383
	public readonly int count;
}
