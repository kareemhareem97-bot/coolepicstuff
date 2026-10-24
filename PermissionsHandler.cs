using System;
using System.Collections.Generic;
using System.Linq;
using GameCore;

// Token: 0x02000134 RID: 308
public class PermissionsHandler
{
	// Token: 0x060005E7 RID: 1511 RVA: 0x00016424 File Offset: 0x00014624
	public PermissionsHandler(ref YamlConfig configuration, ref YamlConfig sharedGroups, ref YamlConfig sharedGroupsMembers)
	{
		this._config = new YamlConfig(configuration.Path);
		this._sharedGroups = sharedGroups;
		this.OverridePassword = configuration.GetString("override_password", "none");
		this._overrideRole = configuration.GetString("override_password_role", "owner");
		this.StaffAccess = configuration.GetBool("enable_staff_access", false);
		this._managerAccess = configuration.GetBool("enable_manager_access", true);
		this._banTeamAccess = configuration.GetBool("enable_banteam_access", true);
		this._banTeamSlots = configuration.GetBool("enable_banteam_reserved_slots", true);
		this._banTeamGeoBypass = configuration.GetBool("enable_banteam_bypass_geoblocking", true);
		this.NorthwoodAccess = configuration.GetBool("enable_northwood_access", false);
		if (this.NorthwoodAccess)
		{
			ServerConsole.AddLog("WARNING - Northwood staff access is enabled! All NW Studios staff members will have FULL Remote Admin access (this should only be used on testing servers)! You can disable this by setting 'enable_northwood_access' to false in your remote admin config file.", ConsoleColor.Yellow, false);
		}
		this.Groups = new Dictionary<string, UserGroup>();
		this._raPermissions = new HashSet<ulong>();
		List<string> stringList = configuration.GetStringList("Roles");
		List<string> stringList2 = configuration.GetStringList("Roles");
		if (sharedGroups != null)
		{
			List<string> stringList3 = sharedGroups.GetStringList("SharedRoles");
			string a = ConfigFile.SharingConfig.GetString("groups_sharing_mode", "").ToLowerInvariant();
			if (!(a == "all"))
			{
				if (!(a == "opt-in"))
				{
					if (!(a == "opt-out"))
					{
						ServerConsole.AddLog("Invalid group sharing mode set!", ConsoleColor.Gray, false);
					}
					else
					{
						List<string> optOut = ConfigFile.SharingConfig.GetStringList("groups_opt_out_list");
						stringList.AddRange(from @group in stringList3
						where !optOut.Contains(@group)
						select @group);
					}
				}
				else
				{
					List<string> optIn = ConfigFile.SharingConfig.GetStringList("groups_opt_in_list");
					stringList.AddRange(from @group in stringList3
					where optIn.Contains(@group)
					select @group);
				}
			}
			else
			{
				stringList.AddRange(stringList3);
			}
		}
		string[] array = configuration.GetKeys().ToArray<string>();
		foreach (string text in stringList)
		{
			string text2 = (array.Contains(text + "_badge") || sharedGroups == null) ? configuration.GetString(text + "_badge", "") : sharedGroups.GetString(text + "_badge", "");
			string text3 = (array.Contains(text + "_color") || sharedGroups == null) ? configuration.GetString(text + "_color", "") : sharedGroups.GetString(text + "_color", "");
			bool cover = (array.Contains(text + "_cover") || sharedGroups == null) ? configuration.GetBool(text + "_cover", true) : sharedGroups.GetBool(text + "_cover", true);
			bool hiddenByDefault = (array.Contains(text + "_hidden") || sharedGroups == null) ? configuration.GetBool(text + "_hidden", false) : sharedGroups.GetBool(text + "_hidden", false);
			byte kickPower = (array.Contains(text + "_kick_power") || sharedGroups == null) ? configuration.GetByte(text + "_kick_power", 0) : sharedGroups.GetByte(text + "_kick_power", 0);
			byte requiredKickPower = (array.Contains(text + "_required_kick_power") || sharedGroups == null) ? configuration.GetByte(text + "_required_kick_power", 0) : sharedGroups.GetByte(text + "_required_kick_power", 0);
			if (!(text2 == "") && !(text3 == ""))
			{
				if (this.Groups.ContainsKey(text))
				{
					ServerConsole.AddLog("Duplicated group definition: " + text + ".", ConsoleColor.Gray, false);
				}
				else
				{
					this.Groups.Add(text, new UserGroup
					{
						Name = text,
						BadgeColor = text3,
						BadgeText = text2,
						Permissions = 0UL,
						Cover = cover,
						HiddenByDefault = hiddenByDefault,
						Shared = !stringList2.Contains(text),
						KickPower = kickPower,
						RequiredKickPower = requiredKickPower
					});
				}
			}
		}
		this.Members = configuration.GetStringDictionary("Members");
		YamlConfig yamlConfig = sharedGroupsMembers;
		Dictionary<string, string> dictionary = (yamlConfig != null) ? yamlConfig.GetStringDictionary("SharedMembers") : null;
		if (dictionary != null)
		{
			foreach (KeyValuePair<string, string> keyValuePair in dictionary)
			{
				string text4;
				if (this.Members.TryGetValue(keyValuePair.Key, out text4))
				{
					ServerConsole.AddLog(string.Concat(new string[]
					{
						"Duplicated group member: ",
						keyValuePair.Key,
						". Is member of ",
						text4,
						" and ",
						keyValuePair.Value,
						"."
					}), ConsoleColor.Gray, false);
				}
				else
				{
					this.Members.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}
		this._lastPerm = 1UL;
		HashSet<string> hashSet = new HashSet<string>();
		if (this.Members != null)
		{
			foreach (KeyValuePair<string, string> keyValuePair2 in this.Members)
			{
				if (!this.Groups.ContainsKey(keyValuePair2.Value))
				{
					hashSet.Add(keyValuePair2.Key);
				}
			}
		}
		if (hashSet.Count > 0 && this.Members != null)
		{
			foreach (string key in hashSet)
			{
				this.Members.Remove(key);
			}
		}
		hashSet.Clear();
		this.Permissions = new Dictionary<string, ulong>();
		foreach (string text5 in EnumUtils<PlayerPermissions>.Names)
		{
			PlayerPermissions playerPermissions = (PlayerPermissions)Enum.Parse(typeof(PlayerPermissions), text5);
			this.FullPerm |= (ulong)playerPermissions;
			this.Permissions.Add(text5, (ulong)playerPermissions);
			if (playerPermissions.GetPermissionType() == PlayerPermissionType.RaAccess)
			{
				this._raPermissions.Add((ulong)playerPermissions);
			}
			if (playerPermissions > (PlayerPermissions)this._lastPerm)
			{
				this._lastPerm = (ulong)playerPermissions;
			}
		}
		this.RefreshPermissions();
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x00016B38 File Offset: 0x00014D38
	public ulong RegisterPermission(string name, bool remoteAdmin, bool refresh = true)
	{
		this._lastPerm = (ulong)Math.Pow(2.0, Math.Log(this._lastPerm, 2.0) + 1.0);
		this.FullPerm |= this._lastPerm;
		this.Permissions.Add(name, this._lastPerm);
		if (remoteAdmin)
		{
			this._raPermissions.Add(this._lastPerm);
		}
		if (refresh)
		{
			this.RefreshPermissions();
		}
		return this._lastPerm;
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x00016BC4 File Offset: 0x00014DC4
	public void RefreshPermissions()
	{
		foreach (KeyValuePair<string, UserGroup> keyValuePair in this.Groups)
		{
			keyValuePair.Value.Permissions = 0UL;
		}
		Dictionary<string, string> stringDictionary = this._config.GetStringDictionary("Permissions");
		YamlConfig sharedGroups = this._sharedGroups;
		Dictionary<string, string> dictionary = (sharedGroups != null) ? sharedGroups.GetStringDictionary("SharedPermissions") : null;
		foreach (string text in this.Permissions.Keys)
		{
			ulong num = this.Permissions[text];
			string data;
			if (stringDictionary.TryGetValue(text, out data))
			{
				string[] array = YamlConfig.ParseCommaSeparatedString(data);
				if (array == null)
				{
					ServerConsole.AddLog("Failed to process group permissions in remote admin config! Make sure there is no typo.", ConsoleColor.Gray, false);
				}
				else
				{
					foreach (string key in array)
					{
						UserGroup userGroup;
						if (this.Groups.TryGetValue(key, out userGroup))
						{
							userGroup.Permissions |= num;
						}
					}
				}
			}
			else
			{
				ServerConsole.AddLog("RemoteAdmin config is missing permission definition: " + text, ConsoleColor.Gray, false);
			}
			if (dictionary != null)
			{
				string data2;
				if (dictionary.TryGetValue(text, out data2))
				{
					string[] array3 = YamlConfig.ParseCommaSeparatedString(data2);
					if (array3 != null)
					{
						foreach (string key2 in array3)
						{
							UserGroup userGroup2;
							if (this.Groups.TryGetValue(key2, out userGroup2))
							{
								userGroup2.Permissions |= num;
							}
						}
					}
				}
				else
				{
					ServerConsole.AddLog("Shared groups config is missing permission definition: " + text, ConsoleColor.Gray, false);
				}
			}
		}
	}

	// Token: 0x060005EA RID: 1514 RVA: 0x00016DA4 File Offset: 0x00014FA4
	public bool IsRaPermitted(ulong permissions)
	{
		return this._raPermissions.Any((ulong perm) => PermissionsHandler.IsPermitted(permissions, perm));
	}

	// Token: 0x060005EB RID: 1515 RVA: 0x00016DD5 File Offset: 0x00014FD5
	public UserGroup GetGroup(string name)
	{
		if (this.Groups.ContainsKey(name))
		{
			return this.Groups[name].Clone();
		}
		return null;
	}

	// Token: 0x060005EC RID: 1516 RVA: 0x00016DF8 File Offset: 0x00014FF8
	public List<string> GetAllGroupsNames()
	{
		return this.Groups.Keys.ToList<string>();
	}

	// Token: 0x060005ED RID: 1517 RVA: 0x00016E0A File Offset: 0x0001500A
	public Dictionary<string, UserGroup> GetAllGroups()
	{
		return this.Groups.Keys.ToDictionary((string gr) => gr, (string gr) => this.Groups[gr]);
	}

	// Token: 0x060005EE RID: 1518 RVA: 0x00016E48 File Offset: 0x00015048
	public string GetPermissionName(ulong value)
	{
		return this.Permissions.FirstOrDefault((KeyValuePair<string, ulong> x) => x.Value == value).Key;
	}

	// Token: 0x060005EF RID: 1519 RVA: 0x00016E84 File Offset: 0x00015084
	public ulong GetPermissionValue(string name)
	{
		return this.Permissions.FirstOrDefault((KeyValuePair<string, ulong> x) => x.Key == name).Value;
	}

	// Token: 0x060005F0 RID: 1520 RVA: 0x00016EBD File Offset: 0x000150BD
	public List<string> GetAllPermissions()
	{
		return this.Permissions.Keys.ToList<string>();
	}

	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x060005F1 RID: 1521 RVA: 0x00016ECF File Offset: 0x000150CF
	public bool BanTeamSlots
	{
		get
		{
			return this._banTeamSlots || CustomNetworkManager.IsVerified;
		}
	}

	// Token: 0x170000F4 RID: 244
	// (get) Token: 0x060005F2 RID: 1522 RVA: 0x00016EE0 File Offset: 0x000150E0
	public bool BanTeamBypassGeo
	{
		get
		{
			return this._banTeamGeoBypass || CustomNetworkManager.IsVerified;
		}
	}

	// Token: 0x060005F3 RID: 1523 RVA: 0x00016EF1 File Offset: 0x000150F1
	public static bool IsPermitted(ulong permissions, PlayerPermissions check)
	{
		return PermissionsHandler.IsPermitted(permissions, (ulong)check);
	}

	// Token: 0x060005F4 RID: 1524 RVA: 0x00016EFC File Offset: 0x000150FC
	public static bool IsPermitted(ulong permissions, PlayerPermissions[] check)
	{
		if (check.Length == 0)
		{
			return true;
		}
		ulong check2 = check.Aggregate(0UL, (ulong current, PlayerPermissions c) => current | (ulong)c);
		return PermissionsHandler.IsPermitted(permissions, check2);
	}

	// Token: 0x060005F5 RID: 1525 RVA: 0x00016F41 File Offset: 0x00015141
	public bool IsPermitted(ulong permissions, string check)
	{
		return this.Permissions.ContainsKey(check) && PermissionsHandler.IsPermitted(permissions, this.Permissions[check]);
	}

	// Token: 0x060005F6 RID: 1526 RVA: 0x00016F68 File Offset: 0x00015168
	public bool IsPermitted(ulong permissions, string[] check)
	{
		if (check.Length == 0)
		{
			return true;
		}
		ulong check2 = (from c in check
		where this.Permissions.ContainsKey(c)
		select c).Aggregate(0UL, (ulong current, string c) => current | this.Permissions[c]);
		return PermissionsHandler.IsPermitted(permissions, check2);
	}

	// Token: 0x060005F7 RID: 1527 RVA: 0x00016FA8 File Offset: 0x000151A8
	public static bool IsPermitted(ulong permissions, ulong check)
	{
		return (permissions & check) > 0UL;
	}

	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x060005F8 RID: 1528 RVA: 0x00016FB1 File Offset: 0x000151B1
	public UserGroup OverrideGroup
	{
		get
		{
			if (!this.OverrideEnabled)
			{
				return null;
			}
			if (this.Groups.ContainsKey(this._overrideRole))
			{
				return this.Groups[this._overrideRole];
			}
			return null;
		}
	}

	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x060005F9 RID: 1529 RVA: 0x00016FE4 File Offset: 0x000151E4
	public bool OverrideEnabled
	{
		get
		{
			if (string.IsNullOrEmpty(this.OverridePassword) || this.OverridePassword == "none")
			{
				return false;
			}
			if (CustomNetworkManager.IsVerified)
			{
				ServerConsole.AddLog("Override password refused, Password authentication is disabled for verified servers.", ConsoleColor.Gray, false);
				return false;
			}
			if (this.OverridePassword.Length < 8)
			{
				ServerConsole.AddLog("Override password refused, because it's too short.", ConsoleColor.Gray, false);
				return false;
			}
			if (this.OverridePassword.ToLower() == this.OverridePassword || this.OverridePassword.ToUpper() == this.OverridePassword)
			{
				ServerConsole.AddLog("Override password refused, because it must contain mixed case chars.", ConsoleColor.Gray, false);
				return false;
			}
			if (this.OverridePassword.Any((char c) => !char.IsLetter(c)))
			{
				return true;
			}
			ServerConsole.AddLog("Override password refused, because it must contain digit or special symbol.", ConsoleColor.Gray, false);
			return false;
		}
	}

	// Token: 0x060005FA RID: 1530 RVA: 0x000170BC File Offset: 0x000152BC
	public UserGroup GetUserGroup(string userId)
	{
		if (!string.IsNullOrEmpty(userId) && this.Members.ContainsKey(userId))
		{
			return this.Groups[this.Members[userId]];
		}
		return null;
	}

	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x060005FB RID: 1531 RVA: 0x000170ED File Offset: 0x000152ED
	// (set) Token: 0x060005FC RID: 1532 RVA: 0x000170F5 File Offset: 0x000152F5
	public ulong FullPerm { get; private set; }

	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x060005FD RID: 1533 RVA: 0x000170FE File Offset: 0x000152FE
	public bool StaffAccess { get; }

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x060005FE RID: 1534 RVA: 0x00017106 File Offset: 0x00015306
	public bool ManagersAccess
	{
		get
		{
			return this._managerAccess || this.StaffAccess || CustomNetworkManager.IsVerified;
		}
	}

	// Token: 0x170000FA RID: 250
	// (get) Token: 0x060005FF RID: 1535 RVA: 0x0001711F File Offset: 0x0001531F
	public bool BanningTeamAccess
	{
		get
		{
			return this._banTeamAccess || this.StaffAccess || CustomNetworkManager.IsVerified;
		}
	}

	// Token: 0x170000FB RID: 251
	// (get) Token: 0x06000600 RID: 1536 RVA: 0x00017138 File Offset: 0x00015338
	public bool NorthwoodAccess { get; }

	// Token: 0x0400056C RID: 1388
	public readonly Dictionary<string, UserGroup> Groups;

	// Token: 0x0400056D RID: 1389
	public readonly Dictionary<string, string> Members;

	// Token: 0x0400056E RID: 1390
	public readonly Dictionary<string, ulong> Permissions;

	// Token: 0x0400056F RID: 1391
	internal readonly string OverridePassword;

	// Token: 0x04000570 RID: 1392
	private readonly string _overrideRole;

	// Token: 0x04000571 RID: 1393
	private readonly HashSet<ulong> _raPermissions;

	// Token: 0x04000572 RID: 1394
	private readonly YamlConfig _config;

	// Token: 0x04000573 RID: 1395
	private readonly YamlConfig _sharedGroups;

	// Token: 0x04000574 RID: 1396
	private ulong _lastPerm;

	// Token: 0x04000575 RID: 1397
	private readonly bool _managerAccess;

	// Token: 0x04000576 RID: 1398
	private readonly bool _banTeamAccess;

	// Token: 0x04000577 RID: 1399
	private readonly bool _banTeamSlots;

	// Token: 0x04000578 RID: 1400
	private readonly bool _banTeamGeoBypass;

	// Token: 0x04000579 RID: 1401
	public static readonly Dictionary<PlayerPermissions, string> PermissionCodes = new Dictionary<PlayerPermissions, string>
	{
		{
			PlayerPermissions.KickingAndShortTermBanning,
			"BN1"
		},
		{
			PlayerPermissions.BanningUpToDay,
			"BN2"
		},
		{
			PlayerPermissions.LongTermBanning,
			"BN3"
		},
		{
			PlayerPermissions.ForceclassSelf,
			"FSE"
		},
		{
			PlayerPermissions.ForceclassToSpectator,
			"FSP"
		},
		{
			PlayerPermissions.ForceclassWithoutRestrictions,
			"FWR"
		},
		{
			PlayerPermissions.GivingItems,
			"GIV"
		},
		{
			PlayerPermissions.WarheadEvents,
			"EWA"
		},
		{
			PlayerPermissions.RespawnEvents,
			"ERE"
		},
		{
			PlayerPermissions.RoundEvents,
			"ERO"
		},
		{
			PlayerPermissions.SetGroup,
			"SGR"
		},
		{
			PlayerPermissions.GameplayData,
			"GMD"
		},
		{
			PlayerPermissions.Overwatch,
			"OVR"
		},
		{
			PlayerPermissions.FacilityManagement,
			"FCM"
		},
		{
			PlayerPermissions.PlayersManagement,
			"PLM"
		},
		{
			PlayerPermissions.PermissionsManagement,
			"PRM"
		},
		{
			PlayerPermissions.ServerConsoleCommands,
			"SCC"
		},
		{
			PlayerPermissions.ViewHiddenBadges,
			"VHB"
		},
		{
			PlayerPermissions.ServerConfigs,
			"CFG"
		},
		{
			PlayerPermissions.Broadcasting,
			"BRC"
		},
		{
			PlayerPermissions.PlayerSensitiveDataAccess,
			"CDA"
		},
		{
			PlayerPermissions.Noclip,
			"NCP"
		},
		{
			PlayerPermissions.AFKImmunity,
			"AFK"
		},
		{
			PlayerPermissions.AdminChat,
			"ATC"
		},
		{
			PlayerPermissions.ViewHiddenGlobalBadges,
			"GHB"
		},
		{
			PlayerPermissions.Announcer,
			"ANN"
		},
		{
			PlayerPermissions.Effects,
			"EFF"
		},
		{
			PlayerPermissions.FriendlyFireDetectorImmunity,
			"FFI"
		},
		{
			PlayerPermissions.FriendlyFireDetectorTempDisable,
			"FFT"
		},
		{
			PlayerPermissions.ServerLogLiveFeed,
			"LLF"
		},
		{
			PlayerPermissions.ExecuteAs,
			"EXA"
		},
		{
			(PlayerPermissions)int.MinValue,
			"VAN"
		}
	};
}
