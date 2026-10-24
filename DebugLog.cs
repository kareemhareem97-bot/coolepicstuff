using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

// Token: 0x020001D9 RID: 473
public static class DebugLog
{
	// Token: 0x060009E6 RID: 2534 RVA: 0x00027709 File Offset: 0x00025909
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Log(string text)
	{
		UnityEngine.Debug.Log(text);
	}

	// Token: 0x060009E7 RID: 2535 RVA: 0x00027709 File Offset: 0x00025909
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void Log(object text)
	{
		UnityEngine.Debug.Log(text);
	}

	// Token: 0x060009E8 RID: 2536 RVA: 0x00027711 File Offset: 0x00025911
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogWarning(string text)
	{
		UnityEngine.Debug.LogWarning(text);
	}

	// Token: 0x060009E9 RID: 2537 RVA: 0x00027711 File Offset: 0x00025911
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogWarning(object text)
	{
		UnityEngine.Debug.LogWarning(text);
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x00027719 File Offset: 0x00025919
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogError(string text)
	{
		UnityEngine.Debug.LogError(text);
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x00027719 File Offset: 0x00025919
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogError(object text)
	{
		UnityEngine.Debug.LogError(text);
	}

	// Token: 0x060009EC RID: 2540 RVA: 0x00027721 File Offset: 0x00025921
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogException(Exception exception)
	{
		UnityEngine.Debug.LogException(exception);
	}

	// Token: 0x060009ED RID: 2541 RVA: 0x00027709 File Offset: 0x00025909
	[Conditional("UNITY_EDITOR")]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogEditor(string text)
	{
		UnityEngine.Debug.Log(text);
	}

	// Token: 0x060009EE RID: 2542 RVA: 0x00027709 File Offset: 0x00025909
	[Conditional("UNITY_EDITOR")]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogEditor(object text)
	{
		UnityEngine.Debug.Log(text);
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x00027711 File Offset: 0x00025911
	[Conditional("UNITY_EDITOR")]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogWarningEditor(string text)
	{
		UnityEngine.Debug.LogWarning(text);
	}

	// Token: 0x060009F0 RID: 2544 RVA: 0x00027711 File Offset: 0x00025911
	[Conditional("UNITY_EDITOR")]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogWarningEditor(object text)
	{
		UnityEngine.Debug.LogWarning(text);
	}

	// Token: 0x060009F1 RID: 2545 RVA: 0x00027719 File Offset: 0x00025919
	[Conditional("UNITY_EDITOR")]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogErrorEditor(string text)
	{
		UnityEngine.Debug.LogError(text);
	}

	// Token: 0x060009F2 RID: 2546 RVA: 0x00027719 File Offset: 0x00025919
	[Conditional("UNITY_EDITOR")]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogErrorEditor(object text)
	{
		UnityEngine.Debug.LogError(text);
	}

	// Token: 0x060009F3 RID: 2547 RVA: 0x00027721 File Offset: 0x00025921
	[Conditional("UNITY_EDITOR")]
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogExceptionEditor(Exception exception)
	{
		UnityEngine.Debug.LogException(exception);
	}

	// Token: 0x060009F4 RID: 2548 RVA: 0x00027709 File Offset: 0x00025909
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogBuild(string text)
	{
		UnityEngine.Debug.Log(text);
	}

	// Token: 0x060009F5 RID: 2549 RVA: 0x00027709 File Offset: 0x00025909
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogBuild(object text)
	{
		UnityEngine.Debug.Log(text);
	}

	// Token: 0x060009F6 RID: 2550 RVA: 0x00027711 File Offset: 0x00025911
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogWarningBuild(string text)
	{
		UnityEngine.Debug.LogWarning(text);
	}

	// Token: 0x060009F7 RID: 2551 RVA: 0x00027711 File Offset: 0x00025911
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogWarningBuild(object text)
	{
		UnityEngine.Debug.LogWarning(text);
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x00027719 File Offset: 0x00025919
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogErrorBuild(string text)
	{
		UnityEngine.Debug.LogError(text);
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x00027719 File Offset: 0x00025919
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogErrorBuild(object text)
	{
		UnityEngine.Debug.LogError(text);
	}

	// Token: 0x060009FA RID: 2554 RVA: 0x00027721 File Offset: 0x00025921
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void LogExceptionBuild(Exception exception)
	{
		UnityEngine.Debug.LogException(exception);
	}
}
