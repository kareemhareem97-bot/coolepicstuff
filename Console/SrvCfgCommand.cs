using System;
using GameCore;

namespace CommandSystem.Commands.Console
{
	// Token: 0x02000AA3 RID: 2723
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class SrvCfgCommand : ICommand
	{
		// Token: 0x17000DF5 RID: 3573
		// (get) Token: 0x06003D5C RID: 15708 RVA: 0x000C98F5 File Offset: 0x000C7AF5
		public string Command { get; } = "srvcfg";

		// Token: 0x17000DF6 RID: 3574
		// (get) Token: 0x06003D5D RID: 15709 RVA: 0x000C98FD File Offset: 0x000C7AFD
		public string[] Aliases { get; }

		// Token: 0x17000DF7 RID: 3575
		// (get) Token: 0x06003D5E RID: 15710 RVA: 0x000C9905 File Offset: 0x000C7B05
		public string Description { get; } = "Displays the server config.";

		// Token: 0x06003D5F RID: 15711 RVA: 0x000C990D File Offset: 0x000C7B0D
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			GameCore.Console.Singleton.TypeCommand(".srvcfg", null);
			response = null;
			return true;
		}
	}
}
