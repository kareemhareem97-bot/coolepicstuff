using System;

namespace CommandSystem.Commands.Console
{
	// Token: 0x02000A9D RID: 2717
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class LennyCommand : ICommand, IHiddenCommand
	{
		// Token: 0x17000DE3 RID: 3555
		// (get) Token: 0x06003D3E RID: 15678 RVA: 0x000C95DE File Offset: 0x000C77DE
		public string Command { get; } = "lenny";

		// Token: 0x17000DE4 RID: 3556
		// (get) Token: 0x06003D3F RID: 15679 RVA: 0x000C95E6 File Offset: 0x000C77E6
		public string[] Aliases { get; }

		// Token: 0x17000DE5 RID: 3557
		// (get) Token: 0x06003D40 RID: 15680 RVA: 0x000C95EE File Offset: 0x000C77EE
		public string Description { get; } = "( ͡° ͜ʖ ͡°)";

		// Token: 0x06003D41 RID: 15681 RVA: 0x000C95F6 File Offset: 0x000C77F6
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "<size=200>( ͡° ͜ʖ ͡°)</size>";
			return true;
		}
	}
}
