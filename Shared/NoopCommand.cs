using System;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009B9 RID: 2489
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	[CommandHandler(typeof(ClientCommandHandler))]
	public class NoopCommand : ICommand, IHiddenCommand
	{
		// Token: 0x17000B3E RID: 2878
		// (get) Token: 0x06003845 RID: 14405 RVA: 0x000B6E9B File Offset: 0x000B509B
		public string Command { get; } = "noop";

		// Token: 0x17000B3F RID: 2879
		// (get) Token: 0x06003846 RID: 14406 RVA: 0x000B6EA3 File Offset: 0x000B50A3
		public string[] Aliases { get; }

		// Token: 0x17000B40 RID: 2880
		// (get) Token: 0x06003847 RID: 14407 RVA: 0x000B6EAB File Offset: 0x000B50AB
		public string Description { get; } = "Technical command that doesn't do anything (noop = no operation).";

		// Token: 0x06003848 RID: 14408 RVA: 0x000B6EB3 File Offset: 0x000B50B3
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = null;
			return true;
		}
	}
}
