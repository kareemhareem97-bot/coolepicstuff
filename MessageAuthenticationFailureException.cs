using System;

// Token: 0x02000085 RID: 133
public class MessageAuthenticationFailureException : Exception
{
	// Token: 0x060002CB RID: 715 RVA: 0x0000AB12 File Offset: 0x00008D12
	public MessageAuthenticationFailureException()
	{
	}

	// Token: 0x060002CC RID: 716 RVA: 0x0000AB1A File Offset: 0x00008D1A
	public MessageAuthenticationFailureException(string message) : base(message)
	{
	}

	// Token: 0x060002CD RID: 717 RVA: 0x0000AB23 File Offset: 0x00008D23
	public MessageAuthenticationFailureException(string message, Exception inner) : base(message, inner)
	{
	}
}
