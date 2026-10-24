using System;
using Mirror;
using Mirror.LiteNetLib4Mirror;
using UnityEngine;

// Token: 0x020000D2 RID: 210
public class NetManagerValueSetter : MonoBehaviour
{
	// Token: 0x0600040D RID: 1037 RVA: 0x0000FDB5 File Offset: 0x0000DFB5
	private void Start()
	{
		this._singleton = NetworkManager.singleton.GetComponent<CustomNetworkManager>();
	}

	// Token: 0x0600040E RID: 1038 RVA: 0x0000FDC7 File Offset: 0x0000DFC7
	public void ChangeIP(string ip)
	{
		this._singleton.networkAddress = ip;
		CustomNetworkManager.ConnectionIp = ip;
	}

	// Token: 0x0600040F RID: 1039 RVA: 0x0000FDDB File Offset: 0x0000DFDB
	public void ChangePort(ushort port)
	{
		LiteNetLib4MirrorTransport.Singleton.port = port;
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x0000FDE8 File Offset: 0x0000DFE8
	public void JoinGame()
	{
		this._singleton.StartClient();
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x0000FDF5 File Offset: 0x0000DFF5
	public void HostGame()
	{
		this._singleton.StartHost();
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x0000FE02 File Offset: 0x0000E002
	public void Disconnect()
	{
		this._singleton.StopHost();
	}

	// Token: 0x040003A4 RID: 932
	private CustomNetworkManager _singleton;
}
