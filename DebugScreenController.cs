using System;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using CommandSystem.Commands.Shared;
using GameCore;
using NorthwoodLib;
using UnityEngine;
using UserSettings;
using UserSettings.UserInterfaceSettings;

// Token: 0x020000B8 RID: 184
public class DebugScreenController : MonoBehaviour
{
	// Token: 0x060003AC RID: 940 RVA: 0x0000E95C File Offset: 0x0000CB5C
	private void Awake()
	{
		Directory.SetCurrentDirectory(Path.GetDirectoryName(Path.GetFullPath(Application.dataPath)));
		if (!Environment.GetCommandLineArgs().Contains("-nographics"))
		{
			Shutdown.Quit(true, false);
		}
	}

	// Token: 0x060003AD RID: 941 RVA: 0x0000E98A File Offset: 0x0000CB8A
	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		Application.LogCallback value;
		if ((value = DebugScreenController.<>O.<0>__LogMessage) == null)
		{
			value = (DebugScreenController.<>O.<0>__LogMessage = new Application.LogCallback(DebugScreenController.LogMessage));
		}
		Application.logMessageReceivedThreaded += value;
		DebugScreenController.Log();
	}

	// Token: 0x060003AE RID: 942 RVA: 0x0000E9BC File Offset: 0x0000CBBC
	private static void Log()
	{
		if (DebugScreenController._logged)
		{
			return;
		}
		Debug.Log(string.Concat(new string[]
		{
			"Time: ",
			TimeBehaviour.Rfc3339Time(),
			"\nGPU: ",
			SystemInfo.graphicsDeviceName,
			"\nGPU Driver version: ",
			GpuDriver.DriverVersion,
			"\nVRAM: ",
			SystemInfo.graphicsMemorySize.ToString(),
			"MB\nShaderLevel: ",
			SystemInfo.graphicsShaderLevel.ToString().Insert(1, "."),
			"\nVendor: ",
			SystemInfo.graphicsDeviceVendor,
			"\nAPI: ",
			SystemInfo.graphicsDeviceType.ToString(),
			"\nInfo: ",
			SystemInfo.graphicsDeviceVersion,
			"\nResolution: ",
			Screen.width.ToString(),
			"x",
			Screen.height.ToString(),
			"\nFPS Limit: ",
			Application.targetFrameRate.ToString(),
			"\nFullscreen: ",
			Screen.fullScreenMode.ToString(),
			"\nCPU: ",
			SystemInfo.processorType,
			"\nThreads: ",
			SystemInfo.processorCount.ToString(),
			"\nFrequency: ",
			SystemInfo.processorFrequency.ToString(),
			"MHz\nRAM: ",
			SystemInfo.systemMemorySize.ToString(),
			"MB\nAudio Supported: ",
			SystemInfo.supportsAudio.ToString(),
			"\nOS: ",
			NorthwoodLib.OperatingSystem.VersionString,
			"\nUnity: ",
			Application.unityVersion,
			"\nFramework: ",
			Misc.GetRuntimeVersion(),
			"\nIL2CPP: ",
			PlatformInfo.singleton.IsIl2Cpp.ToString(),
			"\nVersion: ",
			GameCore.Version.VersionString,
			"\nBuild: ",
			Application.buildGUID,
			"\nSystem Language: ",
			CultureInfo.CurrentCulture.EnglishName,
			" (",
			CultureInfo.CurrentCulture.Name,
			")\nGame Language: ",
			UserSetting<string>.Get<UISetting>(UISetting.Language, "en", false),
			"\nLaunch arguments: ",
			Environment.CommandLine
		}));
		Debug.Log(BuildInfoCommand.BuildInfoString);
		if (WindowsUpdateWarning.UpdateRequired())
		{
			Debug.LogError("Important system file that is needed for voicechat is missing, please install this windows update in order to get your voicechat working https://support.microsoft.com/en-us/help/2999226/update-for-universal-c-runtime-in-windows");
		}
	}

	// Token: 0x060003AF RID: 943 RVA: 0x0000EC67 File Offset: 0x0000CE67
	private static void LogMessage(string condition, string stackTrace, LogType type)
	{
		switch (type)
		{
		case LogType.Error:
			Interlocked.Increment(ref DebugScreenController.Errors);
			return;
		case LogType.Assert:
			Interlocked.Increment(ref DebugScreenController.Asserts);
			return;
		case LogType.Warning:
		case LogType.Log:
			break;
		case LogType.Exception:
			Interlocked.Increment(ref DebugScreenController.Exceptions);
			break;
		default:
			return;
		}
	}

	// Token: 0x04000346 RID: 838
	public static int Asserts;

	// Token: 0x04000347 RID: 839
	public static int Errors;

	// Token: 0x04000348 RID: 840
	public static int Exceptions;

	// Token: 0x04000349 RID: 841
	private static bool _logged;

	// Token: 0x020000B9 RID: 185
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x0400034A RID: 842
		public static Application.LogCallback <0>__LogMessage;
	}
}
