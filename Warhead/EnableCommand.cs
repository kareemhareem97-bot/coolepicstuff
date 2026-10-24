using System;

namespace CommandSystem.Commands.RemoteAdmin.Warhead
{
	// Token: 0x02000A2A RID: 2602
	[CommandHandler(typeof(WarheadCommand))]
	public class EnableCommand : ICommand
	{
		// Token: 0x17000CAE RID: 3246
		// (get) Token: 0x06003ADE RID: 15070 RVA: 0x000C2415 File Offset: 0x000C0615
		public string Command { get; } = "enable";

		// Token: 0x17000CAF RID: 3247
		// (get) Token: 0x06003ADF RID: 15071 RVA: 0x000C241D File Offset: 0x000C061D
		public string[] Aliases { get; } = new string[]
		{
			"e"
		};

		// Token: 0x17000CB0 RID: 3248
		// (get) Token: 0x06003AE0 RID: 15072 RVA: 0x000C2425 File Offset: 0x000C0625
		public string Description { get; } = "Enables the alpha warhead.";

		// Token: 0x06003AE1 RID: 15073 RVA: 0x000C242D File Offset: 0x000C062D
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " enabled the warhead.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			AlphaWarheadOutsitePanel.nukeside.Networkenabled = true;
			response = "Warhead has been enabled.";
			return true;
		}
	}
}
