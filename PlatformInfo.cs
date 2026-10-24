using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using GameCore;
using NorthwoodLib;
using NorthwoodLib.Logging;
using UnityEngine;

// Token: 0x02000092 RID: 146
public class PlatformInfo : MonoBehaviour
{
	// Token: 0x1700008F RID: 143
	// (get) Token: 0x060002F1 RID: 753 RVA: 0x0000B68A File Offset: 0x0000988A
	public bool IsHeadless { get; } = 1;

	// Token: 0x17000090 RID: 144
	// (get) Token: 0x060002F2 RID: 754 RVA: 0x0000B692 File Offset: 0x00009892
	public bool IsIl2Cpp { get; }

	// Token: 0x17000091 RID: 145
	// (get) Token: 0x060002F3 RID: 755 RVA: 0x0000B69A File Offset: 0x0000989A
	public bool IsEditor { get; }

	// Token: 0x17000092 RID: 146
	// (get) Token: 0x060002F4 RID: 756 RVA: 0x0000B6A2 File Offset: 0x000098A2
	public bool UsesCustomLauncher { get; }

	// Token: 0x17000093 RID: 147
	// (get) Token: 0x060002F5 RID: 757 RVA: 0x0000B6AA File Offset: 0x000098AA
	// (set) Token: 0x060002F6 RID: 758 RVA: 0x0000B6B2 File Offset: 0x000098B2
	public string BuildGuid { get; private set; }

	// Token: 0x17000094 RID: 148
	// (get) Token: 0x060002F7 RID: 759 RVA: 0x0000B6BB File Offset: 0x000098BB
	public bool IsWindows { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

	// Token: 0x17000095 RID: 149
	// (get) Token: 0x060002F8 RID: 760 RVA: 0x0000B6C3 File Offset: 0x000098C3
	public bool IsLinux { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

	// Token: 0x17000096 RID: 150
	// (get) Token: 0x060002F9 RID: 761 RVA: 0x0000B6CB File Offset: 0x000098CB
	// (set) Token: 0x060002FA RID: 762 RVA: 0x0000B6D3 File Offset: 0x000098D3
	public int MainThreadId { get; private set; }

	// Token: 0x17000097 RID: 151
	// (get) Token: 0x060002FB RID: 763 RVA: 0x0000B6DC File Offset: 0x000098DC
	public bool IsMainThread
	{
		get
		{
			return Thread.CurrentThread.ManagedThreadId == this.MainThreadId;
		}
	}

	// Token: 0x060002FC RID: 764 RVA: 0x0000B6F0 File Offset: 0x000098F0
	private void Awake()
	{
		this.MainThreadId = Thread.CurrentThread.ManagedThreadId;
		this.BuildGuid = Application.buildGUID;
		PlatformInfo.singleton = this;
		GameCore.Console.AddLog("Loaded NorthwoodLib " + PlatformSettings.Version, Color.green, false, ConsoleLogType.Log);
		Action<string, NorthwoodLib.Logging.LogType> value;
		if ((value = PlatformInfo.<>O.<0>__OnLogged) == null)
		{
			value = (PlatformInfo.<>O.<0>__OnLogged = new Action<string, NorthwoodLib.Logging.LogType>(PlatformInfo.OnLogged));
		}
		PlatformSettings.Logged += value;
	}

	// Token: 0x060002FD RID: 765 RVA: 0x0000B75C File Offset: 0x0000995C
	private static void OnLogged(string text, NorthwoodLib.Logging.LogType type)
	{
		switch (type)
		{
		case NorthwoodLib.Logging.LogType.Debug:
			Debug.Log(text);
			return;
		case NorthwoodLib.Logging.LogType.Info:
			GameCore.Console.AddLog(text, Color.blue, false, ConsoleLogType.Log);
			return;
		case NorthwoodLib.Logging.LogType.Warning:
			GameCore.Console.AddLog(text, Color.yellow, false, ConsoleLogType.Log);
			return;
		case NorthwoodLib.Logging.LogType.Error:
			GameCore.Console.AddLog(text, Color.red, false, ConsoleLogType.Log);
			return;
		default:
			Debug.Log(text);
			return;
		}
	}

	// Token: 0x040002AA RID: 682
	public static PlatformInfo singleton;

	// Token: 0x02000093 RID: 147
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x040002B3 RID: 691
		public static Action<string, NorthwoodLib.Logging.LogType> <0>__OnLogged;
	}
}
