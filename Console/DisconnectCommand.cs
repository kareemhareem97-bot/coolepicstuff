using System;
using Query;

namespace CommandSystem.Commands.Console
{
	// Token: 0x02000A93 RID: 2707
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class DisconnectCommand : ICommand
	{
		// Token: 0x17000DD0 RID: 3536
		// (get) Token: 0x06003D15 RID: 15637 RVA: 0x000C8DCB File Offset: 0x000C6FCB
		public string Command { get; } = "disconnect";

		// Token: 0x17000DD1 RID: 3537
		// (get) Token: 0x06003D16 RID: 15638 RVA: 0x000C8DD3 File Offset: 0x000C6FD3
		public string[] Aliases { get; } = new string[]
		{
			"dc"
		};

		// Token: 0x17000DD2 RID: 3538
		// (get) Token: 0x06003D17 RID: 15639 RVA: 0x000C8DDB File Offset: 0x000C6FDB
		public string Description { get; } = "Disconnect from server.";

		// Token: 0x06003D18 RID: 15640 RVA: 0x000C8DE4 File Offset: 0x000C6FE4
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			QueryCommandSender queryCommandSender = sender as QueryCommandSender;
			if (queryCommandSender != null)
			{
				queryCommandSender.Disconnect();
				response = "";
				return true;
			}
			response = "This command can be only executed on a game client or server query client!";
			return false;
		}
	}
}
