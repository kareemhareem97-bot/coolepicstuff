using System;

namespace CommandSystem.Commands.RemoteAdmin.Dummies
{
	// Token: 0x02000A5C RID: 2652
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class DummiesCommand : ParentCommand, IUsageProvider
	{
		// Token: 0x17000D3D RID: 3389
		// (get) Token: 0x06003BF3 RID: 15347 RVA: 0x000C5F71 File Offset: 0x000C4171
		public override string Command { get; } = "dummies";

		// Token: 0x17000D3E RID: 3390
		// (get) Token: 0x06003BF4 RID: 15348 RVA: 0x000C5F79 File Offset: 0x000C4179
		public override string[] Aliases { get; } = new string[]
		{
			"dummy"
		};

		// Token: 0x17000D3F RID: 3391
		// (get) Token: 0x06003BF5 RID: 15349 RVA: 0x000C5F81 File Offset: 0x000C4181
		public override string Description { get; } = "Controls dummies in the facility.";

		// Token: 0x17000D40 RID: 3392
		// (get) Token: 0x06003BF6 RID: 15350 RVA: 0x000C5F89 File Offset: 0x000C4189
		public string[] Usage { get; } = new string[]
		{
			"spawn/destroy/follow"
		};

		// Token: 0x06003BF7 RID: 15351 RVA: 0x000C5F91 File Offset: 0x000C4191
		public static DummiesCommand Create()
		{
			DummiesCommand dummiesCommand = new DummiesCommand();
			dummiesCommand.LoadGeneratedCommands();
			return dummiesCommand;
		}

		// Token: 0x06003BF8 RID: 15352 RVA: 0x000C5F9E File Offset: 0x000C419E
		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "Please specify a valid subcommand (" + this.Usage[0] + ").";
			return false;
		}

		// Token: 0x06003BF9 RID: 15353 RVA: 0x000C5FBA File Offset: 0x000C41BA
		public override void LoadGeneratedCommands()
		{
			this.RegisterCommand(new ActionDummyCommand());
			this.RegisterCommand(new DestroyDummyCommand());
			this.RegisterCommand(new FollowDummyCommand());
			this.RegisterCommand(new SpawnDummyCommand());
		}
	}
}
