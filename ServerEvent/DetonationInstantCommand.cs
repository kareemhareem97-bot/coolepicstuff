using System;

namespace CommandSystem.Commands.RemoteAdmin.ServerEvent
{
	// Token: 0x02000A3D RID: 2621
	[CommandHandler(typeof(ServerEventCommand))]
	public class DetonationInstantCommand : ICommand
	{
		// Token: 0x17000CD5 RID: 3285
		// (get) Token: 0x06003B39 RID: 15161 RVA: 0x000C3405 File Offset: 0x000C1605
		public string Command { get; } = "DETONATION_INSTANT";

		// Token: 0x17000CD6 RID: 3286
		// (get) Token: 0x06003B3A RID: 15162 RVA: 0x000C340D File Offset: 0x000C160D
		public string[] Aliases { get; }

		// Token: 0x17000CD7 RID: 3287
		// (get) Token: 0x06003B3B RID: 15163 RVA: 0x000C3415 File Offset: 0x000C1615
		public string Description { get; } = "Instantly detonates the alpha warhead.";

		// Token: 0x06003B3C RID: 15164 RVA: 0x000C3420 File Offset: 0x000C1620
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			AlphaWarheadController.Singleton.ForceTime(0f);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " instantly detonated the warhead.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			response = "Warhead will detonate instantly.";
			return true;
		}
	}
}
