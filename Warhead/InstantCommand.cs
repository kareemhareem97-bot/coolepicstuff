using System;

namespace CommandSystem.Commands.RemoteAdmin.Warhead
{
	// Token: 0x02000A2B RID: 2603
	[CommandHandler(typeof(WarheadCommand))]
	public class InstantCommand : ICommand
	{
		// Token: 0x17000CB1 RID: 3249
		// (get) Token: 0x06003AE3 RID: 15075 RVA: 0x000C249D File Offset: 0x000C069D
		public string Command { get; } = "instant";

		// Token: 0x17000CB2 RID: 3250
		// (get) Token: 0x06003AE4 RID: 15076 RVA: 0x000C24A5 File Offset: 0x000C06A5
		public string[] Aliases { get; } = new string[]
		{
			"insdet",
			"inst",
			"i"
		};

		// Token: 0x17000CB3 RID: 3251
		// (get) Token: 0x06003AE5 RID: 15077 RVA: 0x000C24AD File Offset: 0x000C06AD
		public string Description { get; } = "Instantly detonates the alpha warhead.";

		// Token: 0x06003AE6 RID: 15078 RVA: 0x000C24B8 File Offset: 0x000C06B8
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " instantly detonated the warhead.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			AlphaWarheadController.Singleton.ForceTime(0f);
			response = "Instant detonation started.";
			return true;
		}
	}
}
