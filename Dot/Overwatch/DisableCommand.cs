using System;

namespace CommandSystem.Commands.Dot.Overwatch
{
	// Token: 0x02000A85 RID: 2693
	[CommandHandler(typeof(OverwatchCommand))]
	public class DisableCommand : ICommand
	{
		// Token: 0x17000DAB RID: 3499
		// (get) Token: 0x06003CC8 RID: 15560 RVA: 0x000C805D File Offset: 0x000C625D
		public string Command { get; } = "disable";

		// Token: 0x17000DAC RID: 3500
		// (get) Token: 0x06003CC9 RID: 15561 RVA: 0x000C8065 File Offset: 0x000C6265
		public string[] Aliases { get; } = new string[]
		{
			"off",
			"false",
			"0"
		};

		// Token: 0x17000DAD RID: 3501
		// (get) Token: 0x06003CCA RID: 15562 RVA: 0x000C806D File Offset: 0x000C626D
		public string Description { get; } = "Disables overwatch.";

		// Token: 0x06003CCB RID: 15563 RVA: 0x000C8075 File Offset: 0x000C6275
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			return OverwatchCommand.SetOverwatchStatus(sender, 0, out response);
		}
	}
}
