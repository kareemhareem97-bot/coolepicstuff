using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Footprinting;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using MapGeneration.StaticHelpers;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x02000010 RID: 16
public class BreakableWindow : NetworkBehaviour, IDestructible, IBlockStaticBatching
{
	// Token: 0x17000015 RID: 21
	// (get) Token: 0x06000042 RID: 66 RVA: 0x00002B80 File Offset: 0x00000D80
	public uint NetworkId
	{
		get
		{
			return base.netId;
		}
	}

	// Token: 0x17000016 RID: 22
	// (get) Token: 0x06000043 RID: 67 RVA: 0x00002B88 File Offset: 0x00000D88
	public Vector3 CenterOfMass
	{
		get
		{
			return base.transform.position;
		}
	}

	// Token: 0x06000044 RID: 68 RVA: 0x00002B98 File Offset: 0x00000D98
	public bool Damage(float damage, DamageHandlerBase handler, Vector3 pos)
	{
		AttackerDamageHandler attackerDamageHandler = handler as AttackerDamageHandler;
		if (attackerDamageHandler == null)
		{
			this.ServerDamageWindow(damage);
			return true;
		}
		if (!this.CheckDamagePerms(attackerDamageHandler.Attacker.Role))
		{
			return false;
		}
		PlayerDamagingWindowEventArgs playerDamagingWindowEventArgs = new PlayerDamagingWindowEventArgs(attackerDamageHandler.Attacker.Hub, this, handler);
		PlayerEvents.OnDamagingWindow(playerDamagingWindowEventArgs);
		if (!playerDamagingWindowEventArgs.IsAllowed)
		{
			return false;
		}
		this.LastAttacker = attackerDamageHandler.Attacker;
		this.ServerDamageWindow(damage);
		PlayerEvents.OnDamagedWindow(new PlayerDamagedWindowEventArgs(attackerDamageHandler.Attacker.Hub, this, handler));
		return true;
	}

	// Token: 0x06000045 RID: 69 RVA: 0x00002C19 File Offset: 0x00000E19
	private void Awake()
	{
		this._meshRenderers.AddRange(base.GetComponentsInChildren<MeshRenderer>());
		base.GetComponent<Collider>().enabled = false;
		base.Invoke("EnableColliders", 1f);
	}

	// Token: 0x06000046 RID: 70 RVA: 0x00002C48 File Offset: 0x00000E48
	private void EnableColliders()
	{
		base.GetComponent<Collider>().enabled = true;
	}

	// Token: 0x06000047 RID: 71 RVA: 0x00002C56 File Offset: 0x00000E56
	private void Start()
	{
		Action<BreakableWindow> onAdded = BreakableWindow.OnAdded;
		if (onAdded == null)
		{
			return;
		}
		onAdded(this);
	}

	// Token: 0x06000048 RID: 72 RVA: 0x00002C68 File Offset: 0x00000E68
	private void OnDestroy()
	{
		Action<BreakableWindow> onDestroyed = BreakableWindow.OnDestroyed;
		if (onDestroyed == null)
		{
			return;
		}
		onDestroyed(this);
	}

	// Token: 0x06000049 RID: 73 RVA: 0x00002C7A File Offset: 0x00000E7A
	private void Update()
	{
		if (!this.IsBroken || this._prevStatus)
		{
			return;
		}
		base.StartCoroutine(this.BreakWindow());
		this._prevStatus = true;
	}

	// Token: 0x0600004A RID: 74 RVA: 0x00002CA4 File Offset: 0x00000EA4
	private void LateUpdate()
	{
		for (int i = this._meshRenderers.Count - 1; i >= 0; i--)
		{
			MeshRenderer meshRenderer = this._meshRenderers[i];
			meshRenderer.shadowCastingMode = (this.IsBroken ? ShadowCastingMode.ShadowsOnly : ShadowCastingMode.Off);
			if (this.IsBroken)
			{
				this._meshRenderers.RemoveAt(i);
				UnityEngine.Object.Destroy(meshRenderer);
			}
			meshRenderer.gameObject.layer = (this.IsBroken ? 28 : 14);
		}
	}

	// Token: 0x0600004B RID: 75 RVA: 0x00002D1B File Offset: 0x00000F1B
	private IEnumerator BreakWindow()
	{
		Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
		yield break;
	}

	// Token: 0x0600004C RID: 76 RVA: 0x00002D2C File Offset: 0x00000F2C
	private bool CheckDamagePerms(RoleTypeId roleType)
	{
		PlayerRoleBase playerRoleBase;
		return !this._preventScpDamage || (roleType.TryGetRoleTemplate(out playerRoleBase) && playerRoleBase.Team > Team.SCPs);
	}

	// Token: 0x0600004D RID: 77 RVA: 0x00002D58 File Offset: 0x00000F58
	[ServerCallback]
	private void ServerDamageWindow(float damage)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		this.Health -= damage;
		if (this.Health <= 0f)
		{
			this.NetworkIsBroken = true;
		}
	}

	// Token: 0x0600004F RID: 79 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x17000017 RID: 23
	// (get) Token: 0x06000050 RID: 80 RVA: 0x00002DA8 File Offset: 0x00000FA8
	// (set) Token: 0x06000051 RID: 81 RVA: 0x00002DBB File Offset: 0x00000FBB
	public bool NetworkIsBroken
	{
		get
		{
			return this.IsBroken;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<bool>(value, ref this.IsBroken, 1UL, null);
		}
	}

	// Token: 0x06000052 RID: 82 RVA: 0x00002DD8 File Offset: 0x00000FD8
	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(this.IsBroken);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1UL) != 0UL)
		{
			writer.WriteBool(this.IsBroken);
		}
	}

	// Token: 0x06000053 RID: 83 RVA: 0x00002E30 File Offset: 0x00001030
	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			base.GeneratedSyncVarDeserialize<bool>(ref this.IsBroken, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<bool>(ref this.IsBroken, null, reader.ReadBool());
		}
	}

	// Token: 0x04000017 RID: 23
	public static Action<BreakableWindow> OnAdded;

	// Token: 0x04000018 RID: 24
	public static Action<BreakableWindow> OnDestroyed;

	// Token: 0x04000019 RID: 25
	public Footprint LastAttacker;

	// Token: 0x0400001A RID: 26
	public float Health = 30f;

	// Token: 0x0400001B RID: 27
	[SyncVar]
	public bool IsBroken;

	// Token: 0x0400001C RID: 28
	public GameObject Template;

	// Token: 0x0400001D RID: 29
	public Transform Parent;

	// Token: 0x0400001E RID: 30
	[SerializeField]
	private bool _preventScpDamage;

	// Token: 0x0400001F RID: 31
	private readonly List<MeshRenderer> _meshRenderers = new List<MeshRenderer>();

	// Token: 0x04000020 RID: 32
	private bool _prevStatus;
}
