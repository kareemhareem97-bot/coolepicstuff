using System;
using GameCore;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine;

// Token: 0x02000175 RID: 373
public class VersionCheck : NetworkBehaviour
{
	// Token: 0x06000777 RID: 1911 RVA: 0x0001C684 File Offset: 0x0001A884
	private void Start()
	{
		if (!NetworkServer.active)
		{
			return;
		}
		if (GameCore.Version.AlwaysAcceptReleaseBuilds && GameCore.Version.BuildType == GameCore.Version.VersionType.Release)
		{
			return;
		}
		if (GameCore.Version.ExtendedVersionCheckNeeded)
		{
			this.TargetCheckExactVersion(base.connectionToClient, GameCore.Version.VersionString);
			return;
		}
		this.TargetCheckVersion(base.connectionToClient, GameCore.Version.Major, GameCore.Version.Minor, GameCore.Version.Revision);
	}

	// Token: 0x06000778 RID: 1912 RVA: 0x0001C6DC File Offset: 0x0001A8DC
	[TargetRpc]
	private void TargetCheckVersion(NetworkConnection conn, byte major, byte minor, byte revision)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteByte(major);
		writer.WriteByte(minor);
		writer.WriteByte(revision);
		this.SendTargetRPCInternal(conn, "System.Void VersionCheck::TargetCheckVersion(Mirror.NetworkConnection,System.Byte,System.Byte,System.Byte)", 878887720, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x06000779 RID: 1913 RVA: 0x0001C72C File Offset: 0x0001A92C
	[TargetRpc]
	private void TargetCheckExactVersion(NetworkConnection conn, string version)
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		writer.WriteString(version);
		this.SendTargetRPCInternal(conn, "System.Void VersionCheck::TargetCheckExactVersion(Mirror.NetworkConnection,System.String)", 87233244, writer, 0);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x0600077B RID: 1915 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x0600077C RID: 1916 RVA: 0x00002E9A File Offset: 0x0000109A
	protected void UserCode_TargetCheckVersion__NetworkConnection__Byte__Byte__Byte(NetworkConnection conn, byte major, byte minor, byte revision)
	{
	}

	// Token: 0x0600077D RID: 1917 RVA: 0x0001C766 File Offset: 0x0001A966
	protected static void InvokeUserCode_TargetCheckVersion__NetworkConnection__Byte__Byte__Byte(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC TargetCheckVersion called on server.");
			return;
		}
		((VersionCheck)obj).UserCode_TargetCheckVersion__NetworkConnection__Byte__Byte__Byte(null, reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
	}

	// Token: 0x0600077E RID: 1918 RVA: 0x00002E9A File Offset: 0x0000109A
	protected void UserCode_TargetCheckExactVersion__NetworkConnection__String(NetworkConnection conn, string version)
	{
	}

	// Token: 0x0600077F RID: 1919 RVA: 0x0001C79C File Offset: 0x0001A99C
	protected static void InvokeUserCode_TargetCheckExactVersion__NetworkConnection__String(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("TargetRPC TargetCheckExactVersion called on server.");
			return;
		}
		((VersionCheck)obj).UserCode_TargetCheckExactVersion__NetworkConnection__String(null, reader.ReadString());
	}

	// Token: 0x06000780 RID: 1920 RVA: 0x0001C7C8 File Offset: 0x0001A9C8
	static VersionCheck()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(VersionCheck), "System.Void VersionCheck::TargetCheckVersion(Mirror.NetworkConnection,System.Byte,System.Byte,System.Byte)", new RemoteCallDelegate(VersionCheck.InvokeUserCode_TargetCheckVersion__NetworkConnection__Byte__Byte__Byte));
		RemoteProcedureCalls.RegisterRpc(typeof(VersionCheck), "System.Void VersionCheck::TargetCheckExactVersion(Mirror.NetworkConnection,System.String)", new RemoteCallDelegate(VersionCheck.InvokeUserCode_TargetCheckExactVersion__NetworkConnection__String));
	}
}
