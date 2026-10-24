using System;

namespace CommandSystem.Commands.RemoteAdmin.ServerEvent
{
	// Token: 0x02000A40 RID: 2624
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class ServerEventCommand : ParentCommand, IUsageProvider
	{
		// Token: 0x17000CDE RID: 3294
		// (get) Token: 0x06003B48 RID: 15176 RVA: 0x000C35D5 File Offset: 0x000C17D5
		public override string Command { get; } = "SERVER_EVENT";

		// Token: 0x17000CDF RID: 3295
		// (get) Token: 0x06003B49 RID: 15177 RVA: 0x000C35DD File Offset: 0x000C17DD
		public override string[] Aliases { get; }

		// Token: 0x17000CE0 RID: 3296
		// (get) Token: 0x06003B4A RID: 15178 RVA: 0x000C35E5 File Offset: 0x000C17E5
		public override string Description { get; } = "Various server event controls";

		// Token: 0x17000CE1 RID: 3297
		// (get) Token: 0x06003B4B RID: 15179 RVA: 0x000C35ED File Offset: 0x000C17ED
		public string[] Usage { get; } = new string[]
		{
			"SubCommand"
		};

		// Token: 0x06003B4C RID: 15180 RVA: 0x000C35F5 File Offset: 0x000C17F5
		public static ServerEventCommand Create()
		{
			ServerEventCommand serverEventCommand = new ServerEventCommand();
			serverEventCommand.LoadGeneratedCommands();
			return serverEventCommand;
		}

		// Token: 0x06003B4D RID: 15181 RVA: 0x000C3602 File Offset: 0x000C1802
		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "Invalid subcommand!";
			return false;
		}

		// Token: 0x06003B4E RID: 15182 RVA: 0x000C360C File Offset: 0x000C180C
		public override void LoadGeneratedCommands()
		{
			this.RegisterCommand(new DetonationCancelCommand());
			this.RegisterCommand(new DetonationInstantCommand());
			this.RegisterCommand(new DetonationStartCommand());
			this.RegisterCommand(new RoundRestartCommand());
			this.RegisterCommand(new TerminateUnconnectedCommand());
		}
	}
}
