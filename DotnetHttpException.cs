using System;

// Token: 0x020001EA RID: 490
public class DotnetHttpException : HttpException
{
	// Token: 0x06000A4F RID: 2639 RVA: 0x00028E16 File Offset: 0x00027016
	public DotnetHttpException(Exception innerException) : base(innerException.Message, innerException)
	{
		HttpQuery.InvokeExceptionHandler(this);
	}
}
