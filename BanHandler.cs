using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameCore;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using NorthwoodLib.Pools;

// Token: 0x02000105 RID: 261
public static class BanHandler
{
	// Token: 0x060004E4 RID: 1252 RVA: 0x000124A7 File Offset: 0x000106A7
	public static BanHandler.BanType GetBanType(int type)
	{
		if (type > EnumUtils<BanHandler.BanType>.Values.ToArray<BanHandler.BanType>().Cast<int>().Max() || type < EnumUtils<BanHandler.BanType>.Values.ToArray<BanHandler.BanType>().Cast<int>().Min())
		{
			return BanHandler.BanType.UserId;
		}
		return (BanHandler.BanType)type;
	}

	// Token: 0x060004E5 RID: 1253 RVA: 0x000124DC File Offset: 0x000106DC
	public static void Init()
	{
		try
		{
			if (!File.Exists(BanHandler.GetPath(BanHandler.BanType.UserId)))
			{
				File.Create(BanHandler.GetPath(BanHandler.BanType.UserId)).Close();
			}
			else
			{
				FileManager.RemoveEmptyLines(BanHandler.GetPath(BanHandler.BanType.UserId));
			}
			if (!File.Exists(BanHandler.GetPath(BanHandler.BanType.IP)))
			{
				File.Create(BanHandler.GetPath(BanHandler.BanType.IP)).Close();
			}
			else
			{
				FileManager.RemoveEmptyLines(BanHandler.GetPath(BanHandler.BanType.IP));
			}
		}
		catch
		{
			ServerConsole.AddLog("Can't create ban files!", ConsoleColor.Gray, false);
		}
		BanHandler.ValidateBans();
	}

	// Token: 0x060004E6 RID: 1254 RVA: 0x00012564 File Offset: 0x00010764
	public static bool IssueBan(BanDetails ban, BanHandler.BanType banType, bool forced = false)
	{
		bool result;
		try
		{
			if (banType == BanHandler.BanType.IP && ban.Id.Equals("localClient", StringComparison.OrdinalIgnoreCase))
			{
				result = false;
			}
			else
			{
				ban.OriginalName = ban.OriginalName.Replace(";", ":");
				ban.Issuer = ban.Issuer.Replace(";", ":");
				ban.Reason = ban.Reason.Replace(";", ":");
				Misc.ReplaceUnsafeCharacters(ref ban.OriginalName, '?');
				Misc.ReplaceUnsafeCharacters(ref ban.Issuer, '?');
				List<BanDetails> bans = BanHandler.GetBans(banType);
				if (!bans.Any((BanDetails b) => b.Id == ban.Id))
				{
					BanIssuingEventArgs banIssuingEventArgs = new BanIssuingEventArgs(banType, ban);
					ServerEvents.OnBanIssuing(banIssuingEventArgs);
					if (!forced && !banIssuingEventArgs.IsAllowed)
					{
						return false;
					}
					banType = banIssuingEventArgs.BanType;
					ban = banIssuingEventArgs.BanDetails;
					FileManager.AppendFile(ban.ToString(), BanHandler.GetPath(banType), true);
					FileManager.RemoveEmptyLines(BanHandler.GetPath(banType));
					ServerEvents.OnBanIssued(new BanIssuedEventArgs(banType, ban));
				}
				else
				{
					BanDetails oldBanDetails = bans.First((BanDetails old) => string.Equals(old.Id, ban.Id, StringComparison.OrdinalIgnoreCase));
					BanUpdatingEventArgs banUpdatingEventArgs = new BanUpdatingEventArgs(banType, ban, oldBanDetails);
					ServerEvents.OnBanUpdating(banUpdatingEventArgs);
					if (!forced && !banUpdatingEventArgs.IsAllowed)
					{
						return false;
					}
					BanHandler.RemoveBan(ban.Id, banType, true);
					banType = banUpdatingEventArgs.BanType;
					ban = banUpdatingEventArgs.BanDetails;
					BanHandler.IssueBan(ban, banType, true);
					ServerEvents.OnBanUpdated(new BanUpdatedEventArgs(banType, ban, oldBanDetails));
				}
				result = true;
			}
		}
		catch
		{
			result = false;
		}
		return result;
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x0001276C File Offset: 0x0001096C
	public static void ValidateBans()
	{
		ServerConsole.AddLog("Validating bans...", ConsoleColor.Gray, false);
		BanHandler.ValidateBans(BanHandler.BanType.UserId);
		BanHandler.ValidateBans(BanHandler.BanType.IP);
		ServerConsole.AddLog("Bans has been validated.", ConsoleColor.Gray, false);
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x00012794 File Offset: 0x00010994
	public static void ValidateBans(BanHandler.BanType banType)
	{
		List<string> list = FileManager.ReadAllLinesList(BanHandler.GetPath(banType));
		List<int> list2 = ListPool<int>.Shared.Rent();
		for (int i = list.Count - 1; i >= 0; i--)
		{
			string ban = list[i];
			if (BanHandler.ProcessBanItem(ban, banType) == null || !BanHandler.CheckExpiration(BanHandler.ProcessBanItem(ban, banType), BanHandler.BanType.NULL))
			{
				list2.Add(i);
			}
		}
		List<int> list3 = ListPool<int>.Shared.Rent();
		foreach (int item in list2)
		{
			if (!list3.Contains(item))
			{
				list3.Add(item);
			}
		}
		ListPool<int>.Shared.Return(list2);
		foreach (int index2 in from index in list3
		orderby index descending
		select index)
		{
			list.RemoveAt(index2);
		}
		ListPool<int>.Shared.Return(list3);
		if (FileManager.ReadAllLines(BanHandler.GetPath(banType)) == list.ToArray())
		{
			return;
		}
		FileManager.WriteToFile(list.ToArray(), BanHandler.GetPath(banType), false);
	}

	// Token: 0x060004E9 RID: 1257 RVA: 0x000128EC File Offset: 0x00010AEC
	public static bool CheckExpiration(BanDetails ban, BanHandler.BanType banType)
	{
		if (ban == null)
		{
			return false;
		}
		if (TimeBehaviour.ValidateTimestamp(ban.Expires, TimeBehaviour.CurrentTimestamp(), 0L))
		{
			return true;
		}
		if (banType >= BanHandler.BanType.UserId)
		{
			BanHandler.RemoveBan(ban.Id, banType, true);
		}
		return false;
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x00012928 File Offset: 0x00010B28
	public static BanDetails ReturnChecks(BanDetails ban, BanHandler.BanType banType)
	{
		if (!BanHandler.CheckExpiration(ban, banType))
		{
			return null;
		}
		return ban;
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x00012938 File Offset: 0x00010B38
	public static void RemoveBan(string id, BanHandler.BanType banType, bool forced = false)
	{
		BanRevokingEventArgs banRevokingEventArgs = new BanRevokingEventArgs(banType, BanHandler.GetBan(id, banType));
		ServerEvents.OnBanRevoking(banRevokingEventArgs);
		if (!forced && !banRevokingEventArgs.IsAllowed)
		{
			return;
		}
		id = id.Replace(";", ":").Replace(Environment.NewLine, "").Replace("\n", "");
		FileManager.WriteToFile((from l in FileManager.ReadAllLines(BanHandler.GetPath(banType))
		where BanHandler.ProcessBanItem(l, banType) != null && BanHandler.ProcessBanItem(l, banType).Id != id
		select l).ToArray<string>(), BanHandler.GetPath(banType), false);
		ServerEvents.OnBanRevoked(new BanRevokedEventArgs(banType, banRevokingEventArgs.BanDetails));
	}

	// Token: 0x060004EC RID: 1260 RVA: 0x00012A10 File Offset: 0x00010C10
	public static List<BanDetails> GetBans(BanHandler.BanType banType)
	{
		return (from b in FileManager.ReadAllLines(BanHandler.GetPath(banType))
		select BanHandler.ProcessBanItem(b, banType) into b
		where b != null
		select b).ToList<BanDetails>();
	}

	// Token: 0x060004ED RID: 1261 RVA: 0x00012A74 File Offset: 0x00010C74
	public static BanDetails GetBan(string id, BanHandler.BanType banType)
	{
		return BanHandler.GetBans(banType).FirstOrDefault((BanDetails s) => string.Equals(s.Id, id, StringComparison.OrdinalIgnoreCase));
	}

	// Token: 0x060004EE RID: 1262 RVA: 0x00012AA8 File Offset: 0x00010CA8
	public static KeyValuePair<BanDetails, BanDetails> QueryBan(string userId, string ip)
	{
		string ban = null;
		string ban2 = null;
		if (!string.IsNullOrEmpty(userId))
		{
			userId = userId.Replace(";", ":").Replace(Environment.NewLine, "").Replace("\n", "");
			ban = FileManager.ReadAllLines(BanHandler.GetPath(BanHandler.BanType.UserId)).FirstOrDefault(delegate(string b)
			{
				BanDetails banDetails = BanHandler.ProcessBanItem(b, BanHandler.BanType.UserId);
				return ((banDetails != null) ? banDetails.Id : null) == userId;
			});
		}
		if (!string.IsNullOrEmpty(ip))
		{
			ip = ip.Replace(";", ":").Replace(Environment.NewLine, "").Replace("\n", "");
			ban2 = FileManager.ReadAllLines(BanHandler.GetPath(BanHandler.BanType.IP)).FirstOrDefault(delegate(string b)
			{
				BanDetails banDetails = BanHandler.ProcessBanItem(b, BanHandler.BanType.IP);
				return ((banDetails != null) ? banDetails.Id : null) == ip;
			});
		}
		return new KeyValuePair<BanDetails, BanDetails>(BanHandler.ReturnChecks(BanHandler.ProcessBanItem(ban, BanHandler.BanType.UserId), BanHandler.BanType.UserId), BanHandler.ReturnChecks(BanHandler.ProcessBanItem(ban2, BanHandler.BanType.IP), BanHandler.BanType.IP));
	}

	// Token: 0x060004EF RID: 1263 RVA: 0x00012BB4 File Offset: 0x00010DB4
	public static BanDetails ProcessBanItem(string ban, BanHandler.BanType banType)
	{
		if (string.IsNullOrEmpty(ban) || !ban.Contains(";"))
		{
			return null;
		}
		string[] array = ban.Split(';', StringSplitOptions.None);
		if (array.Length != 6)
		{
			return null;
		}
		if (banType == BanHandler.BanType.UserId && !array[1].Contains("@"))
		{
			array[1] = array[1].Trim() + "@steam";
		}
		return new BanDetails
		{
			OriginalName = array[0],
			Id = array[1].Trim(),
			Expires = Convert.ToInt64(array[2].Trim()),
			Reason = array[3],
			Issuer = array[4],
			IssuanceTime = Convert.ToInt64(array[5].Trim())
		};
	}

	// Token: 0x060004F0 RID: 1264 RVA: 0x00012C65 File Offset: 0x00010E65
	public static string GetPath(BanHandler.BanType banType)
	{
		if (banType != BanHandler.BanType.UserId && banType == BanHandler.BanType.IP)
		{
			return ConfigSharing.Paths[0] + "IpBans.txt";
		}
		return ConfigSharing.Paths[0] + "UserIdBans.txt";
	}

	// Token: 0x02000106 RID: 262
	public enum BanType
	{
		// Token: 0x04000440 RID: 1088
		NULL = -1,
		// Token: 0x04000441 RID: 1089
		UserId,
		// Token: 0x04000442 RID: 1090
		IP
	}
}
