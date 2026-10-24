using System;
using CommandSystem;
using Footprinting;
using GameCore;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using NorthwoodLib;
using RemoteAdmin;

// Token: 0x020001CF RID: 463
public static class BanPlayer
{
	// Token: 0x0600097B RID: 2427 RVA: 0x000262C6 File Offset: 0x000244C6
	public static void GlobalBanUser(ReferenceHub target, ICommandSender issuer, string message)
	{
		if (ConfigFile.ServerConfig.GetBool("gban_ban_ip", false))
		{
			BanPlayer.ApplyIpBan(target, issuer, "You have been Globally Banned.", (long)((ulong)-1));
		}
		ServerConsole.Disconnect(target.gameObject, StringUtils.Base64Decode(message));
	}

	// Token: 0x0600097C RID: 2428 RVA: 0x000262FC File Offset: 0x000244FC
	public static bool KickUser(ReferenceHub target, ICommandSender issuer, string reason)
	{
		PlayerCommandSender playerCommandSender = issuer as PlayerCommandSender;
		ReferenceHub issuer2 = (playerCommandSender != null) ? playerCommandSender.ReferenceHub : ReferenceHub.HostHub;
		PlayerKickingEventArgs playerKickingEventArgs = new PlayerKickingEventArgs(target, issuer2, reason);
		PlayerEvents.OnKicking(playerKickingEventArgs);
		if (!playerKickingEventArgs.IsAllowed)
		{
			return false;
		}
		reason = playerKickingEventArgs.Reason;
		ServerConsole.Disconnect(target.gameObject, "You have been kicked. Reason: " + reason);
		PlayerEvents.OnKicked(new PlayerKickedEventArgs(target, issuer2, reason));
		return true;
	}

	// Token: 0x0600097D RID: 2429 RVA: 0x00026366 File Offset: 0x00024566
	public static bool KickUser(ReferenceHub target, ReferenceHub issuer, string reason)
	{
		return BanPlayer.KickUser(target, new PlayerCommandSender(issuer), reason);
	}

	// Token: 0x0600097E RID: 2430 RVA: 0x00026375 File Offset: 0x00024575
	public static bool KickUser(ReferenceHub target, string reason)
	{
		return BanPlayer.KickUser(target, ServerConsole.Scs, reason);
	}

	// Token: 0x0600097F RID: 2431 RVA: 0x00026383 File Offset: 0x00024583
	public static bool BanUser(ReferenceHub target, string reason, long duration)
	{
		return BanPlayer.BanUser(target, ServerConsole.Scs, reason, duration);
	}

	// Token: 0x06000980 RID: 2432 RVA: 0x00026392 File Offset: 0x00024592
	public static bool BanUser(Footprint target, string reason, long duration)
	{
		return BanPlayer.BanUser(target, ServerConsole.Scs, reason, duration);
	}

	// Token: 0x06000981 RID: 2433 RVA: 0x000263A1 File Offset: 0x000245A1
	public static bool BanUser(ReferenceHub target, ReferenceHub issuer, string reason, long duration)
	{
		return BanPlayer.BanUser(target, new PlayerCommandSender(issuer), reason, duration);
	}

	// Token: 0x06000982 RID: 2434 RVA: 0x000263B1 File Offset: 0x000245B1
	public static bool BanUser(ReferenceHub target, ICommandSender issuer, string reason, long duration)
	{
		return BanPlayer.BanUser(new Footprint(target), issuer, reason, duration);
	}

	// Token: 0x06000983 RID: 2435 RVA: 0x000263C4 File Offset: 0x000245C4
	public static bool BanUser(Footprint target, ICommandSender issuer, string reason, long duration)
	{
		if (duration == 0L && target.Hub != null)
		{
			return BanPlayer.KickUser(target.Hub, issuer, reason);
		}
		if (target.BypassStaff)
		{
			return false;
		}
		PlayerCommandSender playerCommandSender = issuer as PlayerCommandSender;
		ReferenceHub issuer2 = (playerCommandSender != null) ? playerCommandSender.ReferenceHub : ReferenceHub.HostHub;
		PlayerBanningEventArgs playerBanningEventArgs = new PlayerBanningEventArgs(target.Hub, target.LogUserID, issuer2, reason, duration);
		PlayerEvents.OnBanning(playerBanningEventArgs);
		if (!playerBanningEventArgs.IsAllowed)
		{
			return false;
		}
		duration = playerBanningEventArgs.Duration;
		reason = playerBanningEventArgs.Reason;
		BanPlayer.ApplyIpBan(target, issuer, reason, duration);
		long issuanceTime = TimeBehaviour.CurrentTimestamp();
		long banExpirationTime = TimeBehaviour.GetBanExpirationTime((uint)duration);
		string originalName = BanPlayer.ValidateNick(target.Nickname);
		if (!string.IsNullOrEmpty(target.LogUserID))
		{
			BanHandler.IssueBan(new BanDetails
			{
				OriginalName = originalName,
				Id = target.LogUserID,
				IssuanceTime = issuanceTime,
				Expires = banExpirationTime,
				Reason = reason,
				Issuer = issuer.LogName
			}, BanHandler.BanType.UserId, false);
		}
		PlayerEvents.OnBanned(new PlayerBannedEventArgs(target.Hub, target.LogUserID, issuer2, reason, duration));
		if (target.Hub != null)
		{
			ServerConsole.Disconnect(target.Hub.gameObject, "You have been banned. Reason: " + reason);
		}
		return true;
	}

	// Token: 0x06000984 RID: 2436 RVA: 0x000264FF File Offset: 0x000246FF
	public static void ApplyIpBan(ReferenceHub target, ICommandSender issuer, string reason, long duration)
	{
		BanPlayer.ApplyIpBan(new Footprint(target), issuer, reason, duration);
	}

	// Token: 0x06000985 RID: 2437 RVA: 0x00026510 File Offset: 0x00024710
	public static void ApplyIpBan(Footprint target, ICommandSender issuer, string reason, long duration)
	{
		if (!ConfigFile.ServerConfig.GetBool("ip_banning", false) || string.IsNullOrEmpty(target.IpAddress))
		{
			return;
		}
		long issuanceTime = TimeBehaviour.CurrentTimestamp();
		long banExpirationTime = TimeBehaviour.GetBanExpirationTime((uint)duration);
		BanHandler.IssueBan(new BanDetails
		{
			OriginalName = BanPlayer.ValidateNick(target.Nickname),
			Id = target.IpAddress,
			IssuanceTime = issuanceTime,
			Expires = banExpirationTime,
			Reason = reason,
			Issuer = issuer.LogName
		}, BanHandler.BanType.IP, false);
	}

	// Token: 0x06000986 RID: 2438 RVA: 0x00026598 File Offset: 0x00024798
	public static string ValidateNick(string username)
	{
		int @int = ConfigFile.ServerConfig.GetInt("ban_nickname_maxlength", 30);
		bool @bool = ConfigFile.ServerConfig.GetBool("ban_nickname_trimunicode", true);
		string text = string.IsNullOrEmpty(username) ? "(no nick)" : username;
		if (@bool)
		{
			text = StringUtils.StripUnicodeCharacters(text, "");
		}
		if (text.Length > @int)
		{
			text = text.Substring(0, @int);
		}
		return text;
	}

	// Token: 0x0400088F RID: 2191
	private const string _globalBanReason = "You have been Globally Banned.";
}
