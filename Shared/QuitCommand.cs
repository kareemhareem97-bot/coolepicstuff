using System;
using Mirror;
using Query;
using RemoteAdmin;
using ServerOutput;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009BC RID: 2492
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class QuitCommand : ICommand
	{
		// Token: 0x17000B47 RID: 2887
		// (get) Token: 0x06003854 RID: 14420 RVA: 0x000B7216 File Offset: 0x000B5416
		public string Command { get; } = "quit";

		// Token: 0x17000B48 RID: 2888
		// (get) Token: 0x06003855 RID: 14421 RVA: 0x000B721E File Offset: 0x000B541E
		public string[] Aliases { get; } = new string[]
		{
			"exit",
			"stop"
		};

		// Token: 0x17000B49 RID: 2889
		// (get) Token: 0x06003856 RID: 14422 RVA: 0x000B7226 File Offset: 0x000B5426
		public string Description { get; } = "Quit the game.";

		// Token: 0x06003857 RID: 14423 RVA: 0x000B7230 File Offset: 0x000B5430
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if ((sender is QueryCommandSender || sender is PlayerCommandSender) && (arguments.Count == 0 || !arguments.At(0).Equals("-y", StringComparison.OrdinalIgnoreCase)))
			{
				response = "[WARNING] This command stops the entire server. It doesn't disconnect you from the server.\nTo confirm server shutdown add \"-y\" argument.";
				if (sender is QueryCommandSender)
				{
					response += "\nTo disconnect from query use \"disconnect\" command.";
				}
				return false;
			}
			if (!sender.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
			{
				return false;
			}
			if (NetworkServer.active)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " stopped the server.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			}
			IdleMode.SetIdleMode(false);
			ServerConsole.AddOutputEntry(default(ExitActionShutdownEntry));
			Shutdown.Quit(true, false);
			response = "<size=50>GOODBYE!</size>";
			return true;
		}
	}
}
