using System;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// Token: 0x020001A7 RID: 423
public static class MemoryCleaner
{
	// Token: 0x06000853 RID: 2131 RVA: 0x0002133C File Offset: 0x0001F53C
	[RuntimeInitializeOnLoadMethod]
	private static void OnLoad()
	{
		UnityAction<Scene, LoadSceneMode> value;
		if ((value = MemoryCleaner.<>O.<0>__CleanupMemory) == null)
		{
			value = (MemoryCleaner.<>O.<0>__CleanupMemory = new UnityAction<Scene, LoadSceneMode>(MemoryCleaner.CleanupMemory));
		}
		SceneManager.sceneLoaded += value;
	}

	// Token: 0x06000854 RID: 2132 RVA: 0x0002135E File Offset: 0x0001F55E
	private static void CleanupMemory(Scene scene, LoadSceneMode mode)
	{
		Resources.UnloadUnusedAssets();
		new Thread(delegate()
		{
			GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		})
		{
			IsBackground = true
		}.Start();
	}

	// Token: 0x020001A8 RID: 424
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x04000792 RID: 1938
		public static UnityAction<Scene, LoadSceneMode> <0>__CleanupMemory;
	}
}
