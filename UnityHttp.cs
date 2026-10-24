using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MEC;
using Unity.Collections;
using UnityEngine.Networking;

// Token: 0x020001F2 RID: 498
internal static class UnityHttp
{
	// Token: 0x06000A76 RID: 2678 RVA: 0x00029168 File Offset: 0x00027368
	public static string Get(string url)
	{
		if (!PlatformInfo.singleton.IsMainThread)
		{
			return UnityHttp.<Get>g__Dispatch|1_0(url);
		}
		return UnityHttp.<Get>g__Start|1_1(url);
	}

	// Token: 0x06000A77 RID: 2679 RVA: 0x00029184 File Offset: 0x00027384
	public static Task<string> GetAsync(string url)
	{
		TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>();
		if (PlatformInfo.singleton.IsMainThread)
		{
			UnityHttp.<GetAsync>g__Start|2_1(taskCompletionSource, url);
		}
		else
		{
			UnityHttp.<GetAsync>g__Dispatch|2_0(taskCompletionSource, url);
		}
		return taskCompletionSource.Task;
	}

	// Token: 0x06000A78 RID: 2680 RVA: 0x000291B9 File Offset: 0x000273B9
	public static HttpCoroutine GetCoroutine(string url)
	{
		return new UnityHttp.Coroutine(UnityWebRequest.Get(url));
	}

	// Token: 0x06000A79 RID: 2681 RVA: 0x000291C6 File Offset: 0x000273C6
	public static string Post(string url, string data, string contentType)
	{
		if (!PlatformInfo.singleton.IsMainThread)
		{
			return UnityHttp.<Post>g__Dispatch|4_0(url, data, contentType);
		}
		return UnityHttp.<Post>g__Start|4_1(url, data, contentType);
	}

	// Token: 0x06000A7A RID: 2682 RVA: 0x000291E8 File Offset: 0x000273E8
	public static Task<string> PostAsync(string url, string data, string contentType)
	{
		TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>();
		if (PlatformInfo.singleton.IsMainThread)
		{
			UnityHttp.<PostAsync>g__Start|5_1(taskCompletionSource, url, data, contentType);
		}
		else
		{
			UnityHttp.<PostAsync>g__Dispatch|5_0(taskCompletionSource, url, data, contentType);
		}
		return taskCompletionSource.Task;
	}

	// Token: 0x06000A7B RID: 2683 RVA: 0x00029221 File Offset: 0x00027421
	public static HttpCoroutine PostCoroutine(string url, string data, string contentType)
	{
		return new UnityHttp.Coroutine(UnityWebRequest.Post(url, data, contentType));
	}

	// Token: 0x06000A7C RID: 2684 RVA: 0x00029230 File Offset: 0x00027430
	private static IEnumerator<float> Run(TaskCompletionSource<string> taskCompletionSource, UnityWebRequest request)
	{
		using (request)
		{
			request.timeout = 15;
			yield return Timing.WaitUntilDone(request.SendWebRequest());
			try
			{
				if (request.result != UnityWebRequest.Result.Success)
				{
					throw new UnityHttpException(request.error);
				}
				Encoding utf = Encoding.UTF8;
				NativeArray<byte>.ReadOnly nativeData = request.downloadHandler.nativeData;
				taskCompletionSource.SetResult(utf.GetString(nativeData));
			}
			catch (Exception exception)
			{
				taskCompletionSource.SetException(exception);
			}
		}
		UnityWebRequest unityWebRequest = null;
		yield break;
		yield break;
	}

	// Token: 0x06000A7D RID: 2685 RVA: 0x00029248 File Offset: 0x00027448
	[CompilerGenerated]
	internal static string <Get>g__Dispatch|1_0(string url)
	{
		object result = null;
		MainThreadDispatcher.Dispatch(delegate
		{
			try
			{
				Volatile.Write<object>(ref result, UnityHttp.<Get>g__Start|1_1(url));
			}
			catch (Exception value)
			{
				Volatile.Write<object>(ref result, value);
			}
		}, MainThreadDispatcher.DispatchTime.Update);
		while (Volatile.Read<object>(ref result) == null)
		{
			Thread.Sleep(15);
		}
		object obj = Volatile.Read<object>(ref result);
		Exception ex = obj as Exception;
		if (ex == null)
		{
			return (string)obj;
		}
		throw ex;
	}

	// Token: 0x06000A7E RID: 2686 RVA: 0x000292B0 File Offset: 0x000274B0
	[CompilerGenerated]
	internal static string <Get>g__Start|1_1(string url)
	{
		string @string;
		using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(url))
		{
			unityWebRequest.timeout = 15;
			UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = unityWebRequest.SendWebRequest();
			while (!unityWebRequestAsyncOperation.isDone)
			{
				Thread.Sleep(15);
			}
			if (unityWebRequest.result != UnityWebRequest.Result.Success)
			{
				throw new UnityHttpException(unityWebRequest.error);
			}
			Encoding utf = Encoding.UTF8;
			NativeArray<byte>.ReadOnly nativeData = unityWebRequest.downloadHandler.nativeData;
			@string = utf.GetString(nativeData);
		}
		return @string;
	}

	// Token: 0x06000A7F RID: 2687 RVA: 0x00029334 File Offset: 0x00027534
	[CompilerGenerated]
	internal static void <GetAsync>g__Dispatch|2_0(TaskCompletionSource<string> taskCompletionSource, string url)
	{
		MainThreadDispatcher.Dispatch(delegate
		{
			UnityHttp.<GetAsync>g__Start|2_1(taskCompletionSource, url);
		}, MainThreadDispatcher.DispatchTime.Update);
	}

	// Token: 0x06000A80 RID: 2688 RVA: 0x0002935C File Offset: 0x0002755C
	[CompilerGenerated]
	internal static void <GetAsync>g__Start|2_1(TaskCompletionSource<string> taskCompletionSource, string url)
	{
		try
		{
			Timing.RunCoroutine(UnityHttp.Run(taskCompletionSource, UnityWebRequest.Get(url)));
		}
		catch (Exception exception)
		{
			taskCompletionSource.SetException(exception);
		}
	}

	// Token: 0x06000A81 RID: 2689 RVA: 0x00029398 File Offset: 0x00027598
	[CompilerGenerated]
	internal static string <Post>g__Dispatch|4_0(string url, string data, string contentType)
	{
		object result = null;
		MainThreadDispatcher.Dispatch(delegate
		{
			try
			{
				Volatile.Write<object>(ref result, UnityHttp.<Post>g__Start|4_1(url, data, contentType));
			}
			catch (Exception value)
			{
				Volatile.Write<object>(ref result, value);
			}
		}, MainThreadDispatcher.DispatchTime.Update);
		while (Volatile.Read<object>(ref result) == null)
		{
			Thread.Sleep(15);
		}
		object obj = Volatile.Read<object>(ref result);
		Exception ex = obj as Exception;
		if (ex == null)
		{
			return (string)obj;
		}
		throw ex;
	}

	// Token: 0x06000A82 RID: 2690 RVA: 0x00029410 File Offset: 0x00027610
	[CompilerGenerated]
	internal static string <Post>g__Start|4_1(string url, string data, string contentType)
	{
		string @string;
		using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(url, data, contentType))
		{
			unityWebRequest.timeout = 15;
			UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = unityWebRequest.SendWebRequest();
			while (!unityWebRequestAsyncOperation.isDone)
			{
				Thread.Sleep(15);
			}
			if (unityWebRequest.result != UnityWebRequest.Result.Success)
			{
				throw new UnityHttpException(unityWebRequest.error);
			}
			Encoding utf = Encoding.UTF8;
			NativeArray<byte>.ReadOnly nativeData = unityWebRequest.downloadHandler.nativeData;
			@string = utf.GetString(nativeData);
		}
		return @string;
	}

	// Token: 0x06000A83 RID: 2691 RVA: 0x00029498 File Offset: 0x00027698
	[CompilerGenerated]
	internal static void <PostAsync>g__Dispatch|5_0(TaskCompletionSource<string> taskCompletionSource, string url, string data, string contentType)
	{
		MainThreadDispatcher.Dispatch(delegate
		{
			UnityHttp.<PostAsync>g__Start|5_1(taskCompletionSource, url, data, contentType);
		}, MainThreadDispatcher.DispatchTime.Update);
	}

	// Token: 0x06000A84 RID: 2692 RVA: 0x000294CC File Offset: 0x000276CC
	[CompilerGenerated]
	internal static void <PostAsync>g__Start|5_1(TaskCompletionSource<string> taskCompletionSource, string url, string data, string contentType)
	{
		try
		{
			Timing.RunCoroutine(UnityHttp.Run(taskCompletionSource, UnityWebRequest.Post(url, data, contentType)));
		}
		catch (Exception exception)
		{
			taskCompletionSource.SetException(exception);
		}
	}

	// Token: 0x040008F4 RID: 2292
	private const int SleepTime = 15;

	// Token: 0x020001F3 RID: 499
	private sealed class Coroutine : HttpCoroutine
	{
		// Token: 0x06000A85 RID: 2693 RVA: 0x0002950C File Offset: 0x0002770C
		public Coroutine(UnityWebRequest request)
		{
			this._request = request;
			request.timeout = 15;
		}

		// Token: 0x06000A86 RID: 2694 RVA: 0x00029523 File Offset: 0x00027723
		public override void CheckResult()
		{
			if (this._request.result != UnityWebRequest.Result.Success)
			{
				throw new UnityHttpException(this._request.error);
			}
		}

		// Token: 0x06000A87 RID: 2695 RVA: 0x00029544 File Offset: 0x00027744
		public override string GetText()
		{
			return Encoding.UTF8.GetString(this.GetSpan());
		}

		// Token: 0x06000A88 RID: 2696 RVA: 0x00029558 File Offset: 0x00027758
		public override byte[] GetBytes()
		{
			return this.GetSpan().ToArray();
		}

		// Token: 0x06000A89 RID: 2697 RVA: 0x00029574 File Offset: 0x00027774
		public override ReadOnlySpan<byte> GetSpan()
		{
			NativeArray<byte>.ReadOnly nativeData = this._request.downloadHandler.nativeData;
			return nativeData;
		}

		// Token: 0x06000A8A RID: 2698 RVA: 0x00029599 File Offset: 0x00027799
		public override float Wait()
		{
			return Timing.WaitUntilDone(this._request.SendWebRequest());
		}

		// Token: 0x06000A8B RID: 2699 RVA: 0x000295AB File Offset: 0x000277AB
		public override void Dispose()
		{
			this._request.Dispose();
		}

		// Token: 0x040008F5 RID: 2293
		private readonly UnityWebRequest _request;
	}
}
