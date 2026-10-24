using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Interactables;
using Interactables.Interobjects.DoorUtils;
using Interactables.Verification;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

// Token: 0x0200003E RID: 62
public class AlphaWarheadNukesitePanel : NetworkBehaviour, IServerInteractable, IInteractable
{
	// Token: 0x17000066 RID: 102
	// (get) Token: 0x06000199 RID: 409 RVA: 0x0000759C File Offset: 0x0000579C
	public bool AnyBlastdoorClosed
	{
		get
		{
			using (HashSet<BlastDoor>.Enumerator enumerator = BlastDoor.Instances.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.IsOpen)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	// Token: 0x0600019A RID: 410 RVA: 0x000075F4 File Offset: 0x000057F4
	private void Awake()
	{
		AlphaWarheadNukesitePanel.Singleton = this;
		int num = this._ledRenderer.sharedMaterials.Length;
		this._matSet = new Material[num];
		this._prevMats = new bool[num];
		for (int i = 0; i < num; i++)
		{
			this._matSet[i] = this._offMat;
		}
	}

	// Token: 0x17000067 RID: 103
	// (get) Token: 0x0600019B RID: 411 RVA: 0x00007647 File Offset: 0x00005847
	private bool OutsideDoorOpen
	{
		get
		{
			if (!this._doorFound)
			{
				this._doorFound = DoorNametagExtension.NamedDoors.TryGetValue("SURFACE_NUKE", out this._outsideDoor);
			}
			return this._doorFound && this._outsideDoor.TargetDoor.TargetState;
		}
	}

	// Token: 0x17000068 RID: 104
	// (get) Token: 0x0600019C RID: 412 RVA: 0x00003897 File Offset: 0x00001A97
	public IVerificationRule VerificationRule
	{
		get
		{
			return StandardDistanceVerification.Default;
		}
	}

	// Token: 0x0600019D RID: 413 RVA: 0x00007688 File Offset: 0x00005888
	public void ServerInteract(ReferenceHub ply, byte colliderId)
	{
		if (colliderId == 1)
		{
			AlphaWarheadController.Singleton.CancelDetonation(ply);
			ServerLogs.AddLog(ServerLogs.Modules.Warhead, ply.LoggedNameFromRefHub() + " cancelled the Alpha Warhead detonation.", ServerLogs.ServerLogType.GameEvent, false);
			return;
		}
		if (colliderId != 2)
		{
			return;
		}
		if (this._leverStatus > 0f && this._leverStatus < 1f)
		{
			return;
		}
		bool networkenabled = !this.enabled;
		PlayerInteractingWarheadLeverEventArgs playerInteractingWarheadLeverEventArgs = new PlayerInteractingWarheadLeverEventArgs(ply, networkenabled);
		PlayerEvents.OnInteractingWarheadLever(playerInteractingWarheadLeverEventArgs);
		if (!playerInteractingWarheadLeverEventArgs.IsAllowed)
		{
			return;
		}
		networkenabled = playerInteractingWarheadLeverEventArgs.Enabled;
		this.Networkenabled = networkenabled;
		this.RpcLeverSound();
		ServerLogs.AddLog(ServerLogs.Modules.Warhead, ply.LoggedNameFromRefHub() + " set the Alpha Warhead status to " + this.enabled.ToString() + ".", ServerLogs.ServerLogType.GameEvent, false);
		PlayerEvents.OnInteractedWarheadLever(new PlayerInteractedWarheadLeverEventArgs(ply, networkenabled));
	}

	// Token: 0x0600019E RID: 414 RVA: 0x00007748 File Offset: 0x00005948
	private void SetDiode(AlphaWarheadNukesitePanel.DiodeType diode, bool status)
	{
		if (this._prevMats[(int)diode] == status)
		{
			return;
		}
		this._matSet[(int)diode] = (status ? this._onMat : this._offMat);
		this._prevMats[(int)diode] = status;
		this._anyModified = true;
	}

	// Token: 0x0600019F RID: 415 RVA: 0x0000778C File Offset: 0x0000598C
	private void Update()
	{
		this._anyModified = false;
		this.SetDiode(AlphaWarheadNukesitePanel.DiodeType.InProgress, AlphaWarheadController.InProgress);
		this.SetDiode(AlphaWarheadNukesitePanel.DiodeType.OutsideDoor, this.OutsideDoorOpen);
		this.SetDiode(AlphaWarheadNukesitePanel.DiodeType.BlastDoor, this.AnyBlastdoorClosed);
		if (this._anyModified)
		{
			this._ledRenderer.sharedMaterials = this._matSet;
		}
		this._leverStatus += (this.enabled ? 0.04f : -0.04f);
		this._leverStatus = Mathf.Clamp01(this._leverStatus);
		this.lever.localRotation = Quaternion.Slerp(this._disabledLeverRotation, this._enabledLeverRotation, this._leverStatus);
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x00007834 File Offset: 0x00005A34
	[ClientRpc]
	private void RpcLeverSound()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		this.SendRPCInternal("System.Void AlphaWarheadNukesitePanel::RpcLeverSound()", 791711933, writer, 0, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x060001A2 RID: 418 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x17000069 RID: 105
	// (get) Token: 0x060001A3 RID: 419 RVA: 0x00007864 File Offset: 0x00005A64
	// (set) Token: 0x060001A4 RID: 420 RVA: 0x00007877 File Offset: 0x00005A77
	public bool Networkenabled
	{
		get
		{
			return this.enabled;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<bool>(value, ref this.enabled, 1UL, null);
		}
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x00002E9A File Offset: 0x0000109A
	protected void UserCode_RpcLeverSound()
	{
	}

	// Token: 0x060001A6 RID: 422 RVA: 0x00007891 File Offset: 0x00005A91
	protected static void InvokeUserCode_RpcLeverSound(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcLeverSound called on server.");
			return;
		}
		((AlphaWarheadNukesitePanel)obj).UserCode_RpcLeverSound();
	}

	// Token: 0x060001A7 RID: 423 RVA: 0x000078B4 File Offset: 0x00005AB4
	static AlphaWarheadNukesitePanel()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(AlphaWarheadNukesitePanel), "System.Void AlphaWarheadNukesitePanel::RpcLeverSound()", new RemoteCallDelegate(AlphaWarheadNukesitePanel.InvokeUserCode_RpcLeverSound));
	}

	// Token: 0x060001A8 RID: 424 RVA: 0x000078D8 File Offset: 0x00005AD8
	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(this.enabled);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1UL) != 0UL)
		{
			writer.WriteBool(this.enabled);
		}
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x00007930 File Offset: 0x00005B30
	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			base.GeneratedSyncVarDeserialize<bool>(ref this.enabled, null, reader.ReadBool());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<bool>(ref this.enabled, null, reader.ReadBool());
		}
	}

	// Token: 0x04000155 RID: 341
	public Transform lever;

	// Token: 0x04000156 RID: 342
	[SerializeField]
	private MeshRenderer _ledRenderer;

	// Token: 0x04000157 RID: 343
	[SerializeField]
	private Material _onMat;

	// Token: 0x04000158 RID: 344
	[SerializeField]
	private Material _offMat;

	// Token: 0x04000159 RID: 345
	[SerializeField]
	private Quaternion _enabledLeverRotation;

	// Token: 0x0400015A RID: 346
	[SerializeField]
	private Quaternion _disabledLeverRotation;

	// Token: 0x0400015B RID: 347
	[SyncVar]
	public new bool enabled;

	// Token: 0x0400015C RID: 348
	private float _leverStatus;

	// Token: 0x0400015D RID: 349
	private Material[] _matSet;

	// Token: 0x0400015E RID: 350
	private bool[] _prevMats;

	// Token: 0x0400015F RID: 351
	private bool _anyModified;

	// Token: 0x04000160 RID: 352
	private const string OutsideDoorName = "SURFACE_NUKE";

	// Token: 0x04000161 RID: 353
	public static AlphaWarheadNukesitePanel Singleton;

	// Token: 0x04000162 RID: 354
	private bool _doorFound;

	// Token: 0x04000163 RID: 355
	private DoorNametagExtension _outsideDoor;

	// Token: 0x0200003F RID: 63
	private enum DiodeType
	{
		// Token: 0x04000165 RID: 357
		InProgress,
		// Token: 0x04000166 RID: 358
		BlastDoor,
		// Token: 0x04000167 RID: 359
		OutsideDoor
	}

	// Token: 0x02000040 RID: 64
	private enum PanelColliderId
	{
		// Token: 0x04000169 RID: 361
		Cancel = 1,
		// Token: 0x0400016A RID: 362
		Lever
	}
}
