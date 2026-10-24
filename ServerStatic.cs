using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using LabApi.Loader;
using ServerOutput;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x020001CC RID: 460
public class ServerStatic : MonoBehaviour
{
	// Token: 0x17000164 RID: 356
	// (get) Token: 0x0600096D RID: 2413 RVA: 0x00025D74 File Offset: 0x00023F74
	// (set) Token: 0x0600096E RID: 2414 RVA: 0x00025D7B File Offset: 0x00023F7B
	public static bool IsDedicated { get; private set; }

	// Token: 0x17000165 RID: 357
	// (get) Token: 0x0600096F RID: 2415 RVA: 0x00025D83 File Offset: 0x00023F83
	// (set) Token: 0x06000970 RID: 2416 RVA: 0x00025D8A File Offset: 0x00023F8A
	public static ServerStatic.NextRoundAction StopNextRound { get; set; } = ServerStatic.NextRoundAction.DoNothing;

	// Token: 0x17000166 RID: 358
	// (get) Token: 0x06000971 RID: 2417 RVA: 0x00025D92 File Offset: 0x00023F92
	// (set) Token: 0x06000972 RID: 2418 RVA: 0x00025D9C File Offset: 0x00023F9C
	public static short ServerTickrate
	{
		get
		{
			return ServerStatic._serverTickrate;
		}
		set
		{
			if (value < -1 || value == 0)
			{
				ServerStatic._serverTickrate = 60;
			}
			else
			{
				ServerStatic._serverTickrate = value;
			}
			if (!ServerStatic.IsDedicated)
			{
				return;
			}
			Application.targetFrameRate = (int)ServerStatic._serverTickrate;
			ServerConsole.AddLog("Server tickrate set to: " + ServerStatic._serverTickrate.ToString(), ConsoleColor.Gray, false);
		}
	}

	// Token: 0x17000167 RID: 359
	// (get) Token: 0x06000973 RID: 2419 RVA: 0x00025DEC File Offset: 0x00023FEC
	// (set) Token: 0x06000974 RID: 2420 RVA: 0x00025DF3 File Offset: 0x00023FF3
	public static ushort ServerPort { get; private set; }

	// Token: 0x06000975 RID: 2421 RVA: 0x00025DFC File Offset: 0x00023FFC
	private void Awake()
	{
		ServerStatic.ProcessServerArgs();
		if (!ServerStatic.IsDedicated)
		{
			Shutdown.Quit(true, false);
			return;
		}
		if (ServerStatic.ServerOutput == null)
		{
			Shutdown.Quit(true, false);
			return;
		}
		ServerStatic.ServerOutput.Start();
		if (ServerStatic.IsDedicated)
		{
			AudioListener.volume = 0f;
			AudioListener.pause = true;
			QualitySettings.pixelLightCount = 0;
			ServerConsole.AddLog("SCP Secret Laboratory process started. Creating match...", ConsoleColor.Green, false);
			ServerStatic.ServerTickrate = 60;
			if (!ServerStatic._serverPortSet)
			{
				ServerConsole.AddLog("\"-port\" argument is required for dedicated server. Aborting startup.", ConsoleColor.DarkRed, false);
				ServerConsole.AddLog("Make sure you are using latest version of LocalAdmin.", ConsoleColor.DarkRed, false);
				Shutdown.Quit(true, false);
				return;
			}
		}
		PluginLoader.Initialize();
		SceneManager.sceneLoaded += this.OnSceneWasLoaded;
	}

	// Token: 0x06000976 RID: 2422 RVA: 0x00025EA4 File Offset: 0x000240A4
	[RuntimeInitializeOnLoadMethod]
	internal static void ProcessServerArgs()
	{
		if (ServerStatic._serverArgsProcessed)
		{
			return;
		}
		ServerStatic._serverArgsProcessed = true;
		int sendBufferSize = 0;
		int receiveBufferSize = 0;
		int i = 0;
		while (i < StartupArgs.Args.Length)
		{
			string text = StartupArgs.Args[i];
			string text2 = text;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text2);
			if (num <= 944841373U)
			{
				if (num <= 412146683U)
				{
					if (num != 330659931U)
					{
						if (num != 412146683U)
						{
							goto IL_258;
						}
						if (!(text2 == "-stdout"))
						{
							goto IL_258;
						}
						if (ServerStatic._serverPortSet || ServerStatic.ServerOutput != null)
						{
							goto IL_258;
						}
						ServerStatic.ServerOutput = new StandardOutput();
					}
					else
					{
						if (!(text2 == "-keepsession"))
						{
							goto IL_258;
						}
						ServerStatic.KeepSession = true;
					}
				}
				else if (num != 533076168U)
				{
					if (num != 944841373U)
					{
						goto IL_258;
					}
					if (!(text2 == "-configpath"))
					{
						goto IL_258;
					}
					if (i >= StartupArgs.Args.Length - 1)
					{
						goto IL_258;
					}
					FileManager.SetConfigFolder(StartupArgs.Args[i + 1]);
				}
				else
				{
					if (!(text2 == "-heartbeat"))
					{
						goto IL_258;
					}
					ServerStatic.EnableConsoleHeartbeat = true;
				}
			}
			else if (num <= 1273769260U)
			{
				if (num != 972287825U)
				{
					if (num != 1273769260U)
					{
						goto IL_258;
					}
					if (!(text2 == "-appdatapath"))
					{
						goto IL_258;
					}
					if (i >= StartupArgs.Args.Length - 1)
					{
						goto IL_258;
					}
					FileManager.SetAppFolder(StartupArgs.Args[i + 1]);
				}
				else
				{
					if (!(text2 == "-disableconfigvalidation"))
					{
						goto IL_258;
					}
					ServerStatic.DisableConfigValidation = true;
				}
			}
			else if (num != 1486336508U)
			{
				if (num != 2759251362U)
				{
					if (num != 3057086030U)
					{
						goto IL_258;
					}
					if (!(text2 == "-txbuffer"))
					{
						goto IL_258;
					}
					if (i >= StartupArgs.Args.Length - 1)
					{
						goto IL_258;
					}
					if (!int.TryParse(StartupArgs.Args[i + 1], out sendBufferSize))
					{
						goto IL_258;
					}
				}
				else
				{
					if (!(text2 == "-nographics"))
					{
						goto IL_258;
					}
					ServerStatic.IsDedicated = true;
				}
			}
			else
			{
				if (!(text2 == "-rxbuffer"))
				{
					goto IL_258;
				}
				if (i >= StartupArgs.Args.Length - 1)
				{
					goto IL_258;
				}
				if (!int.TryParse(StartupArgs.Args[i + 1], out receiveBufferSize))
				{
					goto IL_258;
				}
			}
			IL_38F:
			i++;
			continue;
			IL_258:
			if (text.StartsWith("-key", StringComparison.Ordinal) && text.Length > 4 && !ServerStatic._serverPortSet && ServerStatic.ServerOutput == null)
			{
				ServerStatic.ServerOutput = new FileConsole(text.Remove(0, 4));
				goto IL_38F;
			}
			if (text.StartsWith("-console", StringComparison.Ordinal) && ServerStatic.ServerOutput == null)
			{
				ushort port;
				if (ushort.TryParse(text.Remove(0, 8), out port))
				{
					ServerStatic.ServerOutput = new TcpConsole(port, receiveBufferSize, sendBufferSize);
					goto IL_38F;
				}
				goto IL_38F;
			}
			else
			{
				if (text.StartsWith("-id", StringComparison.Ordinal) && !ServerStatic.ProcessIdPassed)
				{
					ServerStatic.ProcessIdPassed = true;
					int processId;
					if (int.TryParse(text.Remove(0, 3), out processId))
					{
						ServerConsole.ConsoleProcess = Process.GetProcessById(processId);
					}
					if (ServerConsole.ConsoleProcess == null)
					{
						ServerStatic.OnConsoleExited(null, null);
					}
					ServerConsole.ConsoleProcess.EnableRaisingEvents = true;
					Process consoleProcess = ServerConsole.ConsoleProcess;
					EventHandler value;
					if ((value = ServerStatic.<>O.<0>__OnConsoleExited) == null)
					{
						value = (ServerStatic.<>O.<0>__OnConsoleExited = new EventHandler(ServerStatic.OnConsoleExited));
					}
					consoleProcess.Exited += value;
					goto IL_38F;
				}
				if (!text.StartsWith("-port", StringComparison.Ordinal) || ServerStatic._serverPortSet)
				{
					goto IL_38F;
				}
				ushort serverPort;
				if (!ushort.TryParse(text.Remove(0, 5), out serverPort))
				{
					ServerConsole.AddLog("\"-port\" argument value is not a valid unsigned short integer (0 - 65535). Aborting startup.", ConsoleColor.Gray, false);
					Shutdown.Quit(true, false);
					return;
				}
				ServerStatic.ServerPort = serverPort;
				ServerStatic._serverPortSet = true;
				goto IL_38F;
			}
		}
	}

	// Token: 0x06000977 RID: 2423 RVA: 0x00026251 File Offset: 0x00024451
	private static void OnConsoleExited(object sender, EventArgs e)
	{
		ServerConsole.DisposeStatic();
		ServerStatic.IsDedicated = false;
		UnityEngine.Debug.Log("OnConsoleExited");
		Process consoleProcess = ServerConsole.ConsoleProcess;
		if (consoleProcess != null)
		{
			consoleProcess.Dispose();
		}
		ServerConsole.ConsoleProcess = null;
		Shutdown.Quit(true, false);
	}

	// Token: 0x06000978 RID: 2424 RVA: 0x00026288 File Offset: 0x00024488
	private void OnSceneWasLoaded(Scene scene, LoadSceneMode mode)
	{
		if (ServerStatic.IsDedicated)
		{
			int buildIndex = scene.buildIndex;
			if (buildIndex == 3 || buildIndex == 4)
			{
				base.GetComponent<CustomNetworkManager>().CreateMatch();
			}
		}
	}

	// Token: 0x0400087B RID: 2171
	public static ushort ShutdownRedirectPort;

	// Token: 0x0400087D RID: 2173
	public static YamlConfig RolesConfig;

	// Token: 0x0400087E RID: 2174
	public static YamlConfig SharedGroupsConfig;

	// Token: 0x0400087F RID: 2175
	public static YamlConfig SharedGroupsMembersConfig;

	// Token: 0x04000880 RID: 2176
	public static string RolesConfigPath;

	// Token: 0x04000881 RID: 2177
	public static PermissionsHandler PermissionsHandler;

	// Token: 0x04000882 RID: 2178
	public static IServerOutput ServerOutput;

	// Token: 0x04000883 RID: 2179
	internal static bool ProcessIdPassed;

	// Token: 0x04000884 RID: 2180
	internal static bool DisableConfigValidation;

	// Token: 0x04000885 RID: 2181
	internal static bool KeepSession;

	// Token: 0x04000886 RID: 2182
	internal static bool EnableConsoleHeartbeat;

	// Token: 0x04000887 RID: 2183
	private static short _serverTickrate = 60;

	// Token: 0x04000888 RID: 2184
	private static bool _serverArgsProcessed;

	// Token: 0x04000889 RID: 2185
	private static bool _serverPortSet;

	// Token: 0x020001CD RID: 461
	public enum NextRoundAction : byte
	{
		// Token: 0x0400088B RID: 2187
		DoNothing,
		// Token: 0x0400088C RID: 2188
		Restart,
		// Token: 0x0400088D RID: 2189
		Shutdown
	}

	// Token: 0x020001CE RID: 462
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x0400088E RID: 2190
		public static EventHandler <0>__OnConsoleExited;
	}
}
