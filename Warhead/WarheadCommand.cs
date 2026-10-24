using System;

namespace CommandSystem.Commands.RemoteAdmin.Warhead
{
	// Token: 0x02000A2F RID: 2607
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class WarheadCommand : ParentCommand, IUsageProvider
	{
		// Token: 0x17000CBE RID: 3262
		// (get) Token: 0x06003AF8 RID: 15096 RVA: 0x000C284F File Offset: 0x000C0A4F
		public override string Command { get; } = "warhead";

		// Token: 0x17000CBF RID: 3263
		// (get) Token: 0x06003AF9 RID: 15097 RVA: 0x000C2857 File Offset: 0x000C0A57
		public override string[] Aliases { get; } = new string[]
		{
			"wh"
		};

		// Token: 0x17000CC0 RID: 3264
		// (get) Token: 0x06003AFA RID: 15098 RVA: 0x000C285F File Offset: 0x000C0A5F
		public override string Description { get; } = "Manages the alpha warhead.";

		// Token: 0x17000CC1 RID: 3265
		// (get) Token: 0x06003AFB RID: 15099 RVA: 0x000C2867 File Offset: 0x000C0A67
		public string[] Usage { get; } = new string[]
		{
			"SubCommand"
		};

		// Token: 0x06003AFC RID: 15100 RVA: 0x000C286F File Offset: 0x000C0A6F
		public static WarheadCommand Create()
		{
			WarheadCommand warheadCommand = new WarheadCommand();
			warheadCommand.LoadGeneratedCommands();
			return warheadCommand;
		}

		// Token: 0x06003AFD RID: 15101 RVA: 0x000C287C File Offset: 0x000C0A7C
		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "Please specify a valid subcommand!";
			return false;
		}

		// Token: 0x06003AFE RID: 15102 RVA: 0x000C2888 File Offset: 0x000C0A88
		public override void LoadGeneratedCommands()
		{
			this.RegisterCommand(new CancelCommand());
			this.RegisterCommand(new DetonateCommand());
			this.RegisterCommand(new DisableCommand());
			this.RegisterCommand(new EnableCommand());
			this.RegisterCommand(new InstantCommand());
			this.RegisterCommand(new LockCommand());
			this.RegisterCommand(new SetTimeCommand());
			this.RegisterCommand(new StatusCommand());
		}
	}
}
