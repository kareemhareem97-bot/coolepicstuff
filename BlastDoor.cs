using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.Handlers;
using Mirror;
using UnityEngine;

// Token: 0x0200005B RID: 91
[RequireComponent(typeof(NetworkIdentity))]
public class BlastDoor : NetworkBehaviour
{
	// Token: 0x1700007C RID: 124
	// (get) Token: 0x0600021E RID: 542 RVA: 0x00008BC9 File Offset: 0x00006DC9
	public bool IsOpen
	{
		get
		{
			return this._isOpen;
		}
	}

	// Token: 0x0600021F RID: 543 RVA: 0x00008BD1 File Offset: 0x00006DD1
	public override void OnStartClient()
	{
		base.OnStartClient();
		this.SetDoorState(true, this._isOpen);
	}

	// Token: 0x06000220 RID: 544 RVA: 0x00008BE6 File Offset: 0x00006DE6
	private void Start()
	{
		BlastDoor.Instances.Add(this);
	}

	// Token: 0x06000221 RID: 545 RVA: 0x00008BF4 File Offset: 0x00006DF4
	private void OnDestroy()
	{
		BlastDoor.Instances.Remove(this);
	}

	// Token: 0x06000222 RID: 546 RVA: 0x00008C04 File Offset: 0x00006E04
	private void SetDoorState(bool _, bool newState)
	{
		BlastDoorChangingEventArgs blastDoorChangingEventArgs = new BlastDoorChangingEventArgs(this, newState);
		ServerEvents.OnBlastDoorChanging(blastDoorChangingEventArgs);
		if (!blastDoorChangingEventArgs.IsAllowed)
		{
			return;
		}
		this.Network_isOpen = newState;
		if (this._animator == null)
		{
			this._animator = base.GetComponent<Animator>();
		}
		this._animator.SetBool(BlastDoor._isOpenId, newState);
		ServerEvents.OnBlastDoorChanged(new BlastDoorChangedEventArgs(this, newState));
	}

	// Token: 0x06000223 RID: 547 RVA: 0x00008C64 File Offset: 0x00006E64
	[Server]
	public void ServerSetTargetState(bool isOpen)
	{
		if (!NetworkServer.active)
		{
			Debug.LogWarning("[Server] function 'System.Void BlastDoor::ServerSetTargetState(System.Boolean)' called when server was not active");
			return;
		}
		this.SetDoorState(this._isOpen, isOpen);
	}

	// Token: 0x06000226 RID: 550 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x1700007D RID: 125
	// (get) Token: 0x06000227 RID: 551 RVA: 0x00008CB4 File Offset: 0x00006EB4
	// (set) Token: 0x06000228 RID: 552 RVA: 0x00008CC7 File Offset: 0x00006EC7
	public bool Network_isOpen
	{
		get
		{
			return this._isOpen;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<bool>(value, ref this._isOpen, 1UL, new Action<bool, bool>(this.SetDoorState));
		}
	}

	// Token: 0x06000229 RID: 553 RVA: 0x00008CEC File Offset: 0x00006EEC
	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(this._isOpen);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1UL) != 0UL)
		{
			writer.WriteBool(this._isOpen);
		}
	}

	// Token: 0x0600022A RID: 554 RVA: 0x00008D44 File Offset: 0x00006F44
	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			base.GeneratedSyncVarDeserialize<bool>(ref this._isOpen, new Action<bool, bool>(this.SetDoorState), reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<bool>(ref this._isOpen, new Action<bool, bool>(this.SetDoorState), reader.ReadBool());
		}
	}

	// Token: 0x040001C2 RID: 450
	public static readonly HashSet<BlastDoor> Instances = new HashSet<BlastDoor>();

	// Token: 0x040001C3 RID: 451
	private static readonly int _isOpenId = Animator.StringToHash("IsOpen");

	// Token: 0x040001C4 RID: 452
	private Animator _animator;

	// Token: 0x040001C5 RID: 453
	[SyncVar(hook = "SetDoorState")]
	private bool _isOpen = true;
}
