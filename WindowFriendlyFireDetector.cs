using System;
using UnityEngine;

// Token: 0x02000121 RID: 289
internal class WindowFriendlyFireDetector : FriendlyFireDetector
{
	// Token: 0x0600057C RID: 1404 RVA: 0x00014659 File Offset: 0x00012859
	internal WindowFriendlyFireDetector(ReferenceHub hub) : base(hub)
	{
	}

	// Token: 0x0600057D RID: 1405 RVA: 0x00014670 File Offset: 0x00012870
	public override bool RegisterDamage(float damage)
	{
		if (!FriendlyFireConfig.WindowEnabled || this._triggered)
		{
			return false;
		}
		if (Time.unscaledTime > this._lastReset + FriendlyFireConfig.Window)
		{
			this.Reset();
			this._lastReset = Time.unscaledTime;
		}
		base.RegisterDamage(damage);
		if (FriendlyFireConfig.WindowDamageThreshold > 0U && base.Damage >= FriendlyFireConfig.WindowDamageThreshold)
		{
			base.TakeAction(ref FriendlyFireConfig.WindowAction, "Window", ref FriendlyFireConfig.WindowBanTime, ref FriendlyFireConfig.WindowBanReason, ref FriendlyFireConfig.WindowKillReason, ref FriendlyFireConfig.WindowAdminMessage, ref FriendlyFireConfig.WindowBroadcastMessage, ref FriendlyFireConfig.WindowWebhook);
			return true;
		}
		return false;
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x00014708 File Offset: 0x00012908
	public override bool RegisterKill()
	{
		if (!FriendlyFireConfig.WindowEnabled || this._triggered)
		{
			return false;
		}
		if (Time.unscaledTime > this._lastReset + FriendlyFireConfig.Window)
		{
			this.Reset();
			this._lastReset = Time.unscaledTime;
		}
		base.RegisterKill();
		if (FriendlyFireConfig.WindowKillThreshold > 0U && base.Kills >= FriendlyFireConfig.WindowKillThreshold)
		{
			base.TakeAction(ref FriendlyFireConfig.WindowAction, "Window", ref FriendlyFireConfig.WindowBanTime, ref FriendlyFireConfig.WindowBanReason, ref FriendlyFireConfig.WindowKillReason, ref FriendlyFireConfig.WindowAdminMessage, ref FriendlyFireConfig.WindowBroadcastMessage, ref FriendlyFireConfig.WindowWebhook);
			return true;
		}
		return false;
	}

	// Token: 0x04000498 RID: 1176
	private const string Detector = "Window";

	// Token: 0x04000499 RID: 1177
	private float _lastReset = Time.unscaledTime;
}
