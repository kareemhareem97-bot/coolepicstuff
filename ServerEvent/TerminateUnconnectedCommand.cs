using System;
using GameCore;
using Mirror;

namespace CommandSystem.Commands.RemoteAdmin.ServerEvent
{
	// Token: 0x02000A41 RID: 2625
	[CommandHandler(typeof(ServerEventCommand))]
	public class TerminateUnconnectedCommand : ICommand
	{
		// Token: 0x17000CE2 RID: 3298
		// (get) Token: 0x06003B50 RID: 15184 RVA: 0x000C3677 File Offset: 0x000C1877
		public string Command { get; } = "TERMINATE_UNCONN";

		// Token: 0x17000CE3 RID: 3299
		// (get) Token: 0x06003B51 RID: 15185 RVA: 0x000C367F File Offset: 0x000C187F
		public string[] Aliases { get; }

		// Token: 0x17000CE4 RID: 3300
		// (get) Token: 0x06003B52 RID: 15186 RVA: 0x000C3687 File Offset: 0x000C1887
		public string Description { get; } = "Terminates unconnected players.";

		// Token: 0x06003B53 RID: 15187 RVA: 0x000C3690 File Offset: 0x000C1890
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RoundEvents, out response))
			{
				return false;
			}
			foreach (NetworkConnectionToClient networkConnectionToClient in NetworkServer.connections.Values)
			{
				if (GameCore.Console.FindConnectedRoot(networkConnectionToClient) == null)
				{
					networkConnectionToClient.Disconnect();
				}
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " terminated unconnected players.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			response = "Terminated unconnected players.";
			return true;
		}
	}
}
