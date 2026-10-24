using System;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Cryptography;
using GameCore;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using Mirror.RemoteCalls;
using NetworkManagerUtils.Dummies;
using NorthwoodLib;
using UnityEngine;
using VoiceChat;

namespace CentralAuth
{
	// Token: 0x02000AAA RID: 2730
	public class PlayerAuthenticationManager : NetworkBehaviour
	{
		// Token: 0x17000E01 RID: 3585
		// (get) Token: 0x06003D77 RID: 15735 RVA: 0x000C9D2D File Offset: 0x000C7F2D
		// (set) Token: 0x06003D78 RID: 15736 RVA: 0x000C9D35 File Offset: 0x000C7F35
		public AuthenticationResponse AuthenticationResponse { get; private set; }

		// Token: 0x17000E02 RID: 3586
		// (get) Token: 0x06003D79 RID: 15737 RVA: 0x000C9D3E File Offset: 0x000C7F3E
		// (set) Token: 0x06003D7A RID: 15738 RVA: 0x000C9D46 File Offset: 0x000C7F46
		public bool DoNotTrack { get; private set; }

		// Token: 0x17000E03 RID: 3587
		// (get) Token: 0x06003D7B RID: 15739 RVA: 0x000C9D50 File Offset: 0x000C7F50
		// (set) Token: 0x06003D7C RID: 15740 RVA: 0x000C9DAB File Offset: 0x000C7FAB
		public string UserId
		{
			get
			{
				if (!NetworkServer.active)
				{
					return this.SyncedUserId;
				}
				if (this._privUserId == null)
				{
					return null;
				}
				if (this._privUserId.Contains("$"))
				{
					return this._privUserId.Substring(0, this._privUserId.IndexOf("$", StringComparison.Ordinal));
				}
				return this._privUserId;
			}
			set
			{
				if (!NetworkServer.active)
				{
					return;
				}
				this._privUserId = value;
				this.UserIdHook(null, value);
				this.RefreshSyncedId();
				this._hub.serverRoles.RefreshRealId();
			}
		}

		// Token: 0x17000E04 RID: 3588
		// (get) Token: 0x06003D7D RID: 15741 RVA: 0x000C9DDA File Offset: 0x000C7FDA
		public string SaltedUserId
		{
			get
			{
				if (!NetworkServer.active)
				{
					return this.SyncedUserId;
				}
				return this._privUserId;
			}
		}

		// Token: 0x06003D7E RID: 15742 RVA: 0x000C9DF0 File Offset: 0x000C7FF0
		[Server]
		private void RefreshSyncedId()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void CentralAuth.PlayerAuthenticationManager::RefreshSyncedId()' called when server was not active");
				return;
			}
			if (this._privUserId == null)
			{
				this.NetworkSyncedUserId = null;
				return;
			}
			this.NetworkSyncedUserId = ((base.isLocalPlayer || this._hub.IsDummy || (this._privUserId.EndsWith("@steam", StringComparison.Ordinal) && !this.DoNotTrack && !this.AuthenticationResponse.AuthToken.SyncHashed)) ? this._privUserId : Sha.HashToString(Sha.Sha512(this._privUserId)));
		}

		// Token: 0x17000E05 RID: 3589
		// (get) Token: 0x06003D7F RID: 15743 RVA: 0x000C9E90 File Offset: 0x000C8090
		// (set) Token: 0x06003D80 RID: 15744 RVA: 0x000C9E98 File Offset: 0x000C8098
		public ClientInstanceMode InstanceMode
		{
			get
			{
				return this._targetInstanceMode;
			}
			private set
			{
				if (value == this._targetInstanceMode)
				{
					return;
				}
				this._targetInstanceMode = value;
				Action<ReferenceHub, ClientInstanceMode> onInstanceModeChanged = PlayerAuthenticationManager.OnInstanceModeChanged;
				if (onInstanceModeChanged == null)
				{
					return;
				}
				onInstanceModeChanged(this._hub, this._targetInstanceMode);
			}
		}

		// Token: 0x140000AF RID: 175
		// (add) Token: 0x06003D81 RID: 15745 RVA: 0x000C9EC8 File Offset: 0x000C80C8
		// (remove) Token: 0x06003D82 RID: 15746 RVA: 0x000C9EFC File Offset: 0x000C80FC
		public static event Action<ReferenceHub> OnSyncedUserIdAssigned;

		// Token: 0x140000B0 RID: 176
		// (add) Token: 0x06003D83 RID: 15747 RVA: 0x000C9F30 File Offset: 0x000C8130
		// (remove) Token: 0x06003D84 RID: 15748 RVA: 0x000C9F64 File Offset: 0x000C8164
		public static event Action<ReferenceHub, ClientInstanceMode> OnInstanceModeChanged;

		// Token: 0x17000E06 RID: 3590
		// (get) Token: 0x06003D85 RID: 15749 RVA: 0x000C9F98 File Offset: 0x000C8198
		public bool NorthwoodStaff
		{
			get
			{
				BadgeToken badgeToken = this.AuthenticationResponse.BadgeToken;
				return badgeToken != null && badgeToken.Staff;
			}
		}

		// Token: 0x17000E07 RID: 3591
		// (get) Token: 0x06003D86 RID: 15750 RVA: 0x000C9FC0 File Offset: 0x000C81C0
		public bool BypassBansFlagSet
		{
			get
			{
				AuthenticationToken authToken = this.AuthenticationResponse.AuthToken;
				return authToken != null && authToken.BypassBans;
			}
		}

		// Token: 0x17000E08 RID: 3592
		// (get) Token: 0x06003D87 RID: 15751 RVA: 0x000C9FE8 File Offset: 0x000C81E8
		public bool RemoteAdminGlobalAccess
		{
			get
			{
				BadgeToken badgeToken = this.AuthenticationResponse.BadgeToken;
				return badgeToken != null && (badgeToken.Management || badgeToken.GlobalBanning);
			}
		}

		// Token: 0x06003D88 RID: 15752 RVA: 0x000CA01E File Offset: 0x000C821E
		private void Awake()
		{
			this._hub = ReferenceHub.GetHub(this);
		}

		// Token: 0x06003D89 RID: 15753 RVA: 0x000CA02C File Offset: 0x000C822C
		private void FixedUpdate()
		{
			if (this._passwordCooldown > 0f)
			{
				this._passwordCooldown -= Time.fixedDeltaTime;
			}
			if (this.InstanceMode != ClientInstanceMode.Unverified || !NetworkServer.active || !PlayerAuthenticationManager.OnlineMode || base.isLocalPlayer || this._timeoutTimer < 0f)
			{
				return;
			}
			if (!this._authenticationRequested && base.connectionToClient.isReady)
			{
				this.RequestAuthentication();
			}
			this._timeoutTimer += Time.fixedDeltaTime;
			if (this._timeoutTimer <= PlayerAuthenticationManager.AuthenticationTimeout)
			{
				return;
			}
			this._timeoutTimer = -1f;
			this.RejectAuthentication("authentication timeout exceeded.", null, false);
		}

		// Token: 0x06003D8A RID: 15754 RVA: 0x000CA0DC File Offset: 0x000C82DC
		private void Start()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			if (base.isLocalPlayer)
			{
				Action<NetworkConnectionToClient, AuthenticationResponse> handler;
				if ((handler = PlayerAuthenticationManager.<>O.<0>__ServerReceiveAuthenticationResponse) == null)
				{
					handler = (PlayerAuthenticationManager.<>O.<0>__ServerReceiveAuthenticationResponse = new Action<NetworkConnectionToClient, AuthenticationResponse>(PlayerAuthenticationManager.ServerReceiveAuthenticationResponse));
				}
				NetworkServer.ReplaceHandler<AuthenticationResponse>(handler, true);
			}
			if (base.connectionToClient is DummyNetworkConnection)
			{
				this.UserId = "ID_Dummy";
				return;
			}
			if (base.isLocalPlayer && ServerStatic.IsDedicated)
			{
				this.UserId = "ID_Dedicated";
				return;
			}
			if (base.isLocalPlayer)
			{
				this.UserId = "ID_Host";
				if (PlayerAuthenticationManager.OnlineMode)
				{
					this.RequestAuthentication();
					return;
				}
			}
			else if (!PlayerAuthenticationManager.OnlineMode)
			{
				this.UserId = "ID_Offline_" + base.netId.ToString() + "_" + DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
			}
		}

		// Token: 0x06003D8B RID: 15755 RVA: 0x000CA1B0 File Offset: 0x000C83B0
		private static void ServerReceiveAuthenticationResponse(NetworkConnection conn, AuthenticationResponse msg)
		{
			ReferenceHub referenceHub;
			if (!NetworkServer.active || !PlayerAuthenticationManager.OnlineMode || !ReferenceHub.TryGetHub(conn, out referenceHub))
			{
				return;
			}
			referenceHub.authManager.ProcessAuthenticationResponse(msg);
		}

		// Token: 0x06003D8C RID: 15756 RVA: 0x000CA1E2 File Offset: 0x000C83E2
		public string GetAuthToken()
		{
			if (this.AuthenticationResponse.SignedAuthToken != null)
			{
				return JsonSerialize.ToJson<SignedToken>(this.AuthenticationResponse.SignedAuthToken);
			}
			return null;
		}

		// Token: 0x06003D8D RID: 15757 RVA: 0x000CA204 File Offset: 0x000C8404
		[Server]
		private void RequestAuthentication()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void CentralAuth.PlayerAuthenticationManager::RequestAuthentication()' called when server was not active");
				return;
			}
			if (!base.isLocalPlayer)
			{
				this._authenticationRequested = true;
				this._hub.encryptedChannelManager.PrepareExchange();
			}
			this._challenge = RandomGenerator.GetStringSecure(24);
			this.RpcRequestAuthentication(this._challenge, (this._hub.encryptedChannelManager.EcdhKeys == null) ? null : ECDSA.KeyToString(this._hub.encryptedChannelManager.EcdhKeys.Public));
		}

		// Token: 0x06003D8E RID: 15758 RVA: 0x000CA290 File Offset: 0x000C8490
		[TargetRpc]
		private void RpcRequestAuthentication(string challenge, string ecdhPublicKey)
		{
			NetworkWriterPooled writer = NetworkWriterPool.Get();
			writer.WriteString(challenge);
			writer.WriteString(ecdhPublicKey);
			this.SendTargetRPCInternal(null, "System.Void CentralAuth.PlayerAuthenticationManager::RpcRequestAuthentication(System.String,System.String)", -1619731460, writer, 0);
			NetworkWriterPool.Return(writer);
		}

		// Token: 0x06003D8F RID: 15759 RVA: 0x000CA2D4 File Offset: 0x000C84D4
		[TargetRpc]
		internal void TargetSetRealId(NetworkConnection conn, string userId)
		{
			NetworkWriterPooled writer = NetworkWriterPool.Get();
			writer.WriteString(userId);
			this.SendTargetRPCInternal(conn, "System.Void CentralAuth.PlayerAuthenticationManager::TargetSetRealId(Mirror.NetworkConnection,System.String)", 295172299, writer, 0);
			NetworkWriterPool.Return(writer);
		}

		// Token: 0x06003D90 RID: 15760 RVA: 0x000CA310 File Offset: 0x000C8510
		private void ProcessAuthenticationResponse(AuthenticationResponse msg)
		{
			try
			{
				if (!this._authenticationRequested)
				{
					this._hub.gameConsoleTransmission.SendToClient("Authentication token was not requested by the server.", "yellow");
				}
				else if (this._challenge == null)
				{
					this._hub.gameConsoleTransmission.SendToClient("Authentication token has already been sent.", "yellow");
				}
				else
				{
					this.AuthenticationResponse = msg;
					if (msg.SignedAuthToken != null)
					{
						AuthenticationToken authenticationToken;
						string reason;
						string userId;
						if ((msg.EcdhPublicKey == null || msg.EcdhPublicKeySignature == null) && !base.isLocalPlayer)
						{
							this.RejectAuthentication("null ECDH public key or public key signature.", null, false);
						}
						else if (msg.SignedAuthToken.TryGetToken<AuthenticationToken>("Authentication", out authenticationToken, out reason, out userId, 0))
						{
							string text = PlayerAuthenticationManager.RemoveSalt(authenticationToken.UserId);
							if (this._challenge != authenticationToken.Challenge)
							{
								this.RejectAuthentication("invalid authentication challenge.", userId, false);
							}
							else
							{
								this._challenge = null;
								if (authenticationToken.PublicKey != msg.PublicKeyHash)
								{
									this.RejectAuthentication("public key hash mismatch.", userId, false);
								}
								else if (GameCore.Version.PrivateBeta && !authenticationToken.PrivateBetaOwnership)
								{
									this.RejectAuthentication("you don't own the Private Beta Access Pass DLC.", userId, false);
								}
								else if (authenticationToken.PlayerId != this._hub.PlayerId)
								{
									this.RejectAuthentication(string.Format("PlayerID Mismatch.\nYour ID: {0}\nAuth ID: {1}", this._hub.PlayerId, authenticationToken.PlayerId), userId, false);
								}
								else
								{
									if (authenticationToken.GameVersion != null)
									{
										System.Version version = System.Version.Parse(authenticationToken.GameVersion);
										System.Version version2 = System.Version.Parse(string.Format("{0}.{1}.{2}", GameCore.Version.Major, GameCore.Version.Minor, GameCore.Version.Revision));
										if (version2 > version)
										{
											this.RejectAuthentication(string.Format("Version mismatch\nAuthentication token is only valid until version: {0}\nGame server version: {1}", version, version2), userId, false);
											return;
										}
									}
									IPEndPoint ipendPoint = null;
									if (!base.isLocalPlayer)
									{
										ipendPoint = LiteNetLib4MirrorServer.Peers[base.connectionToClient.connectionId].EndPoint;
										if (ipendPoint != null)
										{
											if (CustomLiteNetLib4MirrorTransport.UserIds.ContainsKey(ipendPoint))
											{
												PreauthItem preauthItem = CustomLiteNetLib4MirrorTransport.UserIds[ipendPoint];
												if (preauthItem.UserId.Equals(text, StringComparison.Ordinal))
												{
													goto IL_2CC;
												}
											}
											if (!CustomLiteNetLib4MirrorTransport.UserIdFastReload.ContainsKey(text))
											{
												this._hub.gameConsoleTransmission.SendToClient("UserID mismatch between authentication and preauthentication token.", "red");
												PreauthItem preauthItem2;
												this._hub.gameConsoleTransmission.SendToClient("Preauth: " + (CustomLiteNetLib4MirrorTransport.UserIds.TryGetValue(ipendPoint, out preauthItem2) ? preauthItem2.UserId : "(null)"), "red");
												this._hub.gameConsoleTransmission.SendToClient("Auth: " + text, "red");
												this.RejectAuthentication("UserID mismatch between authentication and preauthentication token. Check the game console for more details.", text, false);
												return;
											}
										}
										IL_2CC:
										if (ipendPoint != null)
										{
											PreauthItem preauthItem;
											CustomLiteNetLib4MirrorTransport.UserIds.TryRemove(ipendPoint, out preauthItem);
										}
									}
									if (CustomLiteNetLib4MirrorTransport.UserIdFastReload.ContainsKey(text))
									{
										byte b;
										CustomLiteNetLib4MirrorTransport.UserIdFastReload.TryRemove(text, out b);
									}
									if (msg.EcdhPublicKey != null && !ECDSA.VerifyBytes(msg.EcdhPublicKey, msg.EcdhPublicKeySignature, msg.PublicKey))
									{
										this.RejectAuthentication("invalid ECDH exchange public key signature.", userId, false);
									}
									else if (this.CheckBans(authenticationToken, text))
									{
										if (msg.EcdhPublicKey != null)
										{
											this._hub.encryptedChannelManager.ServerProcessExchange(msg.EcdhPublicKey);
										}
										msg.AuthToken = authenticationToken;
										this.AuthenticationResponse = msg;
										string text2 = string.Format("{0} authenticated from endpoint {1}. Player ID assigned: {2}. Auth token serial number: {3}.", new object[]
										{
											PlayerAuthenticationManager.RemoveSalt(msg.AuthToken.UserId),
											(ipendPoint == null) ? "(null)" : ipendPoint.ToString(),
											this._hub.PlayerId,
											msg.AuthToken.Serial
										});
										ServerConsole.AddLog(text2, ConsoleColor.Gray, false);
										ServerLogs.AddLog(ServerLogs.Modules.Networking, text2, ServerLogs.ServerLogType.ConnectionUpdate, false);
										this.FinalizeAuthentication();
										if (msg.SignedBadgeToken != null)
										{
											BadgeToken badgeToken;
											string reason2;
											string text3;
											if (msg.SignedBadgeToken.TryGetToken<BadgeToken>("Badge request", out badgeToken, out reason2, out text3, 0))
											{
												if (badgeToken.Serial != this.AuthenticationResponse.AuthToken.Serial)
												{
													this.RejectAuthentication("token serial number mismatch.", null, false);
													return;
												}
												if (badgeToken.UserId != Sha.HashToString(Sha.Sha512(this.SaltedUserId)))
												{
													this.RejectBadgeToken("badge token UserID mismatch.");
													return;
												}
												if (StringUtils.Base64Decode(badgeToken.Nickname) != this._hub.nicknameSync.MyNick)
												{
													this.RejectBadgeToken("badge token nickname mismatch.");
													return;
												}
												msg.BadgeToken = badgeToken;
												this.AuthenticationResponse = msg;
												ulong num = (badgeToken.RaPermissions == 0UL || ServerStatic.PermissionsHandler.NorthwoodAccess) ? ServerStatic.PermissionsHandler.FullPerm : badgeToken.RaPermissions;
												if ((badgeToken.Management || badgeToken.GlobalBanning) && CustomNetworkManager.IsVerified)
												{
													this._hub.serverRoles.GlobalPerms |= 8388608UL;
													this._hub.serverRoles.GlobalPerms |= 1048576UL;
												}
												if (this.AuthenticationResponse.BadgeToken.OverwatchMode)
												{
													this._hub.serverRoles.GlobalPerms |= 4096UL;
												}
												if (badgeToken.Staff || badgeToken.Management || badgeToken.GlobalBanning)
												{
													this._hub.serverRoles.GlobalPerms |= 16908288UL;
												}
												if ((badgeToken.Staff && ServerStatic.PermissionsHandler.NorthwoodAccess) || (badgeToken.RemoteAdmin && ServerStatic.PermissionsHandler.StaffAccess) || (badgeToken.Management && ServerStatic.PermissionsHandler.ManagersAccess) || (badgeToken.GlobalBanning && ServerStatic.PermissionsHandler.BanningTeamAccess))
												{
													this._hub.serverRoles.GlobalPerms |= num;
												}
												if ((badgeToken.BadgeText != null && badgeToken.BadgeText != "(none)") || (badgeToken.BadgeColor != null && badgeToken.BadgeColor != "(none)"))
												{
													if (this._hub.serverRoles.UserBadgePreferences == ServerRoles.BadgePreferences.PreferGlobal || !this._hub.serverRoles.BadgeCover || this._hub.serverRoles.Group == null)
													{
														bool flag = msg.HideBadge;
														switch (badgeToken.BadgeType)
														{
														case 0:
															if (!ConfigFile.ServerConfig.GetBool("hide_patreon_badges_by_default", false) || CustomNetworkManager.IsVerified)
															{
																goto IL_708;
															}
															break;
														case 1:
															if (!ConfigFile.ServerConfig.GetBool("hide_staff_badges_by_default", false))
															{
																goto IL_708;
															}
															break;
														case 2:
															if (!ConfigFile.ServerConfig.GetBool("hide_management_badges_by_default", false))
															{
																goto IL_708;
															}
															break;
														case 3:
															break;
														default:
															goto IL_708;
														}
														flag = true;
														IL_708:
														if (flag)
														{
															this._hub.serverRoles.HiddenBadge = badgeToken.BadgeText;
															this._hub.serverRoles.GlobalHidden = true;
															this._hub.serverRoles.RefreshHiddenTag();
															this._hub.gameConsoleTransmission.SendToClient("Your global badge has been granted, but it's hidden. Use \".gtag\" command in the game console to show your global badge.", "yellow");
														}
														else
														{
															this._hub.serverRoles.HiddenBadge = null;
															this._hub.serverRoles.RpcResetFixed();
															this._hub.serverRoles.NetworkGlobalBadge = this.AuthenticationResponse.SignedBadgeToken.token;
															this._hub.serverRoles.NetworkGlobalBadgeSignature = this.AuthenticationResponse.SignedBadgeToken.signature;
															this._hub.gameConsoleTransmission.SendToClient("Your global badge has been granted.", "cyan");
														}
													}
													else
													{
														this._hub.gameConsoleTransmission.SendToClient("Your global badge is covered by server badge. Use \".gtag\" command in the game console to show your global badge.", "yellow");
													}
												}
												this._hub.serverRoles.FinalizeSetGroup();
											}
											else
											{
												this.RejectBadgeToken(reason2);
											}
										}
										PlayerEvents.OnJoined(new PlayerJoinedEventArgs(this._hub));
									}
								}
							}
						}
						else
						{
							this.RejectAuthentication(reason, userId, true);
						}
					}
					else
					{
						this.RejectAuthentication("authentication token not provided.", null, false);
					}
				}
			}
			catch (Exception ex)
			{
				ServerConsole.AddLog("Exception during authentication (client address: " + base.connectionToClient.address + "): " + ex.Message, ConsoleColor.Magenta, false);
				ServerConsole.AddLog(ex.StackTrace, ConsoleColor.Magenta, false);
				this.RejectAuthentication("server exception during authentication!", null, false);
			}
		}

		// Token: 0x06003D91 RID: 15761 RVA: 0x000CABA8 File Offset: 0x000C8DA8
		[Server]
		private void RejectAuthentication(string reason, string userId = null, bool removeSalt = false)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void CentralAuth.PlayerAuthenticationManager::RejectAuthentication(System.String,System.String,System.Boolean)' called when server was not active");
				return;
			}
			if (userId != null && removeSalt)
			{
				userId = PlayerAuthenticationManager.RemoveSalt(userId);
			}
			ServerConsole.AddLog(string.Concat(new string[]
			{
				"Player ",
				userId ?? "(unknown)",
				" (",
				base.connectionToClient.address,
				") failed to authenticate: ",
				reason
			}), ConsoleColor.Gray, false);
			this._hub.gameConsoleTransmission.SendToClient("Authentication failure: " + reason, "red");
			ServerConsole.Disconnect(this._hub.connectionToClient, "Authentication failure: " + reason);
		}

		// Token: 0x06003D92 RID: 15762 RVA: 0x000CAC5E File Offset: 0x000C8E5E
		[Server]
		private void RejectBadgeToken(string reason)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void CentralAuth.PlayerAuthenticationManager::RejectBadgeToken(System.String)' called when server was not active");
				return;
			}
			this._hub.gameConsoleTransmission.SendToClient("Your global badge token is invalid. Reason: " + reason, "red");
		}

		// Token: 0x06003D93 RID: 15763 RVA: 0x000CAC98 File Offset: 0x000C8E98
		[Server]
		private void FinalizeAuthentication()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void CentralAuth.PlayerAuthenticationManager::FinalizeAuthentication()' called when server was not active");
				return;
			}
			this.UserId = this.AuthenticationResponse.AuthToken.UserId;
			this.DoNotTrack = (this.AuthenticationResponse.DoNotTrack || this.AuthenticationResponse.AuthToken.DoNotTrack);
			this._hub.nicknameSync.UpdateNickname(StringUtils.Base64Decode(this.AuthenticationResponse.AuthToken.Nickname));
			if (this.DoNotTrack)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Networking, this._hub.LoggedNameFromRefHub() + " connected from IP address " + base.connectionToClient.address + " sent Do Not Track signal.", ServerLogs.ServerLogType.ConnectionUpdate, false);
			}
			this._hub.gameConsoleTransmission.SendToClient("Hi " + this._hub.nicknameSync.MyNick + "! You have been authenticated on this server.", "green");
			this._hub.serverRoles.RefreshPermissions(false);
			if (PlayerAuthenticationManager.AllowSameAccountJoining)
			{
				return;
			}
			int playerId = ReferenceHub.GetHub(base.gameObject).PlayerId;
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				if (referenceHub.authManager.UserId == this.UserId && referenceHub.PlayerId != playerId && !referenceHub.isLocalPlayer)
				{
					ServerConsole.AddLog(string.Format("Player {0} ({1}, {2}) has been kicked from the server, because he has just joined the server again from IP address {3}.", new object[]
					{
						this.UserId,
						referenceHub.PlayerId,
						base.connectionToClient.address,
						base.connectionToClient.address
					}), ConsoleColor.Gray, false);
					ServerConsole.Disconnect(referenceHub.gameObject, "Only one player instance of the same player is allowed.");
				}
			}
		}

		// Token: 0x06003D94 RID: 15764 RVA: 0x000CAE80 File Offset: 0x000C9080
		[Server]
		private bool CheckBans(AuthenticationToken token, string unsalted)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Boolean CentralAuth.PlayerAuthenticationManager::CheckBans(AuthenticationToken,System.String)' called when server was not active");
				return default(bool);
			}
			if ((!token.BypassBans || !CustomNetworkManager.IsVerified) && BanHandler.QueryBan(unsalted, null).Key != null)
			{
				this._hub.gameConsoleTransmission.SendToClient("You are banned from this server.", "red");
				ServerConsole.AddLog("Player kicked due to local UserID ban.", ConsoleColor.Gray, false);
				ServerConsole.Disconnect(this._hub.connectionToClient, "You are banned from this server.");
				return false;
			}
			if (CustomNetworkManager.IsVerified || CustomLiteNetLib4MirrorTransport.UseGlobalBans)
			{
				string globalBan = token.GlobalBan;
				if (!(globalBan == "NO"))
				{
					if (!(globalBan == "M1"))
					{
						if (!(globalBan == "M2"))
						{
							this._hub.gameConsoleTransmission.SendToClient(token.GlobalBan, "red");
							ServerConsole.AddLog("Player " + token.UserId + " has been kicked due to an active global ban: " + token.GlobalBan, ConsoleColor.Gray, false);
							ServerConsole.Disconnect(this._hub.connectionToClient, token.GlobalBan);
							return false;
						}
						ServerConsole.AddLog("Player " + token.UserId + " is globally muted on intercom.", ConsoleColor.Gray, false);
					}
					else
					{
						ServerConsole.AddLog("Player " + token.UserId + " is globally muted.", ConsoleColor.Gray, false);
					}
				}
			}
			if (!token.SkipIpCheck && !token.RequestIp.Equals("N/A", StringComparison.Ordinal) && ServerConsole.EnforceSameIp)
			{
				string address = this._hub.connectionToClient.address;
				if ((address.Contains(".", StringComparison.Ordinal) && token.RequestIp.Contains(".", StringComparison.Ordinal)) || (address.Contains(":", StringComparison.Ordinal) && token.RequestIp.Contains(":", StringComparison.Ordinal)))
				{
					bool flag = false;
					if (ServerConsole.SkipEnforcementForLocalAddresses)
					{
						flag = (address == "127.0.0.1" || address.StartsWith("10.", StringComparison.Ordinal) || address.StartsWith("192.168.", StringComparison.Ordinal));
						if (!flag && address.StartsWith("172.", StringComparison.Ordinal))
						{
							string[] array = address.Split('.', StringSplitOptions.None);
							byte b;
							if (array.Length == 4 && byte.TryParse(array[1], out b) && b >= 16 && b <= 31)
							{
								flag = true;
							}
						}
					}
					if (!flag && address != token.RequestIp)
					{
						this._hub.gameConsoleTransmission.SendToClient("Authentication token has been issued to a different IP address.", "red");
						this._hub.gameConsoleTransmission.SendToClient("Your IP address: " + address, "red");
						this._hub.gameConsoleTransmission.SendToClient("Issued to: " + token.RequestIp, "red");
						ServerConsole.AddLog("Player kicked due to IP addresses mismatch.", ConsoleColor.Gray, false);
						ServerConsole.Disconnect(this._hub.connectionToClient, "Authentication token has been issued to a different IP address. You can find details in the game console.");
						return false;
					}
				}
			}
			VcMuteFlags vcMuteFlags = VcMuteFlags.None;
			if (VoiceChatMutes.QueryLocalMute(unsalted, false))
			{
				vcMuteFlags |= VcMuteFlags.LocalRegular;
				this._hub.gameConsoleTransmission.SendToClient("You are muted on the voice chat by the server administrator.", "red");
			}
			if ((ConfigFile.ServerConfig.GetBool("global_mutes_voicechat", true) || CustomNetworkManager.IsVerified) && token.GlobalBan == "M1")
			{
				vcMuteFlags |= VcMuteFlags.GlobalRegular;
				this._hub.gameConsoleTransmission.SendToClient("You are globally muted on the voice chat.", "red");
			}
			if (VoiceChatMutes.QueryLocalMute(unsalted, true))
			{
				vcMuteFlags |= VcMuteFlags.LocalIntercom;
				this._hub.gameConsoleTransmission.SendToClient("You are muted on the intercom by the server administrator.", "red");
			}
			else if ((ConfigFile.ServerConfig.GetBool("global_mutes_intercom", true) || CustomNetworkManager.IsVerified) && token.GlobalBan == "M2")
			{
				vcMuteFlags |= VcMuteFlags.GlobalIntercom;
				this._hub.gameConsoleTransmission.SendToClient("You are globally muted on the intercom.", "red");
			}
			if (token.BypassBans)
			{
				vcMuteFlags = VcMuteFlags.None;
			}
			VoiceChatMutes.SetFlags(this._hub, vcMuteFlags);
			return true;
		}

		// Token: 0x06003D95 RID: 15765 RVA: 0x000CB26F File Offset: 0x000C946F
		private static string RemoveSalt(string userId)
		{
			if (userId == null)
			{
				return null;
			}
			if (userId.Contains("$"))
			{
				return userId.Substring(0, userId.IndexOf("$", StringComparison.Ordinal));
			}
			return userId;
		}

		// Token: 0x06003D96 RID: 15766 RVA: 0x000CB298 File Offset: 0x000C9498
		[Command]
		private void CmdHandlePasswordAuthentication(string clientSalt, byte[] signature, string ecdhPublicKey)
		{
			NetworkWriterPooled writer = NetworkWriterPool.Get();
			writer.WriteString(clientSalt);
			writer.WriteBytesAndSize(signature);
			writer.WriteString(ecdhPublicKey);
			base.SendCommandInternal("System.Void CentralAuth.PlayerAuthenticationManager::CmdHandlePasswordAuthentication(System.String,System.Byte[],System.String)", 1988415082, writer, 0, true);
			NetworkWriterPool.Return(writer);
		}

		// Token: 0x06003D97 RID: 15767 RVA: 0x000CB2E8 File Offset: 0x000C94E8
		[TargetRpc]
		private void RpcFinishExchange(string publicKey, byte[] signature)
		{
			NetworkWriterPooled writer = NetworkWriterPool.Get();
			writer.WriteString(publicKey);
			writer.WriteBytesAndSize(signature);
			this.SendTargetRPCInternal(null, "System.Void CentralAuth.PlayerAuthenticationManager::RpcFinishExchange(System.String,System.Byte[])", 112993832, writer, 0);
			NetworkWriterPool.Return(writer);
		}

		// Token: 0x06003D98 RID: 15768 RVA: 0x000CB32C File Offset: 0x000C952C
		[TargetRpc]
		private void RpcAnimateInvalidPassword()
		{
			NetworkWriterPooled writer = NetworkWriterPool.Get();
			this.SendTargetRPCInternal(null, "System.Void CentralAuth.PlayerAuthenticationManager::RpcAnimateInvalidPassword()", 2027815992, writer, 0);
			NetworkWriterPool.Return(writer);
		}

		// Token: 0x06003D99 RID: 15769 RVA: 0x000CB35C File Offset: 0x000C955C
		[Server]
		public void ResetPasswordAttempts()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void CentralAuth.PlayerAuthenticationManager::ResetPasswordAttempts()' called when server was not active");
				return;
			}
			this._passwordAttempts = 0U;
		}

		// Token: 0x06003D9A RID: 15770 RVA: 0x000CB37C File Offset: 0x000C957C
		[Server]
		private void AssignPasswordOverrideGroup()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void CentralAuth.PlayerAuthenticationManager::AssignPasswordOverrideGroup()' called when server was not active");
				return;
			}
			UserGroup overrideGroup = ServerStatic.PermissionsHandler.OverrideGroup;
			if (overrideGroup != null)
			{
				string text = this._hub.LoggedNameFromRefHub() + " used a valid RemoteAdmin override password.";
				ServerConsole.AddLog(text, ConsoleColor.DarkYellow, false);
				ServerLogs.AddLog(ServerLogs.Modules.Permissions, text, ServerLogs.ServerLogType.ConnectionUpdate, false);
				this._hub.serverRoles.SetGroup(overrideGroup, true, false);
				return;
			}
			ServerConsole.AddLog("Non-existing group is assigned for override password!", ConsoleColor.Red, false);
			this._hub.gameConsoleTransmission.SendToClient("Non-existing group is assigned for override password!", "red");
		}

		// Token: 0x06003D9B RID: 15771 RVA: 0x000CB410 File Offset: 0x000C9610
		private static byte[] DerivePassword(string password, string serversalt, string clientsalt)
		{
			byte[] salt = Sha.Sha512(serversalt + "/" + clientsalt);
			return PBKDF2.Pbkdf2HashBytes(password, salt, 1600, 512);
		}

		// Token: 0x06003D9C RID: 15772 RVA: 0x000CB440 File Offset: 0x000C9640
		private void UserIdHook(string p, string i)
		{
			Action<ReferenceHub> onSyncedUserIdAssigned = PlayerAuthenticationManager.OnSyncedUserIdAssigned;
			if (onSyncedUserIdAssigned != null)
			{
				onSyncedUserIdAssigned(this._hub);
			}
			if (string.IsNullOrEmpty(i))
			{
				this.InstanceMode = ClientInstanceMode.Unverified;
				return;
			}
			ClientInstanceMode instanceMode;
			if (!(i == "ID_Dedicated"))
			{
				if (!(i == "ID_Host"))
				{
					if (!(i == "ID_Dummy"))
					{
						instanceMode = ClientInstanceMode.ReadyClient;
					}
					else
					{
						instanceMode = ClientInstanceMode.Dummy;
					}
				}
				else
				{
					instanceMode = ClientInstanceMode.Host;
				}
			}
			else
			{
				instanceMode = ClientInstanceMode.DedicatedServer;
			}
			this.InstanceMode = instanceMode;
		}

		// Token: 0x06003D9E RID: 15774 RVA: 0x000CB4B4 File Offset: 0x000C96B4
		static PlayerAuthenticationManager()
		{
			RemoteProcedureCalls.RegisterCommand(typeof(PlayerAuthenticationManager), "System.Void CentralAuth.PlayerAuthenticationManager::CmdHandlePasswordAuthentication(System.String,System.Byte[],System.String)", new RemoteCallDelegate(PlayerAuthenticationManager.InvokeUserCode_CmdHandlePasswordAuthentication__String__Byte[]__String), true);
			RemoteProcedureCalls.RegisterRpc(typeof(PlayerAuthenticationManager), "System.Void CentralAuth.PlayerAuthenticationManager::RpcRequestAuthentication(System.String,System.String)", new RemoteCallDelegate(PlayerAuthenticationManager.InvokeUserCode_RpcRequestAuthentication__String__String));
			RemoteProcedureCalls.RegisterRpc(typeof(PlayerAuthenticationManager), "System.Void CentralAuth.PlayerAuthenticationManager::TargetSetRealId(Mirror.NetworkConnection,System.String)", new RemoteCallDelegate(PlayerAuthenticationManager.InvokeUserCode_TargetSetRealId__NetworkConnection__String));
			RemoteProcedureCalls.RegisterRpc(typeof(PlayerAuthenticationManager), "System.Void CentralAuth.PlayerAuthenticationManager::RpcFinishExchange(System.String,System.Byte[])", new RemoteCallDelegate(PlayerAuthenticationManager.InvokeUserCode_RpcFinishExchange__String__Byte[]));
			RemoteProcedureCalls.RegisterRpc(typeof(PlayerAuthenticationManager), "System.Void CentralAuth.PlayerAuthenticationManager::RpcAnimateInvalidPassword()", new RemoteCallDelegate(PlayerAuthenticationManager.InvokeUserCode_RpcAnimateInvalidPassword));
		}

		// Token: 0x06003D9F RID: 15775 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Weaved()
		{
			return true;
		}

		// Token: 0x17000E09 RID: 3593
		// (get) Token: 0x06003DA0 RID: 15776 RVA: 0x000CB574 File Offset: 0x000C9774
		// (set) Token: 0x06003DA1 RID: 15777 RVA: 0x000CB587 File Offset: 0x000C9787
		public string NetworkSyncedUserId
		{
			get
			{
				return this.SyncedUserId;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<string>(value, ref this.SyncedUserId, 1UL, new Action<string, string>(this.UserIdHook));
			}
		}

		// Token: 0x06003DA2 RID: 15778 RVA: 0x00002E9A File Offset: 0x0000109A
		protected void UserCode_RpcRequestAuthentication__String__String(string challenge, string ecdhPublicKey)
		{
		}

		// Token: 0x06003DA3 RID: 15779 RVA: 0x000CB5AC File Offset: 0x000C97AC
		protected static void InvokeUserCode_RpcRequestAuthentication__String__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("TargetRPC RpcRequestAuthentication called on server.");
				return;
			}
			((PlayerAuthenticationManager)obj).UserCode_RpcRequestAuthentication__String__String(reader.ReadString(), reader.ReadString());
		}

		// Token: 0x06003DA4 RID: 15780 RVA: 0x00002E9A File Offset: 0x0000109A
		protected void UserCode_TargetSetRealId__NetworkConnection__String(NetworkConnection conn, string userId)
		{
		}

		// Token: 0x06003DA5 RID: 15781 RVA: 0x000CB5DB File Offset: 0x000C97DB
		protected static void InvokeUserCode_TargetSetRealId__NetworkConnection__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("TargetRPC TargetSetRealId called on server.");
				return;
			}
			((PlayerAuthenticationManager)obj).UserCode_TargetSetRealId__NetworkConnection__String(null, reader.ReadString());
		}

		// Token: 0x06003DA6 RID: 15782 RVA: 0x000CB608 File Offset: 0x000C9808
		protected void UserCode_CmdHandlePasswordAuthentication__String__Byte[]__String(string clientSalt, byte[] signature, string ecdhPublicKey)
		{
			if (this._passwordAttempts > 2U)
			{
				this._hub.gameConsoleTransmission.SendToClient("Limit of RA password auth attempts exceeded.", "red");
				return;
			}
			if (this._passwordCooldown > 0f)
			{
				this._hub.gameConsoleTransmission.SendToClient("Please wait before trying to use password again!", "red");
				return;
			}
			this._passwordCooldown = 1.8f;
			if (base.isLocalPlayer)
			{
				this._hub.gameConsoleTransmission.SendToClient("Password authentication is not available for the host.", "red");
				return;
			}
			ReferenceHub hostHub = ReferenceHub.HostHub;
			if (!hostHub.queryProcessor.OverridePasswordEnabled)
			{
				this._hub.gameConsoleTransmission.SendToClient("Password authentication is disabled on this server!", "red");
				return;
			}
			if (clientSalt == null || signature == null)
			{
				this._hub.gameConsoleTransmission.SendToClient("Invalid password auth request - null parameters.", "red");
				return;
			}
			if (!PlayerAuthenticationManager._saltRegex.IsMatch(clientSalt))
			{
				this._hub.gameConsoleTransmission.SendToClient("Invalid password auth request - invalid client salt.", "red");
				return;
			}
			byte[] key = PlayerAuthenticationManager.DerivePassword(ServerStatic.PermissionsHandler.OverridePassword, hostHub.encryptedChannelManager.ServerRandom, clientSalt);
			if (PlayerAuthenticationManager.OnlineMode)
			{
				if (this.SyncedUserId == null)
				{
					this._hub.gameConsoleTransmission.SendToClient("Can't process password auth request - not authenticated while server is in online mode.", "red");
					return;
				}
				if (!Sha.Sha512Hmac(key, this.SyncedUserId).SequenceEqual(signature))
				{
					this._passwordAttempts += 1U;
					string text = this._hub.LoggedNameFromRefHub() + " attempted to use an invalid RemoteAdmin override password.";
					ServerConsole.AddLog(text, ConsoleColor.Magenta, false);
					ServerLogs.AddLog(ServerLogs.Modules.Permissions, text, ServerLogs.ServerLogType.ConnectionUpdate, false);
					this.RpcAnimateInvalidPassword();
					return;
				}
				this.AssignPasswordOverrideGroup();
				return;
			}
			else
			{
				if (ecdhPublicKey == null)
				{
					this._hub.gameConsoleTransmission.SendToClient("Can't process password auth request - ecdhPublicKey is null in offline mode.", "red");
					return;
				}
				if (!Sha.Sha512Hmac(key, ecdhPublicKey).SequenceEqual(signature))
				{
					this._passwordAttempts += 1U;
					string text2 = this._hub.LoggedNameFromRefHub() + " attempted to use an invalid RemoteAdmin override password.";
					ServerConsole.AddLog(text2, ConsoleColor.Magenta, false);
					ServerLogs.AddLog(ServerLogs.Modules.Permissions, text2, ServerLogs.ServerLogType.ConnectionUpdate, false);
					this.RpcAnimateInvalidPassword();
					return;
				}
				this._hub.encryptedChannelManager.PrepareExchange();
				this._hub.encryptedChannelManager.ServerProcessExchange(ecdhPublicKey);
				string text3 = ECDSA.KeyToString(this._hub.encryptedChannelManager.EcdhKeys.Public);
				byte[] signature2 = Sha.Sha512Hmac(key, text3);
				this.RpcFinishExchange(text3, signature2);
				this.AssignPasswordOverrideGroup();
				return;
			}
		}

		// Token: 0x06003DA7 RID: 15783 RVA: 0x000CB866 File Offset: 0x000C9A66
		protected static void InvokeUserCode_CmdHandlePasswordAuthentication__String__Byte[]__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
		{
			if (!NetworkServer.active)
			{
				Debug.LogError("Command CmdHandlePasswordAuthentication called on client.");
				return;
			}
			((PlayerAuthenticationManager)obj).UserCode_CmdHandlePasswordAuthentication__String__Byte[]__String(reader.ReadString(), reader.ReadBytesAndSize(), reader.ReadString());
		}

		// Token: 0x06003DA8 RID: 15784 RVA: 0x00002E9A File Offset: 0x0000109A
		protected void UserCode_RpcFinishExchange__String__Byte[](string publicKey, byte[] signature)
		{
		}

		// Token: 0x06003DA9 RID: 15785 RVA: 0x000CB89B File Offset: 0x000C9A9B
		protected static void InvokeUserCode_RpcFinishExchange__String__Byte[](NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("TargetRPC RpcFinishExchange called on server.");
				return;
			}
			((PlayerAuthenticationManager)obj).UserCode_RpcFinishExchange__String__Byte[](reader.ReadString(), reader.ReadBytesAndSize());
		}

		// Token: 0x06003DAA RID: 15786 RVA: 0x00002E9A File Offset: 0x0000109A
		protected void UserCode_RpcAnimateInvalidPassword()
		{
		}

		// Token: 0x06003DAB RID: 15787 RVA: 0x000CB8CA File Offset: 0x000C9ACA
		protected static void InvokeUserCode_RpcAnimateInvalidPassword(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("TargetRPC RpcAnimateInvalidPassword called on server.");
				return;
			}
			((PlayerAuthenticationManager)obj).UserCode_RpcAnimateInvalidPassword();
		}

		// Token: 0x06003DAC RID: 15788 RVA: 0x000CB8F0 File Offset: 0x000C9AF0
		public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
		{
			base.SerializeSyncVars(writer, forceAll);
			if (forceAll)
			{
				writer.WriteString(this.SyncedUserId);
				return;
			}
			writer.WriteULong(base.syncVarDirtyBits);
			if ((base.syncVarDirtyBits & 1UL) != 0UL)
			{
				writer.WriteString(this.SyncedUserId);
			}
		}

		// Token: 0x06003DAD RID: 15789 RVA: 0x000CB948 File Offset: 0x000C9B48
		public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
		{
			base.DeserializeSyncVars(reader, initialState);
			if (initialState)
			{
				base.GeneratedSyncVarDeserialize<string>(ref this.SyncedUserId, new Action<string, string>(this.UserIdHook), reader.ReadString());
				return;
			}
			long num = (long)reader.ReadULong();
			if ((num & 1L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<string>(ref this.SyncedUserId, new Action<string, string>(this.UserIdHook), reader.ReadString());
			}
		}

		// Token: 0x04002D01 RID: 11521
		[SyncVar(hook = "UserIdHook")]
		public string SyncedUserId;

		// Token: 0x04002D02 RID: 11522
		public static bool OnlineMode;

		// Token: 0x04002D03 RID: 11523
		internal static bool AllowSameAccountJoining;

		// Token: 0x04002D04 RID: 11524
		public static uint AuthenticationTimeout;

		// Token: 0x04002D05 RID: 11525
		private static readonly Regex _saltRegex = new Regex("^[a-zA-Z0-9]{32}$", RegexOptions.Compiled);

		// Token: 0x04002D06 RID: 11526
		private bool _authenticationRequested;

		// Token: 0x04002D07 RID: 11527
		private float _timeoutTimer;

		// Token: 0x04002D08 RID: 11528
		private float _passwordCooldown;

		// Token: 0x04002D09 RID: 11529
		private string _privUserId;

		// Token: 0x04002D0A RID: 11530
		private string _challenge;

		// Token: 0x04002D0B RID: 11531
		private string _clientSalt;

		// Token: 0x04002D0C RID: 11532
		private uint _passwordAttempts;

		// Token: 0x04002D0D RID: 11533
		private ReferenceHub _hub;

		// Token: 0x04002D0E RID: 11534
		private ClientInstanceMode _targetInstanceMode;

		// Token: 0x04002D0F RID: 11535
		private const int HashIterations = 1600;

		// Token: 0x04002D10 RID: 11536
		private const string HostId = "ID_Host";

		// Token: 0x04002D11 RID: 11537
		private const string DedicatedId = "ID_Dedicated";

		// Token: 0x04002D12 RID: 11538
		private const string OfflineModeIdPrefix = "ID_Offline_";

		// Token: 0x04002D13 RID: 11539
		private const string DummyId = "ID_Dummy";

		// Token: 0x02000AAB RID: 2731
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002D16 RID: 11542
			public static Action<NetworkConnectionToClient, AuthenticationResponse> <0>__ServerReceiveAuthenticationResponse;
		}
	}
}
