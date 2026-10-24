using System;
using System.Collections.Generic;
using System.IO;
using GameCore;

// Token: 0x020001A4 RID: 420
public static class WhiteList
{
	// Token: 0x06000847 RID: 2119 RVA: 0x00021064 File Offset: 0x0001F264
	static WhiteList()
	{
		WhiteList.Reload();
	}

	// Token: 0x06000848 RID: 2120 RVA: 0x00021078 File Offset: 0x0001F278
	public static void Reload()
	{
		WhiteList.WhitelistEnabled = ConfigFile.ServerConfig.GetBool("enable_whitelist", false);
		string path = ConfigSharing.Paths[2] + "UserIDWhitelist.txt";
		WhiteList.Users.Clear();
		if (!File.Exists(path))
		{
			FileManager.WriteStringToFile("#Put one UserID (eg. 76561198071934271@steam or 274613382353518592@discord) per line. Lines prefixed with \"#\" are ignored.", path);
			return;
		}
		using (StreamReader streamReader = new StreamReader(path))
		{
			for (;;)
			{
				string text = streamReader.ReadLine();
				if (text == null)
				{
					break;
				}
				if (!string.IsNullOrWhiteSpace(text) && !text.TrimStart().StartsWith("#", StringComparison.Ordinal) && text.Contains("@", StringComparison.Ordinal))
				{
					WhiteList.Users.Add(text.Trim());
				}
			}
		}
		ServerConsole.AddLog("Whitelist has been loaded.", ConsoleColor.Gray, false);
	}

	// Token: 0x06000849 RID: 2121 RVA: 0x00021140 File Offset: 0x0001F340
	public static bool IsOnWhitelist(string userId)
	{
		return WhiteList.Users.Contains(userId);
	}

	// Token: 0x0600084A RID: 2122 RVA: 0x0002114D File Offset: 0x0001F34D
	public static bool IsWhitelisted(string userId)
	{
		return !WhiteList.WhitelistEnabled || WhiteList.Users.Contains(userId);
	}

	// Token: 0x0400078D RID: 1933
	public static bool WhitelistEnabled;

	// Token: 0x0400078E RID: 1934
	public static readonly HashSet<string> Users = new HashSet<string>();
}
