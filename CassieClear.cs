using System;
using Cassie;

namespace CommandSystem.Commands
{
	// Token: 0x020009A7 RID: 2471
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class CassieClear : ICommand
	{
		// Token: 0x17000B06 RID: 2822
		// (get) Token: 0x060037E6 RID: 14310 RVA: 0x000B5CAC File Offset: 0x000B3EAC
		public string Command { get; } = "clearcassie";

		// Token: 0x17000B07 RID: 2823
		// (get) Token: 0x060037E7 RID: 14311 RVA: 0x000B5CB4 File Offset: 0x000B3EB4
		public string[] Aliases { get; } = new string[]
		{
			"cassieclear"
		};

		// Token: 0x17000B08 RID: 2824
		// (get) Token: 0x060037E8 RID: 14312 RVA: 0x000B5CBC File Offset: 0x000B3EBC
		public string Description { get; } = "Clears cassie message queue.";

		// Token: 0x060037E9 RID: 14313 RVA: 0x000B5CC4 File Offset: 0x000B3EC4
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.Announcer, out response))
			{
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " ran the clearcassie command", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			CassieAnnouncementDispatcher.ClearAll();
			response = "Cleared Cassie word queue!";
			return true;
		}
	}
}
