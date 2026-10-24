using System;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009D0 RID: 2512
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class UnbanCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000B7F RID: 2943
		// (get) Token: 0x060038BB RID: 14523 RVA: 0x000B8B4D File Offset: 0x000B6D4D
		public string Command { get; } = "unban";

		// Token: 0x17000B80 RID: 2944
		// (get) Token: 0x060038BC RID: 14524 RVA: 0x000B8B55 File Offset: 0x000B6D55
		public string[] Aliases { get; } = new string[]
		{
			"pardon"
		};

		// Token: 0x17000B81 RID: 2945
		// (get) Token: 0x060038BD RID: 14525 RVA: 0x000B8B5D File Offset: 0x000B6D5D
		public string Description { get; } = "Unbans the specified AuthID or IP Address.";

		// Token: 0x17000B82 RID: 2946
		// (get) Token: 0x060038BE RID: 14526 RVA: 0x000B8B65 File Offset: 0x000B6D65
		public string[] Usage { get; } = new string[]
		{
			"id/ip",
			"AuthID/IP Address"
		};

		// Token: 0x060038BF RID: 14527 RVA: 0x000B8B70 File Offset: 0x000B6D70
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.LongTermBanning, out response))
			{
				return false;
			}
			if (arguments.Count < 2)
			{
				response = "To execute this command provide at least 2 arguments!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			string a = arguments.At(0);
			if (a == "id" || a == "playerid" || a == "player")
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " unbanned player with id " + arguments.At(1) + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
				BanHandler.RemoveBan(arguments.At(1), BanHandler.BanType.UserId, false);
				response = "Done!";
				return true;
			}
			if (!(a == "ip") && !(a == "address"))
			{
				response = string.Format("Incorrect syntax! Usage: unban{0}", this.Usage);
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " unbanned IP address " + arguments.At(1) + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			BanHandler.RemoveBan(arguments.At(1), BanHandler.BanType.IP, false);
			response = "Done!";
			return true;
		}
	}
}
