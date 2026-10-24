using System;

namespace CommandSystem.Commands.RemoteAdmin.Decontamination
{
	// Token: 0x02000A72 RID: 2674
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class DecontaminationCommand : ParentCommand, IUsageProvider
	{
		// Token: 0x17000D7B RID: 3451
		// (get) Token: 0x06003C68 RID: 15464 RVA: 0x000C7252 File Offset: 0x000C5452
		public override string Command { get; } = "decontamination";

		// Token: 0x17000D7C RID: 3452
		// (get) Token: 0x06003C69 RID: 15465 RVA: 0x000C725A File Offset: 0x000C545A
		public override string[] Aliases { get; } = new string[]
		{
			"decont"
		};

		// Token: 0x17000D7D RID: 3453
		// (get) Token: 0x06003C6A RID: 15466 RVA: 0x000C7262 File Offset: 0x000C5462
		public override string Description { get; } = "Controls the LCZ decontamination.";

		// Token: 0x17000D7E RID: 3454
		// (get) Token: 0x06003C6B RID: 15467 RVA: 0x000C726A File Offset: 0x000C546A
		public string[] Usage { get; } = new string[]
		{
			"status/enable/disable"
		};

		// Token: 0x06003C6C RID: 15468 RVA: 0x000C7272 File Offset: 0x000C5472
		public static DecontaminationCommand Create()
		{
			DecontaminationCommand decontaminationCommand = new DecontaminationCommand();
			decontaminationCommand.LoadGeneratedCommands();
			return decontaminationCommand;
		}

		// Token: 0x06003C6D RID: 15469 RVA: 0x000C727F File Offset: 0x000C547F
		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "Please specify a valid subcommand (" + this.Usage[0] + ")!";
			return false;
		}

		// Token: 0x06003C6E RID: 15470 RVA: 0x000C729B File Offset: 0x000C549B
		public override void LoadGeneratedCommands()
		{
			this.RegisterCommand(new DisableCommand());
			this.RegisterCommand(new EnableCommand());
			this.RegisterCommand(new ForceCommand());
			this.RegisterCommand(new StatusCommand());
		}
	}
}
