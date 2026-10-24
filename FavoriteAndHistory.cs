using System;
using System.Collections.Generic;
using System.IO;
using GameCore;
using UnityEngine;

// Token: 0x020000D8 RID: 216
public class FavoriteAndHistory : MonoBehaviour
{
	// Token: 0x0600042C RID: 1068 RVA: 0x0001016F File Offset: 0x0000E36F
	public static string GetPath(FavoriteAndHistory.StorageLocation location)
	{
		return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SCP Secret Laboratory/" + FavoriteAndHistory.StorageEnumToPath[location];
	}

	// Token: 0x0600042D RID: 1069 RVA: 0x0001018D File Offset: 0x0000E38D
	public static void ResetServerID()
	{
		FavoriteAndHistory.ServerIDLastJoined = string.Empty;
	}

	// Token: 0x0600042E RID: 1070 RVA: 0x0001019C File Offset: 0x0000E39C
	static FavoriteAndHistory()
	{
		foreach (KeyValuePair<FavoriteAndHistory.StorageLocation, List<string>> keyValuePair in FavoriteAndHistory.LocationToList)
		{
			FavoriteAndHistory.Load(keyValuePair.Key);
		}
	}

	// Token: 0x0600042F RID: 1071 RVA: 0x00010270 File Offset: 0x0000E470
	public static void Load(FavoriteAndHistory.StorageLocation location)
	{
		GameCore.Console.AddLog("Loading " + location.ToString(), Color.grey, false, ConsoleLogType.Log);
		try
		{
			List<string> list = FavoriteAndHistory.LocationToList[location];
			list.Clear();
			if (!File.Exists(FavoriteAndHistory.GetPath(location)))
			{
				FavoriteAndHistory.Revert(FavoriteAndHistory.StorageEnumToPath[location]);
			}
			StreamReader streamReader = new StreamReader(FavoriteAndHistory.GetPath(location));
			string text;
			while ((text = streamReader.ReadLine()) != null)
			{
				if (!string.IsNullOrEmpty(text) && !list.Contains(text))
				{
					list.Add(text);
				}
			}
			streamReader.Close();
		}
		catch (Exception ex)
		{
			Debug.Log("REVENT: " + ex.StackTrace + " - " + ex.Message);
			FavoriteAndHistory.Revert(FavoriteAndHistory.StorageEnumToPath[location]);
		}
	}

	// Token: 0x06000430 RID: 1072 RVA: 0x0001034C File Offset: 0x0000E54C
	public static void Modify(FavoriteAndHistory.StorageLocation location, string id, bool delete = false)
	{
		List<string> list = FavoriteAndHistory.LocationToList[location];
		list.RemoveAll((string x) => x == id);
		if (!delete)
		{
			list.Add(id);
		}
		StreamWriter streamWriter = new StreamWriter(FavoriteAndHistory.GetPath(location), false);
		foreach (string value in list)
		{
			streamWriter.WriteLine(value);
		}
		streamWriter.Close();
		FavoriteAndHistory.Load(location);
		if ((location == FavoriteAndHistory.StorageLocation.History || location == FavoriteAndHistory.StorageLocation.IPHistory) && list.Count >= 10)
		{
			FavoriteAndHistory.HistoryLimit(location, id);
		}
	}

	// Token: 0x06000431 RID: 1073 RVA: 0x00010410 File Offset: 0x0000E610
	private static void HistoryLimit(FavoriteAndHistory.StorageLocation location, string id)
	{
		int num = 0;
		List<string> list = FavoriteAndHistory.LocationToList[location];
		if (list.Contains(id))
		{
			num = list.RemoveAll((string x) => x == id);
		}
		if (num == 0)
		{
			list.RemoveAt(0);
		}
		list.Add(id);
		StreamWriter streamWriter = new StreamWriter(FavoriteAndHistory.GetPath(location), false);
		foreach (string value in list)
		{
			streamWriter.WriteLine(value);
		}
		streamWriter.Close();
		FavoriteAndHistory.Load(location);
	}

	// Token: 0x06000432 RID: 1074 RVA: 0x000104CC File Offset: 0x0000E6CC
	public static bool IsInStorage(FavoriteAndHistory.StorageLocation location, string id)
	{
		return FavoriteAndHistory.LocationToList[location].Contains(id);
	}

	// Token: 0x06000433 RID: 1075 RVA: 0x000104DF File Offset: 0x0000E6DF
	private static void Revert(string fileName)
	{
		Debug.Log("Reverting:" + fileName);
		new StreamWriter(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/SCP Secret Laboratory/" + fileName).Close();
	}

	// Token: 0x040003B0 RID: 944
	public const int MaxHistoryAmount = 10;

	// Token: 0x040003B1 RID: 945
	public static readonly List<string> Favorites = new List<string>();

	// Token: 0x040003B2 RID: 946
	public static readonly List<string> History = new List<string>();

	// Token: 0x040003B3 RID: 947
	public static readonly List<string> IPHistory = new List<string>();

	// Token: 0x040003B4 RID: 948
	public static string ServerIDLastJoined;

	// Token: 0x040003B5 RID: 949
	public static string AccountIDLastJoined;

	// Token: 0x040003B6 RID: 950
	public static readonly Dictionary<FavoriteAndHistory.StorageLocation, List<string>> LocationToList = new Dictionary<FavoriteAndHistory.StorageLocation, List<string>>
	{
		{
			FavoriteAndHistory.StorageLocation.History,
			FavoriteAndHistory.History
		},
		{
			FavoriteAndHistory.StorageLocation.Favorites,
			FavoriteAndHistory.Favorites
		},
		{
			FavoriteAndHistory.StorageLocation.IPHistory,
			FavoriteAndHistory.IPHistory
		}
	};

	// Token: 0x040003B7 RID: 951
	private static readonly Dictionary<FavoriteAndHistory.StorageLocation, string> StorageEnumToPath = new Dictionary<FavoriteAndHistory.StorageLocation, string>
	{
		{
			FavoriteAndHistory.StorageLocation.History,
			"history.txt"
		},
		{
			FavoriteAndHistory.StorageLocation.Favorites,
			"favorites.txt"
		},
		{
			FavoriteAndHistory.StorageLocation.IPHistory,
			"iphistory.txt"
		}
	};

	// Token: 0x020000D9 RID: 217
	public enum StorageLocation
	{
		// Token: 0x040003B9 RID: 953
		History,
		// Token: 0x040003BA RID: 954
		Favorites,
		// Token: 0x040003BB RID: 955
		IPHistory
	}
}
