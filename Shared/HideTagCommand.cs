using System;
using RemoteAdmin;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009B7 RID: 2487
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(ClientCommandHandler))]
	public class HideTagCommand : ICommand
	{
		// Token: 0x17000B38 RID: 2872
		// (get) Token: 0x0600383B RID: 14395 RVA: 0x000B6D14 File Offset: 0x000B4F14
		public string Command { get; } = "hidetag";

		// Token: 0x17000B39 RID: 2873
		// (get) Token: 0x0600383C RID: 14396 RVA: 0x000B6D1C File Offset: 0x000B4F1C
		public string[] Aliases { get; } = new string[]
		{
			"htag",
			"ht"
		};

		// Token: 0x17000B3A RID: 2874
		// (get) Token: 0x0600383D RID: 14397 RVA: 0x000B6D24 File Offset: 0x000B4F24
		public string Description { get; } = "Hides your tag.";

		// Token: 0x0600383E RID: 14398 RVA: 0x000B6D2C File Offset: 0x000B4F2C
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			if (playerCommandSender == null)
			{
				response = "You must be in-game to use this command!";
				return false;
			}
			ServerRoles serverRoles = playerCommandSender.ReferenceHub.serverRoles;
			if (serverRoles.HasBadgeHidden)
			{
				response = "Your badge is already hidden.";
				return false;
			}
			if (!serverRoles.TryHideTag())
			{
				response = "Your don't have any badge.";
				return false;
			}
			response = "Tag hidden.";
			return true;
		}
	}
}
