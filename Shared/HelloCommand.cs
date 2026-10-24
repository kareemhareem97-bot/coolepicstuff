using System;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009B5 RID: 2485
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(ClientCommandHandler))]
	public class HelloCommand : ICommand
	{
		// Token: 0x17000B31 RID: 2865
		// (get) Token: 0x0600382F RID: 14383 RVA: 0x000B69D6 File Offset: 0x000B4BD6
		public string Command { get; } = "hello";

		// Token: 0x17000B32 RID: 2866
		// (get) Token: 0x06003830 RID: 14384 RVA: 0x000B69DE File Offset: 0x000B4BDE
		public string[] Aliases { get; }

		// Token: 0x17000B33 RID: 2867
		// (get) Token: 0x06003831 RID: 14385 RVA: 0x000B69E6 File Offset: 0x000B4BE6
		public string Description { get; } = "Hi!";

		// Token: 0x06003832 RID: 14386 RVA: 0x000B69EE File Offset: 0x000B4BEE
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "Hello World!";
			return true;
		}
	}
}
