using System;
using Mirror;
using Mirror.LiteNetLib4Mirror;

// Token: 0x0200019D RID: 413
public class PlayerIpOverride : NetworkBehaviour
{
	// Token: 0x06000839 RID: 2105 RVA: 0x00020E70 File Offset: 0x0001F070
	private void Start()
	{
		if (CustomLiteNetLib4MirrorTransport.IpPassthroughEnabled && NetworkServer.active && !base.isLocalPlayer)
		{
			NetworkConnectionToClient connectionToClient = base.connectionToClient;
			if (connectionToClient != null)
			{
				try
				{
					int id = LiteNetLib4MirrorServer.Peers[connectionToClient.connectionId].Id;
					if (CustomLiteNetLib4MirrorTransport.RealIpAddresses.ContainsKey(id))
					{
						connectionToClient.IpOverride = CustomLiteNetLib4MirrorTransport.RealIpAddresses[id];
					}
				}
				catch (Exception ex)
				{
					ServerConsole.AddLog("Error during IP passthrough processing: " + ex.Message, ConsoleColor.Gray, false);
				}
				return;
			}
		}
	}

	// Token: 0x0600083B RID: 2107 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}
}
