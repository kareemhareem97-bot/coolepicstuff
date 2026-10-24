using System;
using PlayerRoles.Voice;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009FD RID: 2557
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class IntercomTimeoutCommand : ICommand
	{
		// Token: 0x17000C14 RID: 3092
		// (get) Token: 0x060039C1 RID: 14785 RVA: 0x000BDEFD File Offset: 0x000BC0FD
		public string Command { get; } = "intercom-timeout";

		// Token: 0x17000C15 RID: 3093
		// (get) Token: 0x060039C2 RID: 14786 RVA: 0x000BDF05 File Offset: 0x000BC105
		public string[] Aliases { get; } = new string[]
		{
			"icomstop",
			"icomtimeout",
			"it"
		};

		// Token: 0x17000C16 RID: 3094
		// (get) Token: 0x060039C3 RID: 14787 RVA: 0x000BDF0D File Offset: 0x000BC10D
		public string Description { get; } = "Times out the intercom system if it's currently in use.";

		// Token: 0x060039C4 RID: 14788 RVA: 0x000BDF18 File Offset: 0x000BC118
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(new PlayerPermissions[]
			{
				PlayerPermissions.KickingAndShortTermBanning,
				PlayerPermissions.BanningUpToDay,
				PlayerPermissions.LongTermBanning,
				PlayerPermissions.RoundEvents,
				PlayerPermissions.FacilityManagement,
				PlayerPermissions.PlayersManagement
			}, out response))
			{
				return false;
			}
			Intercom.State = IntercomState.Cooldown;
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " timed out the intercom speaker.", ServerLogs.ServerLogType.RemoteAdminActivity_Misc, false);
			response = "Done! Intercom speaker timed out.";
			return true;
		}
	}
}
