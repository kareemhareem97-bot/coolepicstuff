using System;
using Mirror;
using ServerOutput;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009C0 RID: 2496
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class RestartNextRoundCommand : ICommand
	{
		// Token: 0x17000B53 RID: 2899
		// (get) Token: 0x0600386B RID: 14443 RVA: 0x000B753B File Offset: 0x000B573B
		public string Command { get; } = "restartnextround";

		// Token: 0x17000B54 RID: 2900
		// (get) Token: 0x0600386C RID: 14444 RVA: 0x000B7543 File Offset: 0x000B5743
		public string[] Aliases { get; } = new string[]
		{
			"rnr"
		};

		// Token: 0x17000B55 RID: 2901
		// (get) Token: 0x0600386D RID: 14445 RVA: 0x000B754B File Offset: 0x000B574B
		public string Description { get; } = "Restarts the server after the current round is finished.";

		// Token: 0x0600386E RID: 14446 RVA: 0x000B7554 File Offset: 0x000B5754
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
			if (ServerStatic.StopNextRound == ServerStatic.NextRoundAction.Restart)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " canceled server restart after the round end.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
				ServerStatic.StopNextRound = ServerStatic.NextRoundAction.DoNothing;
				ServerConsole.AddOutputEntry(default(ExitActionResetEntry));
				response = "Server WON'T restart after next round.";
			}
			else
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " scheduled server restart after the round end.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
				ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
				ServerConsole.AddOutputEntry(default(ExitActionRestartEntry));
				response = "Server WILL restart after next round.";
			}
			return true;
		}
	}
}
