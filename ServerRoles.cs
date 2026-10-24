using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CentralAuth;
using Cryptography;
using GameCore;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using Mirror;
using Mirror.RemoteCalls;
using NetworkManagerUtils.Dummies;
using NorthwoodLib;
using PlayerRoles;
using PlayerRoles.Spectating;
using PlayerStatsSystem;
using UnityEngine;

// Token: 0x02000161 RID: 353
public class ServerRoles : NetworkBehaviour
{
	// Token: 0x17000113 RID: 275
	// (get) Token: 0x060006C8 RID: 1736 RVA: 0x000196CC File Offset: 0x000178CC
	// (set) Token: 0x060006C9 RID: 1737 RVA: 0x000196D4 File Offset: 0x000178D4
	public bool BypassMode
	{
		get
		{
			return this._bypassMode;
		}
		set
		{
			this._bypassMode = value;
			this._hub.playerStats.GetModule<AdminFlagsStat>().SetFlag(AdminFlags.BypassMode, value);
		}
	}

	// Token: 0x17000114 RID: 276
	// (get) Token: 0x060006CA RID: 1738 RVA: 0x000196F4 File Offset: 0x000178F4
	// (set) Token: 0x060006CB RID: 1739 RVA: 0x0001970E File Offset: 0x0001790E
	public bool IsInOverwatch
	{
		get
		{
			return this._hub.roleManager.CurrentRole is OverwatchRole;
		}
		set
		{
			if (value == this.IsInOverwatch)
			{
				return;
			}
			this._hub.roleManager.ServerSetRole(value ? RoleTypeId.Overwatch : RoleTypeId.Spectator, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.All, null);
		}
	}

	// Token: 0x17000115 RID: 277
	// (get) Token: 0x060006CC RID: 1740 RVA: 0x00019735 File Offset: 0x00017935
	// (set) Token: 0x060006CD RID: 1741 RVA: 0x0001973D File Offset: 0x0001793D
	public UserGroup Group
	{
		get
		{
			return this._group;
		}
		set
		{
			this.SetGroup(value, false, false);
		}
	}

	// Token: 0x17000116 RID: 278
	// (get) Token: 0x060006CE RID: 1742 RVA: 0x00019748 File Offset: 0x00017948
	private Dictionary<string, ServerRoles.NamedColor> NamedColorsDic
	{
		get
		{
			if (ServerRoles._colorDictionaryCacheSet)
			{
				return ServerRoles.DictionarizedColorsCache;
			}
			foreach (ServerRoles.NamedColor namedColor in this.NamedColors)
			{
				ServerRoles.DictionarizedColorsCache[namedColor.Name] = namedColor;
			}
			ServerRoles._colorDictionaryCacheSet = true;
			return ServerRoles.DictionarizedColorsCache;
		}
	}

	// Token: 0x17000117 RID: 279
	// (get) Token: 0x060006CF RID: 1743 RVA: 0x00019797 File Offset: 0x00017997
	internal byte KickPower
	{
		get
		{
			if (this._hub.authManager.RemoteAdminGlobalAccess)
			{
				return byte.MaxValue;
			}
			UserGroup group = this.Group;
			if (group == null)
			{
				return 0;
			}
			return group.KickPower;
		}
	}

	// Token: 0x17000118 RID: 280
	// (get) Token: 0x060006D0 RID: 1744 RVA: 0x000197C2 File Offset: 0x000179C2
	// (set) Token: 0x060006D1 RID: 1745 RVA: 0x000197CA File Offset: 0x000179CA
	public string MyColor { get; private set; }

	// Token: 0x17000119 RID: 281
	// (get) Token: 0x060006D2 RID: 1746 RVA: 0x000197D3 File Offset: 0x000179D3
	// (set) Token: 0x060006D3 RID: 1747 RVA: 0x000197DB File Offset: 0x000179DB
	public string MyText { get; private set; }

	// Token: 0x1700011A RID: 282
	// (get) Token: 0x060006D4 RID: 1748 RVA: 0x000197E4 File Offset: 0x000179E4
	public bool GlobalSet
	{
		get
		{
			return this.GlobalBadge != null || (this.MyText != null && this.MyText.StartsWith("[", StringComparison.Ordinal) && this.MyText.EndsWith("]", StringComparison.Ordinal));
		}
	}

	// Token: 0x060006D5 RID: 1749 RVA: 0x00004094 File Offset: 0x00002294
	public static ServerRoles.BadgeVisibilityPreferences GetGlobalBadgePreferences()
	{
		return ServerRoles.BadgeVisibilityPreferences.NoPreference;
	}

	// Token: 0x060006D6 RID: 1750 RVA: 0x0001981E File Offset: 0x00017A1E
	public void Start()
	{
		this._hub = ReferenceHub.GetHub(base.gameObject);
		if (this._hub.IsDummy)
		{
			this.SetGroup(DummyUtils.DummyGroup, true, false);
		}
	}

	// Token: 0x060006D7 RID: 1751 RVA: 0x0001984C File Offset: 0x00017A4C
	[TargetRpc]
	private void TargetSetHiddenRole(NetworkConnection connection, string role)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(role);
		this.SendTargetRPCInternal(connection, "System.Void ServerRoles::TargetSetHiddenRole(Mirror.NetworkConnection,System.String)", -1356032325, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x060006D8 RID: 1752 RVA: 0x00019888 File Offset: 0x00017A88
	[ClientRpc]
	public void RpcResetFixed()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		this.SendRPCInternal("System.Void ServerRoles::RpcResetFixed()", 87745685, writer, 0, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x060006D9 RID: 1753 RVA: 0x000198B8 File Offset: 0x00017AB8
	[Command(channel = 4)]
	private void CmdSetLocalTagPreferences(ServerRoles.BadgePreferences userPreferences, ServerRoles.BadgeVisibilityPreferences globalPreferences, ServerRoles.BadgeVisibilityPreferences localPreferences, bool refresh)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		Mirror.GeneratedNetworkCode._Write_ServerRoles/BadgePreferences(writer, userPreferences);
		Mirror.GeneratedNetworkCode._Write_ServerRoles/BadgeVisibilityPreferences(writer, globalPreferences);
		Mirror.GeneratedNetworkCode._Write_ServerRoles/BadgeVisibilityPreferences(writer, localPreferences);
		writer.WriteBool(refresh);
		base.SendCommandInternal("System.Void ServerRoles::CmdSetLocalTagPreferences(ServerRoles/BadgePreferences,ServerRoles/BadgeVisibilityPreferences,ServerRoles/BadgeVisibilityPreferences,System.Boolean)", -1971566213, writer, 4, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x060006DA RID: 1754 RVA: 0x00019910 File Offset: 0x00017B10
	[Server]
	private void RefreshGlobalBadgeVisibility(ServerRoles.BadgeVisibilityPreferences globalPreferences)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void ServerRoles::RefreshGlobalBadgeVisibility(ServerRoles/BadgeVisibilityPreferences)' called when server was not active");
			return;
		}
		this.RefreshGlobalTag();
		if (globalPreferences != ServerRoles.BadgeVisibilityPreferences.Hidden)
		{
			return;
		}
		this.TryHideTag();
	}

	// Token: 0x060006DB RID: 1755 RVA: 0x0001993A File Offset: 0x00017B3A
	[Server]
	private void RefreshLocalBadgeVisibility(ServerRoles.BadgeVisibilityPreferences localPreferences)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void ServerRoles::RefreshLocalBadgeVisibility(ServerRoles/BadgeVisibilityPreferences)' called when server was not active");
			return;
		}
		this.RefreshLocalTag();
		if (localPreferences != ServerRoles.BadgeVisibilityPreferences.Hidden)
		{
			return;
		}
		this.TryHideTag();
	}

	// Token: 0x060006DC RID: 1756 RVA: 0x00019964 File Offset: 0x00017B64
	[Server]
	public void RefreshPermissions(bool disp = false)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void ServerRoles::RefreshPermissions(System.Boolean)' called when server was not active");
			return;
		}
		UserGroup userGroup = ServerStatic.PermissionsHandler.GetUserGroup(this._hub.authManager.UserId);
		if (userGroup != null)
		{
			this.SetGroup(userGroup, false, disp);
		}
	}

	// Token: 0x060006DD RID: 1757 RVA: 0x000199B0 File Offset: 0x00017BB0
	[Server]
	public void SetGroup(UserGroup group, bool byAdmin = false, bool disp = false)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void ServerRoles::SetGroup(UserGroup,System.Boolean,System.Boolean)' called when server was not active");
			return;
		}
		PlayerGroupChangingEventArgs playerGroupChangingEventArgs = new PlayerGroupChangingEventArgs(this._hub, group);
		PlayerEvents.OnGroupChanging(playerGroupChangingEventArgs);
		if (!playerGroupChangingEventArgs.IsAllowed)
		{
			return;
		}
		group = playerGroupChangingEventArgs.Group;
		if (group == null)
		{
			this.RemoteAdmin = (this.GlobalPerms > 0UL);
			this.Permissions = this.GlobalPerms;
			this.GlobalHidden = false;
			this._group = null;
			this.SetColor(null);
			this.SetText(null);
			this.BadgeCover = false;
			this.RpcResetFixed();
			this.FinalizeSetGroup();
			this._hub.gameConsoleTransmission.SendToClient("Your local permissions has been revoked by server administrator.", "red");
			return;
		}
		this._hub.gameConsoleTransmission.SendToClient((!byAdmin) ? "Updating your group on server (local permissions)..." : "Updating your group on server (set by server administrator)...", "cyan");
		this._group = group;
		this.BadgeCover = group.Cover;
		if ((group.Permissions | this.GlobalPerms) > 0UL && ServerStatic.PermissionsHandler.IsRaPermitted(group.Permissions | this.GlobalPerms))
		{
			this.Permissions = (group.Permissions | this.GlobalPerms);
			this._hub.authManager.ResetPasswordAttempts();
			this._hub.gameConsoleTransmission.SendToClient((!byAdmin) ? "Your remote admin access has been granted (local permissions)." : "Your remote admin access has been granted (set by server administrator).", "cyan");
		}
		else
		{
			this.Permissions = (group.Permissions | this.GlobalPerms);
		}
		ServerLogs.AddLog(ServerLogs.Modules.Permissions, this._hub.LoggedNameFromRefHub() + " has been assigned to group " + group.BadgeText + ".", ServerLogs.ServerLogType.ConnectionUpdate, false);
		if (group.BadgeColor == "none")
		{
			this.RpcResetFixed();
			this.FinalizeSetGroup();
			return;
		}
		if ((this._localBadgeVisibilityPreferences == ServerRoles.BadgeVisibilityPreferences.Hidden && !disp) || (group.HiddenByDefault && !disp && this._localBadgeVisibilityPreferences != ServerRoles.BadgeVisibilityPreferences.Visible))
		{
			this.BadgeCover = (this.UserBadgePreferences == ServerRoles.BadgePreferences.PreferLocal);
			if (!string.IsNullOrEmpty(this.MyText))
			{
				return;
			}
			this.SetText(null);
			this.SetColor("default");
			this.GlobalHidden = false;
			this.HiddenBadge = group.BadgeText;
			this.RefreshHiddenTag();
			this.TargetSetHiddenRole(base.connectionToClient, group.BadgeText);
			if (!byAdmin)
			{
				this._hub.gameConsoleTransmission.SendToClient("Your role has been granted, but it's hidden. Use \"showtag\" command in the game console to show your server badge.", "yellow");
			}
			else
			{
				this._hub.gameConsoleTransmission.SendToClient("Your role has been granted to you (set by server administrator), but it's hidden. Use \"showtag\" command in the game console to show your server badge.", "cyan");
			}
		}
		else
		{
			this.HiddenBadge = null;
			this.GlobalHidden = false;
			this.RpcResetFixed();
			this.SetText(group.BadgeText);
			this.SetColor(group.BadgeColor);
			if (!byAdmin)
			{
				this._hub.gameConsoleTransmission.SendToClient(string.Concat(new string[]
				{
					"Your role \"",
					group.BadgeText,
					"\" with color ",
					group.BadgeColor,
					" has been granted to you (local permissions)."
				}), "cyan");
			}
			else
			{
				this._hub.gameConsoleTransmission.SendToClient(string.Concat(new string[]
				{
					"Your role \"",
					group.BadgeText,
					"\" with color ",
					group.BadgeColor,
					" has been granted to you (set by server administrator)."
				}), "cyan");
			}
		}
		this.FinalizeSetGroup();
	}

	// Token: 0x060006DE RID: 1758 RVA: 0x00019CF4 File Offset: 0x00017EF4
	[Server]
	public void FinalizeSetGroup()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void ServerRoles::FinalizeSetGroup()' called when server was not active");
			return;
		}
		this.Permissions |= this.GlobalPerms;
		this.RemoteAdmin = ServerStatic.PermissionsHandler.IsRaPermitted(this.Permissions | this.GlobalPerms);
		this.AdminChatPerms = PermissionsHandler.IsPermitted(this.Permissions, PlayerPermissions.AdminChat);
		this._hub.queryProcessor.GameplayData = PermissionsHandler.IsPermitted(this.Permissions, PlayerPermissions.GameplayData);
		if (this.RemoteAdmin)
		{
			this.OpenRemoteAdmin();
		}
		else
		{
			this.TargetSetRemoteAdmin(false);
		}
		this.SendRealIds();
		bool flag = PermissionsHandler.IsPermitted(this.Permissions, PlayerPermissions.ViewHiddenBadges);
		bool flag2 = PermissionsHandler.IsPermitted(this.Permissions, PlayerPermissions.ViewHiddenGlobalBadges);
		if (flag || flag2)
		{
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				if (referenceHub.Mode != ClientInstanceMode.DedicatedServer)
				{
					ServerRoles serverRoles = referenceHub.serverRoles;
					if (!string.IsNullOrEmpty(serverRoles.HiddenBadge) && (!serverRoles.GlobalHidden || flag2) && (serverRoles.GlobalHidden || flag))
					{
						serverRoles.TargetSetHiddenRole(base.connectionToClient, serverRoles.HiddenBadge);
					}
				}
			}
			if (flag && flag2)
			{
				this._hub.gameConsoleTransmission.SendToClient("Hidden badges (local and global) have been displayed for you (if there are any).", "gray");
			}
			else if (flag)
			{
				this._hub.gameConsoleTransmission.SendToClient("Hidden badges (local only) have been displayed for you (if there are any).", "gray");
			}
			else
			{
				this._hub.gameConsoleTransmission.SendToClient("Hidden badges (global only) have been displayed for you (if there are any).", "gray");
			}
		}
		PlayerEvents.OnGroupChanged(new PlayerGroupChangedEventArgs(this._hub, this._group));
	}

	// Token: 0x060006DF RID: 1759 RVA: 0x00019EC0 File Offset: 0x000180C0
	[ServerCallback]
	public void RefreshHiddenTag()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
		{
			if (referenceHub.Mode != ClientInstanceMode.DedicatedServer)
			{
				ServerRoles serverRoles = referenceHub.serverRoles;
				bool flag = PermissionsHandler.IsPermitted(serverRoles.Permissions, PlayerPermissions.ViewHiddenBadges);
				bool flag2 = PermissionsHandler.IsPermitted(serverRoles.Permissions, PlayerPermissions.ViewHiddenGlobalBadges);
				if ((!this.GlobalHidden || flag2) && (this.GlobalHidden || flag))
				{
					this.TargetSetHiddenRole(serverRoles.connectionToClient, this.HiddenBadge);
				}
			}
		}
	}

	// Token: 0x060006E0 RID: 1760 RVA: 0x00019F74 File Offset: 0x00018174
	[Server]
	public void RefreshRealId()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void ServerRoles::RefreshRealId()' called when server was not active");
			return;
		}
		if (this._hub == null)
		{
			return;
		}
		foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
		{
			if (referenceHub.Mode != ClientInstanceMode.DedicatedServer && (PermissionsHandler.IsPermitted(referenceHub.serverRoles.Permissions, 18007046UL) || referenceHub.authManager.NorthwoodStaff))
			{
				this._hub.authManager.TargetSetRealId(referenceHub.networkIdentity.connectionToClient, this._hub.authManager.UserId);
			}
		}
	}

	// Token: 0x060006E1 RID: 1761 RVA: 0x0001A03C File Offset: 0x0001823C
	[Server]
	private void SendRealIds()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void ServerRoles::SendRealIds()' called when server was not active");
			return;
		}
		if (this._hub.Mode == ClientInstanceMode.DedicatedServer)
		{
			return;
		}
		bool flag = this._hub.authManager.NorthwoodStaff || PermissionsHandler.IsPermitted(this.Permissions, 18007046UL);
		if (!flag && !this._lastRealIdPerm)
		{
			return;
		}
		this._lastRealIdPerm = flag;
		foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
		{
			referenceHub.authManager.TargetSetRealId(this._hub.networkIdentity.connectionToClient, flag ? referenceHub.authManager.UserId : null);
		}
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x0001A114 File Offset: 0x00018314
	public string GetColoredRoleString(bool newLine = false)
	{
		if (string.IsNullOrEmpty(this.MyColor) || string.IsNullOrEmpty(this.MyText) || this.CurrentColor == null)
		{
			return string.Empty;
		}
		if ((this.CurrentColor.Restricted || this.HasNotAllowedText) && !this._authorizeBadge)
		{
			return string.Empty;
		}
		foreach (ServerRoles.NamedColor namedColor in this.NamedColors)
		{
			if (namedColor.Name == this.MyColor)
			{
				return string.Concat(new string[]
				{
					newLine ? "\n" : string.Empty,
					"<color=#",
					namedColor.ColorHex,
					">",
					this.MyText,
					"</color>"
				});
			}
		}
		return string.Empty;
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x0001A1E8 File Offset: 0x000183E8
	public string GetUncoloredRoleString()
	{
		if (string.IsNullOrEmpty(this.MyText) && !string.IsNullOrEmpty(this.FixedBadge))
		{
			return this.FixedBadge;
		}
		if (string.IsNullOrEmpty(this.MyColor) || string.IsNullOrEmpty(this.MyText) || this.CurrentColor == null)
		{
			return string.Empty;
		}
		if ((this.CurrentColor.Restricted || this.HasNotAllowedText) && !this._authorizeBadge)
		{
			return string.Empty;
		}
		return this.MyText;
	}

	// Token: 0x060006E4 RID: 1764 RVA: 0x0001A267 File Offset: 0x00018467
	public Color GetColor()
	{
		return Color.clear;
	}

	// Token: 0x060006E5 RID: 1765 RVA: 0x0001A270 File Offset: 0x00018470
	public Color GetVoiceColor()
	{
		ServerRoles.NamedColor namedColor;
		if (string.IsNullOrEmpty(this.MyColor) || !this.NamedColorsDic.TryGetValue(this.MyColor, out namedColor))
		{
			return this.NamedColorsDic["default"].SpeakingColor;
		}
		return namedColor.SpeakingColor;
	}

	// Token: 0x1700011B RID: 283
	// (get) Token: 0x060006E6 RID: 1766 RVA: 0x0001A2BC File Offset: 0x000184BC
	private bool HasNotAllowedText
	{
		get
		{
			return this.MyText.Contains("[", StringComparison.Ordinal) || this.MyText.Contains("]", StringComparison.Ordinal) || this.MyText.Contains("<", StringComparison.Ordinal) || this.MyText.Contains(">", StringComparison.Ordinal) || this.MyText.Contains("\\u003c", StringComparison.Ordinal) || this.MyText.Contains("\\u003e", StringComparison.Ordinal);
		}
	}

	// Token: 0x060006E7 RID: 1767 RVA: 0x0001A33C File Offset: 0x0001853C
	private void Update()
	{
		if (!string.IsNullOrEmpty(this.FixedBadge) && this.MyText != this.FixedBadge)
		{
			this.SetText(this.FixedBadge);
			this.SetColor("silver");
			return;
		}
		ServerRoles.NamedColor currentColor;
		if (!string.IsNullOrEmpty(this.FixedBadge))
		{
			currentColor = this.CurrentColor;
			if (currentColor == null || !(currentColor.Name == "silver"))
			{
				this.SetColor("silver");
				return;
			}
		}
		if (this.GlobalBadge != this._prevBadge)
		{
			this._prevBadge = this.GlobalBadge;
			if (string.IsNullOrEmpty(this.GlobalBadge))
			{
				this._bgc = null;
				this._bgt = null;
				this._authorizeBadge = false;
				if (this._prevColor != null)
				{
					this._prevColor += ".";
				}
				else
				{
					this._prevColor = ".";
				}
				if (this._prevText != null)
				{
					this._prevText += ".";
					return;
				}
				this._prevText = ".";
				return;
			}
			else
			{
				GameCore.Console.AddDebugLog("SDAUTH", "Validating global badge of user " + this._hub.nicknameSync.MyNick, MessageImportance.LessImportant, false);
				BadgeToken badgeToken;
				string text;
				string text2;
				if (new SignedToken(this.GlobalBadge, this.GlobalBadgeSignature).TryGetToken<BadgeToken>("Badge request", out badgeToken, out text, out text2, 0))
				{
					if (badgeToken.UserId != this._hub.authManager.SaltedUserId && badgeToken.UserId != Sha.HashToString(Sha.Sha512(this._hub.authManager.SaltedUserId)))
					{
						text = "badge token UserID mismatch.";
					}
					else if (StringUtils.Base64Decode(badgeToken.Nickname) != this._hub.nicknameSync.MyNick)
					{
						text = "badge token nickname mismatch.";
					}
					if (text != null)
					{
						GameCore.Console.AddDebugLog("SDAUTH", string.Concat(new string[]
						{
							"<color=red>Validation of global badge of user ",
							this._hub.nicknameSync.MyNick,
							" failed - ",
							text,
							".</color>"
						}), MessageImportance.Normal, false);
						this._bgc = null;
						this._bgt = null;
						this._authorizeBadge = false;
						if (this._prevColor != null)
						{
							this._prevColor += ".";
						}
						else
						{
							this._prevColor = ".";
						}
						if (this._prevText != null)
						{
							this._prevText += ".";
							return;
						}
						this._prevText = ".";
						return;
					}
					else
					{
						GameCore.Console.AddDebugLog("SDAUTH", string.Concat(new string[]
						{
							"Validation of global badge of user ",
							base.GetComponent<NicknameSync>().MyNick,
							" complete - badge signed by central server ",
							badgeToken.IssuedBy,
							"."
						}), MessageImportance.LessImportant, false);
						text2 = badgeToken.BadgeText;
						if (text2 == null || text2 == "(none)")
						{
							text2 = badgeToken.BadgeColor;
							if (text2 == null || text2 == "(none)")
							{
								this._bgc = null;
								this._bgt = null;
								this._authorizeBadge = false;
								goto IL_3FA;
							}
						}
						this._bgc = badgeToken.BadgeColor;
						this.SetColor(this._bgc);
						this._bgt = badgeToken.BadgeText;
						this.MyText = this._bgt;
						this._authorizeBadge = true;
					}
				}
				else
				{
					GameCore.Console.AddDebugLog("SDAUTH", string.Concat(new string[]
					{
						"<color=red>Validation of global badge of user ",
						this._hub.nicknameSync.MyNick,
						" failed - ",
						text,
						".</color>"
					}), MessageImportance.Normal, false);
					this._bgc = null;
					this._bgt = null;
					this._authorizeBadge = false;
					if (this._prevColor != null)
					{
						this._prevColor += ".";
					}
					else
					{
						this._prevColor = ".";
					}
					if (this._prevText != null)
					{
						this._prevText += ".";
						return;
					}
					this._prevText = ".";
					return;
				}
			}
		}
		IL_3FA:
		if (this._prevColor == this.MyColor && this._prevText == this.MyText)
		{
			return;
		}
		currentColor = this.CurrentColor;
		if (currentColor != null && currentColor.Restricted && (this.MyText != this._bgt || this.MyColor != this._bgc))
		{
			GameCore.Console.AddLog(string.Concat(new string[]
			{
				"TAG FAIL 1 - ",
				this.MyText,
				" - ",
				this._bgt,
				" /-/ ",
				this.MyColor,
				" - ",
				this._bgc
			}), Color.gray, false, ConsoleLogType.Log);
			this._authorizeBadge = false;
			this.SetColor(null);
			this._prevColor = null;
			PlayerList.UpdatePlayerRole(this._hub);
			return;
		}
		if (this.MyText != null && this.MyText != this._bgt && this.HasNotAllowedText)
		{
			GameCore.Console.AddLog(string.Concat(new string[]
			{
				"TAG FAIL 2 - ",
				this.MyText,
				" - ",
				this._bgt,
				" /-/ ",
				this.MyColor,
				" - ",
				this._bgc
			}), Color.gray, false, ConsoleLogType.Log);
			this._authorizeBadge = false;
			this.SetText(null);
			this._prevText = null;
			PlayerList.UpdatePlayerRole(this._hub);
			return;
		}
		this._prevColor = this.MyColor;
		this._prevText = this.MyText;
		this._prevBadge = this.GlobalBadge;
		PlayerList.UpdatePlayerRole(this._hub);
	}

	// Token: 0x060006E8 RID: 1768 RVA: 0x0001A8F8 File Offset: 0x00018AF8
	private void SetColorHook(string p, string i)
	{
		this.SetColor(i);
	}

	// Token: 0x060006E9 RID: 1769 RVA: 0x0001A904 File Offset: 0x00018B04
	public void SetColor(string i)
	{
		if (string.IsNullOrEmpty(i))
		{
			i = "default";
		}
		if (NetworkServer.active)
		{
			this.Network_myColor = i;
		}
		this.MyColor = i;
		ServerRoles.NamedColor namedColor = this.NamedColors.FirstOrDefault((ServerRoles.NamedColor row) => row.Name == this.MyColor);
		if (namedColor == null && i != "default")
		{
			this.SetColor("default");
			return;
		}
		this.CurrentColor = namedColor;
	}

	// Token: 0x060006EA RID: 1770 RVA: 0x0001A970 File Offset: 0x00018B70
	private void SetTextHook(string p, string i)
	{
		this.SetText(i);
	}

	// Token: 0x060006EB RID: 1771 RVA: 0x0001A97C File Offset: 0x00018B7C
	public void SetText(string i)
	{
		if (i == string.Empty)
		{
			i = null;
		}
		if (NetworkServer.active)
		{
			this.Network_myText = i;
		}
		this.MyText = i;
		ServerRoles.NamedColor namedColor = this.NamedColors.FirstOrDefault((ServerRoles.NamedColor row) => row.Name == this.MyColor);
		if (namedColor == null)
		{
			return;
		}
		this.CurrentColor = namedColor;
	}

	// Token: 0x060006EC RID: 1772 RVA: 0x00002E9A File Offset: 0x0000109A
	private void HidePlayerList(bool oldValue, bool newValue)
	{
	}

	// Token: 0x1700011C RID: 284
	// (get) Token: 0x060006ED RID: 1773 RVA: 0x0001A9D1 File Offset: 0x00018BD1
	public bool HasBadgeHidden
	{
		get
		{
			return !this._hub.authManager.BypassBansFlagSet && !string.IsNullOrEmpty(this.HiddenBadge);
		}
	}

	// Token: 0x1700011D RID: 285
	// (get) Token: 0x060006EE RID: 1774 RVA: 0x0001A9F5 File Offset: 0x00018BF5
	public bool HasGlobalBadge
	{
		get
		{
			return this._hub.authManager.AuthenticationResponse.SignedBadgeToken != null;
		}
	}

	// Token: 0x060006EF RID: 1775 RVA: 0x0001AA10 File Offset: 0x00018C10
	[Server]
	public bool RefreshGlobalTag()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Boolean ServerRoles::RefreshGlobalTag()' called when server was not active");
			return default(bool);
		}
		if (!this.HasGlobalBadge)
		{
			return false;
		}
		PlayerChangingBadgeVisibilityEventArgs playerChangingBadgeVisibilityEventArgs = new PlayerChangingBadgeVisibilityEventArgs(this._hub, true, false);
		PlayerEvents.OnChangingBadgeVisibility(playerChangingBadgeVisibilityEventArgs);
		if (!playerChangingBadgeVisibilityEventArgs.IsAllowed)
		{
			return false;
		}
		this.SetColor(null);
		this.SetText(null);
		this.NetworkGlobalBadge = this._hub.authManager.AuthenticationResponse.SignedBadgeToken.token;
		this.NetworkGlobalBadgeSignature = this._hub.authManager.AuthenticationResponse.SignedBadgeToken.signature;
		this.HiddenBadge = null;
		this.GlobalHidden = false;
		this.RpcResetFixed();
		PlayerEvents.OnChangedBadgeVisibility(new PlayerChangedBadgeVisibilityEventArgs(this._hub, true, false));
		return true;
	}

	// Token: 0x060006F0 RID: 1776 RVA: 0x0001AADC File Offset: 0x00018CDC
	[Server]
	public void RefreshLocalTag()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void ServerRoles::RefreshLocalTag()' called when server was not active");
			return;
		}
		PlayerChangingBadgeVisibilityEventArgs playerChangingBadgeVisibilityEventArgs = new PlayerChangingBadgeVisibilityEventArgs(this._hub, false, false);
		PlayerEvents.OnChangingBadgeVisibility(playerChangingBadgeVisibilityEventArgs);
		if (!playerChangingBadgeVisibilityEventArgs.IsAllowed)
		{
			return;
		}
		this.NetworkGlobalBadge = null;
		this.NetworkGlobalBadgeSignature = null;
		this.HiddenBadge = null;
		this.GlobalHidden = false;
		this.RpcResetFixed();
		this.RefreshPermissions(true);
		PlayerEvents.OnChangedBadgeVisibility(new PlayerChangedBadgeVisibilityEventArgs(this._hub, false, false));
	}

	// Token: 0x060006F1 RID: 1777 RVA: 0x0001AB54 File Offset: 0x00018D54
	[Server]
	public bool TryHideTag()
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Boolean ServerRoles::TryHideTag()' called when server was not active");
			return default(bool);
		}
		bool isGlobal;
		if (string.IsNullOrEmpty(this.MyText))
		{
			if (this.GlobalBadge != null && this._hub.authManager.AuthenticationResponse.BadgeToken != null)
			{
				isGlobal = true;
				PlayerChangingBadgeVisibilityEventArgs playerChangingBadgeVisibilityEventArgs = new PlayerChangingBadgeVisibilityEventArgs(this._hub, isGlobal, false);
				PlayerEvents.OnChangingBadgeVisibility(playerChangingBadgeVisibilityEventArgs);
				if (!playerChangingBadgeVisibilityEventArgs.IsAllowed)
				{
					return false;
				}
				this.HiddenBadge = this._hub.authManager.AuthenticationResponse.BadgeToken.BadgeText;
				this.GlobalHidden = true;
			}
			else
			{
				if (!this._hub.authManager.BypassBansFlagSet)
				{
					return false;
				}
				isGlobal = false;
				PlayerChangingBadgeVisibilityEventArgs playerChangingBadgeVisibilityEventArgs2 = new PlayerChangingBadgeVisibilityEventArgs(this._hub, isGlobal, false);
				PlayerEvents.OnChangingBadgeVisibility(playerChangingBadgeVisibilityEventArgs2);
				if (!playerChangingBadgeVisibilityEventArgs2.IsAllowed)
				{
					return false;
				}
				this.GlobalHidden = false;
				this.HiddenBadge = null;
			}
		}
		else
		{
			isGlobal = false;
			PlayerChangingBadgeVisibilityEventArgs playerChangingBadgeVisibilityEventArgs3 = new PlayerChangingBadgeVisibilityEventArgs(this._hub, isGlobal, false);
			PlayerEvents.OnChangingBadgeVisibility(playerChangingBadgeVisibilityEventArgs3);
			if (!playerChangingBadgeVisibilityEventArgs3.IsAllowed)
			{
				return false;
			}
			this.GlobalHidden = this.GlobalSet;
			this.HiddenBadge = this.MyText;
		}
		this.NetworkGlobalBadge = null;
		this.SetText(null);
		this.SetColor(null);
		this.RefreshHiddenTag();
		PlayerEvents.OnChangedBadgeVisibility(new PlayerChangedBadgeVisibilityEventArgs(this._hub, isGlobal, false));
		return true;
	}

	// Token: 0x060006F2 RID: 1778 RVA: 0x0001ACA4 File Offset: 0x00018EA4
	internal void OpenRemoteAdmin()
	{
		this.TargetSetRemoteAdmin(true);
		this._hub.queryProcessor.SyncCommandsToClient();
	}

	// Token: 0x060006F3 RID: 1779 RVA: 0x0001ACC0 File Offset: 0x00018EC0
	[TargetRpc]
	private void TargetSetRemoteAdmin(bool open)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteBool(open);
		this.SendTargetRPCInternal(null, "System.Void ServerRoles::TargetSetRemoteAdmin(System.Boolean)", -586263322, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x060006F5 RID: 1781 RVA: 0x0001ACFC File Offset: 0x00018EFC
	static ServerRoles()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(ServerRoles), "System.Void ServerRoles::CmdSetLocalTagPreferences(ServerRoles/BadgePreferences,ServerRoles/BadgeVisibilityPreferences,ServerRoles/BadgeVisibilityPreferences,System.Boolean)", new RemoteCallDelegate(ServerRoles.InvokeUserCode_CmdSetLocalTagPreferences__BadgePreferences__BadgeVisibilityPreferences__BadgeVisibilityPreferences__Boolean), true);
		RemoteProcedureCalls.RegisterRpc(typeof(ServerRoles), "System.Void ServerRoles::RpcResetFixed()", new RemoteCallDelegate(ServerRoles.InvokeUserCode_RpcResetFixed));
		RemoteProcedureCalls.RegisterRpc(typeof(ServerRoles), "System.Void ServerRoles::TargetSetHiddenRole(Mirror.NetworkConnection,System.String)", new RemoteCallDelegate(ServerRoles.InvokeUserCode_TargetSetHiddenRole__NetworkConnection__String));
		RemoteProcedureCalls.RegisterRpc(typeof(ServerRoles), "System.Void ServerRoles::TargetSetRemoteAdmin(System.Boolean)", new RemoteCallDelegate(ServerRoles.InvokeUserCode_TargetSetRemoteAdmin__Boolean));
	}

	// Token: 0x060006F8 RID: 1784 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x1700011E RID: 286
	// (get) Token: 0x060006F9 RID: 1785 RVA: 0x0001ADA8 File Offset: 0x00018FA8
	// (set) Token: 0x060006FA RID: 1786 RVA: 0x0001ADBB File Offset: 0x00018FBB
	public string Network_myText
	{
		get
		{
			return this._myText;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<string>(value, ref this._myText, 1UL, new Action<string, string>(this.SetTextHook));
		}
	}

	// Token: 0x1700011F RID: 287
	// (get) Token: 0x060006FB RID: 1787 RVA: 0x0001ADE0 File Offset: 0x00018FE0
	// (set) Token: 0x060006FC RID: 1788 RVA: 0x0001ADF3 File Offset: 0x00018FF3
	public string Network_myColor
	{
		get
		{
			return this._myColor;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<string>(value, ref this._myColor, 2UL, new Action<string, string>(this.SetColorHook));
		}
	}

	// Token: 0x17000120 RID: 288
	// (get) Token: 0x060006FD RID: 1789 RVA: 0x0001AE18 File Offset: 0x00019018
	// (set) Token: 0x060006FE RID: 1790 RVA: 0x0001AE2B File Offset: 0x0001902B
	public string NetworkGlobalBadge
	{
		get
		{
			return this.GlobalBadge;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<string>(value, ref this.GlobalBadge, 4UL, null);
		}
	}

	// Token: 0x17000121 RID: 289
	// (get) Token: 0x060006FF RID: 1791 RVA: 0x0001AE48 File Offset: 0x00019048
	// (set) Token: 0x06000700 RID: 1792 RVA: 0x0001AE5B File Offset: 0x0001905B
	public string NetworkGlobalBadgeSignature
	{
		get
		{
			return this.GlobalBadgeSignature;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<string>(value, ref this.GlobalBadgeSignature, 8UL, null);
		}
	}

	// Token: 0x17000122 RID: 290
	// (get) Token: 0x06000701 RID: 1793 RVA: 0x0001AE78 File Offset: 0x00019078
	// (set) Token: 0x06000702 RID: 1794 RVA: 0x0001AE8B File Offset: 0x0001908B
	public bool NetworkHideFromPlayerList
	{
		get
		{
			return this.HideFromPlayerList;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<bool>(value, ref this.HideFromPlayerList, 16UL, new Action<bool, bool>(this.HidePlayerList));
		}
	}

	// Token: 0x06000703 RID: 1795 RVA: 0x0001AEB0 File Offset: 0x000190B0
	protected void UserCode_TargetSetHiddenRole__NetworkConnection__String(NetworkConnection connection, string role)
	{
		if (base.isServer)
		{
			return;
		}
		if (string.IsNullOrEmpty(role))
		{
			this.SetColor(null);
			this.SetText(null);
			this.FixedBadge = null;
			this.SetText(null);
			return;
		}
		this.SetColor("silver");
		this.FixedBadge = Misc.SanitizeRichText(role.Replace("[", string.Empty).Replace("]", string.Empty), "", "") + " " + TranslationReader.Get("Legacy_Interfaces", 18, "(hidden)");
		this.SetText(this.FixedBadge);
	}

	// Token: 0x06000704 RID: 1796 RVA: 0x0001AF51 File Offset: 0x00019151
	protected static void InvokeUserCode_TargetSetHiddenRole__NetworkConnection__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC TargetSetHiddenRole called on server.");
			return;
		}
		((ServerRoles)obj).UserCode_TargetSetHiddenRole__NetworkConnection__String(null, reader.ReadString());
	}

	// Token: 0x06000705 RID: 1797 RVA: 0x0001AF7B File Offset: 0x0001917B
	protected void UserCode_RpcResetFixed()
	{
		this.FixedBadge = null;
	}

	// Token: 0x06000706 RID: 1798 RVA: 0x0001AF84 File Offset: 0x00019184
	protected static void InvokeUserCode_RpcResetFixed(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcResetFixed called on server.");
			return;
		}
		((ServerRoles)obj).UserCode_RpcResetFixed();
	}

	// Token: 0x06000707 RID: 1799 RVA: 0x0001AFA8 File Offset: 0x000191A8
	protected void UserCode_CmdSetLocalTagPreferences__BadgePreferences__BadgeVisibilityPreferences__BadgeVisibilityPreferences__Boolean(ServerRoles.BadgePreferences userPreferences, ServerRoles.BadgeVisibilityPreferences globalPreferences, ServerRoles.BadgeVisibilityPreferences localPreferences, bool refresh)
	{
		this.UserBadgePreferences = userPreferences;
		this._globalBadgeVisibilityPreferences = globalPreferences;
		this._localBadgeVisibilityPreferences = localPreferences;
		if (!refresh)
		{
			return;
		}
		if (userPreferences == ServerRoles.BadgePreferences.PreferGlobal && this.HasGlobalBadge)
		{
			this.RefreshGlobalBadgeVisibility(globalPreferences);
			return;
		}
		if (this.Group == null)
		{
			if (this.HasGlobalBadge)
			{
				this.RefreshGlobalBadgeVisibility(globalPreferences);
			}
			return;
		}
		this.RefreshLocalBadgeVisibility(localPreferences);
	}

	// Token: 0x06000708 RID: 1800 RVA: 0x0001B002 File Offset: 0x00019202
	protected static void InvokeUserCode_CmdSetLocalTagPreferences__BadgePreferences__BadgeVisibilityPreferences__BadgeVisibilityPreferences__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdSetLocalTagPreferences called on client.");
			return;
		}
		((ServerRoles)obj).UserCode_CmdSetLocalTagPreferences__BadgePreferences__BadgeVisibilityPreferences__BadgeVisibilityPreferences__Boolean(Mirror.GeneratedNetworkCode._Read_ServerRoles/BadgePreferences(reader), Mirror.GeneratedNetworkCode._Read_ServerRoles/BadgeVisibilityPreferences(reader), Mirror.GeneratedNetworkCode._Read_ServerRoles/BadgeVisibilityPreferences(reader), reader.ReadBool());
	}

	// Token: 0x06000709 RID: 1801 RVA: 0x00002E9A File Offset: 0x0000109A
	protected void UserCode_TargetSetRemoteAdmin__Boolean(bool open)
	{
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x0001B03D File Offset: 0x0001923D
	protected static void InvokeUserCode_TargetSetRemoteAdmin__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC TargetSetRemoteAdmin called on server.");
			return;
		}
		((ServerRoles)obj).UserCode_TargetSetRemoteAdmin__Boolean(reader.ReadBool());
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x0001B068 File Offset: 0x00019268
	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteString(this._myText);
			writer.WriteString(this._myColor);
			writer.WriteString(this.GlobalBadge);
			writer.WriteString(this.GlobalBadgeSignature);
			writer.WriteBool(this.HideFromPlayerList);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1UL) != 0UL)
		{
			writer.WriteString(this._myText);
		}
		if ((base.syncVarDirtyBits & 2UL) != 0UL)
		{
			writer.WriteString(this._myColor);
		}
		if ((base.syncVarDirtyBits & 4UL) != 0UL)
		{
			writer.WriteString(this.GlobalBadge);
		}
		if ((base.syncVarDirtyBits & 8UL) != 0UL)
		{
			writer.WriteString(this.GlobalBadgeSignature);
		}
		if ((base.syncVarDirtyBits & 16UL) != 0UL)
		{
			writer.WriteBool(this.HideFromPlayerList);
		}
	}

	// Token: 0x0600070C RID: 1804 RVA: 0x0001B178 File Offset: 0x00019378
	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			base.GeneratedSyncVarDeserialize<string>(ref this._myText, new Action<string, string>(this.SetTextHook), reader.ReadString());
			base.GeneratedSyncVarDeserialize<string>(ref this._myColor, new Action<string, string>(this.SetColorHook), reader.ReadString());
			base.GeneratedSyncVarDeserialize<string>(ref this.GlobalBadge, null, reader.ReadString());
			base.GeneratedSyncVarDeserialize<string>(ref this.GlobalBadgeSignature, null, reader.ReadString());
			base.GeneratedSyncVarDeserialize<bool>(ref this.HideFromPlayerList, new Action<bool, bool>(this.HidePlayerList), reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<string>(ref this._myText, new Action<string, string>(this.SetTextHook), reader.ReadString());
		}
		if ((num & 2L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<string>(ref this._myColor, new Action<string, string>(this.SetColorHook), reader.ReadString());
		}
		if ((num & 4L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<string>(ref this.GlobalBadge, null, reader.ReadString());
		}
		if ((num & 8L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<string>(ref this.GlobalBadgeSignature, null, reader.ReadString());
		}
		if ((num & 16L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<bool>(ref this.HideFromPlayerList, new Action<bool, bool>(this.HidePlayerList), reader.ReadBool());
		}
	}

	// Token: 0x0400062E RID: 1582
	public ServerRoles.NamedColor CurrentColor;

	// Token: 0x0400062F RID: 1583
	public ServerRoles.NamedColor[] NamedColors;

	// Token: 0x04000630 RID: 1584
	[NonSerialized]
	private bool _bypassMode;

	// Token: 0x04000631 RID: 1585
	private bool _authorizeBadge;

	// Token: 0x04000632 RID: 1586
	private UserGroup _group;

	// Token: 0x04000633 RID: 1587
	private ReferenceHub _hub;

	// Token: 0x04000634 RID: 1588
	private static readonly Dictionary<string, ServerRoles.NamedColor> DictionarizedColorsCache = new Dictionary<string, ServerRoles.NamedColor>();

	// Token: 0x04000635 RID: 1589
	private static bool _colorDictionaryCacheSet;

	// Token: 0x04000636 RID: 1590
	private const string DefaultColor = "default";

	// Token: 0x04000637 RID: 1591
	private const string HiddenBadgeColor = "silver";

	// Token: 0x04000638 RID: 1592
	public const ulong UserIdPerms = 18007046UL;

	// Token: 0x04000639 RID: 1593
	private string _globalBadgeUnconfirmed;

	// Token: 0x0400063A RID: 1594
	private string _prevColor;

	// Token: 0x0400063B RID: 1595
	private string _prevText;

	// Token: 0x0400063C RID: 1596
	private string _prevBadge;

	// Token: 0x0400063D RID: 1597
	private string _authChallenge;

	// Token: 0x0400063E RID: 1598
	private string _badgeChallenge;

	// Token: 0x0400063F RID: 1599
	private string _bgc;

	// Token: 0x04000640 RID: 1600
	private string _bgt;

	// Token: 0x04000641 RID: 1601
	private bool _requested;

	// Token: 0x04000642 RID: 1602
	private bool _publicPartRequested;

	// Token: 0x04000643 RID: 1603
	private bool _badgeRequested;

	// Token: 0x04000644 RID: 1604
	private bool _authRequested;

	// Token: 0x04000645 RID: 1605
	private bool _noclipReady;

	// Token: 0x04000646 RID: 1606
	internal bool BadgeCover;

	// Token: 0x04000647 RID: 1607
	[NonSerialized]
	public ServerRoles.BadgePreferences UserBadgePreferences;

	// Token: 0x04000648 RID: 1608
	private ServerRoles.BadgeVisibilityPreferences _globalBadgeVisibilityPreferences;

	// Token: 0x04000649 RID: 1609
	private ServerRoles.BadgeVisibilityPreferences _localBadgeVisibilityPreferences;

	// Token: 0x0400064C RID: 1612
	[SyncVar(hook = "SetTextHook")]
	private string _myText;

	// Token: 0x0400064D RID: 1613
	[SyncVar(hook = "SetColorHook")]
	private string _myColor;

	// Token: 0x0400064E RID: 1614
	[SyncVar]
	public string GlobalBadge;

	// Token: 0x0400064F RID: 1615
	[SyncVar]
	public string GlobalBadgeSignature;

	// Token: 0x04000650 RID: 1616
	[SyncVar(hook = "HidePlayerList")]
	public bool HideFromPlayerList;

	// Token: 0x04000651 RID: 1617
	[NonSerialized]
	public bool RemoteAdmin;

	// Token: 0x04000652 RID: 1618
	[NonSerialized]
	public ulong Permissions;

	// Token: 0x04000653 RID: 1619
	[NonSerialized]
	public string HiddenBadge;

	// Token: 0x04000654 RID: 1620
	[NonSerialized]
	public bool GlobalHidden;

	// Token: 0x04000655 RID: 1621
	[NonSerialized]
	internal bool AdminChatPerms;

	// Token: 0x04000656 RID: 1622
	[NonSerialized]
	internal ulong GlobalPerms;

	// Token: 0x04000657 RID: 1623
	[NonSerialized]
	private bool _lastRealIdPerm;

	// Token: 0x04000658 RID: 1624
	[NonSerialized]
	public string FixedBadge;

	// Token: 0x02000162 RID: 354
	[Serializable]
	public class NamedColor
	{
		// Token: 0x17000123 RID: 291
		// (get) Token: 0x0600070D RID: 1805 RVA: 0x0001B2F4 File Offset: 0x000194F4
		public Color SpeakingColor
		{
			get
			{
				if (!this._speakingColorSet)
				{
					this._speakingColorSet = true;
					ColorUtility.TryParseHtmlString("#" + (string.IsNullOrEmpty(this._speakingOverride) ? this.ColorHex : this._speakingOverride), out this._speakingColorCache);
				}
				return this._speakingColorCache;
			}
		}

		// Token: 0x04000659 RID: 1625
		public string Name;

		// Token: 0x0400065A RID: 1626
		public string ColorHex;

		// Token: 0x0400065B RID: 1627
		public bool Restricted;

		// Token: 0x0400065C RID: 1628
		[SerializeField]
		private string _speakingOverride;

		// Token: 0x0400065D RID: 1629
		private Color _speakingColorCache;

		// Token: 0x0400065E RID: 1630
		private bool _speakingColorSet;
	}

	// Token: 0x02000163 RID: 355
	public enum BadgePreferences
	{
		// Token: 0x04000660 RID: 1632
		NoPreference,
		// Token: 0x04000661 RID: 1633
		PreferGlobal,
		// Token: 0x04000662 RID: 1634
		PreferLocal
	}

	// Token: 0x02000164 RID: 356
	public enum BadgeVisibilityPreferences
	{
		// Token: 0x04000664 RID: 1636
		NoPreference,
		// Token: 0x04000665 RID: 1637
		Visible,
		// Token: 0x04000666 RID: 1638
		Hidden
	}
}
