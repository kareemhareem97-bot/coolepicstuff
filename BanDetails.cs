using System;

// Token: 0x02000104 RID: 260
public class BanDetails
{
	// Token: 0x060004E2 RID: 1250 RVA: 0x000123E8 File Offset: 0x000105E8
	public override string ToString()
	{
		return string.Concat(new string[]
		{
			this.OriginalName.Replace(";", ":"),
			";",
			this.Id.Replace(";", ":"),
			";",
			Convert.ToString(this.Expires),
			";",
			this.Reason.Replace(";", ":"),
			";",
			this.Issuer.Replace(";", ":"),
			";",
			Convert.ToString(this.IssuanceTime)
		});
	}

	// Token: 0x04000439 RID: 1081
	public string OriginalName;

	// Token: 0x0400043A RID: 1082
	public string Id;

	// Token: 0x0400043B RID: 1083
	public long Expires;

	// Token: 0x0400043C RID: 1084
	public string Reason;

	// Token: 0x0400043D RID: 1085
	public string Issuer;

	// Token: 0x0400043E RID: 1086
	public long IssuanceTime;
}
