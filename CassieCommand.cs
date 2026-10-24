using System;
using Cassie;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009D4 RID: 2516
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class CassieCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000B8F RID: 2959
		// (get) Token: 0x060038D4 RID: 14548 RVA: 0x000B9403 File Offset: 0x000B7603
		public string Command { get; } = "cassie";

		// Token: 0x17000B90 RID: 2960
		// (get) Token: 0x060038D5 RID: 14549 RVA: 0x000B940B File Offset: 0x000B760B
		public string[] Aliases { get; }

		// Token: 0x17000B91 RID: 2961
		// (get) Token: 0x060038D6 RID: 14550 RVA: 0x000B9413 File Offset: 0x000B7613
		public string Description { get; } = "Sends an announcement over the CASSIE system.";

		// Token: 0x17000B92 RID: 2962
		// (get) Token: 0x060038D7 RID: 14551 RVA: 0x000B941B File Offset: 0x000B761B
		public string[] Usage { get; } = new string[]
		{
			"message"
		};

		// Token: 0x060038D8 RID: 14552 RVA: 0x000B9424 File Offset: 0x000B7624
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.Announcer, out response))
			{
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			string text = RAUtils.FormatArguments(arguments, 0, null).ToUpper();
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " started a cassie announcement: " + text + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			new CassieAnnouncement(new CassieTtsPayload(text, true, true), 0f, 0f).AddToQueue();
			response = "Announcement sent!";
			return true;
		}
	}
}
