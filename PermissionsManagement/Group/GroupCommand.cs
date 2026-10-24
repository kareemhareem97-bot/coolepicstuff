using System;

namespace CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group
{
	// Token: 0x02000A48 RID: 2632
	[CommandHandler(typeof(PermissionsManagementCommand))]
	public class GroupCommand : ParentCommand
	{
		// Token: 0x17000CFB RID: 3323
		// (get) Token: 0x06003B77 RID: 15223 RVA: 0x000C3FB6 File Offset: 0x000C21B6
		public override string Command { get; } = "group";

		// Token: 0x17000CFC RID: 3324
		// (get) Token: 0x06003B78 RID: 15224 RVA: 0x000C3FBE File Offset: 0x000C21BE
		public override string[] Aliases { get; } = new string[]
		{
			"gr"
		};

		// Token: 0x17000CFD RID: 3325
		// (get) Token: 0x06003B79 RID: 15225 RVA: 0x000C3FC6 File Offset: 0x000C21C6
		public override string Description { get; } = "Group management";

		// Token: 0x06003B7A RID: 15226 RVA: 0x000C3FCE File Offset: 0x000C21CE
		public static GroupCommand Create()
		{
			GroupCommand groupCommand = new GroupCommand();
			groupCommand.LoadGeneratedCommands();
			return groupCommand;
		}

		// Token: 0x06003B7B RID: 15227 RVA: 0x000C3FDB File Offset: 0x000C21DB
		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "Invalid subcommand. Available commands: info, grant, setcolor, settag, enablecover.";
			return true;
		}

		// Token: 0x06003B7C RID: 15228 RVA: 0x000C3FE8 File Offset: 0x000C21E8
		public override void LoadGeneratedCommands()
		{
			this.RegisterCommand(new DisableCoverCommand());
			this.RegisterCommand(new EnableCoverCommand());
			this.RegisterCommand(new GrantCommand());
			this.RegisterCommand(new InfoCommand());
			this.RegisterCommand(new RevokeCommand());
			this.RegisterCommand(new SetColorCommand());
			this.RegisterCommand(new SetTagCommand());
		}
	}
}
