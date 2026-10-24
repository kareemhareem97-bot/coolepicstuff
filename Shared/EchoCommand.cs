using System;
using RemoteAdmin;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009B0 RID: 2480
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	[CommandHandler(typeof(ClientCommandHandler))]
	public class EchoCommand : ICommand, IHiddenCommand
	{
		// Token: 0x17000B25 RID: 2853
		// (get) Token: 0x06003819 RID: 14361 RVA: 0x000B650C File Offset: 0x000B470C
		public string Command { get; } = "echo";

		// Token: 0x17000B26 RID: 2854
		// (get) Token: 0x0600381A RID: 14362 RVA: 0x000B6514 File Offset: 0x000B4714
		public string[] Aliases { get; }

		// Token: 0x17000B27 RID: 2855
		// (get) Token: 0x0600381B RID: 14363 RVA: 0x000B651C File Offset: 0x000B471C
		public string Description { get; } = "Echoes the input back to the sender.";

		// Token: 0x0600381C RID: 14364 RVA: 0x000B6524 File Offset: 0x000B4724
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			if (playerCommandSender != null && !playerCommandSender.ReferenceHub.serverRoles.RemoteAdmin)
			{
				response = "You don't have permissions to use this command!";
				return false;
			}
			response = "Echo response: " + string.Join(" ", arguments);
			return true;
		}
	}
}
