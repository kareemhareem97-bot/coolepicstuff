using System;

namespace CommandSystem.Commands.RemoteAdmin.Dms
{
	// Token: 0x02000A6D RID: 2669
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class DmsCommand : ParentCommand, IUsageProvider
	{
		// Token: 0x17000D6B RID: 3435
		// (get) Token: 0x06003C4C RID: 15436 RVA: 0x000C6FE1 File Offset: 0x000C51E1
		public override string Command
		{
			get
			{
				return "deadmanswitch";
			}
		}

		// Token: 0x17000D6C RID: 3436
		// (get) Token: 0x06003C4D RID: 15437 RVA: 0x000C6FE8 File Offset: 0x000C51E8
		public override string[] Aliases
		{
			get
			{
				return new string[]
				{
					"dms"
				};
			}
		}

		// Token: 0x17000D6D RID: 3437
		// (get) Token: 0x06003C4E RID: 15438 RVA: 0x000C6FF8 File Offset: 0x000C51F8
		public override string Description
		{
			get
			{
				return "Manages the Deadman's Switch.";
			}
		}

		// Token: 0x17000D6E RID: 3438
		// (get) Token: 0x06003C4F RID: 15439 RVA: 0x000C6FFF File Offset: 0x000C51FF
		public string[] Usage { get; } = new string[]
		{
			"force/toggle/get/set"
		};

		// Token: 0x06003C50 RID: 15440 RVA: 0x000C7007 File Offset: 0x000C5207
		public static DmsCommand Create()
		{
			DmsCommand dmsCommand = new DmsCommand();
			dmsCommand.LoadGeneratedCommands();
			return dmsCommand;
		}

		// Token: 0x06003C51 RID: 15441 RVA: 0x000C287C File Offset: 0x000C0A7C
		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "Please specify a valid subcommand!";
			return false;
		}

		// Token: 0x06003C52 RID: 15442 RVA: 0x000C7014 File Offset: 0x000C5214
		public override void LoadGeneratedCommands()
		{
			this.RegisterCommand(new ForceCommand());
			this.RegisterCommand(new GetCommand());
			this.RegisterCommand(new SetCommand());
			this.RegisterCommand(new ToggleCommand());
		}
	}
}
