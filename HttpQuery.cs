using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Utils.Http.SDR;

// Token: 0x020001ED RID: 493
public static class HttpQuery
{
	// Token: 0x14000027 RID: 39
	// (add) Token: 0x06000A59 RID: 2649 RVA: 0x00028E4C File Offset: 0x0002704C
	// (remove) Token: 0x06000A5A RID: 2650 RVA: 0x00028E80 File Offset: 0x00027080
	public static event Action<Exception> OnException;

	// Token: 0x06000A5B RID: 2651 RVA: 0x00028EB3 File Offset: 0x000270B3
	private static bool IsWeakSecurity()
	{
		return StartupArgs.Args.Any((string arg) => arg == "--weak-http-security");
	}

	// Token: 0x06000A5C RID: 2652 RVA: 0x00028EDE File Offset: 0x000270DE
	public static string GetInfo()
	{
		return string.Format("HTTP client: {0}\nEncryption: {1}\nWeak Security: {2}", ManagedHttp.Client, ServicePointManager.SecurityProtocol, HttpQuery.WeakSecurity);
	}

	// Token: 0x06000A5D RID: 2653 RVA: 0x00028F08 File Offset: 0x00027108
	public static string Get(string url)
	{
		if (SteamDatagramRelayHandler.ShouldUseSDR(url))
		{
			return SDRHttpQuery.Get(url);
		}
		return ManagedHttp.Get(url);
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x00028F1F File Offset: 0x0002711F
	public static Task<string> GetAsync(string url)
	{
		if (SteamDatagramRelayHandler.ShouldUseSDR(url))
		{
			return SDRHttpQuery.GetAsync(url);
		}
		return ManagedHttp.GetAsync(url);
	}

	// Token: 0x06000A5F RID: 2655 RVA: 0x00028F36 File Offset: 0x00027136
	public static HttpCoroutine GetCoroutine(string url)
	{
		if (!PlatformInfo.singleton.IsMainThread)
		{
			throw new InvalidOperationException("Coroutines are only supported on the main thread!");
		}
		if (SteamDatagramRelayHandler.ShouldUseSDR(url))
		{
			return SDRHttpQuery.GetCoroutine(url);
		}
		return ManagedHttp.GetCoroutine(url);
	}

	// Token: 0x06000A60 RID: 2656 RVA: 0x00028F64 File Offset: 0x00027164
	public static string Post(string url, string data, string contentType)
	{
		if (SteamDatagramRelayHandler.ShouldUseSDR(url))
		{
			return SDRHttpQuery.Post(url, data, contentType);
		}
		return ManagedHttp.Post(url, data, contentType);
	}

	// Token: 0x06000A61 RID: 2657 RVA: 0x00028F7F File Offset: 0x0002717F
	public static Task<string> PostAsync(string url, string data, string contentType)
	{
		if (SteamDatagramRelayHandler.ShouldUseSDR(url))
		{
			return SDRHttpQuery.PostAsync(url, data, contentType);
		}
		return ManagedHttp.PostAsync(url, data, contentType);
	}

	// Token: 0x06000A62 RID: 2658 RVA: 0x00028F9A File Offset: 0x0002719A
	public static HttpCoroutine PostCoroutine(string url, string data, string contentType)
	{
		if (!PlatformInfo.singleton.IsMainThread)
		{
			throw new InvalidOperationException("Coroutines are only supported on the main thread!");
		}
		if (SteamDatagramRelayHandler.ShouldUseSDR(url))
		{
			return SDRHttpQuery.PostCoroutine(url, data, contentType);
		}
		return ManagedHttp.PostCoroutine(url, data, contentType);
	}

	// Token: 0x06000A63 RID: 2659 RVA: 0x00028FCC File Offset: 0x000271CC
	public static string ToPostArgs(IEnumerable<string> data)
	{
		return string.Join<string>('&', from element in data
		select element.Replace("&", "[AMP]"));
	}

	// Token: 0x06000A64 RID: 2660 RVA: 0x00028FFA File Offset: 0x000271FA
	public static string ToPostArgs(IEnumerable<ValueTuple<string, string>> data)
	{
		return string.Join<string>('&', from element in data
		select (element.Item1 + "=" + element.Item2).Replace("&", "[AMP]"));
	}

	// Token: 0x06000A65 RID: 2661 RVA: 0x00029028 File Offset: 0x00027228
	internal static void InvokeExceptionHandler(Exception exception)
	{
		Action<Exception> onException = HttpQuery.OnException;
		if (onException == null)
		{
			return;
		}
		onException(exception);
	}

	// Token: 0x040008E7 RID: 2279
	private const string ThreadMessage = "Coroutines are only supported on the main thread!";

	// Token: 0x040008E9 RID: 2281
	public static readonly bool WeakSecurity = HttpQuery.IsWeakSecurity();
}
