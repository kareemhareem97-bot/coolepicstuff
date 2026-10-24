using System;

// Token: 0x020001DB RID: 475
public static class EnumUtils<T> where T : struct, Enum
{
	// Token: 0x040008A8 RID: 2216
	public static readonly T[] Values = Enum.GetValues(typeof(T)).ToArray<T>();

	// Token: 0x040008A9 RID: 2217
	public static readonly string[] Names = Enum.GetNames(typeof(T));

	// Token: 0x040008AA RID: 2218
	public static readonly Type UnderlyingType = Enum.GetUnderlyingType(typeof(T));

	// Token: 0x040008AB RID: 2219
	public static readonly TypeCode TypeCode = Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T)));
}
