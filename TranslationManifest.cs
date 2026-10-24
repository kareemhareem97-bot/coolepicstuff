using System;
using Utf8Json;

// Token: 0x02000212 RID: 530
public readonly struct TranslationManifest : IEquatable<TranslationManifest>, IJsonSerializable
{
	// Token: 0x06000B19 RID: 2841 RVA: 0x0002B304 File Offset: 0x00029504
	[SerializationConstructor]
	public TranslationManifest(string name, string[] authors, string[] interfaceLocales, string[] systemLocales, string[] forcedFontOrder)
	{
		this.Name = name;
		this.Authors = authors;
		this.InterfaceLocales = interfaceLocales;
		this.SystemLocales = systemLocales;
		this.ForcedFontOrder = forcedFontOrder;
	}

	// Token: 0x06000B1A RID: 2842 RVA: 0x0002B32C File Offset: 0x0002952C
	public bool Equals(TranslationManifest other)
	{
		return string.Equals(this.Name, other.Name, StringComparison.OrdinalIgnoreCase) && this.Authors == other.Authors && this.InterfaceLocales == other.InterfaceLocales && this.SystemLocales == other.SystemLocales && this.ForcedFontOrder == other.ForcedFontOrder;
	}

	// Token: 0x06000B1B RID: 2843 RVA: 0x0002B388 File Offset: 0x00029588
	public override bool Equals(object obj)
	{
		if (obj is TranslationManifest)
		{
			TranslationManifest other = (TranslationManifest)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06000B1C RID: 2844 RVA: 0x0002B3B0 File Offset: 0x000295B0
	public override int GetHashCode()
	{
		return (((((this.Name != null) ? StringComparer.OrdinalIgnoreCase.GetHashCode(this.Name) : 0) * 397 ^ ((this.Authors != null) ? this.Authors.GetHashCode() : 0)) * 397 ^ ((this.InterfaceLocales != null) ? this.InterfaceLocales.GetHashCode() : 0)) * 397 ^ ((this.SystemLocales != null) ? this.SystemLocales.GetHashCode() : 0)) * 397 ^ ((this.ForcedFontOrder != null) ? this.ForcedFontOrder.GetHashCode() : 0);
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x0002B44C File Offset: 0x0002964C
	public static bool operator ==(TranslationManifest left, TranslationManifest right)
	{
		return left.Equals(right);
	}

	// Token: 0x06000B1E RID: 2846 RVA: 0x0002B456 File Offset: 0x00029656
	public static bool operator !=(TranslationManifest left, TranslationManifest right)
	{
		return !left.Equals(right);
	}

	// Token: 0x0400098C RID: 2444
	public readonly string Name;

	// Token: 0x0400098D RID: 2445
	public readonly string[] Authors;

	// Token: 0x0400098E RID: 2446
	public readonly string[] InterfaceLocales;

	// Token: 0x0400098F RID: 2447
	public readonly string[] SystemLocales;

	// Token: 0x04000990 RID: 2448
	public readonly string[] ForcedFontOrder;
}
