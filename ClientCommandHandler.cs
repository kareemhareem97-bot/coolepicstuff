using System;
using CommandSystem.Commands.Dot.Overwatch;
using CommandSystem.Commands.Shared;

namespace CommandSystem
{
	// Token: 0x020009A4 RID: 2468
	public class ClientCommandHandler : CommandHandler
	{
		// Token: 0x060037DD RID: 14301 RVA: 0x000B5446 File Offset: 0x000B3646
		private ClientCommandHandler()
		{
		}

		// Token: 0x060037DE RID: 14302 RVA: 0x000B544E File Offset: 0x000B364E
		public static ClientCommandHandler Create()
		{
			ClientCommandHandler clientCommandHandler = new ClientCommandHandler();
			clientCommandHandler.LoadGeneratedCommands();
			return clientCommandHandler;
		}

		// Token: 0x060037DF RID: 14303 RVA: 0x000B545C File Offset: 0x000B365C
		public override void LoadGeneratedCommands()
		{
			this.RegisterCommand(new AudioPoolDebug());
			this.RegisterCommand(new ContactCommand());
			this.RegisterCommand(new EchoCommand());
			this.RegisterCommand(new GlobalTagCommand());
			this.RegisterCommand(new GroupsCommand());
			this.RegisterCommand(new HelloCommand());
			this.RegisterCommand(new HelpCommand(this));
			this.RegisterCommand(new HideTagCommand());
			this.RegisterCommand(new NoopCommand());
			this.RegisterCommand(new ShowTagCommand());
			this.RegisterCommand(new SrvCfgCommand());
			this.RegisterCommand(OverwatchCommand.Create());
		}
	}
}
