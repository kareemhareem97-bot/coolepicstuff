using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CentralAuth;
using CommandSystem;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Enums;
using Mirror;
using NorthwoodLib.Pools;
using Org.BouncyCastle.Crypto;
using RemoteAdmin;
using TMPro;
using ToggleableMenus;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UserSettings;
using UserSettings.OtherSettings;

namespace GameCore
{
	// Token: 0x02000858 RID: 2136
	public class Console : SimpleToggleableMenu
	{
		// Token: 0x170008F9 RID: 2297
		// (get) Token: 0x060030C2 RID: 12482 RVA: 0x000A35EA File Offset: 0x000A17EA
		// (set) Token: 0x060030C3 RID: 12483 RVA: 0x000A35F1 File Offset: 0x000A17F1
		public static Console Singleton { get; private set; }

		// Token: 0x170008FA RID: 2298
		// (get) Token: 0x060030C4 RID: 12484 RVA: 0x000A35F9 File Offset: 0x000A17F9
		// (set) Token: 0x060030C5 RID: 12485 RVA: 0x000A3600 File Offset: 0x000A1800
		public static bool RequestDNT { get; private set; }

		// Token: 0x170008FB RID: 2299
		// (get) Token: 0x060030C6 RID: 12486 RVA: 0x000A3608 File Offset: 0x000A1808
		// (set) Token: 0x060030C7 RID: 12487 RVA: 0x000A360F File Offset: 0x000A180F
		public static bool PrintingPaused { get; set; }

		// Token: 0x170008FC RID: 2300
		// (get) Token: 0x060030C8 RID: 12488 RVA: 0x000A3617 File Offset: 0x000A1817
		internal static string BinariesRootPath
		{
			get
			{
				if (Application.isEditor)
				{
					return "\\BuildFiles\\windows";
				}
				return Path.GetDirectoryName(Path.GetFullPath(Application.dataPath));
			}
		}

		// Token: 0x170008FD RID: 2301
		// (get) Token: 0x060030C9 RID: 12489 RVA: 0x000A3635 File Offset: 0x000A1835
		// (set) Token: 0x060030CA RID: 12490 RVA: 0x000A363C File Offset: 0x000A183C
		internal static bool DisableSRV { get; private set; }

		// Token: 0x170008FE RID: 2302
		// (get) Token: 0x060030CB RID: 12491 RVA: 0x000A3644 File Offset: 0x000A1844
		// (set) Token: 0x060030CC RID: 12492 RVA: 0x000A364B File Offset: 0x000A184B
		internal static bool SkipIpValidation { get; private set; }

		// Token: 0x170008FF RID: 2303
		// (get) Token: 0x060030CD RID: 12493 RVA: 0x000A3653 File Offset: 0x000A1853
		// (set) Token: 0x060030CE RID: 12494 RVA: 0x000A365A File Offset: 0x000A185A
		[Obsolete("Replaced by SSS")]
		internal static bool BindSyncingEnabled { get; set; }

		// Token: 0x17000900 RID: 2304
		// (get) Token: 0x060030CF RID: 12495 RVA: 0x000A3662 File Offset: 0x000A1862
		public override bool CanToggle
		{
			get
			{
				return !this.IsEnabled || !this._commandField.isFocused || this._commandField.text.Length <= 1;
			}
		}

		// Token: 0x17000901 RID: 2305
		// (get) Token: 0x060030D0 RID: 12496 RVA: 0x000A3691 File Offset: 0x000A1891
		public override bool LockMovement
		{
			get
			{
				return this.IsEnabled;
			}
		}

		// Token: 0x060030D1 RID: 12497 RVA: 0x000A369C File Offset: 0x000A189C
		public static void AddDebugLog(string debugKey, string message, MessageImportance importance, bool nospace = false)
		{
			Color32 c;
			if (ConsoleDebugMode.CheckImportance(debugKey, importance, out c))
			{
				Console.AddLog("[DEBUG_" + debugKey + "] " + message, c, nospace, ConsoleLogType.Log);
			}
		}

		// Token: 0x060030D2 RID: 12498 RVA: 0x000A36D4 File Offset: 0x000A18D4
		public static void AddLog(string text, Color color, bool nospace = false, ConsoleLogType type = ConsoleLogType.Log)
		{
			DateTimeOffset now = DateTimeOffset.Now;
			if (!Console._noConsoleLogOutput && !ServerStatic.IsDedicated)
			{
				switch (type)
				{
				case ConsoleLogType.DoNotLog:
					break;
				case ConsoleLogType.Log:
					if (Application.isEditor)
					{
						Debug.Log(string.Concat(new string[]
						{
							"<color=",
							color.ToHex(),
							">",
							text,
							"</color>"
						}));
					}
					else
					{
						Debug.Log(text);
					}
					break;
				case ConsoleLogType.Warning:
					Debug.LogWarning(text);
					break;
				case ConsoleLogType.Error:
					Debug.LogError(text);
					break;
				default:
					throw new ArgumentOutOfRangeException("type", type, null);
				}
			}
			if (Application.isEditor)
			{
				Debug.Log(text);
			}
			if (ServerStatic.IsDedicated)
			{
				ServerConsole.AddLog(text, Misc.ClosestConsoleColor(color, true), false);
				return;
			}
			if (Console.PrintingPaused)
			{
				return;
			}
			text = string.Concat(new string[]
			{
				"<color=#808080><size=18>[",
				TimeBehaviour.FormatTime("HH:mm:ss", now),
				"<size=16>.",
				TimeBehaviour.FormatTime("fff", now),
				"</size>]</size></color> ",
				text
			});
			Console.LogQueue.Enqueue(new ConsoleLog(text, color, nospace));
		}

		// Token: 0x060030D3 RID: 12499 RVA: 0x000A37FD File Offset: 0x000A19FD
		public static void ClearConsole()
		{
			Console.Logs.Clear();
			Console.Singleton.RefreshConsoleScreen();
		}

		// Token: 0x060030D4 RID: 12500 RVA: 0x000A3814 File Offset: 0x000A1A14
		public static GameObject FindConnectedRoot(NetworkConnection conn)
		{
			try
			{
				GameObject gameObject = conn.identity.gameObject;
				if (gameObject.CompareTag("Player"))
				{
					return gameObject;
				}
			}
			catch
			{
				return null;
			}
			return null;
		}

		// Token: 0x060030D5 RID: 12501 RVA: 0x000A3858 File Offset: 0x000A1A58
		internal string TypeCommand(string cmd, CommandSender sender = null)
		{
			if (!NetworkServer.active)
			{
				Debug.Log(">>> " + cmd);
				if (sender == null)
				{
					sender = Console.Ccs;
				}
				this._clientCommandLogs.Add(cmd);
			}
			else if (sender == null)
			{
				sender = ServerConsole.Scs;
			}
			if (cmd.StartsWith('.') && cmd.Length > 1)
			{
				if (!NetworkClient.active && !NetworkServer.active)
				{
					Console.AddLog("You must be connected to a server to use this command.", Color.red, false, ConsoleLogType.Log);
					return "You must be connected to a server to use this command.";
				}
				string text = cmd;
				string text2 = text.Substring(1, text.Length - 1);
				string text3 = "Sending command to server: " + text2;
				if (sender != null)
				{
					sender.Print(text3, ConsoleColor.Green);
				}
				ReferenceHub referenceHub;
				if (!ReferenceHub.TryGetLocalHub(out referenceHub))
				{
					return "You must be connected to a server to use this command.";
				}
				referenceHub.gameConsoleTransmission.SendToServer(text2);
				return text3;
			}
			else
			{
				bool flag = cmd.StartsWith('@');
				string text;
				if ((cmd.StartsWith('/') || flag) && cmd.Length > 1)
				{
					string text4;
					if (!flag)
					{
						text = cmd;
						text4 = text.Substring(1, text.Length - 1);
					}
					else
					{
						text4 = cmd;
					}
					string text5 = text4;
					if (!flag)
					{
						text5 = text5.TrimStart('$');
						if (string.IsNullOrEmpty(text5))
						{
							if (sender != null)
							{
								sender.Print("Command cant be empty!", ConsoleColor.Green);
							}
							return "Command cant be empty!";
						}
					}
					if (NetworkServer.active)
					{
						if (!flag)
						{
							return CommandProcessor.ProcessQuery(text5, sender);
						}
						text = text5;
						CommandProcessor.ProcessAdminChat(text.Substring(1, text.Length - 1), sender);
						return null;
					}
				}
				string[] array = cmd.Trim().Split(QueryProcessor.SpaceArray, 512, StringSplitOptions.RemoveEmptyEntries);
				cmd = array[0];
				ICommand command;
				bool flag2 = Console.ConsoleCommandHandler.TryGetCommand(cmd, out command);
				ArraySegment<string> arguments = array.Segment(1);
				CommandExecutingEventArgs commandExecutingEventArgs = new CommandExecutingEventArgs(sender, CommandType.Console, flag2, command, arguments);
				ServerEvents.OnCommandExecuting(commandExecutingEventArgs);
				if (!commandExecutingEventArgs.IsAllowed)
				{
					return null;
				}
				arguments = commandExecutingEventArgs.Arguments;
				sender = commandExecutingEventArgs.Sender;
				command = commandExecutingEventArgs.Command;
				if (!flag2)
				{
					string text6 = "Command " + cmd + " does not exist!";
					if (sender != null)
					{
						sender.Print(text6, ConsoleColor.DarkYellow, new Color32(byte.MaxValue, 180, 0, byte.MaxValue));
					}
					return text6;
				}
				try
				{
					string text7;
					bool flag3 = command.Execute(arguments, sender, out text7);
					text7 = Misc.CloseAllRichTextTags(text7);
					CommandExecutedEventArgs commandExecutedEventArgs = new CommandExecutedEventArgs(sender, CommandType.Console, command, arguments, flag3, text7);
					ServerEvents.OnCommandExecuted(commandExecutedEventArgs);
					text7 = commandExecutedEventArgs.Response;
					flag3 = commandExecutedEventArgs.ExecutedSuccessfully;
					if (string.IsNullOrWhiteSpace(text7))
					{
						text = null;
					}
					else
					{
						if (sender != null)
						{
							sender.Print(text7, flag3 ? ConsoleColor.Green : ConsoleColor.Red);
						}
						text = text7;
					}
				}
				catch (Exception ex)
				{
					string text8 = "Command execution failed! Error: " + Misc.RemoveStacktraceZeroes(ex.ToString());
					CommandExecutedEventArgs commandExecutedEventArgs2 = new CommandExecutedEventArgs(sender, CommandType.Console, command, arguments, false, text8);
					ServerEvents.OnCommandExecuted(commandExecutedEventArgs2);
					text8 = commandExecutedEventArgs2.Response;
					if (sender != null)
					{
						sender.Print(text8, ConsoleColor.Red);
					}
					text = text8;
				}
				return text;
			}
		}

		// Token: 0x060030D6 RID: 12502 RVA: 0x00002E9A File Offset: 0x0000109A
		internal static void CopyPastebin()
		{
		}

		// Token: 0x060030D7 RID: 12503 RVA: 0x000A3B20 File Offset: 0x000A1D20
		private static void HandleStartupArgs()
		{
			bool flag = false;
			string[] args = StartupArgs.Args;
			for (int i = 0; i < args.Length; i++)
			{
				string text = args[i].ToLowerInvariant();
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 847953999U)
				{
					if (num <= 201040821U)
					{
						if (num != 63733902U)
						{
							if (num == 201040821U)
							{
								if (text == "-skipipvalidation")
								{
									Console.SkipIpValidation = true;
									Console.AddLog("IP validation will be skipped.", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false, ConsoleLogType.Log);
								}
							}
						}
						else if (text == "-noconsolelog")
						{
							Console._noConsoleLogOutput = true;
						}
					}
					else if (num != 581847921U)
					{
						if (num != 701709518U)
						{
							if (num == 847953999U)
							{
								if (text == "-neverhidelocaltag")
								{
									UserSetting<int>.Set<MiscPrivacySetting>(MiscPrivacySetting.LocalBadgeVisibility, 1);
									Console.AddLog("Your local badge won't be automatically hidden.", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false, ConsoleLogType.Log);
								}
							}
						}
						else if (text == "-dnt")
						{
							Console.RequestDNT = true;
							Console.AddLog("\"Do not track\" request will be sent to all servers you are joining - enabled by startup argument.", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false, ConsoleLogType.Log);
						}
					}
					else if (text == "-hidelocaltag")
					{
						UserSetting<int>.Set<MiscPrivacySetting>(MiscPrivacySetting.LocalBadgeVisibility, 2);
						Console.AddLog("Your local badge will be automatically hidden.", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false, ConsoleLogType.Log);
					}
				}
				else if (num <= 2020690992U)
				{
					if (num != 1089330135U)
					{
						if (num != 1674418289U)
						{
							if (num == 2020690992U)
							{
								if (text == "-allow-syncbind")
								{
									flag = true;
									Console.AddLog("[WARNING] You have \"-allow-syncbind\" startup argument added.", Color.yellow, false, ConsoleLogType.Log);
									Console.AddLog("[WARNING] Your command binding might be messed up, and your key might be logged by the server you join.", Color.yellow, false, ConsoleLogType.Log);
									Console.AllowBindSyncing = true;
									if (File.Exists(Console.SyncbindPath))
									{
										if (File.ReadAllText(Console.SyncbindPath) == SystemInfo.deviceUniqueIdentifier)
										{
											Console.AddLog("[WARNING] Bind syncing is ENABLED.", Color.red, false, ConsoleLogType.Log);
											Console.BindSyncingEnabled = true;
										}
										else
										{
											Console.AddLog("Bind syncing has been disabled - HWID mismatch.", Color.gray, false, ConsoleLogType.Log);
											File.Delete(Console.SyncbindPath);
										}
									}
									else
									{
										Console.BindSyncingEnabled = false;
									}
								}
							}
						}
						else if (text == "-nevercover")
						{
							UserSetting<int>.Set<MiscPrivacySetting>(MiscPrivacySetting.BadgePreferences, 1);
							Console.AddLog("Your global badge won't be covered.", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false, ConsoleLogType.Log);
						}
					}
					else if (text == "-tdm")
					{
						Console.TranslationDebugMode = true;
						Console.AddLog("Translation debug mode has been enabled (startup argument).", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false, ConsoleLogType.Log);
					}
				}
				else if (num != 2757569412U)
				{
					if (num != 3803288287U)
					{
						if (num == 4033230344U)
						{
							if (text == "-hidetag")
							{
								UserSetting<int>.Set<MiscPrivacySetting>(MiscPrivacySetting.GlobalBadgeVisibility, 2);
								Console.AddLog("Your global badge will be automatically hidden.", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false, ConsoleLogType.Log);
							}
						}
					}
					else if (text == "-scs")
					{
						Console.DisplayCreateServer = true;
						Console.AddLog("Create Server button will be displayed (startup argument).", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false, ConsoleLogType.Log);
					}
				}
				else if (text == "-nosrv")
				{
					Console.DisableSRV = true;
					Console.AddLog("SRV DNS records resolution has been disabled.", Color.yellow, false, ConsoleLogType.Log);
				}
			}
			if (!flag && File.Exists(Console.SyncbindPath))
			{
				File.Delete(Console.SyncbindPath);
			}
		}

		// Token: 0x060030D8 RID: 12504 RVA: 0x00002E9A File Offset: 0x0000109A
		public static void RefreshPublicKey()
		{
		}

		// Token: 0x060030D9 RID: 12505 RVA: 0x000A3F1C File Offset: 0x000A211C
		public void ProceedButton()
		{
			if (this._commandField.text != string.Empty)
			{
				this.TypeCommand(this._commandField.text, null);
			}
			this._commandField.text = string.Empty;
			EventSystem.current.SetSelectedGameObject(this._commandField.gameObject);
		}

		// Token: 0x060030DA RID: 12506 RVA: 0x000A3F78 File Offset: 0x000A2178
		protected override void OnToggled()
		{
			base.OnToggled();
			if (ServerStatic.IsDedicated)
			{
				return;
			}
			this._background.SetActive(this.IsEnabled);
			this._commandField.text = string.Empty;
			if (this.IsEnabled)
			{
				EventSystem.current.SetSelectedGameObject(this._commandField.gameObject);
			}
		}

		// Token: 0x060030DB RID: 12507 RVA: 0x000A3FD1 File Offset: 0x000A21D1
		protected override void Awake()
		{
			if (Console.Singleton != null)
			{
				UnityEngine.Object.DestroyImmediate(base.gameObject);
				return;
			}
			Console.Singleton = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			base.Awake();
		}

		// Token: 0x060030DC RID: 12508 RVA: 0x000A4004 File Offset: 0x000A2204
		private void Start()
		{
			SceneManager.sceneLoaded += this.OnSceneLoaded;
			Console.AddLog("Hi there! Initializing console...", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false, ConsoleLogType.Log);
			Console.AddLog("Done! Type 'help' to print the list of available commands.", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false, ConsoleLogType.Log);
			CentralAuthManager.InitAuth();
			Console.HandleStartupArgs();
			this._scaleNonAlloc[0] = this._resizableWindow.CurrentSize.x;
			this._scaleNonAlloc[1] = this._resizableWindow.CurrentSize.y;
			this._scaleNonAlloc = PlayerPrefsSl.Get("GameConsoleScale", this._scaleNonAlloc);
			if (!ServerStatic.IsDedicated)
			{
				Console.AddLog(string.Format("Loading console size preference: ({0:0.0},{1:0.0})", this._scaleNonAlloc[0], this._scaleNonAlloc[1]), new Color32(0, byte.MaxValue, 0, byte.MaxValue), false, ConsoleLogType.Log);
			}
			this._resizableWindow.SetScaleAndResetPosition(new Vector2(this._scaleNonAlloc[0], this._scaleNonAlloc[1]));
			this._resizableWindow.OnResizingEnded += this.OnWindowResized;
		}

		// Token: 0x060030DD RID: 12509 RVA: 0x00002E9A File Offset: 0x0000109A
		private void Update()
		{
		}

		// Token: 0x060030DE RID: 12510 RVA: 0x00002E9A File Offset: 0x0000109A
		private void LateUpdate()
		{
		}

		// Token: 0x060030DF RID: 12511 RVA: 0x000A4138 File Offset: 0x000A2338
		private void FixedUpdate()
		{
			bool flag = false;
			ConsoleLog consoleLog;
			while (Console.LogQueue.TryDequeue(out consoleLog))
			{
				string[] array = consoleLog.Text.Split('\n', StringSplitOptions.None);
				bool nospace = consoleLog.NoSpace;
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text = array2[i].Replace("\r", string.Empty);
					ConsoleLog item = new ConsoleLog(text, consoleLog.Color, nospace);
					Console.Logs.Enqueue(item);
					nospace = true;
					while (Console.Logs.Count > 200)
					{
						Console.Logs.Dequeue();
					}
				}
				flag = true;
			}
			if (!flag)
			{
				return;
			}
			this.RefreshConsoleScreen();
		}

		// Token: 0x060030E0 RID: 12512 RVA: 0x000A41E2 File Offset: 0x000A23E2
		private void OnApplicationQuit()
		{
			this._resizableWindow.OnResizingEnded -= this.OnWindowResized;
			Shutdown.Quit(false, false);
		}

		// Token: 0x060030E1 RID: 12513 RVA: 0x000A4204 File Offset: 0x000A2404
		private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			Console.AddLog(string.Concat(new string[]
			{
				"Scene Manager: Loaded scene '",
				scene.name,
				"' [",
				scene.path,
				"]"
			}), new Color32(0, byte.MaxValue, 0, byte.MaxValue), false, ConsoleLogType.Log);
			this.RefreshConsoleScreen();
		}

		// Token: 0x060030E2 RID: 12514 RVA: 0x000A426C File Offset: 0x000A246C
		private void RefreshConsoleScreen()
		{
			if (this._consoleText == null)
			{
				return;
			}
			if (Console.Logs.Count == 0)
			{
				this._content = string.Empty;
				this._consoleText.text = this._content;
				Console._refreshText = true;
				return;
			}
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			foreach (ConsoleLog consoleLog in Console.Logs)
			{
				consoleLog.AppendFormatted(stringBuilder);
			}
			this._content = StringBuilderPool.Shared.ToStringReturn(stringBuilder);
			this._consoleText.SetText(this._content);
			Console._refreshText = true;
		}

		// Token: 0x060030E3 RID: 12515 RVA: 0x000A4330 File Offset: 0x000A2530
		private void OnWindowResized(Vector2 size)
		{
			this._scaleNonAlloc[0] = size.x;
			this._scaleNonAlloc[1] = size.y;
			PlayerPrefsSl.Set("GameConsoleScale", this._scaleNonAlloc);
			Console.AddLog(string.Format("Console size preference set to ({0:0.0},{1:0.0})", this._scaleNonAlloc[0], this._scaleNonAlloc[1]), new Color32(0, byte.MaxValue, 0, byte.MaxValue), false, ConsoleLogType.Log);
		}

		// Token: 0x040025B8 RID: 9656
		private const string PrefsKeyName = "GameConsoleScale";

		// Token: 0x040025B9 RID: 9657
		private const string NotConnectedMessage = "You must be connected to a server to use this command.";

		// Token: 0x040025BA RID: 9658
		private const int MaxLogAmount = 200;

		// Token: 0x040025BE RID: 9662
		public static readonly GameConsoleCommandHandler ConsoleCommandHandler = GameConsoleCommandHandler.Create();

		// Token: 0x040025BF RID: 9663
		internal static bool DisplayCreateServer;

		// Token: 0x040025C0 RID: 9664
		internal static bool TranslationDebugMode;

		// Token: 0x040025C1 RID: 9665
		internal static AsymmetricKeyParameter _publicKey;

		// Token: 0x040025C3 RID: 9667
		internal static readonly Queue<ConsoleLog> Logs = new Queue<ConsoleLog>();

		// Token: 0x040025C4 RID: 9668
		internal static readonly string SyncbindPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SCP Secret Laboratory/internal/SyncCmd";

		// Token: 0x040025C6 RID: 9670
		internal static bool AlwaysRefreshing = false;

		// Token: 0x040025C8 RID: 9672
		[Obsolete("Replaced by SSS")]
		internal static bool AllowBindSyncing;

		// Token: 0x040025C9 RID: 9673
		[Obsolete("Replaced by SSS")]
		internal static bool BindSyncingContinue;

		// Token: 0x040025CA RID: 9674
		private static bool _noConsoleLogOutput;

		// Token: 0x040025CB RID: 9675
		private static bool _refreshText;

		// Token: 0x040025CC RID: 9676
		private static readonly ConcurrentQueue<ConsoleLog> LogQueue = new ConcurrentQueue<ConsoleLog>();

		// Token: 0x040025CD RID: 9677
		private static readonly ConsoleCommandSender Ccs = new ConsoleCommandSender();

		// Token: 0x040025CE RID: 9678
		[SerializeField]
		private TextMeshProUGUI _consoleText;

		// Token: 0x040025CF RID: 9679
		[SerializeField]
		private TMP_InputField _commandField;

		// Token: 0x040025D0 RID: 9680
		[SerializeField]
		private Scrollbar _scrollBar;

		// Token: 0x040025D1 RID: 9681
		[SerializeField]
		private GameObject _background;

		// Token: 0x040025D2 RID: 9682
		[SerializeField]
		private ResizableWindow _resizableWindow;

		// Token: 0x040025D3 RID: 9683
		private readonly List<string> _clientCommandLogs = new List<string>();

		// Token: 0x040025D4 RID: 9684
		private float[] _scaleNonAlloc = new float[2];

		// Token: 0x040025D5 RID: 9685
		private int _clientCommandPosition;

		// Token: 0x040025D6 RID: 9686
		private string _content;
	}
}
