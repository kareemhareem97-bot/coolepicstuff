using System;

namespace CommandSystem.Commands.Console
{
	// Token: 0x02000A94 RID: 2708
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class HttpInfoCommand : ICommand
	{
		// Token: 0x17000DD3 RID: 3539
		// (get) Token: 0x06003D1A RID: 15642 RVA: 0x000C8E44 File Offset: 0x000C7044
		public string Command { get; } = "httpinfo";

		// Token: 0x17000DD4 RID: 3540
		// (get) Token: 0x06003D1B RID: 15643 RVA: 0x000C8E4C File Offset: 0x000C704C
		public string[] Aliases { get; } = new string[]
		{
			"httpconfig",
			"httpconfiguration"
		};

		// Token: 0x17000DD5 RID: 3541
		// (get) Token: 0x06003D1C RID: 15644 RVA: 0x000C8E54 File Offset: 0x000C7054
		public string Description { get; } = "Returns information about the currently used HTTP handling";

		// Token: 0x06003D1D RID: 15645 RVA: 0x000C8E5C File Offset: 0x000C705C
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = HttpQuery.GetInfo();
			return true;
		}
	}
}
