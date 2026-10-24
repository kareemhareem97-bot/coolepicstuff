using System;
using System.Runtime.CompilerServices;
using Decals;
using Mirror;
using UnityEngine;

namespace CommandSystem.Commands.RemoteAdmin.Cleanup
{
	// Token: 0x02000A7D RID: 2685
	public static class DecalCleanupMessageExtensions
	{
		// Token: 0x06003CA1 RID: 15521 RVA: 0x000C79EB File Offset: 0x000C5BEB
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			CustomNetworkManager.OnClientReady += delegate()
			{
				Action<DecalCleanupMessage> handler;
				if ((handler = DecalCleanupMessageExtensions.<>O.<0>__ClientMessageReceived) == null)
				{
					handler = (DecalCleanupMessageExtensions.<>O.<0>__ClientMessageReceived = new Action<DecalCleanupMessage>(DecalCleanupMessageExtensions.ClientMessageReceived));
				}
				NetworkClient.ReplaceHandler<DecalCleanupMessage>(handler, true);
			};
		}

		// Token: 0x06003CA2 RID: 15522 RVA: 0x000C7A11 File Offset: 0x000C5C11
		private static void ClientMessageReceived(DecalCleanupMessage message)
		{
			DecalPoolManager.ClientClear(message.DecalPoolType, message.Amount);
		}

		// Token: 0x06003CA3 RID: 15523 RVA: 0x000C7A24 File Offset: 0x000C5C24
		public static void WriteDecalCleanupMessage(this NetworkWriter writer, DecalCleanupMessage msg)
		{
			msg.Serialize(writer);
		}

		// Token: 0x06003CA4 RID: 15524 RVA: 0x000C7A2E File Offset: 0x000C5C2E
		public static DecalCleanupMessage ReadRadioStatusMessage(this NetworkReader reader)
		{
			return new DecalCleanupMessage(reader);
		}

		// Token: 0x02000A7E RID: 2686
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002C72 RID: 11378
			public static Action<DecalCleanupMessage> <0>__ClientMessageReceived;
		}
	}
}
