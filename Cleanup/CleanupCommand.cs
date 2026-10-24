using System;

namespace CommandSystem.Commands.RemoteAdmin.Cleanup
{
	// Token: 0x02000A79 RID: 2681
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class CleanupCommand : ParentCommand, IUsageProvider
	{
		// Token: 0x17000D91 RID: 3473
		// (get) Token: 0x06003C8E RID: 15502 RVA: 0x000C7799 File Offset: 0x000C5999
		public override string Command { get; } = "cleanup";

		// Token: 0x17000D92 RID: 3474
		// (get) Token: 0x06003C8F RID: 15503 RVA: 0x000C77A1 File Offset: 0x000C59A1
		public override string[] Aliases { get; } = new string[0];

		// Token: 0x17000D93 RID: 3475
		// (get) Token: 0x06003C90 RID: 15504 RVA: 0x000C77A9 File Offset: 0x000C59A9
		public override string Description { get; } = "Controls and cleans several elements of the map.";

		// Token: 0x17000D94 RID: 3476
		// (get) Token: 0x06003C91 RID: 15505 RVA: 0x000C77B1 File Offset: 0x000C59B1
		public string[] Usage { get; } = new string[]
		{
			"ragdolls/items/decals/blood/bulletholes"
		};

		// Token: 0x06003C92 RID: 15506 RVA: 0x000C77B9 File Offset: 0x000C59B9
		public static CleanupCommand Create()
		{
			CleanupCommand cleanupCommand = new CleanupCommand();
			cleanupCommand.LoadGeneratedCommands();
			return cleanupCommand;
		}

		// Token: 0x06003C93 RID: 15507 RVA: 0x000C77C6 File Offset: 0x000C59C6
		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "Please specify a valid subcommand (" + this.Usage[0] + ").";
			return false;
		}

		// Token: 0x06003C94 RID: 15508 RVA: 0x000C77E2 File Offset: 0x000C59E2
		public override void LoadGeneratedCommands()
		{
			this.RegisterCommand(new BloodCommand());
			this.RegisterCommand(new BulletHolesCommand());
			this.RegisterCommand(new CorpsesCommand());
			this.RegisterCommand(new ItemsCommand());
		}
	}
}
