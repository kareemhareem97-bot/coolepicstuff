using System;

namespace CommandSystem.Commands.RemoteAdmin.Warhead
{
	// Token: 0x02000A29 RID: 2601
	[CommandHandler(typeof(WarheadCommand))]
	public class DisableCommand : ICommand
	{
		// Token: 0x17000CAB RID: 3243
		// (get) Token: 0x06003AD9 RID: 15065 RVA: 0x000C238D File Offset: 0x000C058D
		public string Command { get; } = "disable";

		// Token: 0x17000CAC RID: 3244
		// (get) Token: 0x06003ADA RID: 15066 RVA: 0x000C2395 File Offset: 0x000C0595
		public string[] Aliases { get; } = new string[]
		{
			"d"
		};

		// Token: 0x17000CAD RID: 3245
		// (get) Token: 0x06003ADB RID: 15067 RVA: 0x000C239D File Offset: 0x000C059D
		public string Description { get; } = "Disables the alpha warhead.";

		// Token: 0x06003ADC RID: 15068 RVA: 0x000C23A5 File Offset: 0x000C05A5
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " disabled the warhead.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			AlphaWarheadOutsitePanel.nukeside.Networkenabled = false;
			response = "Warhead has been disabled.";
			return true;
		}
	}
}
