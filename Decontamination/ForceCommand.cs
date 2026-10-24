using System;
using LightContainmentZoneDecontamination;

namespace CommandSystem.Commands.RemoteAdmin.Decontamination
{
	// Token: 0x02000A75 RID: 2677
	[CommandHandler(typeof(DecontaminationCommand))]
	public class ForceCommand : ICommand
	{
		// Token: 0x17000D85 RID: 3461
		// (get) Token: 0x06003C7A RID: 15482 RVA: 0x000C74AD File Offset: 0x000C56AD
		public string Command { get; } = "force";

		// Token: 0x17000D86 RID: 3462
		// (get) Token: 0x06003C7B RID: 15483 RVA: 0x000C74B5 File Offset: 0x000C56B5
		public string[] Aliases { get; } = new string[]
		{
			"f"
		};

		// Token: 0x17000D87 RID: 3463
		// (get) Token: 0x06003C7C RID: 15484 RVA: 0x000C74BD File Offset: 0x000C56BD
		public string Description { get; } = "Forces the LCZ decontamination to begin.";

		// Token: 0x06003C7D RID: 15485 RVA: 0x000C74C8 File Offset: 0x000C56C8
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RoundEvents, out response))
			{
				return false;
			}
			if (DecontaminationController.Singleton.DecontaminationOverride == DecontaminationController.DecontaminationStatus.Disabled)
			{
				response = "Decontamination is currently disabled.";
				return true;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " forced the LCZ decontamination to begin.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			DecontaminationController.Singleton.DecontaminationOverride = DecontaminationController.DecontaminationStatus.Forced;
			response = "Decontamination has begun.";
			return true;
		}
	}
}
