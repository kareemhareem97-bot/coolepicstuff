using System;

namespace CommandSystem.Commands.Console
{
	// Token: 0x02000A8E RID: 2702
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class LineCommand : ParentCommand, IUsageProvider
	{
		// Token: 0x17000DC0 RID: 3520
		// (get) Token: 0x06003CF7 RID: 15607 RVA: 0x000C884C File Offset: 0x000C6A4C
		public override string Command { get; } = "line";

		// Token: 0x17000DC1 RID: 3521
		// (get) Token: 0x06003CF8 RID: 15608 RVA: 0x000C8854 File Offset: 0x000C6A54
		public override string[] Aliases { get; } = new string[]
		{
			"debugline",
			"drawline",
			"drawableline",
			"lines"
		};

		// Token: 0x17000DC2 RID: 3522
		// (get) Token: 0x06003CF9 RID: 15609 RVA: 0x000C885C File Offset: 0x000C6A5C
		public override string Description { get; } = "Controls or displays information about the Wave system.";

		// Token: 0x17000DC3 RID: 3523
		// (get) Token: 0x06003CFA RID: 15610 RVA: 0x000C8864 File Offset: 0x000C6A64
		public string[] Usage { get; } = new string[]
		{
			"help/toggle/test/duration",
			"[value]"
		};

		// Token: 0x06003CFB RID: 15611 RVA: 0x000C886C File Offset: 0x000C6A6C
		public static LineCommand Create()
		{
			LineCommand lineCommand = new LineCommand();
			lineCommand.LoadGeneratedCommands();
			return lineCommand;
		}

		// Token: 0x06003CFC RID: 15612 RVA: 0x000BF520 File Offset: 0x000BD720
		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = string.Concat(new string[]
			{
				"Unknown subcommand.\nUsage: ",
				this.Command,
				" ",
				this.DisplayCommandUsage(),
				"."
			});
			return false;
		}

		// Token: 0x06003CFD RID: 15613 RVA: 0x000C8879 File Offset: 0x000C6A79
		public override void LoadGeneratedCommands()
		{
			this.RegisterCommand(new LineDurationCommand());
			this.RegisterCommand(new LineHelpCommand());
			this.RegisterCommand(new LineTestCommand());
			this.RegisterCommand(new LineToggleCommand());
		}
	}
}
