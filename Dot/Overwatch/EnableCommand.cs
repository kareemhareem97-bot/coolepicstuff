using System;

namespace CommandSystem.Commands.Dot.Overwatch
{
	// Token: 0x02000A86 RID: 2694
	[CommandHandler(typeof(OverwatchCommand))]
	public class EnableCommand : ICommand
	{
		// Token: 0x17000DAE RID: 3502
		// (get) Token: 0x06003CCD RID: 15565 RVA: 0x000C80CD File Offset: 0x000C62CD
		public string Command { get; } = "enable";

		// Token: 0x17000DAF RID: 3503
		// (get) Token: 0x06003CCE RID: 15566 RVA: 0x000C80D5 File Offset: 0x000C62D5
		public string[] Aliases { get; } = new string[]
		{
			"on",
			"true",
			"1"
		};

		// Token: 0x17000DB0 RID: 3504
		// (get) Token: 0x06003CCF RID: 15567 RVA: 0x000C80DD File Offset: 0x000C62DD
		public string Description { get; } = "Enables overwatch.";

		// Token: 0x06003CD0 RID: 15568 RVA: 0x000C80E5 File Offset: 0x000C62E5
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			return OverwatchCommand.SetOverwatchStatus(sender, 1, out response);
		}
	}
}
