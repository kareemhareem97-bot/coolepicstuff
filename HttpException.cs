using System;

// Token: 0x020001EC RID: 492
public abstract class HttpException : Exception
{
	// Token: 0x06000A57 RID: 2647 RVA: 0x00028E2B File Offset: 0x0002702B
	protected HttpException(string message) : base(message)
	{
		HttpQuery.InvokeExceptionHandler(this);
	}

	// Token: 0x06000A58 RID: 2648 RVA: 0x00028E3A File Offset: 0x0002703A
	protected HttpException(string message, Exception innerException) : base(message, innerException)
	{
		HttpQuery.InvokeExceptionHandler(this);
	}
}
