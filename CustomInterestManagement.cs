using System;
using System.Collections.Generic;
using Mirror;

// Token: 0x02000182 RID: 386
public class CustomInterestManagement : InterestManagement
{
	// Token: 0x060007B1 RID: 1969 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool OnCheckObserver(NetworkIdentity identity, NetworkConnectionToClient newObserver)
	{
		return true;
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x0001D554 File Offset: 0x0001B754
	public override void OnRebuildObservers(NetworkIdentity identity, HashSet<NetworkConnectionToClient> newObservers)
	{
		if (identity.visible == Visibility.ForceHidden)
		{
			return;
		}
		foreach (NetworkConnectionToClient networkConnectionToClient in NetworkServer.connections.Values)
		{
			if (networkConnectionToClient.isReady)
			{
				newObservers.Add(networkConnectionToClient);
			}
		}
		if (NetworkServer.localConnection != null && NetworkServer.localConnection.isReady)
		{
			newObservers.Add(NetworkServer.localConnection);
		}
	}
}
