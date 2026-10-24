using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Interactables;
using Interactables.Verification;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.ThrowableProjectiles;
using Mirror;
using UnityEngine;

// Token: 0x02000033 RID: 51
public class Snowpile : NetworkBehaviour, IServerInteractable, IInteractable
{
	// Token: 0x1700004D RID: 77
	// (get) Token: 0x06000119 RID: 281 RVA: 0x000058E4 File Offset: 0x00003AE4
	// (set) Token: 0x0600011A RID: 282 RVA: 0x000058EC File Offset: 0x00003AEC
	public IVerificationRule VerificationRule { get; private set; } = new StandardDistanceVerification(8f, false, true);

	// Token: 0x0600011B RID: 283 RVA: 0x000058F8 File Offset: 0x00003AF8
	private void Awake()
	{
		this._mat = new Material(this._renderers[0].sharedMaterial);
		Renderer[] renderers = this._renderers;
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].sharedMaterial = this._mat;
		}
		this._prevSize = 1f;
		this._initialUses = this._remainingUses;
	}

	// Token: 0x0600011C RID: 284 RVA: 0x00005958 File Offset: 0x00003B58
	private void Update()
	{
		if (NetworkServer.active)
		{
			this.ServerUpdate();
		}
		else
		{
			base.transform.SetPositionAndRotation(this._position, this._rotation);
		}
		float num = Mathf.Lerp(this._prevSize, (float)this._remainingUses, Time.deltaTime * this._scaleAdjustSpeed);
		if (Mathf.Abs(num - (float)this._remainingUses) <= 0.03f)
		{
			return;
		}
		this._prevSize = num;
		float num2 = this._fadeOverUses.Evaluate(num);
		this._mat.SetFloat(Snowpile.FadeHash, num2);
		this._scaler.gameObject.SetActive(num2 > 0f);
		this._scaler.localScale = new Vector3(1f, this._scaleOverUses.Evaluate(num), 1f);
	}

	// Token: 0x0600011D RID: 285 RVA: 0x00005A24 File Offset: 0x00003C24
	private void ServerUpdate()
	{
		this.Network_position = base.transform.position;
		this.Network_rotation = base.transform.rotation;
		if (!this._regenerate || this._remainingUses >= this._initialUses)
		{
			return;
		}
		this._regenerationTimer += Time.deltaTime;
		if (this._regenerationTimer < this._regenerationDuration)
		{
			return;
		}
		this.Network_remainingUses = this._remainingUses + 1;
		this._regenerationTimer = 0f;
	}

	// Token: 0x0600011E RID: 286 RVA: 0x00005AA4 File Offset: 0x00003CA4
	public void ServerInteract(ReferenceHub ply, byte colliderId)
	{
		if (ply == null || this._remainingUses <= 0)
		{
			return;
		}
		if (!(ply.roleManager.CurrentRole is IInventoryRole))
		{
			return;
		}
		foreach (KeyValuePair<ushort, ItemBase> keyValuePair in ply.inventory.UserInventory.Items)
		{
			if (keyValuePair.Value is SnowballItem)
			{
				return;
			}
		}
		if (ply.inventory.ServerAddItem(ItemType.Snowball, ItemAddReason.PickedUp, 0, null) == null)
		{
			return;
		}
		this.Network_remainingUses = this._remainingUses - 1;
	}

	// Token: 0x06000121 RID: 289 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x1700004E RID: 78
	// (get) Token: 0x06000122 RID: 290 RVA: 0x00005B84 File Offset: 0x00003D84
	// (set) Token: 0x06000123 RID: 291 RVA: 0x00005B97 File Offset: 0x00003D97
	public Vector3 Network_position
	{
		get
		{
			return this._position;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<Vector3>(value, ref this._position, 1UL, null);
		}
	}

	// Token: 0x1700004F RID: 79
	// (get) Token: 0x06000124 RID: 292 RVA: 0x00005BB4 File Offset: 0x00003DB4
	// (set) Token: 0x06000125 RID: 293 RVA: 0x00005BC7 File Offset: 0x00003DC7
	public Quaternion Network_rotation
	{
		get
		{
			return this._rotation;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<Quaternion>(value, ref this._rotation, 2UL, null);
		}
	}

	// Token: 0x17000050 RID: 80
	// (get) Token: 0x06000126 RID: 294 RVA: 0x00005BE4 File Offset: 0x00003DE4
	// (set) Token: 0x06000127 RID: 295 RVA: 0x00005BF7 File Offset: 0x00003DF7
	public int Network_remainingUses
	{
		get
		{
			return this._remainingUses;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<int>(value, ref this._remainingUses, 4UL, null);
		}
	}

	// Token: 0x06000128 RID: 296 RVA: 0x00005C14 File Offset: 0x00003E14
	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteVector3(this._position);
			writer.WriteQuaternion(this._rotation);
			writer.WriteInt(this._remainingUses);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1UL) != 0UL)
		{
			writer.WriteVector3(this._position);
		}
		if ((base.syncVarDirtyBits & 2UL) != 0UL)
		{
			writer.WriteQuaternion(this._rotation);
		}
		if ((base.syncVarDirtyBits & 4UL) != 0UL)
		{
			writer.WriteInt(this._remainingUses);
		}
	}

	// Token: 0x06000129 RID: 297 RVA: 0x00005CC8 File Offset: 0x00003EC8
	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			base.GeneratedSyncVarDeserialize<Vector3>(ref this._position, null, reader.ReadVector3());
			base.GeneratedSyncVarDeserialize<Quaternion>(ref this._rotation, null, reader.ReadQuaternion());
			base.GeneratedSyncVarDeserialize<int>(ref this._remainingUses, null, reader.ReadInt());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<Vector3>(ref this._position, null, reader.ReadVector3());
		}
		if ((num & 2L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<Quaternion>(ref this._rotation, null, reader.ReadQuaternion());
		}
		if ((num & 4L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<int>(ref this._remainingUses, null, reader.ReadInt());
		}
	}

	// Token: 0x04000100 RID: 256
	private static readonly int FadeHash = Shader.PropertyToID("_Fade");

	// Token: 0x04000101 RID: 257
	private const float MinDiff = 0.03f;

	// Token: 0x04000102 RID: 258
	private Material _mat;

	// Token: 0x04000103 RID: 259
	private float _prevSize;

	// Token: 0x04000104 RID: 260
	private float _regenerationTimer;

	// Token: 0x04000105 RID: 261
	private int _initialUses;

	// Token: 0x04000106 RID: 262
	[SyncVar]
	private Vector3 _position;

	// Token: 0x04000107 RID: 263
	[SyncVar]
	private Quaternion _rotation;

	// Token: 0x04000108 RID: 264
	[SyncVar]
	[SerializeField]
	private int _remainingUses;

	// Token: 0x04000109 RID: 265
	[SerializeField]
	private Transform _scaler;

	// Token: 0x0400010A RID: 266
	[SerializeField]
	private AnimationCurve _scaleOverUses;

	// Token: 0x0400010B RID: 267
	[SerializeField]
	private AnimationCurve _fadeOverUses;

	// Token: 0x0400010C RID: 268
	[SerializeField]
	private float _scaleAdjustSpeed;

	// Token: 0x0400010D RID: 269
	[SerializeField]
	private bool _regenerate;

	// Token: 0x0400010E RID: 270
	[SerializeField]
	private float _regenerationDuration;

	// Token: 0x0400010F RID: 271
	[SerializeField]
	private Renderer[] _renderers;
}
