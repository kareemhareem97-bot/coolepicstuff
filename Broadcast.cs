using System;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

// Token: 0x0200010D RID: 269
public class Broadcast : NetworkBehaviour
{
	// Token: 0x170000CB RID: 203
	// (get) Token: 0x06000501 RID: 1281 RVA: 0x00012D67 File Offset: 0x00010F67
	public static Broadcast Singleton
	{
		get
		{
			if (!Broadcast._broadcastSet)
			{
				Broadcast._broadcastSet = true;
				Broadcast._broadcast = ReferenceHub.LocalHub.GetComponent<Broadcast>();
			}
			return Broadcast._broadcast;
		}
	}

	// Token: 0x06000502 RID: 1282 RVA: 0x00002E9A File Offset: 0x0000109A
	private void Start()
	{
	}

	// Token: 0x06000503 RID: 1283 RVA: 0x00012D8A File Offset: 0x00010F8A
	private void OnDestroy()
	{
		if (this == Broadcast._broadcast)
		{
			Broadcast._broadcastSet = false;
		}
	}

	// Token: 0x06000504 RID: 1284 RVA: 0x00012DA0 File Offset: 0x00010FA0
	[TargetRpc]
	public void TargetAddElement(NetworkConnection conn, string data, ushort time, Broadcast.BroadcastFlags flags)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(data);
		writer.WriteUShort(time);
		Mirror.GeneratedNetworkCode._Write_Broadcast/BroadcastFlags(writer, flags);
		this.SendTargetRPCInternal(conn, "System.Void Broadcast::TargetAddElement(Mirror.NetworkConnection,System.String,System.UInt16,Broadcast/BroadcastFlags)", 624805019, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x06000505 RID: 1285 RVA: 0x00012DF0 File Offset: 0x00010FF0
	[ClientRpc]
	public void RpcAddElement(string data, ushort time, Broadcast.BroadcastFlags flags)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(data);
		writer.WriteUShort(time);
		Mirror.GeneratedNetworkCode._Write_Broadcast/BroadcastFlags(writer, flags);
		this.SendRPCInternal("System.Void Broadcast::RpcAddElement(System.String,System.UInt16,Broadcast/BroadcastFlags)", -775219482, writer, 0, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x06000506 RID: 1286 RVA: 0x00012E40 File Offset: 0x00011040
	[TargetRpc]
	public void TargetClearElements(NetworkConnection conn)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		this.SendTargetRPCInternal(conn, "System.Void Broadcast::TargetClearElements(Mirror.NetworkConnection)", -1104952326, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x06000507 RID: 1287 RVA: 0x00012E70 File Offset: 0x00011070
	[ClientRpc]
	public void RpcClearElements()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		this.SendRPCInternal("System.Void Broadcast::RpcClearElements()", -701809763, writer, 0, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x06000509 RID: 1289 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x0600050A RID: 1290 RVA: 0x00002E9A File Offset: 0x0000109A
	protected void UserCode_TargetAddElement__NetworkConnection__String__UInt16__BroadcastFlags(NetworkConnection conn, string data, ushort time, Broadcast.BroadcastFlags flags)
	{
	}

	// Token: 0x0600050B RID: 1291 RVA: 0x00012EA0 File Offset: 0x000110A0
	protected static void InvokeUserCode_TargetAddElement__NetworkConnection__String__UInt16__BroadcastFlags(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC TargetAddElement called on server.");
			return;
		}
		((Broadcast)obj).UserCode_TargetAddElement__NetworkConnection__String__UInt16__BroadcastFlags(null, reader.ReadString(), reader.ReadUShort(), Mirror.GeneratedNetworkCode._Read_Broadcast/BroadcastFlags(reader));
	}

	// Token: 0x0600050C RID: 1292 RVA: 0x00002E9A File Offset: 0x0000109A
	protected void UserCode_RpcAddElement__String__UInt16__BroadcastFlags(string data, ushort time, Broadcast.BroadcastFlags flags)
	{
	}

	// Token: 0x0600050D RID: 1293 RVA: 0x00012ED6 File Offset: 0x000110D6
	protected static void InvokeUserCode_RpcAddElement__String__UInt16__BroadcastFlags(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcAddElement called on server.");
			return;
		}
		((Broadcast)obj).UserCode_RpcAddElement__String__UInt16__BroadcastFlags(reader.ReadString(), reader.ReadUShort(), Mirror.GeneratedNetworkCode._Read_Broadcast/BroadcastFlags(reader));
	}

	// Token: 0x0600050E RID: 1294 RVA: 0x00002E9A File Offset: 0x0000109A
	protected void UserCode_TargetClearElements__NetworkConnection(NetworkConnection conn)
	{
	}

	// Token: 0x0600050F RID: 1295 RVA: 0x00012F0B File Offset: 0x0001110B
	protected static void InvokeUserCode_TargetClearElements__NetworkConnection(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC TargetClearElements called on server.");
			return;
		}
		((Broadcast)obj).UserCode_TargetClearElements__NetworkConnection(null);
	}

	// Token: 0x06000510 RID: 1296 RVA: 0x00002E9A File Offset: 0x0000109A
	protected void UserCode_RpcClearElements()
	{
	}

	// Token: 0x06000511 RID: 1297 RVA: 0x00012F2F File Offset: 0x0001112F
	protected static void InvokeUserCode_RpcClearElements(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcClearElements called on server.");
			return;
		}
		((Broadcast)obj).UserCode_RpcClearElements();
	}

	// Token: 0x06000512 RID: 1298 RVA: 0x00012F54 File Offset: 0x00011154
	static Broadcast()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(Broadcast), "System.Void Broadcast::RpcAddElement(System.String,System.UInt16,Broadcast/BroadcastFlags)", new RemoteCallDelegate(Broadcast.InvokeUserCode_RpcAddElement__String__UInt16__BroadcastFlags));
		RemoteProcedureCalls.RegisterRpc(typeof(Broadcast), "System.Void Broadcast::RpcClearElements()", new RemoteCallDelegate(Broadcast.InvokeUserCode_RpcClearElements));
		RemoteProcedureCalls.RegisterRpc(typeof(Broadcast), "System.Void Broadcast::TargetAddElement(Mirror.NetworkConnection,System.String,System.UInt16,Broadcast/BroadcastFlags)", new RemoteCallDelegate(Broadcast.InvokeUserCode_TargetAddElement__NetworkConnection__String__UInt16__BroadcastFlags));
		RemoteProcedureCalls.RegisterRpc(typeof(Broadcast), "System.Void Broadcast::TargetClearElements(Mirror.NetworkConnection)", new RemoteCallDelegate(Broadcast.InvokeUserCode_TargetClearElements__NetworkConnection));
	}

	// Token: 0x0400044D RID: 1101
	private static Broadcast _broadcast;

	// Token: 0x0400044E RID: 1102
	private static bool _broadcastSet;

	// Token: 0x0200010E RID: 270
	[Flags]
	public enum BroadcastFlags : byte
	{
		// Token: 0x04000450 RID: 1104
		Normal = 0,
		// Token: 0x04000451 RID: 1105
		Truncated = 1,
		// Token: 0x04000452 RID: 1106
		AdminChat = 2
	}
}
