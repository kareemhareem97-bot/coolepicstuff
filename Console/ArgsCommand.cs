using System;
using System.Linq;

namespace CommandSystem.Commands.Console
{
	// Token: 0x02000A8C RID: 2700
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class ArgsCommand : ICommand
	{
		// Token: 0x17000DBD RID: 3517
		// (get) Token: 0x06003CEF RID: 15599 RVA: 0x000C87A8 File Offset: 0x000C69A8
		public string Command { get; } = "args";

		// Token: 0x17000DBE RID: 3518
		// (get) Token: 0x06003CF0 RID: 15600 RVA: 0x000C87B0 File Offset: 0x000C69B0
		public string[] Aliases { get; }

		// Token: 0x17000DBF RID: 3519
		// (get) Token: 0x06003CF1 RID: 15601 RVA: 0x000C87B8 File Offset: 0x000C69B8
		public string Description { get; } = "Prints all startup args";

		// Token: 0x06003CF2 RID: 15602 RVA: 0x000C87C0 File Offset: 0x000C69C0
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
			{
				return false;
			}
			response = StartupArgs.Args.Aggregate("Startup args:\n", (string current, string arg) => current + "- " + arg + "\n");
			return true;
		}
	}
}
