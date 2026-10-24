using System;

namespace CommandSystem.Commands.RemoteAdmin.Warhead
{
	// Token: 0x02000A28 RID: 2600
	[CommandHandler(typeof(WarheadCommand))]
	public class DetonateCommand : ICommand
	{
		// Token: 0x17000CA8 RID: 3240
		// (get) Token: 0x06003AD4 RID: 15060 RVA: 0x000C22FB File Offset: 0x000C04FB
		public string Command { get; } = "detonate";

		// Token: 0x17000CA9 RID: 3241
		// (get) Token: 0x06003AD5 RID: 15061 RVA: 0x000C2303 File Offset: 0x000C0503
		public string[] Aliases { get; } = new string[]
		{
			"det",
			"start"
		};

		// Token: 0x17000CAA RID: 3242
		// (get) Token: 0x06003AD6 RID: 15062 RVA: 0x000C230B File Offset: 0x000C050B
		public string Description { get; } = "Starts the alpha warhead detonation sequence.";

		// Token: 0x06003AD7 RID: 15063 RVA: 0x000C2313 File Offset: 0x000C0513
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " started warhead detonation.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			AlphaWarheadController.Singleton.StartDetonation(false, false, null);
			response = "Detonation sequence started.";
			return true;
		}
	}
}
