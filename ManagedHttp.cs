using System;
using System.Linq;
using System.Threading.Tasks;

// Token: 0x020001EF RID: 495
internal static class ManagedHttp
{
	// Token: 0x06000A6C RID: 2668 RVA: 0x00029098 File Offset: 0x00027298
	public static string Get(string url)
	{
		if (ManagedHttp.Client != ManagedHttp.Mode.HttpClient)
		{
			return UnityHttp.Get(url);
		}
		return DotnetHttp.Get(url);
	}

	// Token: 0x06000A6D RID: 2669 RVA: 0x000290AE File Offset: 0x000272AE
	public static Task<string> GetAsync(string url)
	{
		if (ManagedHttp.Client != ManagedHttp.Mode.HttpClient)
		{
			return UnityHttp.GetAsync(url);
		}
		return DotnetHttp.GetAsync(url);
	}

	// Token: 0x06000A6E RID: 2670 RVA: 0x000290C4 File Offset: 0x000272C4
	public static HttpCoroutine GetCoroutine(string url)
	{
		if (ManagedHttp.Client != ManagedHttp.Mode.HttpClient)
		{
			return UnityHttp.GetCoroutine(url);
		}
		return DotnetHttp.GetCoroutine(url);
	}

	// Token: 0x06000A6F RID: 2671 RVA: 0x000290DA File Offset: 0x000272DA
	public static string Post(string url, string data, string contentType)
	{
		if (ManagedHttp.Client != ManagedHttp.Mode.HttpClient)
		{
			return UnityHttp.Post(url, data, contentType);
		}
		return DotnetHttp.Post(url, data, contentType);
	}

	// Token: 0x06000A70 RID: 2672 RVA: 0x000290F4 File Offset: 0x000272F4
	public static Task<string> PostAsync(string url, string data, string contentType)
	{
		if (ManagedHttp.Client != ManagedHttp.Mode.HttpClient)
		{
			return UnityHttp.PostAsync(url, data, contentType);
		}
		return DotnetHttp.PostAsync(url, data, contentType);
	}

	// Token: 0x06000A71 RID: 2673 RVA: 0x0002910E File Offset: 0x0002730E
	public static HttpCoroutine PostCoroutine(string url, string data, string contentType)
	{
		if (ManagedHttp.Client != ManagedHttp.Mode.HttpClient)
		{
			return UnityHttp.PostCoroutine(url, data, contentType);
		}
		return DotnetHttp.PostCoroutine(url, data, contentType);
	}

	// Token: 0x040008EE RID: 2286
	public const int TimeOut = 15;

	// Token: 0x040008EF RID: 2287
	public static readonly ManagedHttp.Mode Client = StartupArgs.Args.Any((string arg) => arg == "--unity-web-request") ? ManagedHttp.Mode.UnityWebRequest : ManagedHttp.Mode.HttpClient;

	// Token: 0x020001F0 RID: 496
	public enum Mode : byte
	{
		// Token: 0x040008F1 RID: 2289
		HttpClient,
		// Token: 0x040008F2 RID: 2290
		UnityWebRequest
	}
}
