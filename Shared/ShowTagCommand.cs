using System;
using RemoteAdmin;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009C3 RID: 2499
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(ClientCommandHandler))]
	public class ShowTagCommand : ICommand
	{
		// Token: 0x17000B59 RID: 2905
		// (get) Token: 0x06003879 RID: 14457 RVA: 0x000B7816 File Offset: 0x000B5A16
		public string Command { get; } = "showtag";

		// Token: 0x17000B5A RID: 2906
		// (get) Token: 0x0600387A RID: 14458 RVA: 0x000B781E File Offset: 0x000B5A1E
		public string[] Aliases { get; } = new string[]
		{
			"tag",
			"stag",
			"st",
			"sh"
		};

		// Token: 0x17000B5B RID: 2907
		// (get) Token: 0x0600387B RID: 14459 RVA: 0x000B7826 File Offset: 0x000B5A26
		public string Description { get; } = "Shows your local tag.";

		// Token: 0x0600387C RID: 14460 RVA: 0x000B7830 File Offset: 0x000B5A30
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			if (playerCommandSender == null)
			{
				response = "You must be in-game to use this command!";
				return false;
			}
			playerCommandSender.ReferenceHub.serverRoles.RefreshLocalTag();
			response = "Local tag refreshed.";
			return true;
		}
	}
}
