using System;
using LightContainmentZoneDecontamination;

namespace CommandSystem.Commands.RemoteAdmin.Decontamination
{
	// Token: 0x02000A73 RID: 2675
	[CommandHandler(typeof(DecontaminationCommand))]
	public class DisableCommand : ICommand
	{
		// Token: 0x17000D7F RID: 3455
		// (get) Token: 0x06003C70 RID: 15472 RVA: 0x000C731D File Offset: 0x000C551D
		public string Command { get; } = "disable";

		// Token: 0x17000D80 RID: 3456
		// (get) Token: 0x06003C71 RID: 15473 RVA: 0x000C7325 File Offset: 0x000C5525
		public string[] Aliases { get; } = new string[]
		{
			"d",
			"0",
			"off"
		};

		// Token: 0x17000D81 RID: 3457
		// (get) Token: 0x06003C72 RID: 15474 RVA: 0x000C732D File Offset: 0x000C552D
		public string Description { get; } = "Disables the LCZ decontamination.";

		// Token: 0x06003C73 RID: 15475 RVA: 0x000C7338 File Offset: 0x000C5538
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RoundEvents, out response))
			{
				return false;
			}
			if (DecontaminationController.Singleton.DecontaminationOverride == DecontaminationController.DecontaminationStatus.Disabled)
			{
				response = "Decontamination is already disabled.";
				return true;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " disabled the LCZ decontamination.", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			DecontaminationController.Singleton.DecontaminationOverride = DecontaminationController.DecontaminationStatus.Disabled;
			response = "Decontamination has been disabled.";
			return true;
		}
	}
}
