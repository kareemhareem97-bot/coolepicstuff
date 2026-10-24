using System;
using Utf8Json;

// Token: 0x02000207 RID: 519
public readonly struct NewsListItem : IEquatable<NewsListItem>, IJsonSerializable
{
	// Token: 0x06000ADE RID: 2782 RVA: 0x0002A528 File Offset: 0x00028728
	[SerializationConstructor]
	public NewsListItem(string gid, string title, string url, bool is_external_url, string author, string contents, string feedlabel, long date, string feedname, int feedtype)
	{
		this.gid = gid;
		this.title = title;
		this.url = url;
		this.is_external_url = is_external_url;
		this.author = author;
		this.contents = contents;
		this.feedlabel = feedlabel;
		this.date = date;
		this.feedname = feedname;
		this.feedtype = feedtype;
	}

	// Token: 0x06000ADF RID: 2783 RVA: 0x0002A584 File Offset: 0x00028784
	public bool Equals(NewsListItem other)
	{
		return this.gid == other.gid && this.title == other.title && this.url == other.url && this.is_external_url == other.is_external_url && this.author == other.author && this.contents == other.contents && this.feedlabel == other.feedlabel && this.date == other.date && this.feedname == other.feedname && this.feedtype == other.feedtype;
	}

	// Token: 0x06000AE0 RID: 2784 RVA: 0x0002A648 File Offset: 0x00028848
	public override bool Equals(object obj)
	{
		if (obj is NewsListItem)
		{
			NewsListItem other = (NewsListItem)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000AE1 RID: 2785 RVA: 0x0002A66D File Offset: 0x0002886D
	public override int GetHashCode()
	{
		return this.gid.GetHashCode();
	}

	// Token: 0x06000AE2 RID: 2786 RVA: 0x0002A67A File Offset: 0x0002887A
	public static bool operator ==(NewsListItem left, NewsListItem right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000AE3 RID: 2787 RVA: 0x0002A684 File Offset: 0x00028884
	public static bool operator !=(NewsListItem left, NewsListItem right)
	{
		return !left.Equals(right);
	}

	// Token: 0x04000950 RID: 2384
	public readonly string gid;

	// Token: 0x04000951 RID: 2385
	public readonly string title;

	// Token: 0x04000952 RID: 2386
	public readonly string url;

	// Token: 0x04000953 RID: 2387
	public readonly bool is_external_url;

	// Token: 0x04000954 RID: 2388
	public readonly string author;

	// Token: 0x04000955 RID: 2389
	public readonly string contents;

	// Token: 0x04000956 RID: 2390
	public readonly string feedlabel;

	// Token: 0x04000957 RID: 2391
	public readonly long date;

	// Token: 0x04000958 RID: 2392
	public readonly string feedname;

	// Token: 0x04000959 RID: 2393
	public readonly int feedtype;
}
