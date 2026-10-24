using System;
using DrawableLine;

namespace CommandSystem.Commands.Console
{
	// Token: 0x02000A92 RID: 2706
	[CommandHandler(typeof(LineCommand))]
	public class LineToggleCommand : ICommand
	{
		// Token: 0x17000DCD RID: 3533
		// (get) Token: 0x06003D10 RID: 15632 RVA: 0x000C8D41 File Offset: 0x000C6F41
		public string Command { get; } = "toggle";

		// Token: 0x17000DCE RID: 3534
		// (get) Token: 0x06003D11 RID: 15633 RVA: 0x000C8D49 File Offset: 0x000C6F49
		public string[] Aliases { get; } = new string[]
		{
			"tg"
		};

		// Token: 0x17000DCF RID: 3535
		// (get) Token: 0x06003D12 RID: 15634 RVA: 0x000C8D51 File Offset: 0x000C6F51
		public string Description { get; } = "Toggles whether the line system can generate new instances.";

		// Token: 0x06003D13 RID: 15635 RVA: 0x000C8D5C File Offset: 0x000C6F5C
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			bool flag = !DrawableLines.IsDebugModeEnabled;
			string str = flag ? "<color=green>enabled</color>" : "<color=red>disabled</color>";
			DrawableLines.IsDebugModeEnabled = flag;
			response = "<color=white>The drawable line system is now <b>" + str + "</b>.</color>";
			return true;
		}
	}
}
