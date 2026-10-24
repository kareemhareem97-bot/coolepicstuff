using System;
using System.IO;
using System.Runtime.CompilerServices;
using Mirror;
using UnityEngine;

namespace CommandSystem.Commands.RemoteAdmin.Stripdown
{
	// Token: 0x02000A33 RID: 2611
	public static class StripdownNetworking
	{
		// Token: 0x17000CCB RID: 3275
		// (get) Token: 0x06003B13 RID: 15123 RVA: 0x000C2BC0 File Offset: 0x000C0DC0
		// (set) Token: 0x06003B14 RID: 15124 RVA: 0x000C2BC7 File Offset: 0x000C0DC7
		public static bool FileExportEnabled { get; set; }

		// Token: 0x06003B15 RID: 15125 RVA: 0x000C2BCF File Offset: 0x000C0DCF
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			CustomNetworkManager.OnClientReady += delegate()
			{
				Action<StripdownNetworking.StripdownResponse> handler;
				if ((handler = StripdownNetworking.<>O.<0>__ProcessMessage) == null)
				{
					handler = (StripdownNetworking.<>O.<0>__ProcessMessage = new Action<StripdownNetworking.StripdownResponse>(StripdownNetworking.ProcessMessage));
				}
				NetworkClient.ReplaceHandler<StripdownNetworking.StripdownResponse>(handler, true);
			};
		}

		// Token: 0x06003B16 RID: 15126 RVA: 0x000C2BF8 File Offset: 0x000C0DF8
		public static void ProcessMessage(StripdownNetworking.StripdownResponse file)
		{
			file.Lines.ForEach(delegate(string x)
			{
				Debug.Log(x);
			});
			if (!StripdownNetworking.FileExportEnabled)
			{
				return;
			}
			long num = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			if (num == StripdownNetworking._lastTime)
			{
				Debug.LogError("Stripdown file creation rate limited.");
				return;
			}
			File.WriteAllLines(string.Format("{0}/{1}.txt", Application.dataPath, num), file.Lines);
			StripdownNetworking._lastTime = num;
		}

		// Token: 0x04002B91 RID: 11153
		private static long _lastTime;

		// Token: 0x02000A34 RID: 2612
		public struct StripdownResponse : NetworkMessage
		{
			// Token: 0x04002B93 RID: 11155
			public string[] Lines;
		}

		// Token: 0x02000A35 RID: 2613
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002B94 RID: 11156
			public static Action<StripdownNetworking.StripdownResponse> <0>__ProcessMessage;
		}
	}
}
