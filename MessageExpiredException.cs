using System;

// Token: 0x02000086 RID: 134
public class MessageExpiredException : Exception
{
	// Token: 0x060002CE RID: 718 RVA: 0x0000AB12 File Offset: 0x00008D12
	public MessageExpiredException()
	{
	}

	// Token: 0x060002CF RID: 719 RVA: 0x0000AB1A File Offset: 0x00008D1A
	public MessageExpiredException(string message) : base(message)
	{
	}

	// Token: 0x060002D0 RID: 720 RVA: 0x0000AB23 File Offset: 0x00008D23
	public MessageExpiredException(string message, Exception inner) : base(message, inner)
	{
	}
}
