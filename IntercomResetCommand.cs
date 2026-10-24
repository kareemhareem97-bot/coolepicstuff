using System;
using PlayerRoles.Voice;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009FB RID: 2555
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class IntercomResetCommand : ICommand
	{
		// Token: 0x17000C0D RID: 3085
		// (get) Token: 0x060039B6 RID: 14774 RVA: 0x000BDADC File Offset: 0x000BBCDC
		public string Command { get; } = "intercom-reset";

		// Token: 0x17000C0E RID: 3086
		// (get) Token: 0x060039B7 RID: 14775 RVA: 0x000BDAE4 File Offset: 0x000BBCE4
		public string[] Aliases { get; } = new string[]
		{
			"icomreset",
			"ir"
		};

		// Token: 0x17000C0F RID: 3087
		// (get) Token: 0x060039B8 RID: 14776 RVA: 0x000BDAEC File Offset: 0x000BBCEC
		public string Description { get; } = "Resets the timer on the intercom to ready-to-speak.";

		// Token: 0x060039B9 RID: 14777 RVA: 0x000BDAF4 File Offset: 0x000BBCF4
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(new PlayerPermissions[]
			{
				PlayerPermissions.RoundEvents,
				PlayerPermissions.FacilityManagement,
				PlayerPermissions.PlayersManagement
			}, out response))
			{
				return false;
			}
			if (Intercom.State != IntercomState.Cooldown)
			{
				response = "Intercom is not currently on cooldown. Current state: " + Intercom.State.ToString();
				return false;
			}
			Intercom.State = IntercomState.Ready;
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " reset the intercom cooldown.", ServerLogs.ServerLogType.RemoteAdminActivity_Misc, false);
			response = "Done! Intercom cooldown reset.";
			return true;
		}
	}
}
