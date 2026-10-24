using System;

// Token: 0x02000120 RID: 288
internal class LifeFriendlyFireDetector : FriendlyFireDetector
{
	// Token: 0x06000579 RID: 1401 RVA: 0x00014490 File Offset: 0x00012690
	internal LifeFriendlyFireDetector(ReferenceHub hub) : base(hub)
	{
	}

	// Token: 0x0600057A RID: 1402 RVA: 0x0001457C File Offset: 0x0001277C
	public override bool RegisterDamage(float damage)
	{
		if (!FriendlyFireConfig.LifeEnabled || this._triggered)
		{
			return false;
		}
		base.RegisterDamage(damage);
		if (FriendlyFireConfig.LifeDamageThreshold > 0U && base.Damage >= FriendlyFireConfig.LifeDamageThreshold)
		{
			base.TakeAction(ref FriendlyFireConfig.LifeAction, "Life", ref FriendlyFireConfig.LifeBanTime, ref FriendlyFireConfig.LifeBanReason, ref FriendlyFireConfig.LifeKillReason, ref FriendlyFireConfig.LifeAdminMessage, ref FriendlyFireConfig.LifeBroadcastMessage, ref FriendlyFireConfig.LifeWebhook);
			return true;
		}
		return false;
	}

	// Token: 0x0600057B RID: 1403 RVA: 0x000145EC File Offset: 0x000127EC
	public override bool RegisterKill()
	{
		if (!FriendlyFireConfig.LifeEnabled || this._triggered)
		{
			return false;
		}
		base.RegisterKill();
		if (FriendlyFireConfig.LifeKillThreshold > 0U && base.Kills >= FriendlyFireConfig.LifeKillThreshold)
		{
			base.TakeAction(ref FriendlyFireConfig.LifeAction, "Life", ref FriendlyFireConfig.LifeBanTime, ref FriendlyFireConfig.LifeBanReason, ref FriendlyFireConfig.LifeKillReason, ref FriendlyFireConfig.LifeAdminMessage, ref FriendlyFireConfig.LifeBroadcastMessage, ref FriendlyFireConfig.LifeWebhook);
			return true;
		}
		return false;
	}

	// Token: 0x04000497 RID: 1175
	private const string Detector = "Life";
}
