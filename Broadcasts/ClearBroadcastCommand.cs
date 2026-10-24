using System;

namespace CommandSystem.Commands.RemoteAdmin.Broadcasts
{
	// Token: 0x02000A83 RID: 2691
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class ClearBroadcastCommand : ICommand
	{
		// Token: 0x17000DA4 RID: 3492
		// (get) Token: 0x06003CBD RID: 15549 RVA: 0x000C7D63 File Offset: 0x000C5F63
		public string Command { get; } = "clearbroadcasts";

		// Token: 0x17000DA5 RID: 3493
		// (get) Token: 0x06003CBE RID: 15550 RVA: 0x000C7D6B File Offset: 0x000C5F6B
		public string[] Aliases { get; } = new string[]
		{
			"cl",
			"clearbc",
			"bcclear",
			"clearalert",
			"alertclear"
		};

		// Token: 0x17000DA6 RID: 3494
		// (get) Token: 0x06003CBF RID: 15551 RVA: 0x000C7D73 File Offset: 0x000C5F73
		public string Description { get; } = "Clears all active administrative broadcasts.";

		// Token: 0x06003CC0 RID: 15552 RVA: 0x000C7D7B File Offset: 0x000C5F7B
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.Broadcasting, out response))
			{
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " cleared all broadcasts.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			Broadcast.Singleton.RpcClearElements();
			response = "All broadcasts cleared.";
			return true;
		}
	}
}
