using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using GameCore;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using NorthwoodLib.Pools;
using UnityEngine;

// Token: 0x020001B2 RID: 434
public class ServerLogs : MonoBehaviour
{
	// Token: 0x06000880 RID: 2176 RVA: 0x00021914 File Offset: 0x0001FB14
	static ServerLogs()
	{
		foreach (string text in ServerLogs.Txt)
		{
			ServerLogs._maxlen = Math.Max(ServerLogs._maxlen, text.Length);
		}
		foreach (string text2 in ServerLogs.Modulestxt)
		{
			ServerLogs._modulemaxlen = Math.Max(ServerLogs._modulemaxlen, text2.Length);
		}
	}

	// Token: 0x06000881 RID: 2177 RVA: 0x00021A68 File Offset: 0x0001FC68
	internal static void StartLogging()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (ServerLogs._state != ServerLogs.LoggingState.Off)
		{
			ServerLogs._state = ServerLogs.LoggingState.Restart;
			return;
		}
		Thread appendThread = ServerLogs._appendThread;
		if (appendThread != null && appendThread.IsAlive)
		{
			return;
		}
		ThreadStart start;
		if ((start = ServerLogs.<>O.<0>__AppendLog) == null)
		{
			start = (ServerLogs.<>O.<0>__AppendLog = new ThreadStart(ServerLogs.AppendLog));
		}
		ServerLogs._appendThread = new Thread(start)
		{
			Name = "Saving server logs to file",
			Priority = System.Threading.ThreadPriority.BelowNormal,
			IsBackground = true
		};
		ServerLogs._appendThread.Start();
	}

	// Token: 0x06000882 RID: 2178 RVA: 0x00021AEC File Offset: 0x0001FCEC
	public static void AddLog(ServerLogs.Modules module, string msg, ServerLogs.ServerLogType type, bool init = false)
	{
		string time = TimeBehaviour.Rfc3339Time();
		object lockObject = ServerLogs.LockObject;
		lock (lockObject)
		{
			ServerLogs.Queue.Enqueue(new ServerLogs.ServerLog(msg, ServerLogs.Txt[(int)type], ServerLogs.Modulestxt[(int)module], time));
		}
		if (init)
		{
			return;
		}
		ServerLogs._state = ServerLogs.LoggingState.Write;
	}

	// Token: 0x06000883 RID: 2179 RVA: 0x00021B58 File Offset: 0x0001FD58
	private void OnApplicationQuit()
	{
		ServerLogs._state = ServerLogs.LoggingState.Terminate;
	}

	// Token: 0x06000884 RID: 2180 RVA: 0x00021B64 File Offset: 0x0001FD64
	private static void AppendLog()
	{
		ServerLogs._state = ServerLogs.LoggingState.Standby;
		StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
		while (ServerLogs._state != ServerLogs.LoggingState.Terminate)
		{
			object lockObject = ServerLogs.LockObject;
			lock (lockObject)
			{
				ServerLogs.Queue.Clear();
				ServerLogs._state = ServerLogs.LoggingState.Standby;
				goto IL_70;
			}
			goto IL_48;
			IL_70:
			if (NetworkServer.active)
			{
				string text = TimeBehaviour.FormatTime("yyyy-MM-dd HH.mm.ss");
				string text2 = LiteNetLib4MirrorTransport.Singleton.port.ToString();
				ServerLogs.AddLog(ServerLogs.Modules.GameLogic, "Started logging.", ServerLogs.ServerLogType.InternalMessage, true);
				ServerLogs.AddLog(ServerLogs.Modules.GameLogic, "Game version: " + GameCore.Version.VersionString + ".", ServerLogs.ServerLogType.InternalMessage, true);
				ServerLogs.AddLog(ServerLogs.Modules.GameLogic, "Build type: " + GameCore.Version.BuildType.ToString() + ".", ServerLogs.ServerLogType.InternalMessage, true);
				ServerLogs.AddLog(ServerLogs.Modules.GameLogic, "Build timestamp: 2026-10-12 18:32:38Z.", ServerLogs.ServerLogType.InternalMessage, true);
				ServerLogs.Modules module = ServerLogs.Modules.GameLogic;
				string str = "Headless: ";
				bool flag = PlatformInfo.singleton.IsHeadless;
				ServerLogs.AddLog(module, str + flag.ToString() + ".", ServerLogs.ServerLogType.InternalMessage, true);
				while (NetworkServer.active && ServerLogs._state != ServerLogs.LoggingState.Terminate && ServerLogs._state != ServerLogs.LoggingState.Restart)
				{
					Thread.Sleep(100);
					if (ServerLogs._state != ServerLogs.LoggingState.Standby)
					{
						if (!Directory.Exists(FileManager.GetAppFolder(true, false, "")))
						{
							return;
						}
						if (!Directory.Exists(FileManager.GetAppFolder(true, false, "") + "ServerLogs"))
						{
							Directory.CreateDirectory(FileManager.GetAppFolder(true, false, "") + "ServerLogs");
						}
						if (!Directory.Exists(FileManager.GetAppFolder(true, false, "") + "ServerLogs/" + text2))
						{
							Directory.CreateDirectory(FileManager.GetAppFolder(true, false, "") + "ServerLogs/" + text2);
						}
						lockObject = ServerLogs.LockObject;
						lock (lockObject)
						{
							ServerLogs.ServerLog serverLog;
							while (ServerLogs.Queue.TryDequeue(out serverLog))
							{
								string text3 = string.Concat(new string[]
								{
									serverLog.Time,
									" | ",
									ServerLogs.ToMax(serverLog.Type, ServerLogs._maxlen),
									" | ",
									ServerLogs.ToMax(serverLog.Module, ServerLogs._modulemaxlen),
									" | ",
									serverLog.Content
								});
								stringBuilder.AppendLine(text3);
								ServerLogs.PrintOnOutputs(text3);
							}
						}
						using (StreamWriter streamWriter = new StreamWriter(string.Concat(new string[]
						{
							FileManager.GetAppFolder(true, false, ""),
							"ServerLogs/",
							text2,
							"/Round ",
							text,
							".txt"
						}), true))
						{
							streamWriter.Write(stringBuilder.ToString());
						}
						stringBuilder.Clear();
						ServerLogs.LoggingState state = ServerLogs._state;
						if (state == ServerLogs.LoggingState.Terminate || state == ServerLogs.LoggingState.Restart)
						{
							break;
						}
						ServerLogs._state = ServerLogs.LoggingState.Standby;
					}
				}
				continue;
			}
			IL_48:
			if (ServerLogs._state == ServerLogs.LoggingState.Terminate)
			{
				ServerLogs._state = ServerLogs.LoggingState.Off;
				StringBuilderPool.Shared.Return(stringBuilder);
				return;
			}
			Thread.Sleep(200);
			goto IL_70;
		}
		ServerLogs._state = ServerLogs.LoggingState.Off;
		StringBuilderPool.Shared.Return(stringBuilder);
	}

	// Token: 0x06000885 RID: 2181 RVA: 0x00021EB4 File Offset: 0x000200B4
	private static void PrintOnOutputs(string text)
	{
		try
		{
			if (ServerLogs.LiveLogOutput != null)
			{
				using (IEnumerator<KeyValuePair<string, IOutput>> enumerator = ServerLogs.LiveLogOutput.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, IOutput> output = enumerator.Current;
						try
						{
							if (output.Value == null || !output.Value.Available())
							{
								IOutput output2;
								ServerLogs.LiveLogOutput.TryRemove(output.Key, out output2);
							}
							else if (output.Value is ServerConsoleSender)
							{
								ServerConsole.AddLog(text, ConsoleColor.Gray, true);
							}
							else
							{
								MainThreadDispatcher.Dispatch(delegate
								{
									output.Value.Print(text);
								}, MainThreadDispatcher.DispatchTime.Update);
							}
						}
						catch
						{
							IOutput output2;
							ServerLogs.LiveLogOutput.TryRemove(output.Key, out output2);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			ServerConsole.AddLog("Failed to print log to outputs: " + ex.Message + "\n" + ex.StackTrace, ConsoleColor.Red, false);
		}
	}

	// Token: 0x06000886 RID: 2182 RVA: 0x00021FFC File Offset: 0x000201FC
	private static string ToMax(string text, int max)
	{
		while (text.Length < max)
		{
			text += " ";
		}
		return text;
	}

	// Token: 0x040007AA RID: 1962
	private static readonly string[] Txt = new string[]
	{
		"Connection update",
		"Remote Admin",
		"Remote Admin - Misc",
		"Kill",
		"Game Event",
		"Internal",
		"Auth Rate Limit",
		"Teamkill",
		"Suicide",
		"AdminChat",
		"Query"
	};

	// Token: 0x040007AB RID: 1963
	private static readonly string[] Modulestxt = new string[]
	{
		"Warhead",
		"Networking",
		"Class change",
		"Permissions",
		"Administrative",
		"Game logic",
		"Data access",
		"FF Detector",
		"Throwable",
		"Door",
		"Elevator"
	};

	// Token: 0x040007AC RID: 1964
	private static readonly Queue<ServerLogs.ServerLog> Queue = new Queue<ServerLogs.ServerLog>();

	// Token: 0x040007AD RID: 1965
	private static readonly object LockObject = new object();

	// Token: 0x040007AE RID: 1966
	public static readonly ConcurrentDictionary<string, IOutput> LiveLogOutput = new ConcurrentDictionary<string, IOutput>();

	// Token: 0x040007AF RID: 1967
	private static Thread _appendThread;

	// Token: 0x040007B0 RID: 1968
	private static readonly int _maxlen;

	// Token: 0x040007B1 RID: 1969
	private static readonly int _modulemaxlen;

	// Token: 0x040007B2 RID: 1970
	private static volatile ServerLogs.LoggingState _state;

	// Token: 0x020001B3 RID: 435
	public enum ServerLogType : byte
	{
		// Token: 0x040007B4 RID: 1972
		ConnectionUpdate,
		// Token: 0x040007B5 RID: 1973
		RemoteAdminActivity_GameChanging,
		// Token: 0x040007B6 RID: 1974
		RemoteAdminActivity_Misc,
		// Token: 0x040007B7 RID: 1975
		KillLog,
		// Token: 0x040007B8 RID: 1976
		GameEvent,
		// Token: 0x040007B9 RID: 1977
		InternalMessage,
		// Token: 0x040007BA RID: 1978
		AuthRateLimit,
		// Token: 0x040007BB RID: 1979
		Teamkill,
		// Token: 0x040007BC RID: 1980
		Suicide,
		// Token: 0x040007BD RID: 1981
		AdminChat,
		// Token: 0x040007BE RID: 1982
		Query
	}

	// Token: 0x020001B4 RID: 436
	public enum Modules : byte
	{
		// Token: 0x040007C0 RID: 1984
		Warhead,
		// Token: 0x040007C1 RID: 1985
		Networking,
		// Token: 0x040007C2 RID: 1986
		ClassChange,
		// Token: 0x040007C3 RID: 1987
		Permissions,
		// Token: 0x040007C4 RID: 1988
		Administrative,
		// Token: 0x040007C5 RID: 1989
		GameLogic,
		// Token: 0x040007C6 RID: 1990
		DataAccess,
		// Token: 0x040007C7 RID: 1991
		Detector,
		// Token: 0x040007C8 RID: 1992
		Throwable,
		// Token: 0x040007C9 RID: 1993
		Door,
		// Token: 0x040007CA RID: 1994
		Elevator
	}

	// Token: 0x020001B5 RID: 437
	private enum LoggingState : byte
	{
		// Token: 0x040007CC RID: 1996
		Off,
		// Token: 0x040007CD RID: 1997
		Standby,
		// Token: 0x040007CE RID: 1998
		Write,
		// Token: 0x040007CF RID: 1999
		Terminate,
		// Token: 0x040007D0 RID: 2000
		Restart
	}

	// Token: 0x020001B6 RID: 438
	public readonly struct ServerLog : IEquatable<ServerLogs.ServerLog>
	{
		// Token: 0x06000888 RID: 2184 RVA: 0x00022017 File Offset: 0x00020217
		public ServerLog(string content, string type, string module, string time)
		{
			this.Content = content;
			this.Type = type;
			this.Module = module;
			this.Time = time;
		}

		// Token: 0x06000889 RID: 2185 RVA: 0x00022038 File Offset: 0x00020238
		public bool Equals(ServerLogs.ServerLog other)
		{
			return this.Content == other.Content && this.Type == other.Type && this.Module == other.Module && this.Time == other.Time;
		}

		// Token: 0x0600088A RID: 2186 RVA: 0x00022094 File Offset: 0x00020294
		public override bool Equals(object obj)
		{
			if (obj is ServerLogs.ServerLog)
			{
				ServerLogs.ServerLog other = (ServerLogs.ServerLog)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x0600088B RID: 2187 RVA: 0x000220BC File Offset: 0x000202BC
		public override int GetHashCode()
		{
			return ((((this.Content != null) ? this.Content.GetHashCode() : 0) * 397 ^ ((this.Type != null) ? this.Type.GetHashCode() : 0)) * 397 ^ ((this.Module != null) ? this.Module.GetHashCode() : 0)) * 397 ^ ((this.Time != null) ? this.Time.GetHashCode() : 0);
		}

		// Token: 0x0600088C RID: 2188 RVA: 0x00022136 File Offset: 0x00020336
		public static bool operator ==(ServerLogs.ServerLog left, ServerLogs.ServerLog right)
		{
			return left.Equals(right);
		}

		// Token: 0x0600088D RID: 2189 RVA: 0x00022140 File Offset: 0x00020340
		public static bool operator !=(ServerLogs.ServerLog left, ServerLogs.ServerLog right)
		{
			return !left.Equals(right);
		}

		// Token: 0x040007D1 RID: 2001
		public readonly string Content;

		// Token: 0x040007D2 RID: 2002
		public readonly string Type;

		// Token: 0x040007D3 RID: 2003
		public readonly string Module;

		// Token: 0x040007D4 RID: 2004
		public readonly string Time;
	}

	// Token: 0x020001B7 RID: 439
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x040007D5 RID: 2005
		public static ThreadStart <0>__AppendLog;
	}
}
