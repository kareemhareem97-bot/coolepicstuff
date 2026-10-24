using System;

// Token: 0x0200011F RID: 287
internal class RoundFriendlyFireDetector : FriendlyFireDetector
{
	// Token: 0x06000576 RID: 1398 RVA: 0x00014490 File Offset: 0x00012690
	internal RoundFriendlyFireDetector(ReferenceHub hub) : base(hub)
	{
	}

	// Token: 0x06000577 RID: 1399 RVA: 0x0001449C File Offset: 0x0001269C
	public override bool RegisterDamage(float damage)
	{
		if (!FriendlyFireConfig.RoundEnabled || this._triggered)
		{
			return false;
		}
		base.RegisterDamage(damage);
		if (FriendlyFireConfig.RoundDamageThreshold > 0U && base.Damage >= FriendlyFireConfig.RoundDamageThreshold)
		{
			base.TakeAction(ref FriendlyFireConfig.RoundAction, "Round", ref FriendlyFireConfig.RoundBanTime, ref FriendlyFireConfig.RoundBanReason, ref FriendlyFireConfig.RoundKillReason, ref FriendlyFireConfig.RoundAdminMessage, ref FriendlyFireConfig.RoundBroadcastMessage, ref FriendlyFireConfig.RoundWebhook);
			return true;
		}
		return false;
	}

	// Token: 0x06000578 RID: 1400 RVA: 0x0001450C File Offset: 0x0001270C
	public override bool RegisterKill()
	{
		if (!FriendlyFireConfig.RoundEnabled || this._triggered)
		{
			return false;
		}
		base.RegisterKill();
		if (FriendlyFireConfig.RoundKillThreshold > 0U && base.Kills >= FriendlyFireConfig.RoundKillThreshold)
		{
			base.TakeAction(ref FriendlyFireConfig.RoundAction, "Round", ref FriendlyFireConfig.RoundBanTime, ref FriendlyFireConfig.RoundBanReason, ref FriendlyFireConfig.RoundKillReason, ref FriendlyFireConfig.RoundAdminMessage, ref FriendlyFireConfig.RoundBroadcastMessage, ref FriendlyFireConfig.RoundWebhook);
			return true;
		}
		return false;
	}

	// Token: 0x04000496 RID: 1174
	private const string Detector = "Round";
}
