using System;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009AA RID: 2474
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class ConfigCommand : ParentCommand, IUsageProvider
	{
		// Token: 0x17000B11 RID: 2833
		// (get) Token: 0x060037F7 RID: 14327 RVA: 0x000B6009 File Offset: 0x000B4209
		public override string Command { get; } = "config";

		// Token: 0x17000B12 RID: 2834
		// (get) Token: 0x060037F8 RID: 14328 RVA: 0x000B6011 File Offset: 0x000B4211
		public override string[] Aliases { get; } = new string[]
		{
			"cfg"
		};

		// Token: 0x17000B13 RID: 2835
		// (get) Token: 0x060037F9 RID: 14329 RVA: 0x000B6019 File Offset: 0x000B4219
		public override string Description { get; } = "Allows for config debugging and reloading";

		// Token: 0x17000B14 RID: 2836
		// (get) Token: 0x060037FA RID: 14330 RVA: 0x000B6021 File Offset: 0x000B4221
		public string[] Usage { get; } = new string[]
		{
			"Reload/Path/Value/Open"
		};

		// Token: 0x060037FB RID: 14331 RVA: 0x000B6029 File Offset: 0x000B4229
		public static ConfigCommand Create()
		{
			ConfigCommand configCommand = new ConfigCommand();
			configCommand.LoadGeneratedCommands();
			return configCommand;
		}

		// Token: 0x060037FC RID: 14332 RVA: 0x000B6036 File Offset: 0x000B4236
		protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			response = "Please specify a valid subcommand! The valid commands are: Reload/Path/Value/Open.";
			return false;
		}

		// Token: 0x060037FD RID: 14333 RVA: 0x000B6040 File Offset: 0x000B4240
		public override void LoadGeneratedCommands()
		{
			this.RegisterCommand(new OpenCommand());
			this.RegisterCommand(new PathCommand());
			this.RegisterCommand(new ReloadCommand());
			this.RegisterCommand(new ValueCommand());
		}
	}
}
