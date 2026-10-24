using System;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A09 RID: 2569
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class WaveCommand : ParentCommand, IUsageProvider
	{
		// Token: 0x17000C3F RID: 3135
		// (get) Token: 0x06003A0A RID: 14858 RVA: 0x000BF4F3 File Offset: 0x000BD6F3
		public override string Command { get; } = "wave";

		// Token: 0x17000C40 RID: 3136
		// (get) Token: 0x06003A0B RID: 14859 RVA: 0x000BF4FB File Offset: 0x000BD6FB
		public override string[] Aliases { get; } = new string[]
		{
			"wv"
		};

		// Token: 0x17000C41 RID: 3137
		// (get) Token: 0x06003A0C RID: 14860 RVA: 0x000BF503 File Offset: 0x000BD703
		public override string Description { get; } = "Controls or displays information about the Wave system.";

		// Token: 0x17000C42 RID: 3138
		// (get) Token: 0x06003A0D RID: 14861 RVA: 0x000BF50B File Offset: 0x000BD70B
		public string[] Usage { get; } = new string[]
		{
			"set/get/list/spawn",
			"<wave>",
			"[type] [value]"
		};

		// Token: 0x06003A0E RID: 14862 RVA: 0x000BF513 File Offset: 0x000BD713
		public static WaveCommand Create()
		{
			WaveCommand waveCommand = new WaveCommand();
			waveCommand.LoadGeneratedCommands();
			return waveCommand;
		}

		// Token: 0x06003A0F RID: 14863 RVA: 0x000BF520 File Offset: 0x000BD720
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

		// Token: 0x06003A10 RID: 14864 RVA: 0x000BF55A File Offset: 0x000BD75A
		public override void LoadGeneratedCommands()
		{
			this.RegisterCommand(new WaveDebugCommand());
			this.RegisterCommand(new WaveForcePauseCommand());
			this.RegisterCommand(new WaveListCommand());
			this.RegisterCommand(new WaveSetCommand());
			this.RegisterCommand(new WaveSpawnCommand());
		}
	}
}
