using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Mirror;
using ServerOutput;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// Token: 0x020001BD RID: 445
public class IdleMode : MonoBehaviour
{
	// Token: 0x17000158 RID: 344
	// (get) Token: 0x060008C2 RID: 2242 RVA: 0x00022C8E File Offset: 0x00020E8E
	// (set) Token: 0x060008C3 RID: 2243 RVA: 0x00022C95 File Offset: 0x00020E95
	public static bool IdleModeActive { get; private set; }

	// Token: 0x17000159 RID: 345
	// (get) Token: 0x060008C4 RID: 2244 RVA: 0x00022C9D File Offset: 0x00020E9D
	// (set) Token: 0x060008C5 RID: 2245 RVA: 0x00022CA4 File Offset: 0x00020EA4
	public static bool PauseIdleMode
	{
		get
		{
			return IdleMode._pauseIdleMode;
		}
		set
		{
			if (IdleMode._pauseIdleMode == value)
			{
				return;
			}
			IdleMode._pauseIdleMode = value;
			if (value)
			{
				IdleMode.SetIdleMode(false);
				IdleMode.St.Reset();
				IdleMode.PreauthStopwatch.Reset();
				if (!IdleMode._idleModeEnabled)
				{
					return;
				}
				ServerConsole.AddLog("Idle mode is now temporarily blocked.", ConsoleColor.Gray, false);
				return;
			}
			else
			{
				if (!IdleMode._idleModeEnabled)
				{
					return;
				}
				ServerConsole.AddLog("Idle mode is now available.", ConsoleColor.Gray, false);
				IdleMode.St.Restart();
				IdleMode.PreauthStopwatch.Restart();
				return;
			}
		}
	}

	// Token: 0x1700015A RID: 346
	// (get) Token: 0x060008C6 RID: 2246 RVA: 0x00022D1A File Offset: 0x00020F1A
	// (set) Token: 0x060008C7 RID: 2247 RVA: 0x00022D21 File Offset: 0x00020F21
	public static bool IdleModeEnabled
	{
		get
		{
			return IdleMode._idleModeEnabled;
		}
		set
		{
			IdleMode._idleModeEnabled = value;
			if (!IdleMode._idleModeEnabled && IdleMode.IdleModeActive)
			{
				IdleMode.SetIdleMode(false);
			}
		}
	}

	// Token: 0x1700015B RID: 347
	// (get) Token: 0x060008C8 RID: 2248 RVA: 0x00022D3D File Offset: 0x00020F3D
	// (set) Token: 0x060008C9 RID: 2249 RVA: 0x00022D44 File Offset: 0x00020F44
	internal static short IdleModeTickrate
	{
		get
		{
			return IdleMode._idleModeTickrate;
		}
		set
		{
			IdleMode._idleModeTickrate = ((value < -1 || value == 0) ? 1 : value);
			if (IdleMode.IdleModeActive)
			{
				IdleMode.SetIdleMode(true, true);
			}
		}
	}

	// Token: 0x060008CA RID: 2250 RVA: 0x00022D64 File Offset: 0x00020F64
	private void Start()
	{
		IdleMode._fixedTimeStep = Time.fixedDeltaTime;
		UnityAction<Scene, LoadSceneMode> value;
		if ((value = IdleMode.<>O.<0>__OnSceneLoaded) == null)
		{
			value = (IdleMode.<>O.<0>__OnSceneLoaded = new UnityAction<Scene, LoadSceneMode>(IdleMode.OnSceneLoaded));
		}
		SceneManager.sceneLoaded += value;
		ReferenceHub.OnPlayerAdded += delegate(ReferenceHub rh)
		{
			IdleMode.SetIdleMode(false);
		};
		ReferenceHub.OnPlayerRemoved += delegate(ReferenceHub rh)
		{
			if (ReferenceHub.AllHubs.Count <= 1)
			{
				IdleMode.SetIdleMode(true);
			}
		};
	}

	// Token: 0x060008CB RID: 2251 RVA: 0x00022DE4 File Offset: 0x00020FE4
	private void FixedUpdate()
	{
		if (IdleMode.St.ElapsedMilliseconds < (long)((ulong)IdleMode.IdleModeTime) || IdleMode._pauseIdleMode || (IdleMode.PreauthStopwatch.IsRunning && IdleMode.PreauthStopwatch.ElapsedMilliseconds < (long)((ulong)IdleMode.IdleModePreauthTime)))
		{
			return;
		}
		IdleMode.St.Reset();
		IdleMode.PreauthStopwatch.Reset();
		if (ReferenceHub.AllHubs.Count <= 1)
		{
			IdleMode.SetIdleMode(true);
		}
	}

	// Token: 0x060008CC RID: 2252 RVA: 0x00022E50 File Offset: 0x00021050
	private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		IdleMode.St.Reset();
		if (ServerStatic.IsDedicated && !IdleMode._pauseIdleMode && IdleMode.IdleModeEnabled && scene.name == "Facility")
		{
			IdleMode.St.Start();
		}
	}

	// Token: 0x060008CD RID: 2253 RVA: 0x00022E8E File Offset: 0x0002108E
	public static void SetIdleMode(bool state)
	{
		IdleMode.SetIdleMode(state, false);
	}

	// Token: 0x060008CE RID: 2254 RVA: 0x00022E98 File Offset: 0x00021098
	private static void SetIdleMode(bool state, bool force)
	{
		if (!NetworkServer.active || (state && IdleMode._pauseIdleMode && !force) || (state == IdleMode.IdleModeActive && !force) || (state && !IdleMode.IdleModeEnabled) || !ServerStatic.IsDedicated)
		{
			return;
		}
		if (state)
		{
			Application.targetFrameRate = (int)IdleMode.IdleModeTickrate;
			Time.timeScale = 0.01f;
			ServerConsole.AddLog("Server has entered the idle mode.", ConsoleColor.Gray, false);
			ServerConsole.AddOutputEntry(default(IdleEnterEntry));
		}
		else
		{
			Application.targetFrameRate = (int)ServerStatic.ServerTickrate;
			Time.timeScale = 1f;
			ServerConsole.AddLog("Server has exited the idle mode.", ConsoleColor.Gray, false);
			ServerConsole.AddOutputEntry(default(IdleExitEntry));
		}
		Time.fixedDeltaTime = IdleMode._fixedTimeStep * Time.timeScale;
		IdleMode.IdleModeActive = state;
	}

	// Token: 0x04000800 RID: 2048
	public static uint IdleModeTime;

	// Token: 0x04000801 RID: 2049
	public static uint IdleModePreauthTime;

	// Token: 0x04000802 RID: 2050
	internal static readonly Stopwatch PreauthStopwatch = new Stopwatch();

	// Token: 0x04000803 RID: 2051
	private static readonly Stopwatch St = new Stopwatch();

	// Token: 0x04000804 RID: 2052
	private static float _fixedTimeStep;

	// Token: 0x04000805 RID: 2053
	private static bool _idleModeEnabled = true;

	// Token: 0x04000806 RID: 2054
	private static bool _pauseIdleMode = true;

	// Token: 0x04000807 RID: 2055
	private static short _idleModeTickrate = 1;

	// Token: 0x04000808 RID: 2056
	private static object _idleModeLock = new object();

	// Token: 0x020001BE RID: 446
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x04000809 RID: 2057
		public static UnityAction<Scene, LoadSceneMode> <0>__OnSceneLoaded;
	}
}
