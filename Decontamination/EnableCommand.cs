using System;
using LightContainmentZoneDecontamination;

namespace CommandSystem.Commands.RemoteAdmin.Decontamination
{
	// Token: 0x02000A74 RID: 2676
	[CommandHandler(typeof(DecontaminationCommand))]
	public class EnableCommand : ICommand
	{
		// Token: 0x17000D82 RID: 3458
		// (get) Token: 0x06003C75 RID: 15477 RVA: 0x000C73E5 File Offset: 0x000C55E5
		public string Command { get; } = "enable";

		// Token: 0x17000D83 RID: 3459
		// (get) Token: 0x06003C76 RID: 15478 RVA: 0x000C73ED File Offset: 0x000C55ED
		public string[] Aliases { get; } = new string[]
		{
			"e",
			"1",
			"on"
		};

		// Token: 0x17000D84 RID: 3460
		// (get) Token: 0x06003C77 RID: 15479 RVA: 0x000C73F5 File Offset: 0x000C55F5
		public string Description { get; } = "Enables the LCZ decontamination.";

		// Token: 0x06003C78 RID: 15480 RVA: 0x000C7400 File Offset: 0x000C5600
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RoundEvents, out response))
			{
				return false;
			}
			if (DecontaminationController.Singleton.DecontaminationOverride == DecontaminationController.DecontaminationStatus.None)
			{
				response = "Decontamination is already enabled.";
				return true;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " enabled the LCZ decontamination.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			DecontaminationController.Singleton.DecontaminationOverride = DecontaminationController.DecontaminationStatus.None;
			response = "Decontamination has been enabled.";
			return true;
		}
	}
}
