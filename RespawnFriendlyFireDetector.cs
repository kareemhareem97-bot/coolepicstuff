using System;
using UnityEngine;

// Token: 0x02000122 RID: 290
internal class RespawnFriendlyFireDetector : FriendlyFireDetector
{
	// Token: 0x0600057F RID: 1407 RVA: 0x0001479B File Offset: 0x0001299B
	internal RespawnFriendlyFireDetector(ReferenceHub hub) : base(hub)
	{
	}

	// Token: 0x06000580 RID: 1408 RVA: 0x000147B0 File Offset: 0x000129B0
	public override bool RegisterDamage(float damage)
	{
		if (!FriendlyFireConfig.RespawnEnabled || this._triggered || Time.unscaledTime > this._lastReset + FriendlyFireConfig.RespawnWindow)
		{
			return false;
		}
		base.RegisterDamage(damage);
		if (FriendlyFireConfig.RespawnDamageThreshold > 0U && base.Damage >= FriendlyFireConfig.RespawnDamageThreshold)
		{
			base.TakeAction(ref FriendlyFireConfig.RespawnAction, "Respawn", ref FriendlyFireConfig.RespawnBanTime, ref FriendlyFireConfig.RespawnBanReason, ref FriendlyFireConfig.RespawnKillReason, ref FriendlyFireConfig.RespawnAdminMessage, ref FriendlyFireConfig.RespawnBroadcastMessage, ref FriendlyFireConfig.RespawnWebhook);
			return true;
		}
		return false;
	}

	// Token: 0x06000581 RID: 1409 RVA: 0x00014838 File Offset: 0x00012A38
	public override bool RegisterKill()
	{
		if (!FriendlyFireConfig.RespawnEnabled || this._triggered || Time.unscaledTime > this._lastReset + FriendlyFireConfig.RespawnWindow)
		{
			return false;
		}
		base.RegisterKill();
		if (FriendlyFireConfig.RespawnKillThreshold > 0U && base.Kills >= FriendlyFireConfig.RespawnKillThreshold)
		{
			base.TakeAction(ref FriendlyFireConfig.RespawnAction, "Respawn", ref FriendlyFireConfig.RespawnBanTime, ref FriendlyFireConfig.RespawnBanReason, ref FriendlyFireConfig.RespawnKillReason, ref FriendlyFireConfig.RespawnAdminMessage, ref FriendlyFireConfig.RespawnBroadcastMessage, ref FriendlyFireConfig.RespawnWebhook);
			return true;
		}
		return false;
	}

	// Token: 0x06000582 RID: 1410 RVA: 0x000148BA File Offset: 0x00012ABA
	public override void Reset()
	{
		base.Reset();
		this._lastReset = Time.unscaledTime;
	}

	// Token: 0x0400049A RID: 1178
	private const string Detector = "Respawn";

	// Token: 0x0400049B RID: 1179
	private float _lastReset = Time.unscaledTime;
}
