using System;
using GameCore;
using UnityEngine;

namespace CentralAuth
{
	// Token: 0x02000AA9 RID: 2729
	public static class CentralAuthManager
	{
		// Token: 0x17000E00 RID: 3584
		// (get) Token: 0x06003D74 RID: 15732 RVA: 0x000C9CE1 File Offset: 0x000C7EE1
		// (set) Token: 0x06003D75 RID: 15733 RVA: 0x000C9CE8 File Offset: 0x000C7EE8
		public static DistributionPlatform Platform { get; private set; }

		// Token: 0x06003D76 RID: 15734 RVA: 0x000C9CF0 File Offset: 0x000C7EF0
		internal static void InitAuth()
		{
			if (CentralAuthManager._initialized)
			{
				return;
			}
			CentralAuthManager._initialized = true;
			CentralServer.Init();
			CentralAuthManager.Platform = DistributionPlatform.Dedicated;
			GameCore.Console.AddLog("Running as headless dedicated server. Skipping distribution platform detection.", new Color32(0, byte.MaxValue, 0, byte.MaxValue), false, ConsoleLogType.Log);
		}

		// Token: 0x04002CFC RID: 11516
		private static bool _initialized;

		// Token: 0x04002CFD RID: 11517
		private static bool _authDebugEnabled;

		// Token: 0x04002CFE RID: 11518
		public const int TokenVersion = 2;
	}
}
