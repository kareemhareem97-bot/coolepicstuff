using System;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A26 RID: 2598
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class WikiCommand : ICommand
	{
		// Token: 0x17000CA2 RID: 3234
		// (get) Token: 0x06003ACA RID: 15050 RVA: 0x000C222B File Offset: 0x000C042B
		public string Command { get; } = "wiki";

		// Token: 0x17000CA3 RID: 3235
		// (get) Token: 0x06003ACB RID: 15051 RVA: 0x000C2233 File Offset: 0x000C0433
		public string[] Aliases { get; }

		// Token: 0x17000CA4 RID: 3236
		// (get) Token: 0x06003ACC RID: 15052 RVA: 0x000C223B File Offset: 0x000C043B
		public string Description { get; } = "Opens RA help page on the game wiki.";

		// Token: 0x06003ACD RID: 15053 RVA: 0x000C2243 File Offset: 0x000C0443
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "Wiki page has been opened.";
			return true;
		}
	}
}
