using System;
using System.Collections.Generic;
using InventorySystem.Searching;
using Mirror;
using UnityEngine;

namespace Hints
{
	// Token: 0x02000806 RID: 2054
	[RequireComponent(typeof(SearchCoordinator))]
	public class HintDisplay : NetworkBehaviour
	{
		// Token: 0x06002F8C RID: 12172 RVA: 0x000A02EC File Offset: 0x0009E4EC
		public void Show(Hint hint)
		{
			if (hint == null)
			{
				throw new ArgumentNullException("hint");
			}
			if (base.isLocalPlayer)
			{
				throw new InvalidOperationException("Cannot display a hint to the local player (headless server).");
			}
			if (NetworkServer.active)
			{
				NetworkConnection connectionToClient = base.netIdentity.connectionToClient;
				if (!HintDisplay.SuppressedReceivers.Contains(connectionToClient))
				{
					connectionToClient.Send<HintMessage>(new HintMessage(hint), 0);
				}
			}
		}

		// Token: 0x06002F8F RID: 12175 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Weaved()
		{
			return true;
		}

		// Token: 0x0400253A RID: 9530
		public static readonly HashSet<NetworkConnection> SuppressedReceivers = new HashSet<NetworkConnection>();
	}
}
