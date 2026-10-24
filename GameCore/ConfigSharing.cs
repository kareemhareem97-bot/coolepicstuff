using System;
using System.IO;

namespace GameCore
{
	// Token: 0x02000856 RID: 2134
	public static class ConfigSharing
	{
		// Token: 0x060030C0 RID: 12480 RVA: 0x000A33F8 File Offset: 0x000A15F8
		static ConfigSharing()
		{
			ConfigSharing.Reload();
		}

		// Token: 0x060030C1 RID: 12481 RVA: 0x000A3418 File Offset: 0x000A1618
		internal static void Reload()
		{
			ConfigSharing.Shares[0] = ConfigFile.SharingConfig.GetString("bans", "");
			ConfigSharing.Shares[1] = ConfigFile.SharingConfig.GetString("mutes", "");
			ConfigSharing.Shares[2] = ConfigFile.SharingConfig.GetString("whitelist", "");
			ConfigSharing.Shares[3] = ConfigFile.SharingConfig.GetString("reserved_slots", "");
			ConfigSharing.Shares[4] = ConfigFile.SharingConfig.GetString("groups", "");
			ConfigSharing.Shares[5] = ConfigFile.SharingConfig.GetString("groups_members", "");
			ConfigSharing.Shares[6] = ConfigFile.SharingConfig.GetString("gameplay_database", "");
			ushort num = 0;
			while ((int)num < ConfigSharing.Shares.Length)
			{
				if (ConfigSharing.Shares[(int)num] == "disable")
				{
					ConfigSharing.Paths[(int)num] = ((num == 4 || num == 5) ? null : FileManager.GetAppFolder(true, true, ""));
				}
				else
				{
					ConfigSharing.Paths[(int)num] = FileManager.GetAppFolder(true, true, ConfigSharing.Shares[(int)num]);
					string appFolder = FileManager.GetAppFolder(true, true, ConfigSharing.Shares[(int)num]);
					if (!Directory.Exists(appFolder))
					{
						Directory.CreateDirectory(appFolder);
					}
					if (num >= 4 && num <= 5)
					{
						if (num != 4)
						{
							if (num == 5)
							{
								if (!File.Exists(ConfigSharing.Paths[(int)num] + "shared_groups_members.txt"))
								{
									File.Copy("ConfigTemplates/shared_groups_members.template.txt", ConfigSharing.Paths[(int)num] + "shared_groups_members.txt");
								}
							}
						}
						else if (!File.Exists(ConfigSharing.Paths[(int)num] + "shared_groups.txt"))
						{
							File.Copy("ConfigTemplates/shared_groups.template.txt", ConfigSharing.Paths[(int)num] + "shared_groups.txt");
						}
					}
				}
				num += 1;
			}
			ServerConsole.AddLog("Config sharing loaded.", ConsoleColor.Gray, false);
		}

		// Token: 0x040025AE RID: 9646
		public static readonly string[] Shares = new string[7];

		// Token: 0x040025AF RID: 9647
		public static readonly string[] Paths = new string[7];

		// Token: 0x02000857 RID: 2135
		public enum ConfigShare
		{
			// Token: 0x040025B1 RID: 9649
			Bans,
			// Token: 0x040025B2 RID: 9650
			Mutes,
			// Token: 0x040025B3 RID: 9651
			Whitelist,
			// Token: 0x040025B4 RID: 9652
			ReservedSlots,
			// Token: 0x040025B5 RID: 9653
			Groups,
			// Token: 0x040025B6 RID: 9654
			GroupsMembers,
			// Token: 0x040025B7 RID: 9655
			GameplayDatabase
		}
	}
}
