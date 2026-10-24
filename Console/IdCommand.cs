using System;

namespace CommandSystem.Commands.Console
{
	// Token: 0x02000A95 RID: 2709
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class IdCommand : ICommand
	{
		// Token: 0x17000DD6 RID: 3542
		// (get) Token: 0x06003D1F RID: 15647 RVA: 0x000C8EA0 File Offset: 0x000C70A0
		public string Command { get; } = "id";

		// Token: 0x17000DD7 RID: 3543
		// (get) Token: 0x06003D20 RID: 15648 RVA: 0x000C8EA8 File Offset: 0x000C70A8
		public string[] Aliases { get; } = new string[]
		{
			"myid"
		};

		// Token: 0x17000DD8 RID: 3544
		// (get) Token: 0x06003D21 RID: 15649 RVA: 0x000C8EB0 File Offset: 0x000C70B0
		public string Description { get; } = "Displays your current player ID.";

		// Token: 0x06003D22 RID: 15650 RVA: 0x000C8EB8 File Offset: 0x000C70B8
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			ReferenceHub localHub = ReferenceHub.LocalHub;
			if (localHub == null)
			{
				response = "You must join a server to execute this command.";
				return false;
			}
			response = string.Format("Your Player ID on the current server: {0}", localHub.PlayerId);
			return true;
		}
	}
}
