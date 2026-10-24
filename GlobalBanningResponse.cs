using System;
using Utf8Json;

// Token: 0x02000205 RID: 517
public struct GlobalBanningResponse : IEquatable<GlobalBanningResponse>, IJsonSerializable
{
	// Token: 0x06000AD4 RID: 2772 RVA: 0x0002A438 File Offset: 0x00028638
	[SerializationConstructor]
	public GlobalBanningResponse(bool success, string message)
	{
		this.Success = success;
		this.Message = message;
	}

	// Token: 0x06000AD5 RID: 2773 RVA: 0x0002A448 File Offset: 0x00028648
	public bool Equals(GlobalBanningResponse other)
	{
		return this.Success == other.Success && this.Message == other.Message;
	}

	// Token: 0x06000AD6 RID: 2774 RVA: 0x0002A46C File Offset: 0x0002866C
	public override bool Equals(object obj)
	{
		if (obj is GlobalBanningResponse)
		{
			GlobalBanningResponse other = (GlobalBanningResponse)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000AD7 RID: 2775 RVA: 0x0002A491 File Offset: 0x00028691
	public override int GetHashCode()
	{
		return HashCode.Combine<bool, string>(this.Success, this.Message);
	}

	// Token: 0x0400094B RID: 2379
	public readonly bool Success;

	// Token: 0x0400094C RID: 2380
	public readonly string Message;
}
