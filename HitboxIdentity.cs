using System;
using System.Collections.Generic;
using MapGeneration.StaticHelpers;
using PlayerRoles;
using PlayerRoles.FirstPersonControl.Thirdperson;
using PlayerStatsSystem;
using UnityEngine;

// Token: 0x0200012A RID: 298
public class HitboxIdentity : ModelReferencePoint, IDestructible, IBlockStaticBatching
{
	// Token: 0x170000DC RID: 220
	// (get) Token: 0x0600059A RID: 1434 RVA: 0x00014FEE File Offset: 0x000131EE
	private static bool AllowFriendlyFire
	{
		get
		{
			return (ServerConfigSynchronizer.Singleton.MainBoolsSync & 1) == 1;
		}
	}

	// Token: 0x170000DD RID: 221
	// (get) Token: 0x0600059B RID: 1435 RVA: 0x00015000 File Offset: 0x00013200
	public int Index
	{
		get
		{
			int num = this._indexCache.GetValueOrDefault();
			if (this._indexCache == null)
			{
				num = this.CharacterModel.Hitboxes.IndexOf(this);
				this._indexCache = new int?(num);
				return num;
			}
			return num;
		}
	}

	// Token: 0x170000DE RID: 222
	// (get) Token: 0x0600059C RID: 1436 RVA: 0x00015047 File Offset: 0x00013247
	public ReferenceHub TargetHub
	{
		get
		{
			return this.CharacterModel.OwnerHub;
		}
	}

	// Token: 0x170000DF RID: 223
	// (get) Token: 0x0600059D RID: 1437 RVA: 0x00015054 File Offset: 0x00013254
	public HitboxType HitboxType
	{
		get
		{
			return this._dmgMultiplier;
		}
	}

	// Token: 0x170000E0 RID: 224
	// (get) Token: 0x0600059E RID: 1438 RVA: 0x0001505C File Offset: 0x0001325C
	// (set) Token: 0x0600059F RID: 1439 RVA: 0x00015064 File Offset: 0x00013264
	public CharacterModel CharacterModel { get; private set; }

	// Token: 0x170000E1 RID: 225
	// (get) Token: 0x060005A0 RID: 1440 RVA: 0x0001506D File Offset: 0x0001326D
	// (set) Token: 0x060005A1 RID: 1441 RVA: 0x00015075 File Offset: 0x00013275
	public Collider[] TargetColliders { get; private set; }

	// Token: 0x170000E2 RID: 226
	// (get) Token: 0x060005A2 RID: 1442 RVA: 0x0001507E File Offset: 0x0001327E
	public uint NetworkId
	{
		get
		{
			return this.TargetHub.inventory.netId;
		}
	}

	// Token: 0x170000E3 RID: 227
	// (get) Token: 0x060005A3 RID: 1443 RVA: 0x00015090 File Offset: 0x00013290
	public Vector3 CenterOfMass
	{
		get
		{
			return base.FastTr.position;
		}
	}

	// Token: 0x060005A4 RID: 1444 RVA: 0x000150A0 File Offset: 0x000132A0
	public bool Damage(float damage, DamageHandlerBase handler, Vector3 exactPos)
	{
		if (this.TargetHub == null)
		{
			return false;
		}
		StandardDamageHandler standardDamageHandler = handler as StandardDamageHandler;
		if (standardDamageHandler != null)
		{
			standardDamageHandler.Hitbox = this._dmgMultiplier;
		}
		return this.TargetHub.playerStats.DealDamage(handler);
	}

	// Token: 0x060005A5 RID: 1445 RVA: 0x000150E4 File Offset: 0x000132E4
	public void SetColliders(bool newState)
	{
		Collider[] targetColliders = this.TargetColliders;
		for (int i = 0; i < targetColliders.Length; i++)
		{
			targetColliders[i].enabled = newState;
		}
	}

	// Token: 0x060005A6 RID: 1446 RVA: 0x0001510F File Offset: 0x0001330F
	private void Awake()
	{
		this.CharacterModel = base.GetComponentInParent<CharacterModel>();
		this.TargetColliders = base.GetComponents<Collider>();
	}

	// Token: 0x060005A7 RID: 1447 RVA: 0x00015129 File Offset: 0x00013329
	private void OnValidate()
	{
		this.Type |= RefpointType.Hitbox;
	}

	// Token: 0x060005A8 RID: 1448 RVA: 0x00015139 File Offset: 0x00013339
	private void Reset()
	{
		this.Type = (RefpointType.LineOfSightPoint | RefpointType.Hitbox);
	}

	// Token: 0x060005A9 RID: 1449 RVA: 0x00015142 File Offset: 0x00013342
	private void OnDestroy()
	{
		HitboxIdentity.Instances.Remove(this);
	}

	// Token: 0x060005AA RID: 1450 RVA: 0x00015150 File Offset: 0x00013350
	public static bool IsEnemy(ReferenceHub attacker, ReferenceHub victim)
	{
		return HitboxIdentity.IsEnemy(attacker.GetTeam(), victim.GetTeam());
	}

	// Token: 0x060005AB RID: 1451 RVA: 0x00015164 File Offset: 0x00013364
	public static bool IsEnemy(RoleTypeId attackerRole, RoleTypeId victimRole)
	{
		Team team = attackerRole.GetTeam();
		Team team2 = victimRole.GetTeam();
		return HitboxIdentity.IsEnemy(team, team2);
	}

	// Token: 0x060005AC RID: 1452 RVA: 0x00015184 File Offset: 0x00013384
	public static bool IsEnemy(Team attackerTeam, Team victimTeam)
	{
		return attackerTeam != Team.Dead && victimTeam != Team.Dead && (attackerTeam != Team.SCPs || victimTeam != Team.SCPs) && attackerTeam.GetFaction() != victimTeam.GetFaction();
	}

	// Token: 0x060005AD RID: 1453 RVA: 0x000151A9 File Offset: 0x000133A9
	public static bool IsDamageable(ReferenceHub attacker, ReferenceHub victim)
	{
		return HitboxIdentity.AllowFriendlyFire || HitboxIdentity.IsEnemy(attacker, victim);
	}

	// Token: 0x060005AE RID: 1454 RVA: 0x000151BB File Offset: 0x000133BB
	public static bool IsDamageable(RoleTypeId attackerRole, RoleTypeId victimRole)
	{
		return HitboxIdentity.AllowFriendlyFire || HitboxIdentity.IsEnemy(attackerRole, victimRole);
	}

	// Token: 0x060005AF RID: 1455 RVA: 0x000151CD File Offset: 0x000133CD
	public static bool IsDamageable(Team attackerTeam, Team victimTeam)
	{
		if (!HitboxIdentity.AllowFriendlyFire)
		{
			return HitboxIdentity.IsEnemy(attackerTeam, victimTeam);
		}
		return attackerTeam != Team.Dead && victimTeam != Team.Dead && (attackerTeam != Team.SCPs || victimTeam != Team.SCPs);
	}

	// Token: 0x040004E8 RID: 1256
	public static readonly HashSet<HitboxIdentity> Instances = new HashSet<HitboxIdentity>();

	// Token: 0x040004EB RID: 1259
	[SerializeField]
	private HitboxType _dmgMultiplier;

	// Token: 0x040004EC RID: 1260
	private int? _indexCache;
}
