using System;
using Mirror;
using RoundRestarting;

namespace CommandSystem.Commands.Console
{
	// Token: 0x02000AA5 RID: 2725
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class RoundRestartCommand : ICommand
	{
		// Token: 0x17000DFB RID: 3579
		// (get) Token: 0x06003D66 RID: 15718 RVA: 0x000C9A61 File Offset: 0x000C7C61
		public string Command { get; } = "roundrestart";

		// Token: 0x17000DFC RID: 3580
		// (get) Token: 0x06003D67 RID: 15719 RVA: 0x000C9A69 File Offset: 0x000C7C69
		public string[] Aliases { get; } = new string[]
		{
			"rr",
			"restart"
		};

		// Token: 0x17000DFD RID: 3581
		// (get) Token: 0x06003D68 RID: 15720 RVA: 0x000C9A71 File Offset: 0x000C7C71
		public string Description { get; } = "Restarts the round.";

		// Token: 0x06003D69 RID: 15721 RVA: 0x000C9A7C File Offset: 0x000C7C7C
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!NetworkServer.active)
			{
				response = "You are not connected to a local server.";
				return false;
			}
			if (!sender.CheckPermission(PlayerPermissions.RoundEvents, out response))
			{
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " forced round restart.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			RoundRestart.InitiateRoundRestart();
			response = "Round restart forced.";
			return true;
		}
	}
}
