using System;
using Mirror;

namespace CommandSystem.Commands.RemoteAdmin.Warhead
{
	// Token: 0x02000A2E RID: 2606
	[CommandHandler(typeof(WarheadCommand))]
	public class StatusCommand : ICommand
	{
		// Token: 0x17000CBB RID: 3259
		// (get) Token: 0x06003AF3 RID: 15091 RVA: 0x000C278D File Offset: 0x000C098D
		public string Command { get; } = "status";

		// Token: 0x17000CBC RID: 3260
		// (get) Token: 0x06003AF4 RID: 15092 RVA: 0x000C2795 File Offset: 0x000C0995
		public string[] Aliases { get; } = new string[]
		{
			"s"
		};

		// Token: 0x17000CBD RID: 3261
		// (get) Token: 0x06003AF5 RID: 15093 RVA: 0x000C279D File Offset: 0x000C099D
		public string Description { get; } = "Returns the status of the alpha warhead.";

		// Token: 0x06003AF6 RID: 15094 RVA: 0x000C27A8 File Offset: 0x000C09A8
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			if (AlphaWarheadController.Detonated)
			{
				response = "Warhead has been detonated.";
			}
			else if (AlphaWarheadController.InProgress)
			{
				response = "Detonation is in progress.";
			}
			else if (!AlphaWarheadOutsitePanel.nukeside.enabled)
			{
				response = "Warhead is disabled.";
			}
			else if (AlphaWarheadController.Singleton.CooldownEndTime > NetworkTime.time)
			{
				response = "Warhead is restarting.";
			}
			else
			{
				response = "Warhead is ready to detonation.";
			}
			return true;
		}
	}
}
