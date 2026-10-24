using System;

// Token: 0x0200017A RID: 378
public static class CentralAuthPreauthUtils
{
	// Token: 0x06000786 RID: 1926 RVA: 0x0001C83C File Offset: 0x0001AA3C
	public static bool HasFlagFast(this CentralAuthPreauthFlags flags, CentralAuthPreauthFlags flag)
	{
		return (flags & flag) == flag;
	}
}
