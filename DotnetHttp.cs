using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using MEC;

// Token: 0x020001E2 RID: 482
internal static class DotnetHttp
{
	// Token: 0x06000A2C RID: 2604 RVA: 0x000282A8 File Offset: 0x000264A8
	private static HttpClient CreateClient()
	{
		ValueTuple<SslProtocols, SecurityProtocolType> valueTuple = new ValueTuple<SslProtocols, SecurityProtocolType>(SslProtocols.None, SecurityProtocolType.SystemDefault);
		if (StartupArgs.Args.Any((string arg) => arg == "--tls13"))
		{
			valueTuple.Item1 |= SslProtocols.Tls13;
			valueTuple.Item2 |= SecurityProtocolType.Tls13;
		}
		if (StartupArgs.Args.Any((string arg) => arg == "--tls12"))
		{
			valueTuple.Item1 |= SslProtocols.Tls12;
			valueTuple.Item2 |= SecurityProtocolType.Tls12;
		}
		ValueTuple<SslProtocols, SecurityProtocolType> valueTuple2 = valueTuple;
		if (valueTuple2.Item1 == SslProtocols.None && valueTuple2.Item2 == SecurityProtocolType.SystemDefault)
		{
			valueTuple = new ValueTuple<SslProtocols, SecurityProtocolType>(SslProtocols.Tls12 | SslProtocols.Tls13, SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13);
		}
		if (HttpQuery.WeakSecurity)
		{
			valueTuple = new ValueTuple<SslProtocols, SecurityProtocolType>(SslProtocols.Tls12, SecurityProtocolType.Tls12);
		}
		ServicePointManager.SecurityProtocol = valueTuple.Item2;
		return new HttpClient(new HttpClientHandler
		{
			SslProtocols = valueTuple.Item1,
			AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate)
		})
		{
			Timeout = TimeSpan.FromSeconds(15.0),
			DefaultRequestHeaders = 
			{
				{
					"User-Agent",
					"SCPSL"
				}
			}
		};
	}

	// Token: 0x06000A2D RID: 2605 RVA: 0x000283E0 File Offset: 0x000265E0
	private static Task<byte[]> GetCore(string url)
	{
		DotnetHttp.<GetCore>d__3 <GetCore>d__;
		<GetCore>d__.<>t__builder = AsyncTaskMethodBuilder<byte[]>.Create();
		<GetCore>d__.url = url;
		<GetCore>d__.<>1__state = -1;
		<GetCore>d__.<>t__builder.Start<DotnetHttp.<GetCore>d__3>(ref <GetCore>d__);
		return <GetCore>d__.<>t__builder.Task;
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x00028424 File Offset: 0x00026624
	private static Task<byte[]> PostCore(string url, string data, string contentType)
	{
		DotnetHttp.<PostCore>d__4 <PostCore>d__;
		<PostCore>d__.<>t__builder = AsyncTaskMethodBuilder<byte[]>.Create();
		<PostCore>d__.url = url;
		<PostCore>d__.data = data;
		<PostCore>d__.contentType = contentType;
		<PostCore>d__.<>1__state = -1;
		<PostCore>d__.<>t__builder.Start<DotnetHttp.<PostCore>d__4>(ref <PostCore>d__);
		return <PostCore>d__.<>t__builder.Task;
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x00028478 File Offset: 0x00026678
	public static string Get(string url)
	{
		return DotnetHttp.GetAsync(url).GetAwaiter().GetResult();
	}

	// Token: 0x06000A30 RID: 2608 RVA: 0x00028498 File Offset: 0x00026698
	public static Task<string> GetAsync(string url)
	{
		DotnetHttp.<GetAsync>d__6 <GetAsync>d__;
		<GetAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
		<GetAsync>d__.url = url;
		<GetAsync>d__.<>1__state = -1;
		<GetAsync>d__.<>t__builder.Start<DotnetHttp.<GetAsync>d__6>(ref <GetAsync>d__);
		return <GetAsync>d__.<>t__builder.Task;
	}

	// Token: 0x06000A31 RID: 2609 RVA: 0x000284DC File Offset: 0x000266DC
	public static HttpCoroutine GetCoroutine(string url)
	{
		DotnetHttp.Coroutine coroutine = new DotnetHttp.Coroutine();
		try
		{
			coroutine.Run(DotnetHttp.GetCore(url));
		}
		catch (Exception exception)
		{
			coroutine.SetException(exception);
		}
		return coroutine;
	}

	// Token: 0x06000A32 RID: 2610 RVA: 0x00028518 File Offset: 0x00026718
	public static string Post(string url, string data, string contentType)
	{
		return DotnetHttp.PostAsync(url, data, contentType).GetAwaiter().GetResult();
	}

	// Token: 0x06000A33 RID: 2611 RVA: 0x0002853C File Offset: 0x0002673C
	public static Task<string> PostAsync(string url, string data, string contentType)
	{
		DotnetHttp.<PostAsync>d__9 <PostAsync>d__;
		<PostAsync>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
		<PostAsync>d__.url = url;
		<PostAsync>d__.data = data;
		<PostAsync>d__.contentType = contentType;
		<PostAsync>d__.<>1__state = -1;
		<PostAsync>d__.<>t__builder.Start<DotnetHttp.<PostAsync>d__9>(ref <PostAsync>d__);
		return <PostAsync>d__.<>t__builder.Task;
	}

	// Token: 0x06000A34 RID: 2612 RVA: 0x00028590 File Offset: 0x00026790
	public static HttpCoroutine PostCoroutine(string url, string data, string contentType)
	{
		DotnetHttp.Coroutine coroutine = new DotnetHttp.Coroutine();
		try
		{
			coroutine.Run(DotnetHttp.PostCore(url, data, contentType));
		}
		catch (Exception exception)
		{
			coroutine.SetException(exception);
		}
		return coroutine;
	}

	// Token: 0x040008BE RID: 2238
	private const string UserAgent = "SCPSL";

	// Token: 0x040008BF RID: 2239
	private static readonly HttpClient Client = DotnetHttp.CreateClient();

	// Token: 0x020001E3 RID: 483
	private sealed class Coroutine : HttpCoroutine
	{
		// Token: 0x06000A36 RID: 2614 RVA: 0x000285DC File Offset: 0x000267DC
		internal void Run(Task<byte[]> task)
		{
			DotnetHttp.Coroutine.<Run>d__2 <Run>d__;
			<Run>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Run>d__.<>4__this = this;
			<Run>d__.task = task;
			<Run>d__.<>1__state = -1;
			<Run>d__.<>t__builder.Start<DotnetHttp.Coroutine.<Run>d__2>(ref <Run>d__);
		}

		// Token: 0x06000A37 RID: 2615 RVA: 0x0002861B File Offset: 0x0002681B
		internal void SetException(Exception exception)
		{
			this._exception = exception;
		}

		// Token: 0x06000A38 RID: 2616 RVA: 0x00028624 File Offset: 0x00026824
		public override void CheckResult()
		{
			if (this._exception != null)
			{
				throw this._exception;
			}
		}

		// Token: 0x06000A39 RID: 2617 RVA: 0x00028635 File Offset: 0x00026835
		public override string GetText()
		{
			return Encoding.UTF8.GetString(this._data);
		}

		// Token: 0x06000A3A RID: 2618 RVA: 0x00028648 File Offset: 0x00026848
		public override byte[] GetBytes()
		{
			return this.GetSpan().ToArray();
		}

		// Token: 0x06000A3B RID: 2619 RVA: 0x00028663 File Offset: 0x00026863
		public override ReadOnlySpan<byte> GetSpan()
		{
			return this._data;
		}

		// Token: 0x06000A3C RID: 2620 RVA: 0x00028670 File Offset: 0x00026870
		public override float Wait()
		{
			if (this._data == null && this._exception == null)
			{
				return this.<Wait>g__WaitAsync|8_0();
			}
			return float.NegativeInfinity;
		}

		// Token: 0x06000A3D RID: 2621 RVA: 0x00002E9A File Offset: 0x0000109A
		public override void Dispose()
		{
		}

		// Token: 0x06000A3F RID: 2623 RVA: 0x00028696 File Offset: 0x00026896
		[CompilerGenerated]
		private float <Wait>g__WaitAsync|8_0()
		{
			return Timing.WaitUntilTrue(() => this._data != null || this._exception != null);
		}

		// Token: 0x040008C0 RID: 2240
		private byte[] _data;

		// Token: 0x040008C1 RID: 2241
		private Exception _exception;
	}
}
