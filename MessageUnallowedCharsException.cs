using System;

// Token: 0x02000087 RID: 135
public class MessageUnallowedCharsException : Exception
{
	// Token: 0x060002D1 RID: 721 RVA: 0x0000AB12 File Offset: 0x00008D12
	public MessageUnallowedCharsException()
	{
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x0000AB1A File Offset: 0x00008D1A
	public MessageUnallowedCharsException(string message) : base(message)
	{
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x0000AB23 File Offset: 0x00008D23
	public MessageUnallowedCharsException(string message, Exception inner) : base(message, inner)
	{
	}
}
