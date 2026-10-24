using System;
using Mirror;
using ServerOutput;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009C6 RID: 2502
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class StopNextRoundCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000B62 RID: 2914
		// (get) Token: 0x06003888 RID: 14472 RVA: 0x000B7C9E File Offset: 0x000B5E9E
		public string Command { get; } = "stopnextround";

		// Token: 0x17000B63 RID: 2915
		// (get) Token: 0x06003889 RID: 14473 RVA: 0x000B7CA6 File Offset: 0x000B5EA6
		public string[] Aliases { get; } = new string[]
		{
			"snr"
		};

		// Token: 0x17000B64 RID: 2916
		// (get) Token: 0x0600388A RID: 14474 RVA: 0x000B7CAE File Offset: 0x000B5EAE
		public string Description { get; } = "Stops the server after the next round.";

		// Token: 0x17000B65 RID: 2917
		// (get) Token: 0x0600388B RID: 14475 RVA: 0x000B7CB6 File Offset: 0x000B5EB6
		public string[] Usage { get; } = new string[]
		{
			"Port number to redirect players (Optional)"
		};

		// Token: 0x0600388C RID: 14476 RVA: 0x000B7CC0 File Offset: 0x000B5EC0
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			CommandSender commandSender = sender as CommandSender;
			if (commandSender != null && !commandSender.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
			{
				return false;
			}
			if (!NetworkServer.active)
			{
				response = "This command can only be used on a server.";
				return false;
			}
			if (!ServerStatic.IsDedicated)
			{
				response = "This command can be only executed on a dedicated servers.";
				return false;
			}
			if (arguments.Count > 0)
			{
				ushort num;
				if (!ushort.TryParse(arguments.At(0), out num))
				{
					response = "First argument, if set, must be a valid port number.";
					return false;
				}
				if (num == ServerStatic.ShutdownRedirectPort)
				{
					response = string.Format("Server is already set to redirect players to port {0}.", num);
					return true;
				}
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} set the server shutdown redirection port to {1}.", sender.LogName, num), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
				ServerStatic.ShutdownRedirectPort = num;
			}
			else if (ServerStatic.ShutdownRedirectPort != 0)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " cleared the server shutdown redirection port.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
				ServerStatic.ShutdownRedirectPort = 0;
			}
			if (ServerStatic.StopNextRound == ServerStatic.NextRoundAction.Shutdown && ServerStatic.ShutdownRedirectPort == 0)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " canceled server stop after the round end.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
				ServerStatic.StopNextRound = ServerStatic.NextRoundAction.DoNothing;
				ServerConsole.AddOutputEntry(default(ExitActionResetEntry));
				response = "Server WON'T stop after next round.";
			}
			else
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " scheduled server stop after the round end.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
				ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Shutdown;
				ServerConsole.AddOutputEntry(default(ExitActionShutdownEntry));
				response = "Server WILL stop after next round" + ((ServerStatic.ShutdownRedirectPort == 0) ? "" : string.Format(" and players will be redirected to port {0}", ServerStatic.ShutdownRedirectPort)) + ".";
			}
			return true;
		}
	}
}
