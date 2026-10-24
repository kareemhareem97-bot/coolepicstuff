using System;
using Utf8Json;

// Token: 0x020001FF RID: 511
public readonly struct CreditsList : IEquatable<CreditsList>, IJsonSerializable
{
	// Token: 0x06000AB0 RID: 2736 RVA: 0x00029EFE File Offset: 0x000280FE
	[SerializationConstructor]
	public CreditsList(CreditsListCategory[] credits)
	{
		this.credits = credits;
	}

	// Token: 0x06000AB1 RID: 2737 RVA: 0x00029F07 File Offset: 0x00028107
	public bool Equals(CreditsList other)
	{
		return this.credits == other.credits;
	}

	// Token: 0x06000AB2 RID: 2738 RVA: 0x00029F18 File Offset: 0x00028118
	public override bool Equals(object obj)
	{
		if (obj is CreditsList)
		{
			CreditsList other = (CreditsList)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000AB3 RID: 2739 RVA: 0x00029F3D File Offset: 0x0002813D
	public override int GetHashCode()
	{
		if (this.credits == null)
		{
			return 0;
		}
		return this.credits.GetHashCode();
	}

	// Token: 0x06000AB4 RID: 2740 RVA: 0x00029F54 File Offset: 0x00028154
	public static bool operator ==(CreditsList left, CreditsList right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000AB5 RID: 2741 RVA: 0x00029F5E File Offset: 0x0002815E
	public static bool operator !=(CreditsList left, CreditsList right)
	{
		return !left.Equals(right);
	}

	// Token: 0x04000938 RID: 2360
	public readonly CreditsListCategory[] credits;
}
