using System;
using Cassie;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009D5 RID: 2517
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class CassieSilentCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000B93 RID: 2963
		// (get) Token: 0x060038DA RID: 14554 RVA: 0x000B94F9 File Offset: 0x000B76F9
		public string Command { get; } = "cassie_silent";

		// Token: 0x17000B94 RID: 2964
		// (get) Token: 0x060038DB RID: 14555 RVA: 0x000B9501 File Offset: 0x000B7701
		public string[] Aliases { get; } = new string[]
		{
			"cassie_silentnoise",
			"cassie_sn",
			"cassie_sl"
		};

		// Token: 0x17000B95 RID: 2965
		// (get) Token: 0x060038DC RID: 14556 RVA: 0x000B9509 File Offset: 0x000B7709
		public string Description { get; } = "Sends a silent (no preceding tone) announcement over the CASSIE system.";

		// Token: 0x17000B96 RID: 2966
		// (get) Token: 0x060038DD RID: 14557 RVA: 0x000B9511 File Offset: 0x000B7711
		public string[] Usage { get; } = new string[]
		{
			"message"
		};

		// Token: 0x060038DE RID: 14558 RVA: 0x000B951C File Offset: 0x000B771C
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
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " started a silent cassie announcement: " + text + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			new CassieAnnouncement(new CassieTtsPayload(text, true, false), 0f, 0f).AddToQueue();
			response = "Silent announcement sent!";
			return true;
		}
	}
}
