using System;
using System.Runtime.CompilerServices;
using Mirror;
using Query;
using UnityEngine;

// Token: 0x0200019E RID: 414
public class ServerShutdown : MonoBehaviour
{
	// Token: 0x0600083C RID: 2108 RVA: 0x00020F00 File Offset: 0x0001F100
	[RuntimeInitializeOnLoadMethod]
	private static void InitOnLoad()
	{
		CustomNetworkManager.OnClientReady += delegate()
		{
			Action<ServerShutdown.ServerShutdownMessage> handler;
			if ((handler = ServerShutdown.<>O.<0>__HandleServerShutdown) == null)
			{
				handler = (ServerShutdown.<>O.<0>__HandleServerShutdown = new Action<ServerShutdown.ServerShutdownMessage>(ServerShutdown.HandleServerShutdown));
			}
			NetworkClient.ReplaceHandler<ServerShutdown.ServerShutdownMessage>(handler, true);
		};
	}

	// Token: 0x17000148 RID: 328
	// (get) Token: 0x0600083D RID: 2109 RVA: 0x00020F26 File Offset: 0x0001F126
	// (set) Token: 0x0600083E RID: 2110 RVA: 0x00020F2D File Offset: 0x0001F12D
	public static ServerShutdown.ServerShutdownState ShutdownState { get; set; }

	// Token: 0x0600083F RID: 2111 RVA: 0x00020F38 File Offset: 0x0001F138
	private void Update()
	{
		switch (ServerShutdown.ShutdownState)
		{
		case ServerShutdown.ServerShutdownState.NotInitiated:
		case ServerShutdown.ServerShutdownState.Complete:
			return;
		case ServerShutdown.ServerShutdownState.BroadcastingShutdown:
			if (ServerShutdown._c > 400f)
			{
				ServerConsole.AddLog("Shutting down the server...", ConsoleColor.DarkCyan, false);
				ServerShutdown.ShutdownState = ServerShutdown.ServerShutdownState.ShuttingDown;
				ServerShutdown._c = 0f;
				NetworkServer.Shutdown();
				return;
			}
			break;
		case ServerShutdown.ServerShutdownState.ShuttingDown:
			if (ServerShutdown._c > 1000f)
			{
				ServerConsole.AddLog("Server shutdown completed.", ConsoleColor.DarkCyan, false);
				ServerShutdown.ShutdownState = ServerShutdown.ServerShutdownState.Complete;
				return;
			}
			break;
		default:
			return;
		}
		ServerShutdown._c += Time.unscaledDeltaTime;
	}

	// Token: 0x06000840 RID: 2112 RVA: 0x00020FC0 File Offset: 0x0001F1C0
	internal static void Shutdown(bool noBroadcast = false)
	{
		if (ServerShutdown.ShutdownState != ServerShutdown.ServerShutdownState.NotInitiated)
		{
			return;
		}
		if (!NetworkServer.active)
		{
			ServerShutdown.ShutdownState = ServerShutdown.ServerShutdownState.Complete;
			return;
		}
		ServerConsole.AddLog("Server shutdown initiated.", ConsoleColor.DarkCyan, false);
		ServerShutdown.ShutdownState = ServerShutdown.ServerShutdownState.BroadcastingShutdown;
		IdleMode.SetIdleMode(false);
		CustomLiteNetLib4MirrorTransport.DelayConnections = true;
		QueryServer queryServer = CustomNetworkManager.QueryServer;
		if (queryServer != null)
		{
			queryServer.StopServer();
		}
		if (noBroadcast)
		{
			return;
		}
		if (noBroadcast)
		{
			return;
		}
		ServerConsole.AddLog("Broadcasting server shutdown to all connected players...", ConsoleColor.DarkCyan, false);
		NetworkServer.SendToAll<ServerShutdown.ServerShutdownMessage>(default(ServerShutdown.ServerShutdownMessage), 4, true);
	}

	// Token: 0x06000841 RID: 2113 RVA: 0x00002E9A File Offset: 0x0000109A
	private static void HandleServerShutdown(ServerShutdown.ServerShutdownMessage ssm)
	{
	}

	// Token: 0x04000784 RID: 1924
	private static float _c;

	// Token: 0x0200019F RID: 415
	public enum ServerShutdownState : byte
	{
		// Token: 0x04000786 RID: 1926
		NotInitiated,
		// Token: 0x04000787 RID: 1927
		BroadcastingShutdown,
		// Token: 0x04000788 RID: 1928
		ShuttingDown,
		// Token: 0x04000789 RID: 1929
		Complete
	}

	// Token: 0x020001A0 RID: 416
	private struct ServerShutdownMessage : NetworkMessage
	{
	}

	// Token: 0x020001A1 RID: 417
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x0400078A RID: 1930
		public static Action<ServerShutdown.ServerShutdownMessage> <0>__HandleServerShutdown;
	}
}
