using System;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A05 RID: 2565
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class RconCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000C31 RID: 3121
		// (get) Token: 0x060039F2 RID: 14834 RVA: 0x000BEFC9 File Offset: 0x000BD1C9
		public string Command { get; } = "remoteconsole";

		// Token: 0x17000C32 RID: 3122
		// (get) Token: 0x060039F3 RID: 14835 RVA: 0x000BEFD1 File Offset: 0x000BD1D1
		public string[] Aliases { get; } = new string[]
		{
			"remoteconsole",
			"rcon",
			"sudo"
		};

		// Token: 0x17000C33 RID: 3123
		// (get) Token: 0x060039F4 RID: 14836 RVA: 0x000BEFD9 File Offset: 0x000BD1D9
		public string Description { get; } = "Runs a command as the server console.";

		// Token: 0x17000C34 RID: 3124
		// (get) Token: 0x060039F5 RID: 14837 RVA: 0x000BEFE1 File Offset: 0x000BD1E1
		public string[] Usage { get; } = new string[]
		{
			"Command"
		};

		// Token: 0x060039F6 RID: 14838 RVA: 0x000BEFEC File Offset: 0x000BD1EC
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
			{
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			if (arguments.At(0).StartsWith("!") && !ServerStatic.RolesConfig.GetBool("allow_central_server_commands_as_ServerConsoleCommands", false))
			{
				response = "Running central server commands in Remote Admin is disabled in RA config file!";
				return false;
			}
			CommandSender commandSender = sender as CommandSender;
			if (commandSender == null)
			{
				response = "You must be a CommandSender to execute this command!";
				return false;
			}
			string text = RAUtils.FormatArguments(arguments, 0, null);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " executed command as server console: " + text + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			ServerConsole.EnterCommand(text, commandSender);
			response = "Command \"" + text + "\" executed in server console!";
			return true;
		}
	}
}
