using System;
using RemoteAdmin;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009B2 RID: 2482
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(ClientCommandHandler))]
	public class GlobalTagCommand : ICommand
	{
		// Token: 0x17000B2B RID: 2859
		// (get) Token: 0x06003823 RID: 14371 RVA: 0x000B6665 File Offset: 0x000B4865
		public string Command { get; } = "globaltag";

		// Token: 0x17000B2C RID: 2860
		// (get) Token: 0x06003824 RID: 14372 RVA: 0x000B666D File Offset: 0x000B486D
		public string[] Aliases { get; } = new string[]
		{
			"gtag",
			"gtg",
			"gt"
		};

		// Token: 0x17000B2D RID: 2861
		// (get) Token: 0x06003825 RID: 14373 RVA: 0x000B6675 File Offset: 0x000B4875
		public string Description { get; } = "Shows your global tag.";

		// Token: 0x06003826 RID: 14374 RVA: 0x000B6680 File Offset: 0x000B4880
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			if (playerCommandSender == null)
			{
				response = "You must be in-game to use this command!";
				return false;
			}
			if (!playerCommandSender.ReferenceHub.serverRoles.RefreshGlobalTag())
			{
				response = "You don't have a global tag.";
				return false;
			}
			response = "Global tag refreshed.";
			return true;
		}
	}
}
