using System;
using System.Runtime.CompilerServices;
using Mirror;
using UnityEngine;

namespace DrawableLine
{
	// Token: 0x0200098A RID: 2442
	public static class DrawableLineMessageHandler
	{
		// Token: 0x06003787 RID: 14215 RVA: 0x000B436F File Offset: 0x000B256F
		public static void Serialize(this NetworkWriter writer, DrawableLineMessage value)
		{
			value.WriteMessage(writer);
		}

		// Token: 0x06003788 RID: 14216 RVA: 0x000B4379 File Offset: 0x000B2579
		public static DrawableLineMessage Deserialize(this NetworkReader reader)
		{
			return new DrawableLineMessage(reader);
		}

		// Token: 0x06003789 RID: 14217 RVA: 0x000B4381 File Offset: 0x000B2581
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			CustomNetworkManager.OnClientReady += delegate()
			{
				Action<DrawableLineMessage> handler;
				if ((handler = DrawableLineMessageHandler.<>O.<0>__OnMessageReceived) == null)
				{
					handler = (DrawableLineMessageHandler.<>O.<0>__OnMessageReceived = new Action<DrawableLineMessage>(DrawableLineMessageHandler.OnMessageReceived));
				}
				NetworkClient.ReplaceHandler<DrawableLineMessage>(handler, true);
			};
		}

		// Token: 0x0600378A RID: 14218 RVA: 0x000B43A7 File Offset: 0x000B25A7
		private static void OnMessageReceived(DrawableLineMessage msg)
		{
			msg.GenerateLine();
		}

		// Token: 0x0200098B RID: 2443
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040029AB RID: 10667
			public static Action<DrawableLineMessage> <0>__OnMessageReceived;
		}
	}
}
