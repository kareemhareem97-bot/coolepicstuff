using System;

// Token: 0x020001EB RID: 491
public abstract class HttpCoroutine : IDisposable
{
	// Token: 0x06000A50 RID: 2640
	public abstract void CheckResult();

	// Token: 0x06000A51 RID: 2641
	public abstract string GetText();

	// Token: 0x06000A52 RID: 2642
	public abstract byte[] GetBytes();

	// Token: 0x06000A53 RID: 2643
	public abstract ReadOnlySpan<byte> GetSpan();

	// Token: 0x06000A54 RID: 2644
	public abstract float Wait();

	// Token: 0x06000A55 RID: 2645
	public abstract void Dispose();
}
