using System;
using Footprinting;
using PlayerStatsSystem;
using UnityEngine;

// Token: 0x02000031 RID: 49
public class SnowballDamageHandler : AttackerDamageHandler
{
	// Token: 0x17000046 RID: 70
	// (get) Token: 0x0600010C RID: 268 RVA: 0x0000578F File Offset: 0x0000398F
	// (set) Token: 0x0600010D RID: 269 RVA: 0x00005797 File Offset: 0x00003997
	public override float Damage { get; set; }

	// Token: 0x17000047 RID: 71
	// (get) Token: 0x0600010E RID: 270 RVA: 0x000057A0 File Offset: 0x000039A0
	// (set) Token: 0x0600010F RID: 271 RVA: 0x000057A8 File Offset: 0x000039A8
	public override Footprint Attacker { get; protected set; }

	// Token: 0x17000048 RID: 72
	// (get) Token: 0x06000110 RID: 272 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool AllowSelfDamage
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000049 RID: 73
	// (get) Token: 0x06000111 RID: 273 RVA: 0x000057B1 File Offset: 0x000039B1
	public override string ServerLogsText
	{
		get
		{
			return "Snowballed by " + this.Attacker.Nickname;
		}
	}

	// Token: 0x1700004A RID: 74
	// (get) Token: 0x06000112 RID: 274 RVA: 0x000057C8 File Offset: 0x000039C8
	public override string RagdollInspectText
	{
		get
		{
			return DeathTranslations.Crushed.RagdollTranslation;
		}
	}

	// Token: 0x1700004B RID: 75
	// (get) Token: 0x06000113 RID: 275 RVA: 0x000057D4 File Offset: 0x000039D4
	public override string DeathScreenText
	{
		get
		{
			return string.Empty;
		}
	}

	// Token: 0x06000114 RID: 276 RVA: 0x000057DC File Offset: 0x000039DC
	public SnowballDamageHandler()
	{
		this.Attacker = default(Footprint);
		this.Damage = 0f;
		this._moveDirection = Vector3.zero;
	}

	// Token: 0x06000115 RID: 277 RVA: 0x00005814 File Offset: 0x00003A14
	public SnowballDamageHandler(Footprint attacker, float damage, Vector3 moveDirection)
	{
		this.Attacker = attacker;
		this.Damage = damage;
		this._moveDirection = moveDirection;
	}

	// Token: 0x06000116 RID: 278 RVA: 0x00005834 File Offset: 0x00003A34
	public override DamageHandlerBase.HandlerOutput ApplyDamage(ReferenceHub ply)
	{
		HealthStat module = ply.playerStats.GetModule<HealthStat>();
		this.ProcessDamage(ply);
		if (this.Damage <= 0f)
		{
			return DamageHandlerBase.HandlerOutput.Nothing;
		}
		module.CurValue -= this.Damage;
		this.StartVelocity += this._moveDirection.NormalizeIgnoreY() * 15f + Vector3.up * 2f;
		if (module.CurValue > 0f)
		{
			return DamageHandlerBase.HandlerOutput.Damaged;
		}
		return DamageHandlerBase.HandlerOutput.Death;
	}

	// Token: 0x040000FA RID: 250
	private const float UpwardsForce = 2f;

	// Token: 0x040000FB RID: 251
	private const float HorizontalForce = 15f;

	// Token: 0x040000FC RID: 252
	private readonly Vector3 _moveDirection;
}
