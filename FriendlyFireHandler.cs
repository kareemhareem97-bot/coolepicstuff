using System;
using System.Runtime.CompilerServices;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;

// Token: 0x02000123 RID: 291
internal class FriendlyFireHandler
{
	// Token: 0x06000583 RID: 1411 RVA: 0x000148D0 File Offset: 0x00012AD0
	internal FriendlyFireHandler(ReferenceHub hub)
	{
		this.Round = new RoundFriendlyFireDetector(hub);
		this.Life = new LifeFriendlyFireDetector(hub);
		this.Window = new WindowFriendlyFireDetector(hub);
		this.Respawn = new RespawnFriendlyFireDetector(hub);
		if (!FriendlyFireHandler._eventsAssigned)
		{
			Action<ReferenceHub, DamageHandlerBase> value;
			if ((value = FriendlyFireHandler.<>O.<0>__OnAnyDamaged) == null)
			{
				value = (FriendlyFireHandler.<>O.<0>__OnAnyDamaged = new Action<ReferenceHub, DamageHandlerBase>(FriendlyFireHandler.OnAnyDamaged));
			}
			PlayerStats.OnAnyPlayerDamaged += value;
			Action<ReferenceHub, DamageHandlerBase> value2;
			if ((value2 = FriendlyFireHandler.<>O.<1>__OnAnyDied) == null)
			{
				value2 = (FriendlyFireHandler.<>O.<1>__OnAnyDied = new Action<ReferenceHub, DamageHandlerBase>(FriendlyFireHandler.OnAnyDied));
			}
			PlayerStats.OnAnyPlayerDied += value2;
			FriendlyFireHandler._eventsAssigned = true;
		}
	}

	// Token: 0x06000584 RID: 1412 RVA: 0x00014960 File Offset: 0x00012B60
	private static void OnAnyDied(ReferenceHub deadPlayer, DamageHandlerBase handler)
	{
		AttackerDamageHandler attackerDamageHandler;
		if (!FriendlyFireHandler.IsFriendlyFire(deadPlayer, handler, out attackerDamageHandler))
		{
			return;
		}
		ReferenceHub hub = attackerDamageHandler.Attacker.Hub;
		if (hub == null)
		{
			if (FriendlyFireConfig.ExplosionAfterDisconnecting && attackerDamageHandler is ExplosionDamageHandler)
			{
				FriendlyFireDetector.TakeAction(attackerDamageHandler.Attacker, ref FriendlyFireConfig.ExplosionAfterDisconnectingAction, "Explosion After Disconnection", ref FriendlyFireConfig.ExplosionAfterDisconnectingTime, ref FriendlyFireConfig.ExplosionAfterDisconnectingBanReason, ref FriendlyFireConfig.ExplosionAfterDisconnectingBanReason, ref FriendlyFireConfig.ExplosionAfterDisconnectingAdminMessage, ref FriendlyFireConfig.ExplosionAfterDisconnectingBroadcastMessage, ref FriendlyFireConfig.ExplosionAfterDisconnectingWebhook);
			}
			return;
		}
		if (hub.FriendlyFireHandler.Respawn.RegisterKill())
		{
			return;
		}
		if (hub.FriendlyFireHandler.Window.RegisterKill())
		{
			return;
		}
		if (hub.FriendlyFireHandler.Life.RegisterKill())
		{
			return;
		}
		hub.FriendlyFireHandler.Round.RegisterKill();
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x00014A1C File Offset: 0x00012C1C
	private static void OnAnyDamaged(ReferenceHub damagedPlayer, DamageHandlerBase handler)
	{
		AttackerDamageHandler attackerDamageHandler;
		if (!FriendlyFireHandler.IsFriendlyFire(damagedPlayer, handler, out attackerDamageHandler))
		{
			return;
		}
		ReferenceHub hub = attackerDamageHandler.Attacker.Hub;
		if (hub == null)
		{
			return;
		}
		float damage = attackerDamageHandler.Damage;
		if (hub.FriendlyFireHandler.Respawn.RegisterDamage(damage))
		{
			return;
		}
		if (hub.FriendlyFireHandler.Window.RegisterDamage(damage))
		{
			return;
		}
		if (hub.FriendlyFireHandler.Life.RegisterDamage(damage))
		{
			return;
		}
		hub.FriendlyFireHandler.Round.RegisterDamage(damage);
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x00014AA0 File Offset: 0x00012CA0
	private static bool IsFriendlyFire(ReferenceHub damagedPlayer, DamageHandlerBase handler, out AttackerDamageHandler attackerHandler)
	{
		attackerHandler = null;
		if (FriendlyFireConfig.PauseDetector || !NetworkServer.active)
		{
			return false;
		}
		AttackerDamageHandler attackerDamageHandler = handler as AttackerDamageHandler;
		if (attackerDamageHandler == null || attackerDamageHandler.IgnoreFriendlyFireDetector)
		{
			return false;
		}
		attackerHandler = attackerDamageHandler;
		PlayerRoleBase playerRoleBase;
		if (!attackerHandler.Attacker.Role.TryGetRoleTemplate(out playerRoleBase))
		{
			return false;
		}
		if (damagedPlayer.GetFaction() != playerRoleBase.Team.GetFaction())
		{
			return false;
		}
		ReferenceHub hub = attackerHandler.Attacker.Hub;
		return (!(hub != null) || (!(hub == damagedPlayer) && !PermissionsHandler.IsPermitted(hub.serverRoles.Permissions, PlayerPermissions.FriendlyFireDetectorImmunity))) && (!FriendlyFireConfig.IgnoreClassDTeamkills || damagedPlayer.GetRoleId() != RoleTypeId.ClassD || attackerHandler.Attacker.Role != RoleTypeId.ClassD);
	}

	// Token: 0x0400049C RID: 1180
	private static bool _eventsAssigned;

	// Token: 0x0400049D RID: 1181
	internal readonly RoundFriendlyFireDetector Round;

	// Token: 0x0400049E RID: 1182
	internal readonly LifeFriendlyFireDetector Life;

	// Token: 0x0400049F RID: 1183
	internal readonly WindowFriendlyFireDetector Window;

	// Token: 0x040004A0 RID: 1184
	internal readonly RespawnFriendlyFireDetector Respawn;

	// Token: 0x02000124 RID: 292
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x040004A1 RID: 1185
		public static Action<ReferenceHub, DamageHandlerBase> <0>__OnAnyDamaged;

		// Token: 0x040004A2 RID: 1186
		public static Action<ReferenceHub, DamageHandlerBase> <1>__OnAnyDied;
	}
}
