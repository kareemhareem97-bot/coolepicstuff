using System;
using System.Collections.Generic;
using System.Threading;
using CentralAuth;
using Cryptography;
using GameCore;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using Mirror;
using Mirror.RemoteCalls;
using NorthwoodLib;
using Security;
using UnityEngine;
using Utf8Json;

// Token: 0x020000A9 RID: 169
public class CheaterReport : NetworkBehaviour
{
	// Token: 0x06000377 RID: 887 RVA: 0x0000D011 File Offset: 0x0000B211
	private void Start()
	{
		this._hub = ReferenceHub.GetHub(this);
		this._commandRateLimit = this._hub.playerRateLimitHandler.RateLimits[1];
	}

	// Token: 0x06000378 RID: 888 RVA: 0x0000D038 File Offset: 0x0000B238
	[Command(channel = 4)]
	private void CmdReport(uint playerNetId, string reason, byte[] signature, bool notifyGm)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteUInt(playerNetId);
		writer.WriteString(reason);
		writer.WriteBytesAndSize(signature);
		writer.WriteBool(notifyGm);
		base.SendCommandInternal("System.Void CheaterReport::CmdReport(System.UInt32,System.String,System.Byte[],System.Boolean)", -1325630461, writer, 4, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x06000379 RID: 889 RVA: 0x0000D090 File Offset: 0x0000B290
	[Server]
	private void IssueReport(GameConsoleTransmission reporter, string reporterUserId, string reportedUserId, string reportedAuth, string reportedIp, string reporterAuth, string reporterIp, ref string reason, ref byte[] signature, string reporterPublicKey, uint reportedNetId, string reporterNickname, string reportedNickname)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void CheaterReport::IssueReport(GameConsoleTransmission,System.String,System.String,System.String,System.String,System.String,System.String,System.String&,System.Byte[]&,System.String,System.UInt32,System.String,System.String)' called when server was not active");
			return;
		}
		try
		{
			string data = string.Format("reporterAuth={0}&reporterIp={1}&reportedAuth={2}&reportedIp={3}&reason={4}&signature={5}&reporterKey={6}&token={7}&port={8}&serverIp={9}", new object[]
			{
				StringUtils.Base64Encode(reporterAuth),
				reporterIp,
				StringUtils.Base64Encode(reportedAuth),
				reportedIp,
				StringUtils.Base64Encode(CheaterReport.ConvertToLatin(reason)),
				Convert.ToBase64String(signature),
				StringUtils.Base64Encode(reporterPublicKey),
				ServerConsole.Password,
				ServerConsole.PortToReport,
				ServerConsole.Ip
			});
			string text = HttpQuery.Post(CentralServer.StandardUrl + "ingamereport.php", data, "application/x-www-form-urlencoded");
			if (reporter == null)
			{
				return;
			}
			if (!(text == "OK"))
			{
				if (!(text == "ReportedUserIDAlreadyReported"))
				{
					if (!(text == "RateLimited"))
					{
						this.SendSafeConsoleMessage(reporter, "[REPORTING] Error during **PROCESSING** player report:" + Environment.NewLine + text, "red");
					}
					else
					{
						this.SendSafeConsoleMessage(reporter, "[REPORTING] You are Ratelimited! Try again tomorrow." + Environment.NewLine, "red");
					}
				}
				else
				{
					this.SendSafeConsoleMessage(reporter, "[REPORTING] A report for this User ID already exists!" + Environment.NewLine, "yellow");
				}
			}
			else
			{
				this._reportedPlayers.Add(reportedNetId);
				this.SendSafeConsoleMessage(reporter, "[REPORTING] Player report successfully sent.", "green");
			}
		}
		catch (Exception ex)
		{
			GameCore.Console.AddLog("[HOST] Error during **SENDING** player report:" + Environment.NewLine + ex.Message, Color.red, false, ConsoleLogType.Log);
			if (reporter == null)
			{
				return;
			}
			this.SendSafeConsoleMessage(reporter, "[REPORTING] Error during **SENDING** player report.", "yellow");
		}
		if (CheaterReport.SendReportsByWebhooks)
		{
			this.LogReport(reporter, reporterUserId, reportedUserId, ref reason, reportedNetId, true, reporterNickname, reportedNickname);
		}
	}

	// Token: 0x0600037A RID: 890 RVA: 0x0000D268 File Offset: 0x0000B468
	[Server]
	private void LogReport(GameConsoleTransmission reporter, string reporterUserId, string reportedUserId, ref string reason, uint reportedNetId, bool notifyGm, string reporterNickname, string reportedNickname)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void CheaterReport::LogReport(GameConsoleTransmission,System.String,System.String,System.String&,System.UInt32,System.Boolean,System.String,System.String)' called when server was not active");
			return;
		}
		if (!CheaterReport.SubmitReport(reporterUserId, reportedUserId, reason, reportedNetId, reporterNickname, reportedNickname, false))
		{
			this.SendSafeConsoleMessage(reporter, "[REPORTING] Failed to send report to local administrators by webhooks.", "red");
			return;
		}
		if (notifyGm)
		{
			return;
		}
		this._reportedPlayers.Add(reportedNetId);
		this.SendSafeConsoleMessage(reporter, "[REPORTING] Player report successfully sent to local administrators by webhooks.", "green");
	}

	// Token: 0x0600037B RID: 891 RVA: 0x0000D2D4 File Offset: 0x0000B4D4
	[Server]
	private void SendSafeConsoleMessage(GameConsoleTransmission player, string message, string color)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void CheaterReport::SendSafeConsoleMessage(GameConsoleTransmission,System.String,System.String)' called when server was not active");
			return;
		}
		MainThreadDispatcher.Dispatch(delegate
		{
			player.SendToClient(message, color);
		}, MainThreadDispatcher.DispatchTime.Update);
	}

	// Token: 0x0600037C RID: 892 RVA: 0x0000D324 File Offset: 0x0000B524
	[Server]
	private void SendStaffChatNotification(string reporterUserId, string reportedUserId, string reason, string reporterNickname, string reportedNickname)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void CheaterReport::SendStaffChatNotification(System.String,System.String,System.String,System.String,System.String)' called when server was not active");
			return;
		}
		string content = string.Concat(new string[]
		{
			"0!<align=center><color=red><u>REPORT RECEIVED</u></color></align>\n<color=yellow>Reporter:</color>\n",
			reporterNickname,
			" (",
			reporterUserId,
			")\n<color=yellow>Reported:</color>\n",
			reportedNickname,
			" (",
			reportedUserId,
			")\n<color=yellow>Reason:</color>\n",
			reason
		});
		foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
		{
			ClientInstanceMode mode = referenceHub.Mode;
			if (mode != ClientInstanceMode.Unverified && mode != ClientInstanceMode.DedicatedServer && referenceHub.serverRoles.AdminChatPerms)
			{
				referenceHub.encryptedChannelManager.TrySendMessageToClient(content, EncryptedChannelManager.EncryptedChannel.AdminChat);
			}
		}
	}

	// Token: 0x0600037D RID: 893 RVA: 0x0000D3FC File Offset: 0x0000B5FC
	internal static bool SubmitReport(string reporterUserId, string reportedUserId, string reason, uint reportedId, string reporterNickname, string reportedNickname, bool friendlyFire)
	{
		bool result;
		try
		{
			HttpQuery.Post(friendlyFire ? FriendlyFireConfig.WebhookUrl : CheaterReport.WebhookUrl, JsonSerializer.ToJsonString<DiscordWebhook>(new DiscordWebhook(string.Empty, CheaterReport.WebhookUsername, CheaterReport.WebhookAvatar, false, new DiscordEmbed[]
			{
				new DiscordEmbed(CheaterReport.ReportHeader, "rich", CheaterReport.ReportContent, CheaterReport.WebhookColor, new DiscordEmbedField[]
				{
					new DiscordEmbedField("Server Name", CheaterReport.ServerName, false),
					new DiscordEmbedField("Server Endpoint", string.Format("{0}:{1}", ServerConsole.Ip, ServerConsole.PortToReport), false),
					new DiscordEmbedField("Reporter UserID", CheaterReport.AsDiscordCode(reporterUserId), false),
					new DiscordEmbedField("Reporter Nickname", CheaterReport.DiscordSanitize(reporterNickname), false),
					new DiscordEmbedField("Reported UserID", CheaterReport.AsDiscordCode(reportedUserId), false),
					new DiscordEmbedField("Reported Nickname", CheaterReport.DiscordSanitize(reportedNickname), false),
					new DiscordEmbedField("Reported NetID", reportedId.ToString(), false),
					new DiscordEmbedField("Reason", CheaterReport.DiscordSanitize(reason), false),
					new DiscordEmbedField("Timestamp", TimeBehaviour.Rfc3339Time(), false),
					new DiscordEmbedField("UTC Timestamp", TimeBehaviour.Rfc3339Time(DateTimeOffset.UtcNow), false)
				})
			})), "application/json");
			result = true;
		}
		catch (Exception ex)
		{
			ServerConsole.AddLog("Failed to send report by webhook: " + ex.Message, ConsoleColor.Gray, false);
			Debug.LogException(ex);
			result = false;
		}
		return result;
	}

	// Token: 0x0600037E RID: 894 RVA: 0x0000D5B8 File Offset: 0x0000B7B8
	private static string ConvertToLatin(string str)
	{
		foreach (KeyValuePair<char, string> keyValuePair in CheaterReport.CharacterReplacements)
		{
			str = str.Replace(keyValuePair.Key.ToString(), keyValuePair.Value);
		}
		return str;
	}

	// Token: 0x0600037F RID: 895 RVA: 0x0000D624 File Offset: 0x0000B824
	private static string AsDiscordCode(string text)
	{
		return "`" + text.Replace("`", "'") + "`";
	}

	// Token: 0x06000380 RID: 896 RVA: 0x0000D648 File Offset: 0x0000B848
	private static string DiscordSanitize(string text)
	{
		return text.Replace("<", "(").Replace(">", ")").Replace("@", "@ ").Replace("`", "'").Replace("~~", "∼∼").Replace("*", "★").Replace("_", "＿").Replace("&", " [AMP] ").Replace("?", " [QM] ");
	}

	// Token: 0x06000382 RID: 898 RVA: 0x0000D6E0 File Offset: 0x0000B8E0
	static CheaterReport()
	{
		RemoteProcedureCalls.RegisterCommand(typeof(CheaterReport), "System.Void CheaterReport::CmdReport(System.UInt32,System.String,System.Byte[],System.Boolean)", new RemoteCallDelegate(CheaterReport.InvokeUserCode_CmdReport__UInt32__String__Byte[]__Boolean), true);
	}

	// Token: 0x06000383 RID: 899 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x06000384 RID: 900 RVA: 0x0000DB40 File Offset: 0x0000BD40
	protected void UserCode_CmdReport__UInt32__String__Byte[]__Boolean(uint playerNetId, string reason, byte[] signature, bool notifyGm)
	{
		if (!this._commandRateLimit.CanExecute(true))
		{
			return;
		}
		if (reason == null)
		{
			return;
		}
		float num = Time.time - this._lastReport;
		if (num < 2f)
		{
			this._hub.gameConsoleTransmission.SendToClient("[REPORTING] Reporting rate limit exceeded (1).", "red");
			return;
		}
		if (num > 60f)
		{
			this._reportedPlayersAmount = 0;
		}
		if (this._reportedPlayersAmount > 5)
		{
			this._hub.gameConsoleTransmission.SendToClient("[REPORTING] Reporting rate limit exceeded (2).", "red");
			return;
		}
		if (notifyGm && (!CustomNetworkManager.IsVerified || string.IsNullOrEmpty(ServerConsole.Password)))
		{
			this._hub.gameConsoleTransmission.SendToClient("[REPORTING] Server is not verified - you can't use report feature on this server.", "red");
			return;
		}
		ReferenceHub referenceHub;
		if (!ReferenceHub.TryGetHubNetID(playerNetId, out referenceHub))
		{
			this._hub.gameConsoleTransmission.SendToClient("[REPORTING] Can't find player with that PlayerID.", "red");
			return;
		}
		PlayerAuthenticationManager reportedPam = referenceHub.authManager;
		if (this._reportedPlayers == null)
		{
			this._reportedPlayers = new HashSet<uint>();
		}
		if (this._reportedPlayers.Contains(playerNetId))
		{
			this._hub.gameConsoleTransmission.SendToClient("[REPORTING] You have already reported that player.", "red");
			return;
		}
		if (string.IsNullOrEmpty(reportedPam.UserId))
		{
			this._hub.gameConsoleTransmission.SendToClient("[REPORTING] Failed: User ID of reported player is null.", "red");
			return;
		}
		if (string.IsNullOrEmpty(this._hub.authManager.UserId))
		{
			this._hub.gameConsoleTransmission.SendToClient("[REPORTING] Failed: your User ID of is null.", "red");
			return;
		}
		if (this._hub.authManager.UserId == reportedPam.UserId)
		{
			this._hub.gameConsoleTransmission.SendToClient("[REPORTING] You can't report yourself!" + Environment.NewLine, "yellow");
			return;
		}
		string reportedNickname = referenceHub.nicknameSync.MyNick;
		if (!notifyGm)
		{
			PlayerReportingPlayerEventArgs playerReportingPlayerEventArgs = new PlayerReportingPlayerEventArgs(this._hub, referenceHub, reason);
			PlayerEvents.OnReportingPlayer(playerReportingPlayerEventArgs);
			if (!playerReportingPlayerEventArgs.IsAllowed)
			{
				return;
			}
			reason = playerReportingPlayerEventArgs.Reason;
			GameCore.Console.AddLog(string.Concat(new string[]
			{
				"Player ",
				this._hub.LoggedNameFromRefHub(),
				" reported player ",
				referenceHub.LoggedNameFromRefHub(),
				" with reason ",
				reason,
				"."
			}), Color.gray, false, ConsoleLogType.Log);
			this._hub.gameConsoleTransmission.SendToClient("[REPORTING] Player report successfully sent to local administrators.", "green");
			this.SendStaffChatNotification(this._hub.authManager.UserId, reportedPam.UserId, reason, this._hub.nicknameSync.MyNick, reportedNickname);
			if (CheaterReport.SendReportsByWebhooks)
			{
				new Thread(delegate()
				{
					this.LogReport(this._hub.gameConsoleTransmission, this._hub.authManager.UserId, reportedPam.UserId, ref reason, playerNetId, false, this._hub.nicknameSync.MyNick, reportedNickname);
				})
				{
					Priority = System.Threading.ThreadPriority.Lowest,
					IsBackground = true,
					Name = "Reporting player (locally) - " + reportedPam.UserId + " by " + this._hub.authManager.UserId
				}.Start();
			}
			PlayerEvents.OnReportedPlayer(new PlayerReportedPlayerEventArgs(this._hub, referenceHub, reason));
			return;
		}
		else
		{
			if (signature == null)
			{
				return;
			}
			if (!ECDSA.VerifyBytes(reportedPam.SyncedUserId + ";" + CheaterReport.ConvertToLatin(reason), signature, this._hub.authManager.AuthenticationResponse.PublicKey))
			{
				this._hub.gameConsoleTransmission.SendToClient("[REPORTING] Invalid report signature.", "red");
				return;
			}
			PlayerReportingCheaterEventArgs playerReportingCheaterEventArgs = new PlayerReportingCheaterEventArgs(this._hub, referenceHub, reason);
			PlayerEvents.OnReportingCheater(playerReportingCheaterEventArgs);
			if (!playerReportingCheaterEventArgs.IsAllowed)
			{
				return;
			}
			reason = playerReportingCheaterEventArgs.Reason;
			this._lastReport = Time.time;
			this._reportedPlayersAmount++;
			GameCore.Console.AddLog(string.Concat(new string[]
			{
				"Player ",
				this._hub.LoggedNameFromRefHub(),
				" reported player ",
				referenceHub.LoggedNameFromRefHub(),
				" with reason ",
				reason,
				". Sending report to Global Moderation."
			}), Color.gray, false, ConsoleLogType.Log);
			this.SendStaffChatNotification(this._hub.authManager.UserId, reportedPam.UserId, reason, this._hub.nicknameSync.MyNick, reportedNickname);
			new Thread(delegate()
			{
				this.IssueReport(this._hub.gameConsoleTransmission, this._hub.authManager.UserId, reportedPam.UserId, reportedPam.GetAuthToken(), reportedPam.connectionToClient.address, this._hub.authManager.GetAuthToken(), this._hub.connectionToClient.address, ref reason, ref signature, ECDSA.KeyToString(this._hub.authManager.AuthenticationResponse.PublicKey), playerNetId, this._hub.nicknameSync.MyNick, reportedNickname);
			})
			{
				Priority = System.Threading.ThreadPriority.Lowest,
				IsBackground = true,
				Name = "Reporting player - " + reportedPam.UserId + " by " + this._hub.authManager.UserId
			}.Start();
			PlayerEvents.OnReportedCheater(new PlayerReportedCheaterEventArgs(this._hub, referenceHub, reason));
			return;
		}
	}

	// Token: 0x06000385 RID: 901 RVA: 0x0000E050 File Offset: 0x0000C250
	protected static void InvokeUserCode_CmdReport__UInt32__String__Byte[]__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkServer.active)
		{
			Debug.LogError("Command CmdReport called on client.");
			return;
		}
		((CheaterReport)obj).UserCode_CmdReport__UInt32__String__Byte[]__Boolean(reader.ReadUInt(), reader.ReadString(), reader.ReadBytesAndSize(), reader.ReadBool());
	}

	// Token: 0x0400030D RID: 781
	private static readonly Dictionary<char, string> CharacterReplacements = new Dictionary<char, string>
	{
		{
			'а',
			"a"
		},
		{
			'б',
			"b"
		},
		{
			'в',
			"v"
		},
		{
			'г',
			"g"
		},
		{
			'д',
			"d"
		},
		{
			'е',
			"e"
		},
		{
			'ё',
			"yo"
		},
		{
			'ж',
			"zh"
		},
		{
			'з',
			"z"
		},
		{
			'и',
			"i"
		},
		{
			'й',
			"j"
		},
		{
			'к',
			"k"
		},
		{
			'л',
			"l"
		},
		{
			'м',
			"m"
		},
		{
			'н',
			"n"
		},
		{
			'о',
			"o"
		},
		{
			'п',
			"p"
		},
		{
			'р',
			"r"
		},
		{
			'с',
			"s"
		},
		{
			'т',
			"t"
		},
		{
			'у',
			"u"
		},
		{
			'ф',
			"f"
		},
		{
			'х',
			"h"
		},
		{
			'ц',
			"c"
		},
		{
			'ч',
			"ch"
		},
		{
			'ш',
			"sh"
		},
		{
			'щ',
			"sch"
		},
		{
			'ъ',
			"j"
		},
		{
			'ы',
			"i"
		},
		{
			'ь',
			"j"
		},
		{
			'э',
			"e"
		},
		{
			'ю',
			"yu"
		},
		{
			'я',
			"ya"
		},
		{
			'А',
			"A"
		},
		{
			'Б',
			"B"
		},
		{
			'В',
			"V"
		},
		{
			'Г',
			"G"
		},
		{
			'Д',
			"D"
		},
		{
			'Е',
			"E"
		},
		{
			'Ё',
			"Yo"
		},
		{
			'Ж',
			"Zh"
		},
		{
			'З',
			"Z"
		},
		{
			'И',
			"I"
		},
		{
			'Й',
			"J"
		},
		{
			'К',
			"K"
		},
		{
			'Л',
			"L"
		},
		{
			'М',
			"M"
		},
		{
			'Н',
			"N"
		},
		{
			'О',
			"O"
		},
		{
			'П',
			"P"
		},
		{
			'Р',
			"R"
		},
		{
			'С',
			"S"
		},
		{
			'Т',
			"T"
		},
		{
			'У',
			"U"
		},
		{
			'Ф',
			"F"
		},
		{
			'Х',
			"H"
		},
		{
			'Ц',
			"C"
		},
		{
			'Ч',
			"Ch"
		},
		{
			'Ш',
			"Sh"
		},
		{
			'Щ',
			"Sch"
		},
		{
			'Ъ',
			"J"
		},
		{
			'Ы',
			"I"
		},
		{
			'Ь',
			"J"
		},
		{
			'Э',
			"E"
		},
		{
			'Ю',
			"Yu"
		},
		{
			'Я',
			"Ya"
		}
	};

	// Token: 0x0400030E RID: 782
	internal static bool SendReportsByWebhooks = false;

	// Token: 0x0400030F RID: 783
	internal static string WebhookUrl;

	// Token: 0x04000310 RID: 784
	internal static string WebhookUsername;

	// Token: 0x04000311 RID: 785
	internal static string WebhookAvatar;

	// Token: 0x04000312 RID: 786
	internal static string ServerName;

	// Token: 0x04000313 RID: 787
	internal static string ReportHeader;

	// Token: 0x04000314 RID: 788
	internal static string ReportContent;

	// Token: 0x04000315 RID: 789
	internal static int WebhookColor;

	// Token: 0x04000316 RID: 790
	private int _reportedPlayersAmount;

	// Token: 0x04000317 RID: 791
	private float _lastReport;

	// Token: 0x04000318 RID: 792
	private HashSet<uint> _reportedPlayers;

	// Token: 0x04000319 RID: 793
	private RateLimit _commandRateLimit;

	// Token: 0x0400031A RID: 794
	private ReferenceHub _hub;
}
