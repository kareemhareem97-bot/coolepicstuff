using System;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009BF RID: 2495
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class RefreshCommandsCommand : ICommand
	{
		// Token: 0x17000B50 RID: 2896
		// (get) Token: 0x06003866 RID: 14438 RVA: 0x000B74CD File Offset: 0x000B56CD
		public string Command { get; } = "refreshcommands";

		// Token: 0x17000B51 RID: 2897
		// (get) Token: 0x06003867 RID: 14439 RVA: 0x000B74D5 File Offset: 0x000B56D5
		public string[] Aliases { get; }

		// Token: 0x17000B52 RID: 2898
		// (get) Token: 0x06003868 RID: 14440 RVA: 0x000B74DD File Offset: 0x000B56DD
		public string Description { get; } = "Reloads all commands.";

		// Token: 0x06003869 RID: 14441 RVA: 0x000B74E5 File Offset: 0x000B56E5
		public RefreshCommandsCommand(ICommandHandler commandHandler)
		{
			this._commandHandler = commandHandler;
		}

		// Token: 0x0600386A RID: 14442 RVA: 0x000B750A File Offset: 0x000B570A
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
			{
				return false;
			}
			this._commandHandler.ClearCommands();
			this._commandHandler.LoadGeneratedCommands();
			response = "Successfully reloaded all commands!";
			return true;
		}

		// Token: 0x04002A11 RID: 10769
		private readonly ICommandHandler _commandHandler;
	}
}
