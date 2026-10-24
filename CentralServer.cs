using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using GameCore;
using UnityEngine;

// Token: 0x0200017B RID: 379
public class CentralServer : MonoBehaviour
{
	// Token: 0x17000130 RID: 304
	// (get) Token: 0x06000787 RID: 1927 RVA: 0x0001C844 File Offset: 0x0001AA44
	// (set) Token: 0x06000788 RID: 1928 RVA: 0x0001C84B File Offset: 0x0001AA4B
	public static string CurrentDomain { get; private set; } = "scpslgame.com";

	// Token: 0x17000131 RID: 305
	// (get) Token: 0x06000789 RID: 1929 RVA: 0x0001C853 File Offset: 0x0001AA53
	// (set) Token: 0x0600078A RID: 1930 RVA: 0x0001C85A File Offset: 0x0001AA5A
	public static string MasterUrl { get; private set; }

	// Token: 0x17000132 RID: 306
	// (get) Token: 0x0600078B RID: 1931 RVA: 0x0001C862 File Offset: 0x0001AA62
	// (set) Token: 0x0600078C RID: 1932 RVA: 0x0001C869 File Offset: 0x0001AA69
	public static string StandardUrl { get; private set; }

	// Token: 0x17000133 RID: 307
	// (get) Token: 0x0600078D RID: 1933 RVA: 0x0001C871 File Offset: 0x0001AA71
	// (set) Token: 0x0600078E RID: 1934 RVA: 0x0001C878 File Offset: 0x0001AA78
	public static string SelectedServer { get; internal set; }

	// Token: 0x17000134 RID: 308
	// (get) Token: 0x0600078F RID: 1935 RVA: 0x0001C880 File Offset: 0x0001AA80
	// (set) Token: 0x06000790 RID: 1936 RVA: 0x0001C887 File Offset: 0x0001AA87
	public static bool TestServer { get; internal set; }

	// Token: 0x17000135 RID: 309
	// (get) Token: 0x06000791 RID: 1937 RVA: 0x0001C88F File Offset: 0x0001AA8F
	// (set) Token: 0x06000792 RID: 1938 RVA: 0x0001C896 File Offset: 0x0001AA96
	public static bool ServerSelected { get; private set; }

	// Token: 0x17000136 RID: 310
	// (get) Token: 0x06000793 RID: 1939 RVA: 0x0001C89E File Offset: 0x0001AA9E
	// (set) Token: 0x06000794 RID: 1940 RVA: 0x0001C8A5 File Offset: 0x0001AAA5
	internal static string[] Servers { get; private set; }

	// Token: 0x06000795 RID: 1941 RVA: 0x0001C8AD File Offset: 0x0001AAAD
	private void Start()
	{
		CentralServer.Init();
	}

	// Token: 0x06000796 RID: 1942 RVA: 0x0001C8B4 File Offset: 0x0001AAB4
	internal static void Init()
	{
		if (CentralServer._started)
		{
			return;
		}
		CentralServer._started = true;
		if (HttpQuery.WeakSecurity)
		{
			CentralServer.CurrentDomain = "scpsecretlaboratory.com";
		}
		CentralServer._lastReset = DateTime.MinValue;
		CentralServer.Servers = Array.Empty<string>();
		CentralServer._workingServers = new List<string>();
		CentralServer._refreshLock = new object();
		CentralServer._serversPath = FileManager.GetAppFolder(true, false, "") + "internal/";
		CentralServer.LoadPrefs();
		if (CentralServer.SelectedServer != null)
		{
			CentralServer.ServerSelected = true;
			ServerConsole.AddLog(string.Concat(new string[]
			{
				"Using central server selected by the startup arg: ",
				CentralServer.SelectedServer,
				" (",
				CentralServer.StandardUrl,
				")"
			}), ConsoleColor.Gray, false);
			return;
		}
		if (!File.Exists(FileManager.GetAppFolder(true, false, "") + "testserver.txt"))
		{
			if (!StartupArgs.Args.Any((string arg) => string.Equals(arg, "-cstest", StringComparison.OrdinalIgnoreCase)))
			{
				CentralServer.TestServer = false;
				CentralServer.UpdateUrl();
				if (!Directory.Exists(CentralServer._serversPath))
				{
					Directory.CreateDirectory(CentralServer._serversPath);
				}
				CentralServer._serversPath += "CentralServers";
				if (File.Exists(CentralServer._serversPath))
				{
					CentralServer.Servers = FileManager.ReadAllLines(CentralServer._serversPath);
					if (CentralServer.Servers.Any((string server) => !CentralServer.CentralServerRegex.IsMatch(server)))
					{
						GameCore.Console.AddLog("Malformed server found on the list. Removing the list and redownloading it from api." + CentralServer.CurrentDomain + ".", Color.yellow, false, ConsoleLogType.Log);
						CentralServer.Servers = Array.Empty<string>();
						try
						{
							File.Delete(CentralServer._serversPath);
						}
						catch (Exception ex)
						{
							GameCore.Console.AddLog("Failed to delete malformed central server list.\nException: " + ex.Message, Color.red, false, ConsoleLogType.Log);
						}
						new Thread(delegate()
						{
							CentralServer.RefreshServerList(true, true);
						})
						{
							IsBackground = true,
							Priority = System.Threading.ThreadPriority.BelowNormal,
							Name = "SCP:SL Server list refreshing"
						}.Start();
						return;
					}
					CentralServer._workingServers = CentralServer.Servers.ToList<string>();
					if (!ServerStatic.IsDedicated)
					{
						GameCore.Console.AddLog("Cached central servers count: " + CentralServer.Servers.Length.ToString(), Color.grey, false, ConsoleLogType.Log);
					}
					if (CentralServer.Servers.Length != 0)
					{
						System.Random random = new System.Random();
						CentralServer.SelectedServer = CentralServer.Servers[random.Next(CentralServer.Servers.Length)];
						CentralServer.UpdateUrl();
						if (ServerStatic.IsDedicated)
						{
							ServerConsole.AddLog(string.Concat(new string[]
							{
								"Selected central server: ",
								CentralServer.SelectedServer,
								" (",
								CentralServer.StandardUrl,
								")"
							}), ConsoleColor.Gray, false);
						}
						else
						{
							GameCore.Console.AddLog(string.Concat(new string[]
							{
								"Selected central server: ",
								CentralServer.SelectedServer,
								" (",
								CentralServer.StandardUrl,
								")"
							}), Color.grey, false, ConsoleLogType.Log);
						}
					}
				}
				new Thread(delegate()
				{
					CentralServer.RefreshServerList(true, true);
				})
				{
					IsBackground = true,
					Priority = System.Threading.ThreadPriority.BelowNormal,
					Name = "SCP:SL Server list refreshing"
				}.Start();
				return;
			}
		}
		CentralServer.SelectedServer = "test";
		CentralServer.TestServer = true;
		CentralServer.UpdateUrl();
		CentralServer.ServerSelected = true;
		ServerConsole.AddLog("Using TEST central server: " + CentralServer.MasterUrl, ConsoleColor.Gray, false);
	}

	// Token: 0x06000797 RID: 1943 RVA: 0x0001CC44 File Offset: 0x0001AE44
	private static void LoadPrefs()
	{
		foreach (string text in StartupArgs.Args)
		{
			if (text.StartsWith("-cs:", StringComparison.OrdinalIgnoreCase))
			{
				string text2 = text;
				string text3 = text2.Substring(4, text2.Length - 4);
				if (!CentralServer.CentralServerRegex.IsMatch(text3))
				{
					GameCore.Console.AddLog("Failed to change Central Server - startup arg.", Color.red, false, ConsoleLogType.Log);
				}
				else
				{
					CentralServer.SelectedServer = text3;
					GameCore.Console.AddLog("Central Server changed - startup arg.", Color.yellow, false, ConsoleLogType.Log);
					CentralServer.UpdateUrl();
				}
			}
			else if (text.StartsWith("-csd:", StringComparison.OrdinalIgnoreCase))
			{
				string text2 = text;
				if (CentralServer.ChangeDomain(text2.Substring(5, text2.Length - 5)))
				{
					GameCore.Console.AddLog("Central Server second-level domain has been changed - startup arg.", Color.yellow, false, ConsoleLogType.Log);
				}
				else
				{
					GameCore.Console.AddLog("Failed to change Central Server second-level domain - startup arg.", Color.red, false, ConsoleLogType.Log);
				}
			}
		}
	}

	// Token: 0x06000798 RID: 1944 RVA: 0x0001CD1D File Offset: 0x0001AF1D
	internal static bool ChangeDomain(string domain)
	{
		domain = domain.ToLowerInvariant();
		if (!CentralServer.AllowedDomains.Contains(domain))
		{
			return false;
		}
		CentralServer.CurrentDomain = domain;
		CentralServer.UpdateUrl();
		return true;
	}

	// Token: 0x06000799 RID: 1945 RVA: 0x0001CD44 File Offset: 0x0001AF44
	internal static void UpdateUrl()
	{
		CentralServer.StandardUrl = string.Concat(new string[]
		{
			"https://",
			CentralServer.SelectedServer,
			".",
			CentralServer.CurrentDomain,
			"/"
		});
		CentralServer.MasterUrl = (CentralServer.TestServer ? ("https://test." + CentralServer.CurrentDomain + "/") : ("https://api." + CentralServer.CurrentDomain + "/"));
	}

	// Token: 0x0600079A RID: 1946 RVA: 0x0001CDC0 File Offset: 0x0001AFC0
	private static void RefreshServerList(bool planned = false, bool loop = false)
	{
		while (!CentralServer.Abort)
		{
			object refreshLock = CentralServer._refreshLock;
			lock (refreshLock)
			{
				if (CentralServer.ServerSelected)
				{
					break;
				}
				if (CentralServer._workingServers.Count == 0)
				{
					if (CentralServer.Servers.Length == 0)
					{
						CentralServer.SelectedServer = "api";
					}
					else
					{
						CentralServer._workingServers = CentralServer.Servers.ToList<string>();
						CentralServer.SelectedServer = CentralServer._workingServers[0];
					}
					CentralServer.UpdateUrl();
				}
				byte b = 1;
				while (!CentralServer.Abort)
				{
					if (b == 3)
					{
						break;
					}
					b += 1;
					try
					{
						string[] array = HttpQuery.Get(CentralServer.StandardUrl + "servers.php").Split(';', StringSplitOptions.None);
						if (File.Exists(CentralServer._serversPath))
						{
							File.Delete(CentralServer._serversPath);
						}
						FileManager.WriteToFile(array, CentralServer._serversPath, false);
						GameCore.Console.AddLog("Updated list of central servers.", Color.green, false, ConsoleLogType.Log);
						GameCore.Console.AddLog("Central servers count: " + array.Length.ToString(), Color.cyan, false, ConsoleLogType.Log);
						CentralServer.Servers = array;
						if (planned)
						{
							if (CentralServer.Servers.All((string srv) => srv != CentralServer.SelectedServer))
							{
								CentralServer._workingServers = CentralServer.Servers.ToList<string>();
								CentralServer.ChangeCentralServer(false);
							}
						}
						CentralServer.ServerSelected = true;
						break;
					}
					catch (Exception ex)
					{
						GameCore.Console.AddLog("Can't update central servers list!", Color.red, false, ConsoleLogType.Log);
						GameCore.Console.AddLog("Error: " + ex.Message, Color.red, false, ConsoleLogType.Log);
						if (CentralServer.SelectedServer == "api")
						{
							CentralServer.ServerSelected = true;
							break;
						}
						CentralServer.ChangeCentralServer(true);
					}
				}
			}
			if (!loop)
			{
				return;
			}
			uint num = 0U;
			while (num < 180U && !CentralServer.Abort)
			{
				Thread.Sleep(5000);
				num += 1U;
			}
		}
	}

	// Token: 0x0600079B RID: 1947 RVA: 0x0001CFDC File Offset: 0x0001B1DC
	internal static bool ChangeCentralServer(bool remove)
	{
		CentralServer.ServerSelected = false;
		CentralServer.TestServer = false;
		if (CentralServer.SelectedServer == "api")
		{
			if (CentralServer._lastReset >= DateTime.Now.AddMinutes(-2.0))
			{
				return false;
			}
			CentralServer.RefreshServerList(false, false);
			return true;
		}
		else
		{
			if (CentralServer._workingServers.Count == 0)
			{
				GameCore.Console.AddLog("All known central servers aren't working.", Color.yellow, false, ConsoleLogType.Log);
				CentralServer._workingServers.Add("api");
				CentralServer.SelectedServer = "api";
				CentralServer.UpdateUrl();
				GameCore.Console.AddLog(string.Concat(new string[]
				{
					"Changed central server: ",
					CentralServer.SelectedServer,
					" (",
					CentralServer.StandardUrl,
					")"
				}), Color.yellow, false, ConsoleLogType.Log);
				return true;
			}
			if (remove && CentralServer._workingServers.Contains(CentralServer.SelectedServer))
			{
				CentralServer._workingServers.Remove(CentralServer.SelectedServer);
			}
			if (CentralServer._workingServers.Count == 0)
			{
				CentralServer._workingServers.Add("api");
				CentralServer.SelectedServer = "api";
				CentralServer.UpdateUrl();
				GameCore.Console.AddLog(string.Concat(new string[]
				{
					"Changed central server: ",
					CentralServer.SelectedServer,
					" (",
					CentralServer.StandardUrl,
					")"
				}), Color.yellow, false, ConsoleLogType.Log);
				return true;
			}
			System.Random random = new System.Random();
			CentralServer.SelectedServer = CentralServer._workingServers[random.Next(0, CentralServer._workingServers.Count)];
			CentralServer.UpdateUrl();
			GameCore.Console.AddLog(string.Concat(new string[]
			{
				"Changed central server: ",
				CentralServer.SelectedServer,
				" (",
				CentralServer.StandardUrl,
				")"
			}), Color.yellow, false, ConsoleLogType.Log);
			return true;
		}
	}

	// Token: 0x040006BE RID: 1726
	internal static readonly string[] AllowedDomains = new string[]
	{
		"scpslgame.com",
		"northwoodstudios.org",
		"scpsecretlab.com",
		"scpsecretlaboratory.com",
		"youshouldnthavebeenhacking.com"
	};

	// Token: 0x040006C4 RID: 1732
	public static readonly Regex CentralServerRegex = new Regex("^[a-zA-Z0-9]*$", RegexOptions.CultureInvariant);

	// Token: 0x040006C7 RID: 1735
	private static string _serversPath;

	// Token: 0x040006C8 RID: 1736
	private static bool _started;

	// Token: 0x040006C9 RID: 1737
	private static object _refreshLock;

	// Token: 0x040006CA RID: 1738
	private static List<string> _workingServers;

	// Token: 0x040006CB RID: 1739
	private static DateTime _lastReset;

	// Token: 0x040006CC RID: 1740
	internal static bool Abort;
}
