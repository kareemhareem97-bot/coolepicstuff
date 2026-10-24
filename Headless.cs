using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using Windows;

// Token: 0x02000015 RID: 21
public static class Headless
{
	// Token: 0x06000060 RID: 96 RVA: 0x00002EF1 File Offset: 0x000010F1
	public static string GetProfileName()
	{
		if (!Headless.IsHeadless())
		{
			return null;
		}
		Headless.InitializeHeadless();
		return Headless.currentProfile;
	}

	// Token: 0x06000061 RID: 97 RVA: 0x00002F08 File Offset: 0x00001108
	public static bool IsHeadless()
	{
		if (Headless.checkedHeadless)
		{
			return Headless.isHeadless;
		}
		if (File.Exists(Application.dataPath + "/~HeadlessDebug.txt"))
		{
			Headless.debuggingHeadless = true;
			Headless.isHeadless = true;
		}
		else if (Array.IndexOf<string>(Environment.GetCommandLineArgs(), "-batchmode") >= 0)
		{
			Headless.isHeadless = true;
		}
		else if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
		{
			Headless.isHeadless = true;
		}
		Headless.checkedHeadless = true;
		return Headless.isHeadless;
	}

	// Token: 0x06000062 RID: 98 RVA: 0x00002F79 File Offset: 0x00001179
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void OnBeforeSceneLoadRuntimeMethod()
	{
		if (Headless.IsHeadless())
		{
			Headless.InitializeHeadless();
			HeadlessCallbacks.InvokeCallbacks("HeadlessBeforeSceneLoad");
		}
	}

	// Token: 0x06000063 RID: 99 RVA: 0x00002F94 File Offset: 0x00001194
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
	private static void OnAfterSceneLoadRuntimeMethod()
	{
		if (Headless.IsHeadless())
		{
			if (Headless.headlessRuntime.valueCamera)
			{
				GameObject gameObject = GameObject.Find("HeadlessBehaviour");
				if (gameObject == null)
				{
					gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("HeadlessBehaviour"));
				}
				HeadlessBehaviour headlessBehaviour = gameObject.GetComponent<HeadlessBehaviour>();
				if (headlessBehaviour == null)
				{
					headlessBehaviour = gameObject.AddComponent<HeadlessBehaviour>();
				}
				Camera.onPreCull = (Camera.CameraCallback)Delegate.Combine(Camera.onPreCull, new Camera.CameraCallback(headlessBehaviour.GetComponent<HeadlessBehaviour>().NullifyCamera));
			}
			HeadlessCallbacks.InvokeCallbacks("HeadlessAfterSceneLoad");
		}
	}

	// Token: 0x06000064 RID: 100 RVA: 0x00003024 File Offset: 0x00001224
	private static void InitializeHeadless()
	{
		if (Headless.initializedHeadless)
		{
			return;
		}
		Headless.headlessRuntime = (Resources.Load("HeadlessRuntime") as HeadlessRuntime);
		if (Headless.headlessRuntime != null)
		{
			Headless.currentProfile = Headless.headlessRuntime.profileName;
			if (Headless.headlessRuntime.valueConsole && !Application.isEditor)
			{
				HeadlessConsole headlessConsole = new HeadlessConsole();
				headlessConsole.Initialize();
				headlessConsole.SetTitle(Application.productName);
				Application.LogCallback value;
				if ((value = Headless.<>O.<0>__HandleLog) == null)
				{
					value = (Headless.<>O.<0>__HandleLog = new Application.LogCallback(Headless.HandleLog));
				}
				Application.logMessageReceived += value;
			}
			if (Headless.headlessRuntime.valueLimitFramerate)
			{
				Application.targetFrameRate = Headless.headlessRuntime.valueFramerate;
				QualitySettings.vSyncCount = 0;
				Debug.Log("Application target framerate set to " + Headless.headlessRuntime.valueFramerate.ToString());
			}
		}
		Headless.initializedHeadless = true;
		HeadlessCallbacks.InvokeCallbacks("HeadlessBeforeFirstSceneLoad");
	}

	// Token: 0x06000065 RID: 101 RVA: 0x00003103 File Offset: 0x00001303
	private static void HandleLog(string logString, string stackTrace, LogType type)
	{
		Console.WriteLine(logString);
		if (stackTrace.Length > 1)
		{
			Console.WriteLine("in: " + stackTrace);
		}
	}

	// Token: 0x06000066 RID: 102 RVA: 0x00003124 File Offset: 0x00001324
	public static bool IsBuildingHeadless()
	{
		return Headless.buildingHeadless;
	}

	// Token: 0x06000067 RID: 103 RVA: 0x00003130 File Offset: 0x00001330
	public static bool IsDebuggingHeadless()
	{
		return Headless.debuggingHeadless;
	}

	// Token: 0x06000068 RID: 104 RVA: 0x0000313C File Offset: 0x0000133C
	public static void SetBuildingHeadless(bool value, string profileName)
	{
		Headless.buildingHeadless = value;
		Headless.currentProfile = profileName;
	}

	// Token: 0x04000027 RID: 39
	public static readonly string version = "1.6.4";

	// Token: 0x04000028 RID: 40
	private static bool isHeadless = false;

	// Token: 0x04000029 RID: 41
	private static bool checkedHeadless = false;

	// Token: 0x0400002A RID: 42
	private static bool initializedHeadless = false;

	// Token: 0x0400002B RID: 43
	private static bool buildingHeadless = false;

	// Token: 0x0400002C RID: 44
	private static bool debuggingHeadless = false;

	// Token: 0x0400002D RID: 45
	private static HeadlessRuntime headlessRuntime;

	// Token: 0x0400002E RID: 46
	private static string currentProfile = "";

	// Token: 0x02000016 RID: 22
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x0400002F RID: 47
		public static Application.LogCallback <0>__HandleLog;
	}
}
