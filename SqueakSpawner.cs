using System;
using System.Runtime.InteropServices;
using Interactables.Interobjects;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

// Token: 0x02000037 RID: 55
public class SqueakSpawner : NetworkBehaviour
{
	// Token: 0x06000135 RID: 309 RVA: 0x00005F19 File Offset: 0x00004119
	private void Awake()
	{
		if (NetworkServer.active && UnityEngine.Random.Range(0, 100) <= this.spawnChancePercent)
		{
			this.NetworksyncSpawn = (byte)UnityEngine.Random.Range(1, this.mice.Length + 1);
			this.SyncMouseSpawn(0, this.syncSpawn);
		}
	}

	// Token: 0x06000136 RID: 310 RVA: 0x00005F58 File Offset: 0x00004158
	[TargetRpc]
	public void TargetHitMouse(NetworkConnection networkConnection)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		this.SendTargetRPCInternal(networkConnection, "System.Void SqueakSpawner::TargetHitMouse(Mirror.NetworkConnection)", 1242807795, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x06000137 RID: 311 RVA: 0x00005F88 File Offset: 0x00004188
	private void SyncMouseSpawn(byte oldValue, byte newValue)
	{
		if (newValue == 0)
		{
			return;
		}
		GameObject gameObject = this.mice[(int)(newValue - 1)];
		gameObject.SetActive(true);
		this._spawnedMouse = gameObject.GetComponent<SqueakInteraction>();
	}

	// Token: 0x06000139 RID: 313 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x17000052 RID: 82
	// (get) Token: 0x0600013A RID: 314 RVA: 0x00005FC8 File Offset: 0x000041C8
	// (set) Token: 0x0600013B RID: 315 RVA: 0x00005FDB File Offset: 0x000041DB
	public byte NetworksyncSpawn
	{
		get
		{
			return this.syncSpawn;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<byte>(value, ref this.syncSpawn, 1UL, new Action<byte, byte>(this.SyncMouseSpawn));
		}
	}

	// Token: 0x0600013C RID: 316 RVA: 0x00002E9A File Offset: 0x0000109A
	protected void UserCode_TargetHitMouse__NetworkConnection(NetworkConnection networkConnection)
	{
	}

	// Token: 0x0600013D RID: 317 RVA: 0x00006000 File Offset: 0x00004200
	protected static void InvokeUserCode_TargetHitMouse__NetworkConnection(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC TargetHitMouse called on server.");
			return;
		}
		((SqueakSpawner)obj).UserCode_TargetHitMouse__NetworkConnection(null);
	}

	// Token: 0x0600013E RID: 318 RVA: 0x00006024 File Offset: 0x00004224
	static SqueakSpawner()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(SqueakSpawner), "System.Void SqueakSpawner::TargetHitMouse(Mirror.NetworkConnection)", new RemoteCallDelegate(SqueakSpawner.InvokeUserCode_TargetHitMouse__NetworkConnection));
	}

	// Token: 0x0600013F RID: 319 RVA: 0x00006048 File Offset: 0x00004248
	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteByte(this.syncSpawn);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1UL) != 0UL)
		{
			writer.WriteByte(this.syncSpawn);
		}
	}

	// Token: 0x06000140 RID: 320 RVA: 0x000060A0 File Offset: 0x000042A0
	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			base.GeneratedSyncVarDeserialize<byte>(ref this.syncSpawn, new Action<byte, byte>(this.SyncMouseSpawn), reader.ReadByte());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<byte>(ref this.syncSpawn, new Action<byte, byte>(this.SyncMouseSpawn), reader.ReadByte());
		}
	}

	// Token: 0x04000118 RID: 280
	[SerializeField]
	private int spawnChancePercent = 10;

	// Token: 0x04000119 RID: 281
	[SerializeField]
	private GameObject[] mice;

	// Token: 0x0400011A RID: 282
	[SyncVar(hook = "SyncMouseSpawn")]
	private byte syncSpawn;

	// Token: 0x0400011B RID: 283
	private SqueakInteraction _spawnedMouse;
}
