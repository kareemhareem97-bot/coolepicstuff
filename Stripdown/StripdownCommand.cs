using System;

namespace CommandSystem.Commands.RemoteAdmin.Stripdown
{
	// Token: 0x02000A30 RID: 2608
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class StripdownCommand : ParentCommand, IUsageProvider
	{
		// Token: 0x17000CC2 RID: 3266
		// (get) Token: 0x06003B00 RID: 15104 RVA: 0x000C2941 File Offset: 0x000C0B41
		public override string Command { get; } = "stripdown";

		// Token: 0x17000CC3 RID: 3267
		// (get) Token: 0x06003B01 RID: 15105 RVA: 0x000042EB File Offset: 0x000024EB
		public override string[] Aliases
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17000CC4 RID: 3268
		// (get) Token: 0x06003B02 RID: 15106 RVA: 0x000C2949 File Offset: 0x000C0B49
		public override string Description { get; } = "Advanced debug command. Allows to print out properties and fields of any instantiated object.";

		// Token: 0x17000CC5 RID: 3269
		// (get) Token: 0x06003B03 RID: 15107 RVA: 0x000C2951 File Offset: 0x000C0B51
		public string[] Usage { get; } = new string[]
		{
			"SubCommand"
		};

		// Token: 0x06003B04 RID: 15108 RVA: 0x000C2959 File Offset: 0x000C0B59
		public static StripdownCommand Create()
		{
			StripdownCommand stripdownCommand = new StripdownCommand();
			stripdownCommand.LoadGeneratedCommands();
			return stripdownCommand;
		}

		// Token: 0x06003B05 RID: 15109 RVA: 0x000C2968 File Offset: 0x000C0B68
		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PermissionsManagement, out response))
			{
				return false;
			}
			response = "Please specify a valid subcommand:";
			foreach (ICommand command in this.AllCommands)
			{
				response = string.Concat(new string[]
				{
					response,
					"\n",
					command.Command,
					" - ",
					command.Description
				});
			}
			return false;
		}

		// Token: 0x06003B06 RID: 15110 RVA: 0x000C29FC File Offset: 0x000C0BFC
		public override void LoadGeneratedCommands()
		{
			this.RegisterCommand(new StripdownComponentCommand());
			this.RegisterCommand(new StripdownObjectCommand());
			this.RegisterCommand(new StripdownPrintCommand());
			this.RegisterCommand(new StripdownValueCommand());
		}
	}
}
