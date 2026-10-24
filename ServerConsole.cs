using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using Authenticator;
using CentralAuth;
using Cryptography;
using GameCore;
using LabApi.Features.Wrappers;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using NorthwoodLib;
using NorthwoodLib.Pools;
using Org.BouncyCastle.Crypto;
using PlayerRoles;
using Respawning.NamingRules;
using ServerOutput;
using Steam;
using UnityEngine;
using Utils.CommandInterpolation;

// Token: 0x020001C0 RID: 448
public class ServerConsole : MonoBehaviour, IDisposable
{
	// Token: 0x1700015C RID: 348
	// (get) Token: 0x060008D5 RID: 2261 RVA: 0x00022FB2 File Offset: 0x000211B2
	// (set) Token: 0x060008D6 RID: 2262 RVA: 0x00022FB9 File Offset: 0x000211B9
	public static ServerConsole Singleton { get; private set; }

	// Token: 0x1700015D RID: 349
	// (get) Token: 0x060008D7 RID: 2263 RVA: 0x00022FC1 File Offset: 0x000211C1
	// (set) Token: 0x060008D8 RID: 2264 RVA: 0x00022FC9 File Offset: 0x000211C9
	public InterpolatedCommandFormatter NameFormatter { get; private set; }

	// Token: 0x060008D9 RID: 2265 RVA: 0x00022FD2 File Offset: 0x000211D2
	public static void ReloadServerName()
	{
		ServerConsole.ServerName = ConfigFile.ServerConfig.GetString("server_name", "My Server Name");
	}

	// Token: 0x060008DA RID: 2266 RVA: 0x00022FF0 File Offset: 0x000211F0
	public void Dispose()
	{
		ServerConsole._disposing = true;
		IServerOutput serverOutput = ServerStatic.ServerOutput;
		if (serverOutput != null)
		{
			serverOutput.Dispose();
		}
		if (ServerConsole._checkProcessThread != null && ServerConsole._checkProcessThread.IsAlive)
		{
			ServerConsole._checkProcessThread.Abort();
		}
		if (ServerConsole._verificationRequestThread != null && ServerConsole._verificationRequestThread.IsAlive)
		{
			ServerConsole._verificationRequestThread.Abort();
		}
		if (ServerConsole._refreshPublicKeyThread != null && ServerConsole._refreshPublicKeyThread.IsAlive)
		{
			ServerConsole._refreshPublicKeyThread.Abort();
		}
		if (ServerConsole._refreshPublicKeyOnceThread != null && ServerConsole._refreshPublicKeyOnceThread.IsAlive)
		{
			ServerConsole._refreshPublicKeyOnceThread.Abort();
		}
		if (ServerConsole._verificationRequestThread != null && ServerConsole._verificationRequestThread.IsAlive)
		{
			ServerConsole._verificationRequestThread.Abort();
		}
	}

	// Token: 0x060008DB RID: 2267
	[DllImport("libc", EntryPoint = "getuid")]
	private static extern uint GetUserId();

	// Token: 0x060008DC RID: 2268 RVA: 0x000230A4 File Offset: 0x000212A4
	private static void CheckRoot()
	{
		if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
		{
			return;
		}
		if (ServerConsole.GetUserId() != 0U)
		{
			return;
		}
		GameCore.Console.AddLog("Running the game as ROOT is NOT recommended, please create a separate user!", Color.red, false, ConsoleLogType.Log);
	}

	// Token: 0x060008DD RID: 2269 RVA: 0x000230CC File Offset: 0x000212CC
	private void Start()
	{
		ServerConsole.CheckRoot();
		InterpolatedCommandFormatter interpolatedCommandFormatter = new InterpolatedCommandFormatter(4);
		interpolatedCommandFormatter.StartClosure = '{';
		interpolatedCommandFormatter.EndClosure = '}';
		interpolatedCommandFormatter.Escape = '\\';
		interpolatedCommandFormatter.ArgumentSplitter = ',';
		Dictionary<string, Func<List<string>, string>> dictionary = new Dictionary<string, Func<List<string>, string>>();
		dictionary.Add("ip", (List<string> args) => ServerConsole.Ip);
		dictionary.Add("port", (List<string> args) => LiteNetLib4MirrorTransport.Singleton.port.ToString());
		dictionary.Add("number", (List<string> args) => ((int)(LiteNetLib4MirrorTransport.Singleton.port - 7776)).ToString());
		dictionary.Add("version", (List<string> args) => GameCore.Version.VersionString);
		dictionary.Add("player_count", (List<string> args) => ReferenceHub.GetPlayerCount(ClientInstanceMode.ReadyClient, ClientInstanceMode.Host).ToString());
		dictionary.Add("full_player_count", delegate(List<string> args)
		{
			int playerCount = ReferenceHub.GetPlayerCount(ClientInstanceMode.ReadyClient, ClientInstanceMode.Host);
			if (playerCount != CustomNetworkManager.TypedSingleton.ReservedMaxPlayers)
			{
				return string.Format("{0}/{1}", playerCount, CustomNetworkManager.TypedSingleton.ReservedMaxPlayers);
			}
			int count = args.Count;
			if (count == 1)
			{
				return "FULL";
			}
			if (count != 2)
			{
				throw new ArgumentOutOfRangeException("args", args, "Invalid arguments. Use: full_player_count OR full_player_count,[full]");
			}
			return this.NameFormatter.ProcessExpression(args[1]);
		});
		dictionary.Add("max_players", (List<string> args) => CustomNetworkManager.TypedSingleton.ReservedMaxPlayers.ToString());
		dictionary.Add("round_duration_minutes", (List<string> args) => RoundStart.RoundLength.Minutes.ToString("00"));
		dictionary.Add("round_duration_seconds", (List<string> args) => RoundStart.RoundLength.Seconds.ToString("00"));
		dictionary.Add("kills", (List<string> args) => ServerConsole.GetRoundInfo((RoundSummary summary) => RoundSummary.Kills.ToString(), true));
		dictionary.Add("alive_role", delegate(List<string> args)
		{
			if (args.Count != 2)
			{
				throw new CommandInputException("args", args, "Invalid arguments. Use: alive_role,[role ID]");
			}
			RoleTypeId role;
			if (!Enum.TryParse<RoleTypeId>(this.NameFormatter.ProcessExpression(args[1]), out role))
			{
				throw new CommandInputException("role ID", args[1], "Could not parse.");
			}
			return ServerConsole.GetRoundInfo((RoundSummary s) => s.CountRole(role).ToString(), true);
		});
		dictionary.Add("alive_team", delegate(List<string> args)
		{
			if (args.Count != 2)
			{
				throw new CommandInputException("args", args, "Invalid arguments. Use: alive_team,[team ID]");
			}
			Team team;
			if (!Enum.TryParse<Team>(this.NameFormatter.ProcessExpression(args[1]), out team))
			{
				throw new CommandInputException("team ID", args[1], "Could not parse.");
			}
			return ServerConsole.GetRoundInfo((RoundSummary s) => s.CountTeam(team).ToString(), true);
		});
		dictionary.Add("zombies_recalled", (List<string> args) => ServerConsole.GetRoundInfo((RoundSummary summary) => RoundSummary.ChangedIntoZombies.ToString(), true));
		dictionary.Add("scp_counter", (List<string> args) => ServerConsole.GetRoundInfo((RoundSummary summary) => string.Format("{0}/{1}", summary.CountTeam(Team.SCPs) - summary.CountRole(RoleTypeId.Scp0492), summary.classlistStart.scps_except_zombies), false));
		dictionary.Add("scp_start", (List<string> args) => ServerConsole.GetRoundInfo((RoundSummary summary) => summary.classlistStart.scps_except_zombies.ToString(), true));
		dictionary.Add("scp_killed", (List<string> args) => ServerConsole.GetRoundInfo((RoundSummary summary) => (summary.classlistStart.scps_except_zombies - summary.CountTeam(Team.SCPs) - summary.CountRole(RoleTypeId.Scp0492)).ToString(), true));
		dictionary.Add("scp_kills", (List<string> args) => ServerConsole.GetRoundInfo((RoundSummary summary) => RoundSummary.KilledBySCPs.ToString(), true));
		dictionary.Add("classd_counter", (List<string> args) => ServerConsole.GetRoundInfo((RoundSummary summary) => string.Format("{0}/{1}", RoundSummary.EscapedClassD, summary.classlistStart.class_ds), true));
		dictionary.Add("classd_start", (List<string> args) => ServerConsole.GetRoundInfo((RoundSummary summary) => summary.classlistStart.class_ds.ToString(), true));
		dictionary.Add("classd_escaped", (List<string> args) => ServerConsole.GetRoundInfo((RoundSummary summary) => RoundSummary.EscapedClassD.ToString(), true));
		dictionary.Add("scientist_counter", (List<string> args) => ServerConsole.GetRoundInfo((RoundSummary summary) => string.Format("{0}/{1}", RoundSummary.EscapedScientists, summary.classlistStart.scientists), false));
		dictionary.Add("scientist_start", (List<string> args) => ServerConsole.GetRoundInfo((RoundSummary summary) => summary.classlistStart.scientists.ToString(), true));
		dictionary.Add("scientist_escaped", (List<string> args) => ServerConsole.GetRoundInfo((RoundSummary summary) => RoundSummary.EscapedScientists.ToString(), true));
		dictionary.Add("mtf_respawns", (List<string> args) => ServerConsole.GetRoundInfo(delegate(RoundSummary summary)
		{
			List<string> list;
			return (NamingRulesManager.GeneratedNames.TryGetValue(Team.FoundationForces, out list) ? (list.Count - 1) : 0).ToString();
		}, true));
		dictionary.Add("warhead_detonated", delegate(List<string> args)
		{
			int count = args.Count;
			if (count == 1)
			{
				return ServerConsole.GetRoundInfo(delegate(RoundSummary s)
				{
					if (!AlphaWarheadController.Detonated)
					{
						return string.Empty;
					}
					return "☢ WARHEAD DETONATED ☢";
				}, false);
			}
			if (count != 3)
			{
				throw new CommandInputException("args", args, "Invalid arguments. Use: warhead_detonated OR warhead_detonated,[detonated],[undetonated]");
			}
			return ServerConsole.GetRoundInfo((RoundSummary s) => this.NameFormatter.ProcessExpression(args[AlphaWarheadController.Detonated ? 1 : 2]), false);
		});
		dictionary.Add("random", delegate(List<string> args)
		{
			int count = args.Count;
			float minInclusive;
			float maxInclusive;
			if (count != 2)
			{
				if (count != 3)
				{
					throw new CommandInputException("args", args, "Invalid arguments. Use: random,[max] OR random,[min],[max]");
				}
				string text = this.NameFormatter.ProcessExpression(args[1]);
				if (!float.TryParse(text, out minInclusive))
				{
					throw new CommandInputException("min", text, "Could not parse.");
				}
				string text2 = this.NameFormatter.ProcessExpression(args[2]);
				if (!float.TryParse(text2, out maxInclusive))
				{
					throw new CommandInputException("max", text2, "Could not parse.");
				}
			}
			else
			{
				minInclusive = 0f;
				string text2 = this.NameFormatter.ProcessExpression(args[1]);
				if (!float.TryParse(text2, out maxInclusive))
				{
					throw new CommandInputException("max", text2, "Could not parse.");
				}
			}
			return UnityEngine.Random.Range(minInclusive, maxInclusive).ToString();
		});
		dictionary.Add("random_list", delegate(List<string> args)
		{
			if (args.Count < 3)
			{
				throw new CommandInputException("args", args, "Invalid arguments. Use: random_list,[entry 1],[entry 2]...");
			}
			return this.NameFormatter.ProcessExpression(args[UnityEngine.Random.Range(1, args.Count)]);
		});
		dictionary.Add("constant_e", (List<string> args) => 2.7182817f.ToString());
		dictionary.Add("constant_pi", (List<string> args) => 3.1415927f.ToString());
		dictionary.Add("add", (List<string> args) => this.StandardizedFloatComparison<float>("add", args, (float a, float b) => a + b));
		dictionary.Add("subtract", (List<string> args) => this.StandardizedFloatComparison<float>("subtract", args, (float a, float b) => a - b));
		dictionary.Add("multiply", (List<string> args) => this.StandardizedFloatComparison<float>("multiply", args, (float a, float b) => a * b));
		dictionary.Add("division", (List<string> args) => this.StandardizedFloatComparison<float>("division", args, (float a, float b) => a / b));
		dictionary.Add("power", (List<string> args) => this.StandardizedFloatComparison<float>("power", args, ServerConsole._pow));
		dictionary.Add("log", delegate(List<string> args)
		{
			int count = args.Count;
			float f;
			float p;
			if (count != 2)
			{
				if (count != 3)
				{
					throw new CommandInputException("args", args, "Invalid arguments. Use log,[value] OR log,[value],[base]");
				}
				string text = this.NameFormatter.ProcessExpression(args[1]);
				if (!float.TryParse(text, out f))
				{
					throw new CommandInputException("value", text, "Could not parse.");
				}
				string text2 = this.NameFormatter.ProcessExpression(args[2]);
				if (!float.TryParse(text2, out p))
				{
					throw new CommandInputException("base", text2, "Could not parse.");
				}
			}
			else
			{
				string text = this.NameFormatter.ProcessExpression(args[1]);
				if (!float.TryParse(text, out f))
				{
					throw new CommandInputException("value", text, "Could not parse.");
				}
				p = 10f;
			}
			return Mathf.Log(f, p).ToString();
		});
		dictionary.Add("ln", delegate(List<string> args)
		{
			if (args.Count < 2)
			{
				throw new CommandInputException("args", args, "Invalid arguments. Use ln,[value]");
			}
			string text = this.NameFormatter.ProcessExpression(args[1]);
			float f;
			if (!float.TryParse(text, out f))
			{
				throw new CommandInputException("value", text, "Error parsing value.");
			}
			return Mathf.Log(f).ToString();
		});
		dictionary.Add("round", (List<string> args) => this.StandardizedFloatRound("round", args, ServerConsole._roundNormal));
		dictionary.Add("round_up", (List<string> args) => this.StandardizedFloatRound("round_up", args, ServerConsole._roundCeil));
		dictionary.Add("round_down", (List<string> args) => this.StandardizedFloatRound("round_down", args, ServerConsole._roundFloor));
		dictionary.Add("equals", delegate(List<string> args)
		{
			if (args.Count != 3)
			{
				throw new CommandInputException("args", args, "Invalid arguments. Use equals,[object A],[object B]");
			}
			return (args[1] == args[2]).ToString();
		});
		dictionary.Add("greater", (List<string> args) => this.StandardizedFloatComparison<bool>("greater", args, (float a, float b) => a > b));
		dictionary.Add("lesser", (List<string> args) => this.StandardizedFloatComparison<bool>("lesser", args, (float a, float b) => a < b));
		dictionary.Add("greater_or_equal", (List<string> args) => this.StandardizedFloatComparison<bool>("greater_or_equal", args, (float a, float b) => a >= b));
		dictionary.Add("lesser_or_equal", (List<string> args) => this.StandardizedFloatComparison<bool>("lesser_or_equal", args, (float a, float b) => a <= b));
		dictionary.Add("not", delegate(List<string> args)
		{
			if (args.Count != 2)
			{
				throw new CommandInputException("args", args, "Invalid arguments. Use not,[value]");
			}
			string text = this.NameFormatter.ProcessExpression(args[1]);
			bool flag;
			if (!bool.TryParse(text, out flag))
			{
				throw new CommandInputException("value", text, "Error parsing value.");
			}
			return (!flag).ToString();
		});
		dictionary.Add("or", (List<string> args) => this.StandardizedBoolComparison<bool>("or", args, (bool a, bool b) => a || b));
		dictionary.Add("xor", (List<string> args) => this.StandardizedBoolComparison<bool>("xor", args, (bool a, bool b) => a ^ b));
		dictionary.Add("and", (List<string> args) => this.StandardizedBoolComparison<bool>("and", args, (bool a, bool b) => a && b));
		dictionary.Add("if", delegate(List<string> args)
		{
			int count = args.Count;
			string text;
			string text2;
			if (count != 3)
			{
				if (count != 4)
				{
					throw new CommandInputException("args", args, "Invalid arguments. Use if,[condition],[action] OR if,[condition],[action],[else action]");
				}
				text = args[2];
				text2 = args[3];
			}
			else
			{
				text = args[2];
				text2 = string.Empty;
			}
			string text3 = this.NameFormatter.ProcessExpression(args[1]);
			bool flag;
			if (!bool.TryParse(text3, out flag))
			{
				throw new CommandInputException("condition", text3, "Could not parse.");
			}
			return this.NameFormatter.ProcessExpression(flag ? text : text2);
		});
		interpolatedCommandFormatter.Commands = dictionary;
		this.NameFormatter = interpolatedCommandFormatter;
		Action<ReferenceHub, ClientInstanceMode> value;
		if ((value = ServerConsole.<>O.<0>__HandlePlayerJoin) == null)
		{
			value = (ServerConsole.<>O.<0>__HandlePlayerJoin = new Action<ReferenceHub, ClientInstanceMode>(ServerConsole.HandlePlayerJoin));
		}
		PlayerAuthenticationManager.OnInstanceModeChanged += value;
		ReferenceHub.OnPlayerRemoved += delegate(ReferenceHub rh)
		{
			ServerConsole.RefreshOnlinePlayers();
		};
		if (!ServerStatic.IsDedicated)
		{
			return;
		}
		if (!ServerStatic.ProcessIdPassed)
		{
			return;
		}
		ThreadStart start;
		if ((start = ServerConsole.<>O.<1>__CheckProcess) == null)
		{
			start = (ServerConsole.<>O.<1>__CheckProcess = new ThreadStart(ServerConsole.CheckProcess));
		}
		ServerConsole._checkProcessThread = new Thread(start)
		{
			Priority = System.Threading.ThreadPriority.Lowest,
			IsBackground = true,
			Name = "Dedicated server console running check"
		};
		ServerConsole._checkProcessThread.Start();
	}

	// Token: 0x060008DE RID: 2270 RVA: 0x000237E0 File Offset: 0x000219E0
	private static void HandlePlayerJoin(ReferenceHub rh, ClientInstanceMode mode)
	{
		if (mode != ClientInstanceMode.ReadyClient)
		{
			return;
		}
		ServerConsole.NewPlayers.Add(rh);
		ServerConsole.RefreshOnlinePlayers();
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x000237F8 File Offset: 0x000219F8
	private void FixedUpdate()
	{
		if (ServerStatic.EnableConsoleHeartbeat)
		{
			ServerConsole._heartbeatTimer += Time.fixedUnscaledDeltaTime;
			if (ServerConsole._heartbeatTimer >= 5f)
			{
				ServerConsole._heartbeatTimer = 0f;
				ServerConsole.AddOutputEntry(default(HeartbeatEntry));
			}
		}
		string text;
		while (ServerConsole.PrompterQueue.TryDequeue(out text))
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				ServerConsole.EnterCommand(text, ServerConsole.Scs);
			}
		}
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x0002386C File Offset: 0x00021A6C
	private static void RefreshOnlinePlayers()
	{
		try
		{
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				ClientInstanceMode mode = referenceHub.Mode;
				if ((mode == ClientInstanceMode.ReadyClient || mode == ClientInstanceMode.Host) && !string.IsNullOrEmpty(referenceHub.authManager.UserId) && (!referenceHub.isLocalPlayer || !ServerStatic.IsDedicated))
				{
					ServerConsole.PlayersListRaw.objects.Add(referenceHub.authManager.UserId);
				}
			}
			ServerConsole._verificationPlayersList = JsonSerialize.ToJson<PlayerListSerialized>(ServerConsole.PlayersListRaw);
			ServerConsole._playersAmount = ServerConsole.PlayersListRaw.objects.Count;
			SteamServerInfo.OnlinePlayers = ServerConsole._playersAmount;
			ServerConsole.PlayersListRaw.objects.Clear();
		}
		catch (Exception ex)
		{
			ServerConsole.AddLog("[VERIFICATION] Exception in Players Online processing: " + ex.Message, ConsoleColor.Gray, false);
			ServerConsole.AddLog(ex.StackTrace, ConsoleColor.Gray, false);
		}
	}

	// Token: 0x060008E1 RID: 2273 RVA: 0x00023974 File Offset: 0x00021B74
	private string StandardizedBoolComparison<T>(string source, IReadOnlyList<string> args, Func<bool, bool, T> comparison)
	{
		return this.StandardizedComparison<bool, T>(source, args, delegate(string arg)
		{
			bool item;
			return new ValueTuple<bool, bool>(bool.TryParse(arg, out item), item);
		}, comparison);
	}

	// Token: 0x060008E2 RID: 2274 RVA: 0x0002399E File Offset: 0x00021B9E
	private string StandardizedFloatComparison<T>(string source, IReadOnlyList<string> args, Func<float, float, T> comparison)
	{
		return this.StandardizedComparison<float, T>(source, args, delegate(string arg)
		{
			float item;
			return new ValueTuple<bool, float>(float.TryParse(arg, out item), item);
		}, comparison);
	}

	// Token: 0x060008E3 RID: 2275 RVA: 0x000239C8 File Offset: 0x00021BC8
	private string StandardizedComparison<TArg, TResult>(string source, IReadOnlyList<string> args, [TupleElementNames(new string[]
	{
		"success",
		"value"
	})] Func<string, ValueTuple<bool, TArg>> parse, Func<TArg, TArg, TResult> comparison)
	{
		if (args.Count != 3)
		{
			throw new CommandInputException("args", args, "Invalid arguments. Use " + source + ",[value A],[value B]");
		}
		string arg = this.NameFormatter.ProcessExpression(args[1]);
		ValueTuple<bool, TArg> valueTuple = parse(arg);
		bool item = valueTuple.Item1;
		TArg item2 = valueTuple.Item2;
		if (!item)
		{
			throw new CommandInputException("value A", args[1], "Could not parse.");
		}
		string text = this.NameFormatter.ProcessExpression(args[2]);
		ValueTuple<bool, TArg> valueTuple2 = parse(text);
		bool item3 = valueTuple2.Item1;
		TArg item4 = valueTuple2.Item2;
		if (!item3)
		{
			throw new CommandInputException("value B", text, "Could not parse.");
		}
		TResult tresult = comparison(item2, item4);
		return tresult.ToString();
	}

	// Token: 0x060008E4 RID: 2276 RVA: 0x00023A94 File Offset: 0x00021C94
	private string StandardizedFloatRound(string source, IReadOnlyList<string> args, Func<float, float> rounder)
	{
		int count = args.Count;
		float num;
		int num2;
		if (count != 2)
		{
			if (count != 3)
			{
				throw new CommandInputException("args", args, string.Concat(new string[]
				{
					"Invalid arguments. Use ",
					source,
					",[value] OR ",
					source,
					",[value],[precision]"
				}));
			}
			string text = this.NameFormatter.ProcessExpression(args[1]);
			if (!float.TryParse(text, out num))
			{
				throw new CommandInputException("value", text, "Could not parse.");
			}
			string text2 = this.NameFormatter.ProcessExpression(args[1]);
			if (!int.TryParse(text2, out num2))
			{
				throw new CommandInputException("precision", text2, "Could not parse.");
			}
		}
		else
		{
			string text = this.NameFormatter.ProcessExpression(args[1]);
			if (!float.TryParse(text, out num))
			{
				throw new CommandInputException("value", text, "Could not parse.");
			}
			num2 = 0;
		}
		float num3 = Mathf.Pow(10f, (float)num2);
		return (rounder(num * num3) / num3).ToString(CultureInfo.InvariantCulture);
	}

	// Token: 0x060008E5 RID: 2277 RVA: 0x00023BA9 File Offset: 0x00021DA9
	private static string GetRoundInfo(Func<RoundSummary, string> getter, bool intValue = false)
	{
		if (!(RoundSummary.singleton == null))
		{
			return getter(RoundSummary.singleton);
		}
		if (!intValue)
		{
			return "-";
		}
		return "-1";
	}

	// Token: 0x060008E6 RID: 2278 RVA: 0x00023BD2 File Offset: 0x00021DD2
	public string RefreshServerName()
	{
		return this.NameFormatter.ProcessExpression(ServerConsole.ServerName);
	}

	// Token: 0x060008E7 RID: 2279 RVA: 0x00023BE4 File Offset: 0x00021DE4
	public string RefreshServerNameSafe()
	{
		string text;
		if (this.NameFormatter.TryProcessExpression(ServerConsole.ServerName, "server name", out text))
		{
			SteamServerInfo.ServerName = Regex.Replace(text, "<[^>]*>", string.Empty);
			return text;
		}
		ServerConsole.AddLog(text, ConsoleColor.Gray, false);
		return "Command errored";
	}

	// Token: 0x060008E8 RID: 2280 RVA: 0x00023C2E File Offset: 0x00021E2E
	private void Awake()
	{
		ServerConsole.Singleton = this;
	}

	// Token: 0x060008E9 RID: 2281 RVA: 0x00023C38 File Offset: 0x00021E38
	private static void CheckProcess()
	{
		while (!ServerConsole._disposing)
		{
			Thread.Sleep(4000);
			if (ServerConsole.ConsoleProcess == null || ServerConsole.ConsoleProcess.HasExited)
			{
				Process consoleProcess = ServerConsole.ConsoleProcess;
				if (consoleProcess != null)
				{
					consoleProcess.Dispose();
				}
				ServerConsole.ConsoleProcess = null;
				ServerConsole.DisposeStatic();
			}
		}
	}

	// Token: 0x060008EA RID: 2282 RVA: 0x00023C86 File Offset: 0x00021E86
	public void OnDestroy()
	{
		this.Dispose();
	}

	// Token: 0x060008EB RID: 2283 RVA: 0x00023C86 File Offset: 0x00021E86
	public void OnApplicationQuit()
	{
		this.Dispose();
	}

	// Token: 0x060008EC RID: 2284 RVA: 0x00023C8E File Offset: 0x00021E8E
	public static void DisposeStatic()
	{
		ServerConsole.Singleton.Dispose();
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x00023C9A File Offset: 0x00021E9A
	public static void AddLog(string q, ConsoleColor color = ConsoleColor.Gray, bool hideFromOutputs = false)
	{
		ServerConsole.PrintFormattedString(q, color);
		if (!hideFromOutputs)
		{
			ServerConsole.PrintOnOutputs(q, color);
		}
	}

	// Token: 0x060008EE RID: 2286 RVA: 0x00023CAD File Offset: 0x00021EAD
	public static void AddOutputEntry(IOutputEntry entry)
	{
		IServerOutput serverOutput = ServerStatic.ServerOutput;
		if (serverOutput != null)
		{
			serverOutput.AddOutput(entry);
		}
		if (entry is TextOutputEntry)
		{
			ServerConsole.PrintOnOutputs(entry.ToString(), ConsoleColor.Gray);
		}
	}

	// Token: 0x060008EF RID: 2287 RVA: 0x00023CD4 File Offset: 0x00021ED4
	public static void Disconnect(GameObject player, string message)
	{
		if (player == null)
		{
			return;
		}
		NetworkBehaviour component = player.GetComponent<NetworkBehaviour>();
		if (component == null)
		{
			return;
		}
		CharacterClassManager component2 = player.GetComponent<CharacterClassManager>();
		if (component2 == null)
		{
			component.connectionToClient.Disconnect();
			return;
		}
		component2.DisconnectClient(component.connectionToClient, message);
	}

	// Token: 0x060008F0 RID: 2288 RVA: 0x00023D28 File Offset: 0x00021F28
	public static void Disconnect(NetworkConnection conn, string message)
	{
		GameObject gameObject = GameCore.Console.FindConnectedRoot(conn);
		if (gameObject == null)
		{
			conn.Disconnect();
			return;
		}
		ServerConsole.Disconnect(gameObject, message);
	}

	// Token: 0x060008F1 RID: 2289 RVA: 0x00023D53 File Offset: 0x00021F53
	public static string ColorText(string text, ConsoleColor color)
	{
		return string.Concat(new string[]
		{
			"<color=",
			ServerConsole.ConsoleColorToHex(color),
			">",
			text,
			"</color>"
		});
	}

	// Token: 0x060008F2 RID: 2290 RVA: 0x00023D85 File Offset: 0x00021F85
	public static void ColorDebugLog(string text, ConsoleColor color)
	{
		UnityEngine.Debug.Log(ServerConsole.ColorText(text, color), null);
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x00023D94 File Offset: 0x00021F94
	public static void PrintFormattedString(string text, ConsoleColor defaultColor)
	{
		text = ServerConsole._sizeRegex.Replace(text, "").Trim();
		string[] array = ServerConsole._colorRegex.Split(text);
		for (int i = 0; i < array.Length; i++)
		{
			string text2 = array[i];
			if (!text2.ToLowerInvariant().StartsWith("<color=", StringComparison.Ordinal))
			{
				IServerOutput serverOutput = ServerStatic.ServerOutput;
				if (serverOutput != null)
				{
					serverOutput.AddLog(text2, defaultColor);
				}
			}
			else
			{
				string text3 = text2.Substring(7);
				text3 = text3.Substring(0, text3.IndexOf('>')).Replace("\"", "").Replace("'", "");
				string text4 = text2.Substring(text2.IndexOf('>') + 1);
				text4 = text4.Substring(0, text4.IndexOf('<'));
				bool flag = text3.StartsWith("#", StringComparison.Ordinal);
				ConsoleColor color = defaultColor;
				Color32 c;
				ConsoleColor consoleColor;
				if (flag && Misc.TryParseColor(text3, out c))
				{
					color = Misc.ClosestConsoleColor(c, false);
				}
				else if (!flag && Enum.TryParse<ConsoleColor>(text3, true, out consoleColor))
				{
					color = consoleColor;
				}
				IServerOutput serverOutput2 = ServerStatic.ServerOutput;
				if (serverOutput2 != null)
				{
					serverOutput2.AddLog(text4, color);
				}
				i++;
			}
		}
	}

	// Token: 0x060008F4 RID: 2292 RVA: 0x00023EC0 File Offset: 0x000220C0
	public static string ConsoleColorToHex(ConsoleColor color)
	{
		switch (color)
		{
		case ConsoleColor.Black:
			return "#000000";
		case ConsoleColor.DarkBlue:
			return "#0058F8";
		case ConsoleColor.DarkGreen:
			return "#005800";
		case ConsoleColor.DarkCyan:
			return "#00B7EB";
		case ConsoleColor.DarkRed:
			return "#A80020";
		case ConsoleColor.DarkMagenta:
			return "#FF0090";
		case ConsoleColor.DarkYellow:
			return "#AC7C00";
		case ConsoleColor.Gray:
			return "#D8D8D8";
		case ConsoleColor.DarkGray:
			return "#787878";
		case ConsoleColor.Blue:
			return "#0078F8";
		case ConsoleColor.Green:
			return "#00B800";
		case ConsoleColor.Cyan:
			return "#00FFFF";
		case ConsoleColor.Red:
			return "#DC143C";
		case ConsoleColor.Magenta:
			return "#FF00FF";
		case ConsoleColor.Yellow:
			return "#F8B800";
		case ConsoleColor.White:
			return "#FCFCFC";
		default:
			return "#6844FC";
		}
	}

	// Token: 0x060008F5 RID: 2293 RVA: 0x00023F7C File Offset: 0x0002217C
	public static Color ConsoleColorToColor(ConsoleColor color)
	{
		switch (color)
		{
		case ConsoleColor.Black:
			return Color.black;
		case ConsoleColor.DarkBlue:
			return new Color(0f, 88f, 248f);
		case ConsoleColor.DarkGreen:
			return new Color(0f, 88f, 0f);
		case ConsoleColor.DarkCyan:
			return new Color(0f, 183f, 235f);
		case ConsoleColor.DarkRed:
			return new Color(168f, 0f, 32f);
		case ConsoleColor.DarkMagenta:
			return new Color(255f, 0f, 144f);
		case ConsoleColor.DarkYellow:
			return new Color(172f, 124f, 0f);
		case ConsoleColor.Gray:
			return new Color(216f, 216f, 216f);
		case ConsoleColor.DarkGray:
			return Color.gray;
		case ConsoleColor.Blue:
			return Color.blue;
		case ConsoleColor.Green:
			return Color.green;
		case ConsoleColor.Cyan:
			return Color.cyan;
		case ConsoleColor.Red:
			return Color.red;
		case ConsoleColor.Magenta:
			return Color.magenta;
		case ConsoleColor.Yellow:
			return Color.yellow;
		case ConsoleColor.White:
			return new Color(252f, 252f, 252f);
		default:
			return new Color(104f, 68f, 252f);
		}
	}

	// Token: 0x060008F6 RID: 2294 RVA: 0x000240C0 File Offset: 0x000222C0
	public static string EnterCommand(string cmds, CommandSender sender = null)
	{
		string[] args = cmds.Split(' ', StringSplitOptions.None);
		if (args.Length == 0)
		{
			return string.Empty;
		}
		if (sender == null)
		{
			sender = ServerConsole.Scs;
		}
		string cmd = args[0];
		if (!cmd.StartsWith("!", StringComparison.Ordinal) || cmd.Length <= 1)
		{
			return GameCore.Console.Singleton.TypeCommand(cmds, sender ?? ServerConsole.Scs);
		}
		if (cmd.StartsWith("!verify", StringComparison.OrdinalIgnoreCase) && !ServerConsole._emailSet)
		{
			return "You have to set the contact email address (\"contact_email\" key in the gameplay config) before running this command!";
		}
		new Thread(delegate()
		{
			string cmd = cmd.Substring(1).ToLower();
			string args;
			if (args.Length != 1)
			{
				args = args.Skip(1).Aggregate((string current, string next) => current + " " + next);
			}
			else
			{
				args = "";
			}
			ServerConsole.RunCentralServerCommand(cmd, args);
		})
		{
			IsBackground = true,
			Priority = System.Threading.ThreadPriority.AboveNormal,
			Name = "SCP:SL Central server command execution"
		}.Start();
		return "Sending command to central servers...";
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x00024194 File Offset: 0x00022394
	public void RunServer()
	{
		Thread verificationRequestThread = ServerConsole._verificationRequestThread;
		if (verificationRequestThread != null && verificationRequestThread.IsAlive)
		{
			return;
		}
		ServerConsole._verificationRequestThread = new Thread(new ThreadStart(this.RefreshServerData))
		{
			IsBackground = true,
			Priority = System.Threading.ThreadPriority.AboveNormal,
			Name = "SCP:SL Server list thread"
		};
		ServerConsole._verificationRequestThread.Start();
	}

	// Token: 0x060008F8 RID: 2296 RVA: 0x000241EC File Offset: 0x000223EC
	internal static void RunRefreshPublicKey()
	{
		Thread refreshPublicKeyThread = ServerConsole._refreshPublicKeyThread;
		if (refreshPublicKeyThread != null && refreshPublicKeyThread.IsAlive)
		{
			return;
		}
		ThreadStart start;
		if ((start = ServerConsole.<>O.<2>__RefreshPublicKey) == null)
		{
			start = (ServerConsole.<>O.<2>__RefreshPublicKey = new ThreadStart(ServerConsole.RefreshPublicKey));
		}
		ServerConsole._refreshPublicKeyThread = new Thread(start)
		{
			IsBackground = true,
			Priority = System.Threading.ThreadPriority.Normal,
			Name = "SCP:SL Public key refreshing"
		};
		ServerConsole._refreshPublicKeyThread.Start();
	}

	// Token: 0x060008F9 RID: 2297 RVA: 0x00024254 File Offset: 0x00022454
	internal static void RunRefreshPublicKeyOnce()
	{
		Thread refreshPublicKeyOnceThread = ServerConsole._refreshPublicKeyOnceThread;
		if (refreshPublicKeyOnceThread != null && refreshPublicKeyOnceThread.IsAlive)
		{
			return;
		}
		ThreadStart start;
		if ((start = ServerConsole.<>O.<3>__RefreshPublicKeyOnce) == null)
		{
			start = (ServerConsole.<>O.<3>__RefreshPublicKeyOnce = new ThreadStart(ServerConsole.RefreshPublicKeyOnce));
		}
		ServerConsole._refreshPublicKeyOnceThread = new Thread(start)
		{
			IsBackground = true,
			Priority = System.Threading.ThreadPriority.AboveNormal,
			Name = "SCP:SL Public key refreshing ON DEMAND"
		};
		ServerConsole._refreshPublicKeyOnceThread.Start();
	}

	// Token: 0x060008FA RID: 2298 RVA: 0x000242BC File Offset: 0x000224BC
	private static void RefreshPublicKey()
	{
		string text = CentralServerKeyCache.ReadCache();
		string text2 = string.Empty;
		string b = string.Empty;
		bool flag = true;
		if (!string.IsNullOrEmpty(text))
		{
			ServerConsole.PublicKey = ECDSA.PublicKeyFromString(text);
			text2 = Sha.HashToString(Sha.Sha256(ECDSA.KeyToString(ServerConsole.PublicKey)));
			ServerConsole.AddLog("Loaded central server public key from cache.", ConsoleColor.Gray, false);
			ServerConsole.AddLog("SHA256 of public key: " + text2, ConsoleColor.Gray, false);
		}
		ServerConsole.AddLog("Downloading public key from central server...", ConsoleColor.Gray, false);
		while (!ServerConsole._disposing)
		{
			try
			{
				PublicKeyResponse publicKeyResponse = JsonSerialize.FromJson<PublicKeyResponse>(HttpQuery.Get(string.Format("{0}v5/publickey.php?major={1}", CentralServer.StandardUrl, GameCore.Version.Major)));
				if (!ECDSA.Verify(publicKeyResponse.key, publicKeyResponse.signature, CentralServerKeyCache.MasterKey))
				{
					GameCore.Console.AddLog("Can't refresh central server public key - invalid signature!", Color.red, false, ConsoleLogType.Log);
					Thread.Sleep(360000);
					continue;
				}
				ServerConsole.PublicKey = ECDSA.PublicKeyFromString(publicKeyResponse.key);
				string text3 = Sha.HashToString(Sha.Sha256(ECDSA.KeyToString(ServerConsole.PublicKey)));
				if (text3 != b)
				{
					b = text3;
					ServerConsole.AddLog("Downloaded public key from central server.", ConsoleColor.Gray, false);
					ServerConsole.AddLog("SHA256 of public key: " + text3, ConsoleColor.Gray, false);
					if (text3 != text2)
					{
						CentralServerKeyCache.SaveCache(publicKeyResponse.key, publicKeyResponse.signature);
					}
					else
					{
						ServerConsole.AddLog("SHA256 of cached key matches, no need to update cache.", ConsoleColor.Gray, false);
					}
				}
				else if (flag)
				{
					flag = false;
					ServerConsole.AddLog("Refreshed public key of central server - key hash not changed.", ConsoleColor.Gray, false);
				}
			}
			catch (Exception ex)
			{
				ServerConsole.AddLog("Can't refresh central server public key - " + ex.Message, ConsoleColor.Gray, false);
			}
			Thread.Sleep(360000);
		}
	}

	// Token: 0x060008FB RID: 2299 RVA: 0x00024468 File Offset: 0x00022668
	private static void RefreshPublicKeyOnce()
	{
		try
		{
			PublicKeyResponse publicKeyResponse = JsonSerialize.FromJson<PublicKeyResponse>(HttpQuery.Get(string.Format("{0}v5/publickey.php?major={1}", CentralServer.StandardUrl, GameCore.Version.Major)));
			if (!ECDSA.Verify(publicKeyResponse.key, publicKeyResponse.signature, CentralServerKeyCache.MasterKey))
			{
				GameCore.Console.AddLog("Can't refresh central server public key - invalid signature!", Color.red, false, ConsoleLogType.Log);
			}
			else
			{
				ServerConsole.PublicKey = ECDSA.PublicKeyFromString(publicKeyResponse.key);
				string str = Sha.HashToString(Sha.Sha256(ECDSA.KeyToString(ServerConsole.PublicKey)));
				ServerConsole.AddLog("Downloaded public key from central server.", ConsoleColor.Gray, false);
				ServerConsole.AddLog("SHA256 of public key: " + str, ConsoleColor.Gray, false);
				CentralServerKeyCache.SaveCache(publicKeyResponse.key, publicKeyResponse.signature);
			}
		}
		catch (Exception ex)
		{
			ServerConsole.AddLog("Can't refresh central server public key - " + ex.Message, ConsoleColor.Gray, false);
		}
	}

	// Token: 0x060008FC RID: 2300 RVA: 0x00024544 File Offset: 0x00022744
	private static void RunCentralServerCommand(string cmd, string args)
	{
		cmd = cmd.ToLower();
		List<string> list = new List<string>
		{
			"ip=" + ServerConsole.Ip,
			string.Format("port={0}", ServerConsole.PortToReport),
			"cmd=" + StringUtils.Base64Encode(cmd),
			"args=" + StringUtils.Base64Encode(args)
		};
		if (!string.IsNullOrEmpty(ServerConsole.Password))
		{
			list.Add("passcode=" + ServerConsole.Password);
		}
		try
		{
			string str = HttpQuery.Post(CentralServer.MasterUrl + "centralcommands/" + cmd + ".php", HttpQuery.ToPostArgs(list), "application/x-www-form-urlencoded");
			ServerConsole.AddLog("[" + cmd + "] " + str, ConsoleColor.Gray, false);
		}
		catch (Exception ex)
		{
			ServerConsole.AddLog("Could not execute the central server command \"" + cmd + "\" - (LOCAL EXCEPTION) " + ex.Message, ConsoleColor.Red, false);
		}
	}

	// Token: 0x1700015E RID: 350
	// (get) Token: 0x060008FD RID: 2301 RVA: 0x00024650 File Offset: 0x00022850
	internal static ushort PortToReport
	{
		get
		{
			if (ServerConsole.PortOverride != 0)
			{
				return ServerConsole.PortOverride;
			}
			return LiteNetLib4MirrorTransport.Singleton.port;
		}
	}

	// Token: 0x060008FE RID: 2302 RVA: 0x00024669 File Offset: 0x00022869
	internal static void RefreshEmailSetStatus()
	{
		ServerConsole._emailSet = !string.IsNullOrEmpty(ConfigFile.ServerConfig.GetString("contact_email", ""));
	}

	// Token: 0x060008FF RID: 2303 RVA: 0x0002468C File Offset: 0x0002288C
	private void RefreshServerData()
	{
		bool flag = true;
		byte b = 0;
		ServerConsole.RefreshEmailSetStatus();
		ServerConsole.RefreshToken(true);
		while (!ServerConsole._disposing)
		{
			b += 1;
			if (!flag && string.IsNullOrEmpty(ServerConsole.Password) && b < 15)
			{
				if (b == 5 || b == 12 || ServerConsole.ScheduleTokenRefresh)
				{
					ServerConsole.RefreshToken(false);
				}
			}
			else
			{
				flag = false;
				ServerConsole.Update = (ServerConsole.Update || b == 10);
				string str = string.Empty;
				try
				{
					int count = ServerConsole.NewPlayers.Count;
					int num = 0;
					List<AuthenticatorPlayerObject> list = ListPool<AuthenticatorPlayerObject>.Shared.Rent();
					while (!ServerConsole.NewPlayers.IsEmpty)
					{
						num++;
						if (num > count + 30)
						{
							break;
						}
						try
						{
							ReferenceHub referenceHub;
							if (ServerConsole.NewPlayers.TryTake(out referenceHub) && referenceHub != null)
							{
								list.Add(new AuthenticatorPlayerObject(referenceHub.authManager.UserId, (referenceHub.authManager.connectionToClient == null || string.IsNullOrEmpty(referenceHub.authManager.connectionToClient.address)) ? "N/A" : referenceHub.authManager.connectionToClient.address, referenceHub.authManager.AuthenticationResponse.AuthToken.RequestIp, referenceHub.authManager.AuthenticationResponse.AuthToken.Asn.ToString(), referenceHub.authManager.AuthenticationResponse.AuthToken.Serial, referenceHub.authManager.AuthenticationResponse.AuthToken.VacSession));
							}
						}
						catch (Exception ex)
						{
							ServerConsole.AddLog("[VERIFICATION THREAD] Exception in New Player (inside of loop) processing: " + ex.Message, ConsoleColor.Gray, false);
							ServerConsole.AddLog(ex.StackTrace, ConsoleColor.Gray, false);
						}
					}
					str = JsonSerialize.ToJson<AuthenticatorPlayerObjects>(new AuthenticatorPlayerObjects(list));
					ListPool<AuthenticatorPlayerObject>.Shared.Return(list);
				}
				catch (Exception ex2)
				{
					ServerConsole.AddLog("[VERIFICATION THREAD] Exception in New Players processing: " + ex2.Message, ConsoleColor.Gray, false);
					ServerConsole.AddLog(ex2.StackTrace, ConsoleColor.Gray, false);
				}
				bool flag2 = ServerConsole.TransparentlyModdedServerConfig;
				flag2 |= Server.IsTransparentlyModded;
				List<string> list3;
				if (!ServerConsole.Update)
				{
					List<string> list2 = new List<string>();
					list2.Add("ip=" + ServerConsole.Ip);
					list2.Add("players=" + ServerConsole._playersAmount.ToString() + "/" + CustomNetworkManager.slots.ToString());
					list2.Add("newPlayers=" + str);
					list2.Add("port=" + ServerConsole.PortToReport.ToString());
					list3 = list2;
					list2.Add("version=2");
				}
				else
				{
					List<string> list4 = new List<string>();
					list4.Add("ip=" + ServerConsole.Ip);
					list4.Add("players=" + ServerConsole._playersAmount.ToString() + "/" + CustomNetworkManager.slots.ToString());
					list4.Add("playersList=" + ServerConsole._verificationPlayersList);
					list4.Add("newPlayers=" + str);
					list4.Add("port=" + ServerConsole.PortToReport.ToString());
					list4.Add("pastebin=" + ConfigFile.ServerConfig.GetString("serverinfo_pastebin_id", "7wV681fT"));
					list4.Add("gameVersion=" + GameCore.Version.VersionString);
					list4.Add("version=2");
					list4.Add("update=1");
					list4.Add("info=" + StringUtils.Base64Encode(this.RefreshServerNameSafe()).Replace('+', '-'));
					list4.Add("privateBeta=" + GameCore.Version.PrivateBeta.ToString());
					list4.Add("staffRA=" + ServerStatic.PermissionsHandler.StaffAccess.ToString());
					list4.Add("friendlyFire=" + ServerConsole.FriendlyFire.ToString());
					string str2 = "geoblocking=";
					byte geoblocking = (byte)CustomLiteNetLib4MirrorTransport.Geoblocking;
					list4.Add(str2 + geoblocking.ToString());
					list4.Add("modded=" + (CustomNetworkManager.Modded || ServerConsole.TransparentlyModdedServerConfig).ToString());
					list4.Add("tModded=" + flag2.ToString());
					list4.Add("whitelist=" + ServerConsole.WhiteListEnabled.ToString());
					list4.Add("accessRestriction=" + ServerConsole.AccessRestriction.ToString());
					list4.Add("emailSet=" + ServerConsole._emailSet.ToString());
					list3 = list4;
					list4.Add("enforceSameIp=" + ServerConsole.EnforceSameIp.ToString());
				}
				List<string> list5 = list3;
				if (!string.IsNullOrEmpty(ServerConsole.Password))
				{
					list5.Add("passcode=" + ServerConsole.Password);
				}
				ServerConsole.Update = false;
				if (!AuthenticatorQuery.SendData(list5) && !ServerConsole._printedNotVerifiedMessage)
				{
					ServerConsole._printedNotVerifiedMessage = true;
					ServerConsole.AddLog("Your server won't be visible on the public server list - (" + ServerConsole.Ip + ")", ConsoleColor.Red, false);
					if (!ServerConsole._emailSet)
					{
						ServerConsole.AddLog("If you are 100% sure that the server is working, can be accessed from the Internet and YOU WANT TO MAKE IT PUBLIC, please set up your email in configuration file (\"contact_email\" value) and restart the server.", ConsoleColor.Red, false);
					}
					else
					{
						ServerConsole.AddLog("If you want to make your server PUBLIC, please make sure that:", ConsoleColor.Red, false);
						ServerConsole.AddLog("- Your server is working;", ConsoleColor.Red, false);
						ServerConsole.AddLog("- It can be accessed from the internet without the use of a VPN/proxy;", ConsoleColor.Red, false);
						ServerConsole.AddLog("- It complies with the CSG (PLEASE READ: https://scpslgame.com/CSG.pdf).", ConsoleColor.Red, false);
						ServerConsole.AddLog("If you have checked the above, you can use the following command:", ConsoleColor.Red, false);
						ServerConsole.AddLog("!verify static", ConsoleColor.Red, false);
						ServerConsole.AddLog("If you know that you have a dynamic IP, please use this command instead:", ConsoleColor.Red, false);
						ServerConsole.AddLog("!verify dynamic", ConsoleColor.Red, false);
					}
				}
				else
				{
					ServerConsole._printedNotVerifiedMessage = true;
				}
			}
			if (b >= 15)
			{
				b = 0;
			}
			Thread.Sleep(5000);
			if (ServerConsole.ScheduleTokenRefresh || b == 0)
			{
				ServerConsole.RefreshToken(false);
			}
		}
	}

	// Token: 0x06000900 RID: 2304 RVA: 0x00024C94 File Offset: 0x00022E94
	private static void PrintOnOutputs(string text, ConsoleColor color)
	{
		try
		{
			if (ServerConsole.ConsoleOutputs != null)
			{
				using (IEnumerator<KeyValuePair<string, IOutput>> enumerator = ServerConsole.ConsoleOutputs.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, IOutput> output = enumerator.Current;
						try
						{
							if (output.Value == null || !output.Value.Available())
							{
								IOutput output2;
								ServerConsole.ConsoleOutputs.TryRemove(output.Key, out output2);
							}
							else
							{
								MainThreadDispatcher.Dispatch(delegate
								{
									output.Value.Print(text, color);
								}, MainThreadDispatcher.DispatchTime.Update);
							}
						}
						catch
						{
							IOutput output2;
							ServerConsole.ConsoleOutputs.TryRemove(output.Key, out output2);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			ServerConsole.AddLog("Failed to print to outputs: " + ex.Message + "\n" + ex.StackTrace, ConsoleColor.Red, true);
		}
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x00024DBC File Offset: 0x00022FBC
	private static void RefreshToken(bool init = false)
	{
		ServerConsole.ScheduleTokenRefresh = false;
		string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SCP Secret Laboratory/verkey.txt";
		if (!File.Exists(path))
		{
			return;
		}
		using (StreamReader streamReader = new StreamReader(path))
		{
			string text = streamReader.ReadToEnd().Trim();
			if (!init && string.IsNullOrEmpty(ServerConsole.Password) && !string.IsNullOrEmpty(text))
			{
				ServerConsole.AddLog("Verification token loaded! Server probably will be listed on public list.", ConsoleColor.Gray, false);
			}
			if (ServerConsole.Password != text)
			{
				ServerConsole.AddLog("Verification token reloaded.", ConsoleColor.Gray, false);
				ServerConsole.Update = true;
			}
			ServerConsole.Password = text;
			CustomNetworkManager.IsVerified = true;
		}
	}

	// Token: 0x0400080E RID: 2062
	public static string ServerName = string.Empty;

	// Token: 0x0400080F RID: 2063
	private static readonly Func<float, float> _roundNormal = new Func<float, float>(Mathf.Round);

	// Token: 0x04000810 RID: 2064
	private static readonly Func<float, float> _roundCeil = new Func<float, float>(Mathf.Ceil);

	// Token: 0x04000811 RID: 2065
	private static readonly Func<float, float> _roundFloor = new Func<float, float>(Mathf.Floor);

	// Token: 0x04000812 RID: 2066
	private static readonly Func<float, float, float> _pow = new Func<float, float, float>(Mathf.Pow);

	// Token: 0x04000813 RID: 2067
	private static bool _disposing;

	// Token: 0x04000814 RID: 2068
	public static Process ConsoleProcess;

	// Token: 0x04000815 RID: 2069
	public static string Password;

	// Token: 0x04000816 RID: 2070
	public static string Ip;

	// Token: 0x04000817 RID: 2071
	public static AsymmetricKeyParameter PublicKey;

	// Token: 0x04000818 RID: 2072
	public static bool Update;

	// Token: 0x04000819 RID: 2073
	public static bool ScheduleTokenRefresh;

	// Token: 0x0400081A RID: 2074
	public static bool FriendlyFire = false;

	// Token: 0x0400081B RID: 2075
	public static bool WhiteListEnabled = false;

	// Token: 0x0400081C RID: 2076
	public static bool AccessRestriction = false;

	// Token: 0x0400081D RID: 2077
	public static bool RateLimitKick;

	// Token: 0x0400081E RID: 2078
	internal static bool EnforceSameIp;

	// Token: 0x0400081F RID: 2079
	internal static bool SkipEnforcementForLocalAddresses;

	// Token: 0x04000820 RID: 2080
	public static bool TransparentlyModdedServerConfig;

	// Token: 0x04000821 RID: 2081
	private static bool _printedNotVerifiedMessage;

	// Token: 0x04000822 RID: 2082
	private static bool _emailSet;

	// Token: 0x04000823 RID: 2083
	private static float _heartbeatTimer;

	// Token: 0x04000824 RID: 2084
	public static readonly ServerConsoleSender Scs = new ServerConsoleSender();

	// Token: 0x04000825 RID: 2085
	public static ushort PortOverride;

	// Token: 0x04000826 RID: 2086
	private static readonly ConcurrentBag<ReferenceHub> NewPlayers = new ConcurrentBag<ReferenceHub>();

	// Token: 0x04000827 RID: 2087
	private static int _playersAmount;

	// Token: 0x04000828 RID: 2088
	public static readonly ConcurrentDictionary<string, IOutput> ConsoleOutputs = new ConcurrentDictionary<string, IOutput>();

	// Token: 0x04000829 RID: 2089
	internal static readonly ConcurrentQueue<string> PrompterQueue = new ConcurrentQueue<string>();

	// Token: 0x0400082A RID: 2090
	private static readonly PlayerListSerialized PlayersListRaw = new PlayerListSerialized(new List<string>());

	// Token: 0x0400082B RID: 2091
	private static string _verificationPlayersList = string.Empty;

	// Token: 0x0400082C RID: 2092
	private static Thread _checkProcessThread;

	// Token: 0x0400082D RID: 2093
	private static Thread _refreshPublicKeyThread;

	// Token: 0x0400082E RID: 2094
	private static Thread _refreshPublicKeyOnceThread;

	// Token: 0x0400082F RID: 2095
	private static Thread _verificationRequestThread;

	// Token: 0x04000830 RID: 2096
	private static readonly Regex _sizeRegex = new Regex("(<size=(.*?)<\\/size>)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	// Token: 0x04000831 RID: 2097
	private static readonly Regex _colorRegex = new Regex("(<color=(.*?)<\\/color>)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

	// Token: 0x020001C1 RID: 449
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x04000833 RID: 2099
		public static Action<ReferenceHub, ClientInstanceMode> <0>__HandlePlayerJoin;

		// Token: 0x04000834 RID: 2100
		public static ThreadStart <1>__CheckProcess;

		// Token: 0x04000835 RID: 2101
		public static ThreadStart <2>__RefreshPublicKey;

		// Token: 0x04000836 RID: 2102
		public static ThreadStart <3>__RefreshPublicKeyOnce;
	}
}
