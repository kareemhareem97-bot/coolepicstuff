using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Achievements;
using GameCore;
using InventorySystem.Configs;
using Mirror;
using UnityEngine;

// Token: 0x02000098 RID: 152
public class ServerConfigSynchronizer : NetworkBehaviour
{
	// Token: 0x0600033B RID: 827 RVA: 0x0000C184 File Offset: 0x0000A384
	public static void RefreshAllConfigs()
	{
		if (ServerConfigSynchronizer.Singleton == null)
		{
			return;
		}
		ServerConfigSynchronizer.Singleton.RefreshMainBools();
		ServerConfigSynchronizer.Singleton.RefreshCategoryLimits();
		ServerConfigSynchronizer.Singleton.RefreshAmmoLimits();
		ServerConfigSynchronizer.Singleton.RefreshRAConfigs();
		Action onRefreshed = ServerConfigSynchronizer.OnRefreshed;
		if (onRefreshed == null)
		{
			return;
		}
		onRefreshed();
	}

	// Token: 0x0600033C RID: 828 RVA: 0x0000C1D8 File Offset: 0x0000A3D8
	[Server]
	public void RefreshMainBools()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void ServerConfigSynchronizer::RefreshMainBools()' called when server was not active");
			return;
		}
		this.NetworkMainBoolsSync = Misc.BoolsToByte(ServerConsole.FriendlyFire, AchievementManager.AchievementsDisabled, false, false, false, false, false, false);
	}

	// Token: 0x0600033D RID: 829 RVA: 0x0000C218 File Offset: 0x0000A418
	[Server]
	public void RefreshRAConfigs()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void ServerConfigSynchronizer::RefreshRAConfigs()' called when server was not active");
			return;
		}
		this.NetworkEnableRemoteAdminPredefinedBanTemplates = ServerStatic.RolesConfig.GetBool("enable_predefined_ban_templates", true);
		this.NetworkRemoteAdminExternalPlayerLookupMode = ServerStatic.RolesConfig.GetString("external_player_lookup_mode", "disabled").Trim().ToLower();
		this.NetworkRemoteAdminExternalPlayerLookupURL = ServerStatic.RolesConfig.GetString("external_player_lookup_url", "");
		this.RemoteAdminExternalPlayerLookupToken = ServerStatic.RolesConfig.GetString("external_player_lookup_token", "");
		this.RemoteAdminPredefinedBanTemplates.Clear();
		if (!this.EnableRemoteAdminPredefinedBanTemplates)
		{
			return;
		}
		List<string> stringList = ServerStatic.RolesConfig.GetStringList("PredefinedBanTemplates");
		if (stringList != null)
		{
			foreach (string text in stringList)
			{
				string[] array = YamlConfig.ParseCommaSeparatedString(text);
				if (array != null)
				{
					int num;
					if (array.Length != 2)
					{
						ServerConsole.AddLog("Invalid ban template in RA Config file! Template: " + text, ConsoleColor.Gray, false);
					}
					else if (!int.TryParse(array[0], out num) || num < 0)
					{
						ServerConsole.AddLog("Invalid ban template in RA Config file - duration must be a non-negative integer. Ban template name: " + text, ConsoleColor.Gray, false);
					}
					else
					{
						ServerConfigSynchronizer.PredefinedBanTemplate item;
						item.Reason = array[1];
						TimeSpan timeSpan = TimeSpan.FromSeconds((double)num);
						item.Duration = (int)timeSpan.TotalMinutes;
						int num2 = timeSpan.Days / 365;
						if (num2 > 0)
						{
							item.FormattedDuration = string.Format("{0}y", num2);
						}
						else if (timeSpan.Days > 0)
						{
							item.FormattedDuration = string.Format("{0}d", timeSpan.Days);
						}
						else if (timeSpan.Hours > 0)
						{
							item.FormattedDuration = string.Format("{0}h", timeSpan.Hours);
						}
						else if (timeSpan.Minutes > 0)
						{
							item.FormattedDuration = string.Format("{0}m", timeSpan.Minutes);
						}
						else
						{
							item.FormattedDuration = string.Format("{0}s", timeSpan.Seconds);
						}
						this.RemoteAdminPredefinedBanTemplates.Add(item);
					}
				}
			}
			if (this.RemoteAdminPredefinedBanTemplates.Count == 0)
			{
				this.NetworkEnableRemoteAdminPredefinedBanTemplates = false;
				return;
			}
		}
		else
		{
			this.NetworkEnableRemoteAdminPredefinedBanTemplates = false;
		}
	}

	// Token: 0x0600033E RID: 830 RVA: 0x0000C488 File Offset: 0x0000A688
	[Server]
	public void RefreshCategoryLimits()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void ServerConfigSynchronizer::RefreshCategoryLimits()' called when server was not active");
			return;
		}
		this.CategoryLimits.Clear();
		ItemCategory[] values = EnumUtils<ItemCategory>.Values;
		for (int i = 0; i < values.Length; i++)
		{
			ItemCategory itemCategory = values[i];
			if ((int)itemCategory != i)
			{
				throw new InvalidOperationException(string.Format("Item categories (enum type: {0}) must start at zero and increment by 1.", typeof(ItemCategory)));
			}
			sbyte b;
			sbyte item;
			if (InventoryLimits.StandardCategoryLimits.TryGetValue(itemCategory, out b))
			{
				if (b >= 0)
				{
					string key = "limit_category_" + itemCategory.ToString().ToLowerInvariant();
					item = ConfigFile.ServerConfig.GetSByte(key, b);
				}
				else
				{
					item = b;
				}
			}
			else
			{
				item = sbyte.MaxValue;
			}
			this.CategoryLimits.Add(item);
		}
	}

	// Token: 0x0600033F RID: 831 RVA: 0x0000C548 File Offset: 0x0000A748
	[Server]
	public void RefreshAmmoLimits()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void ServerConfigSynchronizer::RefreshAmmoLimits()' called when server was not active");
			return;
		}
		if (this.AmmoLimitsSync.Count > 0)
		{
			this.AmmoLimitsSync.Clear();
		}
		foreach (KeyValuePair<ItemType, ushort> keyValuePair in InventoryLimits.StandardAmmoLimits)
		{
			ushort @ushort = ConfigFile.ServerConfig.GetUShort("limit_" + keyValuePair.Key.ToString().ToLowerInvariant(), keyValuePair.Value);
			this.AmmoLimitsSync.Add(new ServerConfigSynchronizer.AmmoLimit
			{
				AmmoType = keyValuePair.Key,
				Limit = @ushort
			});
		}
	}

	// Token: 0x06000340 RID: 832 RVA: 0x0000C624 File Offset: 0x0000A824
	private void Awake()
	{
		ServerConfigSynchronizer.Singleton = this;
	}

	// Token: 0x06000341 RID: 833 RVA: 0x0000C62C File Offset: 0x0000A82C
	private void Update()
	{
		if (this._ready)
		{
			return;
		}
		if (!NetworkServer.active)
		{
			this._ready = true;
			return;
		}
		ReferenceHub referenceHub;
		if (!ReferenceHub.TryGetHostHub(out referenceHub))
		{
			return;
		}
		this._ready = true;
		ServerConfigSynchronizer.RefreshAllConfigs();
	}

	// Token: 0x06000342 RID: 834 RVA: 0x0000C668 File Offset: 0x0000A868
	public ServerConfigSynchronizer()
	{
		base.InitSyncObject(this.CategoryLimits);
		base.InitSyncObject(this.AmmoLimitsSync);
		base.InitSyncObject(this.RemoteAdminPredefinedBanTemplates);
	}

	// Token: 0x06000343 RID: 835 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x170000A1 RID: 161
	// (get) Token: 0x06000344 RID: 836 RVA: 0x0000C6E8 File Offset: 0x0000A8E8
	// (set) Token: 0x06000345 RID: 837 RVA: 0x0000C6FB File Offset: 0x0000A8FB
	public byte NetworkMainBoolsSync
	{
		get
		{
			return this.MainBoolsSync;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<byte>(value, ref this.MainBoolsSync, 1UL, null);
		}
	}

	// Token: 0x170000A2 RID: 162
	// (get) Token: 0x06000346 RID: 838 RVA: 0x0000C718 File Offset: 0x0000A918
	// (set) Token: 0x06000347 RID: 839 RVA: 0x0000C72B File Offset: 0x0000A92B
	public string NetworkServerName
	{
		get
		{
			return this.ServerName;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<string>(value, ref this.ServerName, 2UL, null);
		}
	}

	// Token: 0x170000A3 RID: 163
	// (get) Token: 0x06000348 RID: 840 RVA: 0x0000C748 File Offset: 0x0000A948
	// (set) Token: 0x06000349 RID: 841 RVA: 0x0000C75B File Offset: 0x0000A95B
	public bool NetworkEnableRemoteAdminPredefinedBanTemplates
	{
		get
		{
			return this.EnableRemoteAdminPredefinedBanTemplates;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<bool>(value, ref this.EnableRemoteAdminPredefinedBanTemplates, 4UL, null);
		}
	}

	// Token: 0x170000A4 RID: 164
	// (get) Token: 0x0600034A RID: 842 RVA: 0x0000C778 File Offset: 0x0000A978
	// (set) Token: 0x0600034B RID: 843 RVA: 0x0000C78B File Offset: 0x0000A98B
	public string NetworkRemoteAdminExternalPlayerLookupMode
	{
		get
		{
			return this.RemoteAdminExternalPlayerLookupMode;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<string>(value, ref this.RemoteAdminExternalPlayerLookupMode, 8UL, null);
		}
	}

	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x0600034C RID: 844 RVA: 0x0000C7A8 File Offset: 0x0000A9A8
	// (set) Token: 0x0600034D RID: 845 RVA: 0x0000C7BB File Offset: 0x0000A9BB
	public string NetworkRemoteAdminExternalPlayerLookupURL
	{
		get
		{
			return this.RemoteAdminExternalPlayerLookupURL;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<string>(value, ref this.RemoteAdminExternalPlayerLookupURL, 16UL, null);
		}
	}

	// Token: 0x0600034E RID: 846 RVA: 0x0000C7D8 File Offset: 0x0000A9D8
	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteByte(this.MainBoolsSync);
			writer.WriteString(this.ServerName);
			writer.WriteBool(this.EnableRemoteAdminPredefinedBanTemplates);
			writer.WriteString(this.RemoteAdminExternalPlayerLookupMode);
			writer.WriteString(this.RemoteAdminExternalPlayerLookupURL);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1UL) != 0UL)
		{
			writer.WriteByte(this.MainBoolsSync);
		}
		if ((base.syncVarDirtyBits & 2UL) != 0UL)
		{
			writer.WriteString(this.ServerName);
		}
		if ((base.syncVarDirtyBits & 4UL) != 0UL)
		{
			writer.WriteBool(this.EnableRemoteAdminPredefinedBanTemplates);
		}
		if ((base.syncVarDirtyBits & 8UL) != 0UL)
		{
			writer.WriteString(this.RemoteAdminExternalPlayerLookupMode);
		}
		if ((base.syncVarDirtyBits & 16UL) != 0UL)
		{
			writer.WriteString(this.RemoteAdminExternalPlayerLookupURL);
		}
	}

	// Token: 0x0600034F RID: 847 RVA: 0x0000C8E8 File Offset: 0x0000AAE8
	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			base.GeneratedSyncVarDeserialize<byte>(ref this.MainBoolsSync, null, reader.ReadByte());
			base.GeneratedSyncVarDeserialize<string>(ref this.ServerName, null, reader.ReadString());
			base.GeneratedSyncVarDeserialize<bool>(ref this.EnableRemoteAdminPredefinedBanTemplates, null, reader.ReadBool());
			base.GeneratedSyncVarDeserialize<string>(ref this.RemoteAdminExternalPlayerLookupMode, null, reader.ReadString());
			base.GeneratedSyncVarDeserialize<string>(ref this.RemoteAdminExternalPlayerLookupURL, null, reader.ReadString());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<byte>(ref this.MainBoolsSync, null, reader.ReadByte());
		}
		if ((num & 2L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<string>(ref this.ServerName, null, reader.ReadString());
		}
		if ((num & 4L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<bool>(ref this.EnableRemoteAdminPredefinedBanTemplates, null, reader.ReadBool());
		}
		if ((num & 8L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<string>(ref this.RemoteAdminExternalPlayerLookupMode, null, reader.ReadString());
		}
		if ((num & 16L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<string>(ref this.RemoteAdminExternalPlayerLookupURL, null, reader.ReadString());
		}
	}

	// Token: 0x040002D7 RID: 727
	public static ServerConfigSynchronizer Singleton;

	// Token: 0x040002D8 RID: 728
	public static Action OnRefreshed;

	// Token: 0x040002D9 RID: 729
	[SyncVar]
	public byte MainBoolsSync;

	// Token: 0x040002DA RID: 730
	public SyncList<sbyte> CategoryLimits = new SyncList<sbyte>();

	// Token: 0x040002DB RID: 731
	public SyncList<ServerConfigSynchronizer.AmmoLimit> AmmoLimitsSync = new SyncList<ServerConfigSynchronizer.AmmoLimit>();

	// Token: 0x040002DC RID: 732
	[SyncVar]
	public string ServerName;

	// Token: 0x040002DD RID: 733
	[SyncVar]
	public bool EnableRemoteAdminPredefinedBanTemplates = true;

	// Token: 0x040002DE RID: 734
	[SyncVar]
	public string RemoteAdminExternalPlayerLookupMode = "disabled";

	// Token: 0x040002DF RID: 735
	[SyncVar]
	public string RemoteAdminExternalPlayerLookupURL = "";

	// Token: 0x040002E0 RID: 736
	[NonSerialized]
	public string RemoteAdminExternalPlayerLookupToken = string.Empty;

	// Token: 0x040002E1 RID: 737
	public readonly SyncList<ServerConfigSynchronizer.PredefinedBanTemplate> RemoteAdminPredefinedBanTemplates = new SyncList<ServerConfigSynchronizer.PredefinedBanTemplate>();

	// Token: 0x040002E2 RID: 738
	private bool _ready;

	// Token: 0x02000099 RID: 153
	[Flags]
	public enum MainBoolsSettings : byte
	{
		// Token: 0x040002E4 RID: 740
		FriendlyFire = 1,
		// Token: 0x040002E5 RID: 741
		DisabledAchievements = 2
	}

	// Token: 0x0200009A RID: 154
	[Serializable]
	public struct AmmoLimit
	{
		// Token: 0x040002E6 RID: 742
		public ItemType AmmoType;

		// Token: 0x040002E7 RID: 743
		public ushort Limit;
	}

	// Token: 0x0200009B RID: 155
	public struct PredefinedBanTemplate
	{
		// Token: 0x040002E8 RID: 744
		public int Duration;

		// Token: 0x040002E9 RID: 745
		public string FormattedDuration;

		// Token: 0x040002EA RID: 746
		public string Reason;
	}
}
