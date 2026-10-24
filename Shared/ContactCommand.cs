using System;
using GameCore;
using RemoteAdmin;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009AF RID: 2479
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(ClientCommandHandler))]
	public class ContactCommand : ICommand
	{
		// Token: 0x17000B22 RID: 2850
		// (get) Token: 0x06003814 RID: 14356 RVA: 0x000B6461 File Offset: 0x000B4661
		public string Command { get; } = "contact";

		// Token: 0x17000B23 RID: 2851
		// (get) Token: 0x06003815 RID: 14357 RVA: 0x000B6469 File Offset: 0x000B4669
		public string[] Aliases { get; }

		// Token: 0x17000B24 RID: 2852
		// (get) Token: 0x06003816 RID: 14358 RVA: 0x000B6471 File Offset: 0x000B4671
		public string Description { get; } = "Return the contact email address of the server.";

		// Token: 0x06003817 RID: 14359 RVA: 0x000B647C File Offset: 0x000B467C
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			if (playerCommandSender != null && !playerCommandSender.ReferenceHub.serverRoles.RemoteAdmin && !playerCommandSender.ReferenceHub.authManager.BypassBansFlagSet && !playerCommandSender.ReferenceHub.isLocalPlayer)
			{
				response = "You don't have permissions to execute this command.";
				return false;
			}
			response = "Contact email address: " + ConfigFile.ServerConfig.GetString("contact_email", "");
			return true;
		}
	}
}
