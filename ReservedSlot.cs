using System;
using System.Collections.Generic;
using System.IO;
using CentralAuth;
using GameCore;

// Token: 0x02000152 RID: 338
public static class ReservedSlot
{
	// Token: 0x06000665 RID: 1637 RVA: 0x000180A5 File Offset: 0x000162A5
	static ReservedSlot()
	{
		ReservedSlot.Reload();
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x000180B8 File Offset: 0x000162B8
	public static void Reload()
	{
		string path = ConfigSharing.Paths[3] + "UserIDReservedSlots.txt";
		ReservedSlot.Users.Clear();
		if (!File.Exists(path))
		{
			FileManager.WriteStringToFile("#Put one UserID (eg. 76561198071934271@steam or 274613382353518592@discord) per line. Lines prefixed with \"#\" are ignored.", path);
			return;
		}
		using (StreamReader streamReader = new StreamReader(path))
		{
			string text;
			while ((text = streamReader.ReadLine()) != null)
			{
				if (!string.IsNullOrWhiteSpace(text) && !text.TrimStart().StartsWith("#", StringComparison.Ordinal) && text.Contains("@"))
				{
					ReservedSlot.Users.Add(text.Trim());
				}
			}
		}
		ServerConsole.AddLog("Reserved slots list has been loaded.", ConsoleColor.Gray, false);
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x0001816C File Offset: 0x0001636C
	public static bool HasReservedSlot(string userId)
	{
		return ReservedSlot.Users.Contains(userId.Trim()) || !PlayerAuthenticationManager.OnlineMode;
	}

	// Token: 0x040005E7 RID: 1511
	public static readonly HashSet<string> Users = new HashSet<string>();
}
