using System;
using Mirror;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009B8 RID: 2488
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class NextRoundCommand : ICommand
	{
		// Token: 0x17000B3B RID: 2875
		// (get) Token: 0x06003840 RID: 14400 RVA: 0x000B6DBC File Offset: 0x000B4FBC
		public string Command { get; } = "nextround";

		// Token: 0x17000B3C RID: 2876
		// (get) Token: 0x06003841 RID: 14401 RVA: 0x000B6DC4 File Offset: 0x000B4FC4
		public string[] Aliases { get; } = new string[]
		{
			"nr"
		};

		// Token: 0x17000B3D RID: 2877
		// (get) Token: 0x06003842 RID: 14402 RVA: 0x000B6DCC File Offset: 0x000B4FCC
		public string Description { get; } = "Manages server behavior when the current round ends.";

		// Token: 0x06003843 RID: 14403 RVA: 0x000B6DD4 File Offset: 0x000B4FD4
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
			bool flag = ServerStatic.ShutdownRedirectPort > 0;
			ServerStatic.NextRoundAction stopNextRound = ServerStatic.StopNextRound;
			string text;
			if (stopNextRound != ServerStatic.NextRoundAction.Restart)
			{
				if (stopNextRound != ServerStatic.NextRoundAction.Shutdown)
				{
					text = "Round will normally restart.";
				}
				else if (flag)
				{
					text = string.Format("Server will SHUTDOWN and all players will be redirected to port {0}.", ServerStatic.ShutdownRedirectPort);
				}
				else
				{
					text = "Server will SHUTDOWN.";
				}
			}
			else
			{
				text = "Server will RESTART.";
			}
			response = text;
			return true;
		}
	}
}
