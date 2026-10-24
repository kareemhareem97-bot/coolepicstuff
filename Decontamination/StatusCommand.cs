using System;
using LightContainmentZoneDecontamination;

namespace CommandSystem.Commands.RemoteAdmin.Decontamination
{
	// Token: 0x02000A76 RID: 2678
	[CommandHandler(typeof(DecontaminationCommand))]
	public class StatusCommand : ICommand
	{
		// Token: 0x17000D88 RID: 3464
		// (get) Token: 0x06003C7F RID: 15487 RVA: 0x000C7559 File Offset: 0x000C5759
		public string Command { get; } = "status";

		// Token: 0x17000D89 RID: 3465
		// (get) Token: 0x06003C80 RID: 15488 RVA: 0x000C7561 File Offset: 0x000C5761
		public string[] Aliases { get; } = new string[]
		{
			"s",
			"c",
			"check"
		};

		// Token: 0x17000D8A RID: 3466
		// (get) Token: 0x06003C81 RID: 15489 RVA: 0x000C7569 File Offset: 0x000C5769
		public string Description { get; } = "Returns status of the LCZ decontamination.";

		// Token: 0x06003C82 RID: 15490 RVA: 0x000C7571 File Offset: 0x000C5771
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RoundEvents, out response))
			{
				return false;
			}
			response = "Decontamination is " + ((DecontaminationController.Singleton.DecontaminationOverride == DecontaminationController.DecontaminationStatus.Disabled) ? "DISABLED" : "ENABLED") + ".";
			return true;
		}
	}
}
