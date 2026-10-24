using System;
using Mirror;
using RoundRestarting;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009C4 RID: 2500
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class SoftRestartCommand : ICommand
	{
		// Token: 0x17000B5C RID: 2908
		// (get) Token: 0x0600387E RID: 14462 RVA: 0x000B78BD File Offset: 0x000B5ABD
		public string Command { get; } = "softrestart";

		// Token: 0x17000B5D RID: 2909
		// (get) Token: 0x0600387F RID: 14463 RVA: 0x000B78C5 File Offset: 0x000B5AC5
		public string[] Aliases { get; } = new string[]
		{
			"srestart",
			"sr"
		};

		// Token: 0x17000B5E RID: 2910
		// (get) Token: 0x06003880 RID: 14464 RVA: 0x000B78CD File Offset: 0x000B5ACD
		public string Description { get; } = "Restarts the server, but tells all the players to reconnect after the restart.";

		// Token: 0x06003881 RID: 14465 RVA: 0x000B78D8 File Offset: 0x000B5AD8
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!NetworkServer.active)
			{
				response = "This command can only be used on a server.";
				return false;
			}
			CommandSender commandSender = sender as CommandSender;
			if (commandSender != null && !commandSender.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
			{
				return false;
			}
			if (!ServerStatic.IsDedicated)
			{
				response = "This command can be only executed on a dedicated servers.";
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " performed a soft server restart.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
			RoundRestart.ChangeLevel(true);
			response = "Server will softly restart in a couple of seconds.";
			return true;
		}
	}
}
