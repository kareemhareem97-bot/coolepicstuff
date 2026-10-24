using System;
using System.Threading;
using CentralAuth;
using Footprinting;
using PlayerStatsSystem;

// Token: 0x0200011D RID: 285
internal class FriendlyFireDetector
{
	// Token: 0x170000D8 RID: 216
	// (get) Token: 0x0600056A RID: 1386 RVA: 0x00014098 File Offset: 0x00012298
	// (set) Token: 0x0600056B RID: 1387 RVA: 0x000140A0 File Offset: 0x000122A0
	internal uint Kills { get; private set; }

	// Token: 0x170000D9 RID: 217
	// (get) Token: 0x0600056C RID: 1388 RVA: 0x000140A9 File Offset: 0x000122A9
	// (set) Token: 0x0600056D RID: 1389 RVA: 0x000140B1 File Offset: 0x000122B1
	internal float Damage { get; private set; }

	// Token: 0x0600056E RID: 1390 RVA: 0x000140BA File Offset: 0x000122BA
	internal FriendlyFireDetector(ReferenceHub hub)
	{
		this._hub = hub;
	}

	// Token: 0x0600056F RID: 1391 RVA: 0x000140C9 File Offset: 0x000122C9
	public virtual bool RegisterDamage(float damage)
	{
		this.Damage += damage;
		return false;
	}

	// Token: 0x06000570 RID: 1392 RVA: 0x000140DC File Offset: 0x000122DC
	public virtual bool RegisterKill()
	{
		uint kills = this.Kills;
		this.Kills = kills + 1U;
		return false;
	}

	// Token: 0x06000571 RID: 1393 RVA: 0x000140FA File Offset: 0x000122FA
	public virtual void Reset()
	{
		this.Kills = 0U;
		this.Damage = 0f;
	}

	// Token: 0x06000572 RID: 1394 RVA: 0x00014110 File Offset: 0x00012310
	protected void TakeAction(ref FriendlyFireAction action, string detector, ref long banDuration, ref string banReason, ref string killReason, ref string adminchat, ref string broadcast, ref bool webhook)
	{
		this._triggered = true;
		FriendlyFireDetector.TakeAction(new Footprint(this._hub), ref action, detector, ref banDuration, ref banReason, ref killReason, ref adminchat, ref broadcast, ref webhook);
	}

	// Token: 0x06000573 RID: 1395 RVA: 0x00014144 File Offset: 0x00012344
	internal static void TakeAction(Footprint fp, ref FriendlyFireAction action, string detector, ref long banDuration, ref string banReason, ref string killReason, ref string adminchat, ref string broadcast, ref bool webhook)
	{
		string text = (fp.Hub != null) ? fp.Hub.roleManager.CurrentRole.RoleTypeId.ToString() : fp.Role.ToString();
		if (!string.IsNullOrWhiteSpace(adminchat) && FriendlyFireConfig.AdminChatTime > 0)
		{
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				if (referenceHub.serverRoles.AdminChatPerms && referenceHub.Mode == ClientInstanceMode.ReadyClient)
				{
					referenceHub.queryProcessor.SendToClient(string.Format("@!{0} {1}", FriendlyFireConfig.AdminChatTime, adminchat.Replace("%nick", fp.Nickname)), true, false, string.Empty);
				}
			}
		}
		if (!string.IsNullOrWhiteSpace(broadcast) && FriendlyFireConfig.BroadcastTime > 0)
		{
			Broadcast.Singleton.RpcAddElement(broadcast.Replace("%nick", fp.Nickname), FriendlyFireConfig.BroadcastTime, Broadcast.BroadcastFlags.Normal);
		}
		if (webhook)
		{
			new Thread(delegate()
			{
				CheaterReport.SubmitReport("Friendly Fire Detector", fp.LoggedNameFromFootprint(), "Friendly fire has been detected. Detector: " + detector + ".", fp.NetId, "Friendly Fire Detector", fp.Nickname, true);
			})
			{
				Priority = ThreadPriority.BelowNormal,
				Name = "TK Reporter",
				IsBackground = true
			}.Start();
		}
		switch (action)
		{
		case FriendlyFireAction.Kill:
			if (fp.Hub != null)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Detector, string.Concat(new string[]
				{
					fp.Hub.LoggedNameFromRefHub(),
					" playing as ",
					text,
					" has been automatically killed for teamkilling. Detector: ",
					detector,
					"."
				}), ServerLogs.ServerLogType.Teamkill, false);
				fp.Hub.playerStats.DealDamage(new UniversalDamageHandler(-1f, DeathTranslations.FriendlyFireDetector, null));
				return;
			}
			break;
		case FriendlyFireAction.Kick:
			if (fp.Hub != null)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Detector, string.Concat(new string[]
				{
					fp.Hub.LoggedNameFromRefHub(),
					" playing as ",
					text,
					" has been automatically kicked for teamkilling. Detector: ",
					detector,
					"."
				}), ServerLogs.ServerLogType.Teamkill, false);
				BanPlayer.KickUser(fp.Hub, banReason);
				return;
			}
			break;
		case FriendlyFireAction.Ban:
			ServerLogs.AddLog(ServerLogs.Modules.Detector, string.Format("{0} playing as {1} has been automatically banned for teamkilling. Detector: {2}. Duration: {3} seconds.", new object[]
			{
				fp.LoggedNameFromFootprint(),
				text,
				detector,
				banDuration
			}), ServerLogs.ServerLogType.Teamkill, false);
			BanPlayer.BanUser(fp, banReason, banDuration);
			break;
		default:
			return;
		}
	}

	// Token: 0x04000492 RID: 1170
	protected bool _triggered;

	// Token: 0x04000493 RID: 1171
	private readonly ReferenceHub _hub;
}
