using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using CentralAuth;
using GameCore;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using Mirror;
using Mirror.RemoteCalls;
using NorthwoodLib.Pools;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Spectating;
using UnityEngine;

// Token: 0x02000133 RID: 307
public class NicknameSync : NetworkBehaviour
{
	// Token: 0x170000E7 RID: 231
	// (get) Token: 0x060005BE RID: 1470 RVA: 0x000155A8 File Offset: 0x000137A8
	public string CombinedName
	{
		get
		{
			if (!this.HasCustomName)
			{
				return this.MyNick;
			}
			return this._cleanDisplayName + " (" + this.MyNick + ")";
		}
	}

	// Token: 0x170000E8 RID: 232
	// (get) Token: 0x060005BF RID: 1471 RVA: 0x000155D4 File Offset: 0x000137D4
	// (set) Token: 0x060005C0 RID: 1472 RVA: 0x00015668 File Offset: 0x00013868
	public string MyNick
	{
		get
		{
			if (this.NickSet)
			{
				return this._firstNickname;
			}
			if (this._myNickSync == null)
			{
				return "(null)";
			}
			this.NickSet = true;
			this._firstNickname = Misc.SanitizeRichText(this._myNickSync.Replace("\n", string.Empty).Replace("\r", string.Empty), "＜", "＞");
			if (this._firstNickname.Length > 48)
			{
				this._firstNickname = this._firstNickname.Substring(0, 48);
			}
			return this._firstNickname;
		}
		set
		{
			if (value == null)
			{
				value = "(null)";
			}
			string text = Misc.SanitizeRichText(value, "＜", "＞");
			this.Network_myNickSync = ((value.Length > 48) ? text.Substring(0, 48) : text);
			if (!NetworkServer.active)
			{
				return;
			}
			this.NickSet = true;
			this._firstNickname = this._myNickSync;
		}
	}

	// Token: 0x170000E9 RID: 233
	// (get) Token: 0x060005C1 RID: 1473 RVA: 0x000156C7 File Offset: 0x000138C7
	// (set) Token: 0x060005C2 RID: 1474 RVA: 0x000156E8 File Offset: 0x000138E8
	public string DisplayName
	{
		get
		{
			return (this.HasCustomName ? this._cleanDisplayName : this.MyNick) ?? string.Empty;
		}
		set
		{
			PlayerChangingNicknameEventArgs playerChangingNicknameEventArgs = new PlayerChangingNicknameEventArgs(this._hub, this._displayName, value);
			PlayerEvents.OnChangingNickname(playerChangingNicknameEventArgs);
			if (!playerChangingNicknameEventArgs.IsAllowed)
			{
				return;
			}
			string cleanDisplayName = this._cleanDisplayName;
			string newNickname = playerChangingNicknameEventArgs.NewNickname;
			this.Network_displayName = newNickname;
			this.UpdatePlayerlistInstance(null, this._displayName);
			PlayerEvents.OnChangedNickname(new PlayerChangedNicknameEventArgs(this._hub, cleanDisplayName, this._displayName));
		}
	}

	// Token: 0x170000EA RID: 234
	// (get) Token: 0x060005C3 RID: 1475 RVA: 0x00015752 File Offset: 0x00013952
	// (set) Token: 0x060005C4 RID: 1476 RVA: 0x0001575A File Offset: 0x0001395A
	public PlayerInfoArea ShownPlayerInfo
	{
		get
		{
			return this._playerInfoToShow;
		}
		set
		{
			if (!NetworkServer.active)
			{
				return;
			}
			this.Network_playerInfoToShow = value;
		}
	}

	// Token: 0x170000EB RID: 235
	// (get) Token: 0x060005C5 RID: 1477 RVA: 0x0001576B File Offset: 0x0001396B
	// (set) Token: 0x060005C6 RID: 1478 RVA: 0x00015773 File Offset: 0x00013973
	public string CustomPlayerInfo
	{
		get
		{
			return this._customPlayerInfoString;
		}
		set
		{
			if (!NetworkServer.active)
			{
				return;
			}
			this.Network_customPlayerInfoString = value;
		}
	}

	// Token: 0x170000EC RID: 236
	// (get) Token: 0x060005C7 RID: 1479 RVA: 0x00015784 File Offset: 0x00013984
	// (set) Token: 0x060005C8 RID: 1480 RVA: 0x0001578C File Offset: 0x0001398C
	public bool NickSet { get; private set; }

	// Token: 0x170000ED RID: 237
	// (get) Token: 0x060005C9 RID: 1481 RVA: 0x00015795 File Offset: 0x00013995
	public bool HasCustomName
	{
		get
		{
			return this._cleanDisplayName != null;
		}
	}

	// Token: 0x060005CA RID: 1482 RVA: 0x000157A0 File Offset: 0x000139A0
	public static bool ValidateCustomInfo(string info, out string rejectionText)
	{
		rejectionText = string.Empty;
		if (string.IsNullOrEmpty(info))
		{
			return true;
		}
		if (!Misc.PlayerCustomInfoRegex.IsMatch(info))
		{
			rejectionText = "Provided text doesn't match the PlayerCustomInfo regex.";
			return false;
		}
		bool flag = info.Contains("<");
		bool flag2 = info.Contains("\\u003c");
		if (!flag && !flag2)
		{
			return true;
		}
		List<string> list = ListPool<string>.Shared.Rent();
		if (flag)
		{
			list.AddRange(info.Split('<', StringSplitOptions.None));
		}
		if (flag2)
		{
			list.AddRange(info.Split("\\u003c", StringSplitOptions.None));
		}
		bool result = true;
		foreach (string text in list)
		{
			if (!text.StartsWith("/", StringComparison.Ordinal) && !text.StartsWith("b>", StringComparison.Ordinal) && !text.StartsWith("i>", StringComparison.Ordinal) && !text.StartsWith("size=", StringComparison.Ordinal) && text.Length != 0)
			{
				if (text.StartsWith("color=", StringComparison.Ordinal))
				{
					if (text.Length < 14)
					{
						rejectionText = "Provided color tag doesn't match the requirements - Invalid color";
						result = false;
						break;
					}
					if (text[13] != '>')
					{
						rejectionText = "Provided color tag doesn't match the requirements - unclosed color tag (missing '>')";
						result = false;
						break;
					}
					if (!Misc.AcceptedColours.Contains(text.Substring(7, 6)))
					{
						rejectionText = "Provided color tag doesn't match the requirements - This color is not from allowed list";
						result = false;
						break;
					}
				}
				else
				{
					if (!text.StartsWith("#", StringComparison.Ordinal))
					{
						rejectionText = "Provided text has rich text tag which is not allowed";
						result = false;
						break;
					}
					if (text.Length < 8)
					{
						rejectionText = "Provided color tag doesn't match the requirements - Invalid color";
						result = false;
						break;
					}
					if (text[7] != '>')
					{
						rejectionText = "Provided color tag doesn't match the requirements - unclosed color tag (missing '>')";
						result = false;
						break;
					}
					if (!Misc.AcceptedColours.Contains(text.Substring(1, 6)))
					{
						rejectionText = "Provided color tag doesn't match the requirements - This color is not from allowed list";
						result = false;
						break;
					}
				}
			}
		}
		ListPool<string>.Shared.Return(list);
		return result;
	}

	// Token: 0x060005CB RID: 1483 RVA: 0x00002E9A File Offset: 0x0000109A
	public static void WriteDefaultInfo(ReferenceHub owner, StringBuilder sb, PlayerInfoArea? flagsOverride = null)
	{
	}

	// Token: 0x060005CC RID: 1484 RVA: 0x000159A4 File Offset: 0x00013BA4
	[ServerCallback]
	public void UpdateNickname(string n)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		this.NickSet = true;
		if (n == null)
		{
			ServerConsole.AddLog("Banned " + base.connectionToClient.address + " for passing null name.", ConsoleColor.Gray, false);
			BanPlayer.BanUser(this._hub, "Null name", 1577847600L);
			this.SetNick("(null)");
			return;
		}
		if (n.Length > 1024)
		{
			ServerConsole.AddLog("Banned " + base.connectionToClient.address + " for passing a too long name.", ConsoleColor.Gray, false);
			BanPlayer.BanUser(this._hub, "Too long name", 1577847600L);
			this.SetNick("(too long)");
			return;
		}
		bool flag;
		string text = this.CleanNickName(n, out flag);
		if (!flag)
		{
			ServerConsole.AddLog("Kicked " + base.connectionToClient.address + " for having an empty name.", ConsoleColor.Gray, false);
			ServerConsole.Disconnect(base.connectionToClient, "You may not have an empty name.");
			this.SetNick("Empty Name");
			return;
		}
		if (text.Length > 48)
		{
			text = text.Substring(0, 48);
		}
		this.SetNick(text);
	}

	// Token: 0x060005CD RID: 1485 RVA: 0x00015AC0 File Offset: 0x00013CC0
	private void Start()
	{
		this._hub = ReferenceHub.GetHub(base.gameObject);
		this._nickFilter = null;
		this._replacement = "";
		if (NetworkServer.active)
		{
			this.NetworkViewRange = ConfigFile.ServerConfig.GetFloat("player_info_range", 10f);
			string text = ConfigFile.ServerConfig.GetString("nickname_filter", "") ?? string.Empty;
			if (!string.IsNullOrEmpty(text))
			{
				this._nickFilter = new Regex(text, RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(500.0));
				this._replacement = (ConfigFile.ServerConfig.GetString("nickname_filter_replacement", "") ?? string.Empty);
			}
		}
		if (!base.isLocalPlayer)
		{
			return;
		}
		this.SetNick("Dedicated Server");
	}

	// Token: 0x060005CE RID: 1486 RVA: 0x00002E9A File Offset: 0x0000109A
	private void UpdatePlayerlistInstance(string p, string username)
	{
	}

	// Token: 0x060005CF RID: 1487 RVA: 0x00015B8C File Offset: 0x00013D8C
	private void UpdateCustomName(string p, string username)
	{
		if (string.IsNullOrWhiteSpace(username))
		{
			this._cleanDisplayName = null;
		}
		else
		{
			this._cleanDisplayName = Misc.SanitizeRichText(username.Replace("\n", string.Empty).Replace("\r", string.Empty), "", "").Trim();
			if (this._cleanDisplayName.Length > 48)
			{
				this._cleanDisplayName = this._cleanDisplayName.Substring(0, 48);
			}
			this._cleanDisplayName += "<color=#855439>*</color>";
		}
		this.UpdatePlayerlistInstance(p, username);
	}

	// Token: 0x060005D0 RID: 1488 RVA: 0x00015C24 File Offset: 0x00013E24
	private bool TryGetRayTransform(out Transform tr)
	{
		if (this._hub.roleManager.CurrentRole is IFpcRole)
		{
			tr = this._hub.PlayerCameraReference;
			return true;
		}
		ReferenceHub referenceHub;
		if (SpectatorTargetTracker.TryGetTrackedPlayer(out referenceHub) && referenceHub.roleManager.CurrentRole is IFpcRole)
		{
			tr = MainCameraController.CurrentCamera;
			return true;
		}
		tr = null;
		return false;
	}

	// Token: 0x060005D1 RID: 1489 RVA: 0x00002E9A File Offset: 0x0000109A
	private void Update()
	{
	}

	// Token: 0x060005D2 RID: 1490 RVA: 0x00002E9A File Offset: 0x0000109A
	private void SetCustomInfo(string oldValue, string newValue)
	{
	}

	// Token: 0x060005D3 RID: 1491 RVA: 0x00015C80 File Offset: 0x00013E80
	[Command(channel = 4)]
	private void CmdSetNick(string n)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(n);
		base.SendCommandInternal("System.Void NicknameSync::CmdSetNick(System.String)", -1313630199, writer, 4, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x060005D4 RID: 1492 RVA: 0x00015CBC File Offset: 0x00013EBC
	[Server]
	private void SetNick(string nick)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void NicknameSync::SetNick(System.String)' called when server was not active");
			return;
		}
		this.MyNick = nick;
		string text;
		try
		{
			Regex nickFilter = this._nickFilter;
			text = (((nickFilter != null) ? nickFilter.Replace(nick, this._replacement) : null) ?? nick);
		}
		catch (Exception arg)
		{
			ServerConsole.AddLog(string.Format("Error when filtering nick {0}: {1}", nick, arg), ConsoleColor.Gray, false);
			text = "(filter failed)";
		}
		if (nick != text)
		{
			this.DisplayName = text;
		}
		if (base.isLocalPlayer && ServerStatic.IsDedicated)
		{
			return;
		}
		ServerConsole.AddLog(string.Concat(new string[]
		{
			"Nickname of ",
			this._hub.authManager.UserId,
			" is now ",
			nick,
			"."
		}), ConsoleColor.Gray, false);
		ServerLogs.AddLog(ServerLogs.Modules.Networking, string.Concat(new string[]
		{
			"Nickname of ",
			this._hub.authManager.UserId,
			" is now ",
			nick,
			"."
		}), ServerLogs.ServerLogType.ConnectionUpdate, false);
	}

	// Token: 0x060005D5 RID: 1493 RVA: 0x00015DD8 File Offset: 0x00013FD8
	private string CleanNickName(string input, out bool printable)
	{
		StringBuilder stringBuilder = StringBuilderPool.Shared.Rent(input.Length);
		char c = '0';
		printable = false;
		foreach (char c2 in input)
		{
			if (char.IsLetterOrDigit(c2) || char.IsPunctuation(c2) || char.IsSymbol(c2))
			{
				printable = true;
				stringBuilder.Append(c2);
			}
			else if (char.IsWhiteSpace(c2) && c2 != '\n' && c2 != '\r' && c2 != '\t')
			{
				stringBuilder.Append(c2);
			}
			else if (char.IsHighSurrogate(c2))
			{
				c = c2;
			}
			else if (char.IsLowSurrogate(c2) && char.IsSurrogatePair(c, c2))
			{
				stringBuilder.Append(c);
				stringBuilder.Append(c2);
				printable = true;
			}
		}
		return StringBuilderPool.Shared.ToStringReturn(stringBuilder);
	}

	// Token: 0x060005D7 RID: 1495 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x170000EE RID: 238
	// (get) Token: 0x060005D8 RID: 1496 RVA: 0x00015EBC File Offset: 0x000140BC
	// (set) Token: 0x060005D9 RID: 1497 RVA: 0x00015ECF File Offset: 0x000140CF
	public float NetworkViewRange
	{
		get
		{
			return this.ViewRange;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<float>(value, ref this.ViewRange, 1UL, null);
		}
	}

	// Token: 0x170000EF RID: 239
	// (get) Token: 0x060005DA RID: 1498 RVA: 0x00015EEC File Offset: 0x000140EC
	// (set) Token: 0x060005DB RID: 1499 RVA: 0x00015EFF File Offset: 0x000140FF
	public string Network_customPlayerInfoString
	{
		get
		{
			return this._customPlayerInfoString;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<string>(value, ref this._customPlayerInfoString, 2UL, new Action<string, string>(this.SetCustomInfo));
		}
	}

	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x060005DC RID: 1500 RVA: 0x00015F24 File Offset: 0x00014124
	// (set) Token: 0x060005DD RID: 1501 RVA: 0x00015F37 File Offset: 0x00014137
	public PlayerInfoArea Network_playerInfoToShow
	{
		get
		{
			return this._playerInfoToShow;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<PlayerInfoArea>(value, ref this._playerInfoToShow, 4UL, null);
		}
	}

	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x060005DE RID: 1502 RVA: 0x00015F54 File Offset: 0x00014154
	// (set) Token: 0x060005DF RID: 1503 RVA: 0x00015F67 File Offset: 0x00014167
	public string Network_myNickSync
	{
		get
		{
			return this._myNickSync;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<string>(value, ref this._myNickSync, 8UL, new Action<string, string>(this.UpdatePlayerlistInstance));
		}
	}

	// Token: 0x170000F2 RID: 242
	// (get) Token: 0x060005E0 RID: 1504 RVA: 0x00015F8C File Offset: 0x0001418C
	// (set) Token: 0x060005E1 RID: 1505 RVA: 0x00015F9F File Offset: 0x0001419F
	public string Network_displayName
	{
		get
		{
			return this._displayName;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<string>(value, ref this._displayName, 16UL, new Action<string, string>(this.UpdateCustomName));
		}
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x00015FC4 File Offset: 0x000141C4
	protected void UserCode_CmdSetNick__String(string n)
	{
		if (base.isLocalPlayer)
		{
			this.MyNick = n;
			return;
		}
		if (this.NickSet)
		{
			return;
		}
		if (PlayerAuthenticationManager.OnlineMode)
		{
			return;
		}
		this.NickSet = true;
		if (n == null)
		{
			ServerConsole.AddLog("Banned " + base.connectionToClient.address + " for passing null name.", ConsoleColor.Gray, false);
			BanPlayer.BanUser(this._hub, "Null name", 1577847600L);
			this.SetNick("(null)");
			return;
		}
		if (n.Length > 1024)
		{
			ServerConsole.AddLog("Banned " + base.connectionToClient.address + " for passing a too long name.", ConsoleColor.Gray, false);
			BanPlayer.BanUser(this._hub, "Too long name", 1577847600L);
			this.SetNick("(too long)");
			return;
		}
		bool flag;
		string text = this.CleanNickName(n, out flag);
		if (!flag)
		{
			ServerConsole.AddLog("Kicked " + base.connectionToClient.address + " for having an empty name.", ConsoleColor.Gray, false);
			ServerConsole.Disconnect(base.connectionToClient, "You may not have an empty name.");
			this.SetNick("Empty Name");
			return;
		}
		text = Misc.SanitizeRichText(text, "＜", "＞");
		text = text.Replace("[", "(");
		text = text.Replace("]", ")");
		if (text.Length > 48)
		{
			text = text.Substring(0, 48);
		}
		this.SetNick(text);
		this._hub.characterClassManager.SyncServerCmdBinding();
		PlayerEvents.OnJoined(new PlayerJoinedEventArgs(this._hub));
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x00016149 File Offset: 0x00014349
	protected static void InvokeUserCode_CmdSetNick__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetNick called on client.");
			return;
		}
		((NicknameSync)obj).UserCode_CmdSetNick__String(reader.ReadString());
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x00016172 File Offset: 0x00014372
	static NicknameSync()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(NicknameSync), "System.Void NicknameSync::CmdSetNick(System.String)", new RemoteCallDelegate(NicknameSync.InvokeUserCode_CmdSetNick__String), true);
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x00016198 File Offset: 0x00014398
	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteFloat(this.ViewRange);
			writer.WriteString(this._customPlayerInfoString);
			Mirror.GeneratedNetworkCode._Write_PlayerInfoArea(writer, this._playerInfoToShow);
			writer.WriteString(this._myNickSync);
			writer.WriteString(this._displayName);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1UL) != 0UL)
		{
			writer.WriteFloat(this.ViewRange);
		}
		if ((base.syncVarDirtyBits & 2UL) != 0UL)
		{
			writer.WriteString(this._customPlayerInfoString);
		}
		if ((base.syncVarDirtyBits & 4UL) != 0UL)
		{
			Mirror.GeneratedNetworkCode._Write_PlayerInfoArea(writer, this._playerInfoToShow);
		}
		if ((base.syncVarDirtyBits & 8UL) != 0UL)
		{
			writer.WriteString(this._myNickSync);
		}
		if ((base.syncVarDirtyBits & 16UL) != 0UL)
		{
			writer.WriteString(this._displayName);
		}
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x000162A8 File Offset: 0x000144A8
	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			base.GeneratedSyncVarDeserialize<float>(ref this.ViewRange, null, reader.ReadFloat());
			base.GeneratedSyncVarDeserialize<string>(ref this._customPlayerInfoString, new Action<string, string>(this.SetCustomInfo), reader.ReadString());
			base.GeneratedSyncVarDeserialize<PlayerInfoArea>(ref this._playerInfoToShow, null, Mirror.GeneratedNetworkCode._Read_PlayerInfoArea(reader));
			base.GeneratedSyncVarDeserialize<string>(ref this._myNickSync, new Action<string, string>(this.UpdatePlayerlistInstance), reader.ReadString());
			base.GeneratedSyncVarDeserialize<string>(ref this._displayName, new Action<string, string>(this.UpdateCustomName), reader.ReadString());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<float>(ref this.ViewRange, null, reader.ReadFloat());
		}
		if ((num & 2L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<string>(ref this._customPlayerInfoString, new Action<string, string>(this.SetCustomInfo), reader.ReadString());
		}
		if ((num & 4L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<PlayerInfoArea>(ref this._playerInfoToShow, null, Mirror.GeneratedNetworkCode._Read_PlayerInfoArea(reader));
		}
		if ((num & 8L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<string>(ref this._myNickSync, new Action<string, string>(this.UpdatePlayerlistInstance), reader.ReadString());
		}
		if ((num & 16L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<string>(ref this._displayName, new Action<string, string>(this.UpdateCustomName), reader.ReadString());
		}
	}

	// Token: 0x04000559 RID: 1369
	public const string CustomInfoRejectionRegexFail = "Provided text doesn't match the PlayerCustomInfo regex.";

	// Token: 0x0400055A RID: 1370
	public const string CustomInfoRejectionInvalidTextColor = "Provided color tag doesn't match the requirements - Invalid color";

	// Token: 0x0400055B RID: 1371
	public const string CustomInfoRejectionUnclosedColorTag = "Provided color tag doesn't match the requirements - unclosed color tag (missing '>')";

	// Token: 0x0400055C RID: 1372
	public const string CustomInfoRejectionColorNotAllowed = "Provided color tag doesn't match the requirements - This color is not from allowed list";

	// Token: 0x0400055D RID: 1373
	public const string CustomInfoRejectionTagNotAllowed = "Provided text has rich text tag which is not allowed";

	// Token: 0x0400055E RID: 1374
	private const ushort MaxNicknameLen = 48;

	// Token: 0x0400055F RID: 1375
	private const float TextVisiblitySpeedChange = 3f;

	// Token: 0x04000560 RID: 1376
	public LayerMask RaycastMask;

	// Token: 0x04000562 RID: 1378
	[SyncVar]
	public float ViewRange;

	// Token: 0x04000563 RID: 1379
	private ReferenceHub _hub;

	// Token: 0x04000564 RID: 1380
	[SyncVar(hook = "SetCustomInfo")]
	private string _customPlayerInfoString;

	// Token: 0x04000565 RID: 1381
	[SyncVar]
	private PlayerInfoArea _playerInfoToShow = (PlayerInfoArea)(-1);

	// Token: 0x04000566 RID: 1382
	[SyncVar(hook = "UpdatePlayerlistInstance")]
	private string _myNickSync;

	// Token: 0x04000567 RID: 1383
	private string _firstNickname;

	// Token: 0x04000568 RID: 1384
	[SyncVar(hook = "UpdateCustomName")]
	private string _displayName;

	// Token: 0x04000569 RID: 1385
	private string _cleanDisplayName;

	// Token: 0x0400056A RID: 1386
	private Regex _nickFilter;

	// Token: 0x0400056B RID: 1387
	private string _replacement;
}
