using System;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009C7 RID: 2503
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class SubscribeCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000B66 RID: 2918
		// (get) Token: 0x0600388E RID: 14478 RVA: 0x000B7E91 File Offset: 0x000B6091
		public string Command { get; } = "subscribe";

		// Token: 0x17000B67 RID: 2919
		// (get) Token: 0x0600388F RID: 14479 RVA: 0x000B7E99 File Offset: 0x000B6099
		public string[] Aliases { get; } = new string[]
		{
			"sub"
		};

		// Token: 0x17000B68 RID: 2920
		// (get) Token: 0x06003890 RID: 14480 RVA: 0x000B7EA1 File Offset: 0x000B60A1
		public string Description { get; } = "Subscribes you to a data feed.";

		// Token: 0x17000B69 RID: 2921
		// (get) Token: 0x06003891 RID: 14481 RVA: 0x000B7EA9 File Offset: 0x000B60A9
		public string[] Usage { get; } = new string[]
		{
			"console/log/all"
		};

		// Token: 0x06003892 RID: 14482 RVA: 0x000B7EB4 File Offset: 0x000B60B4
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			IOutput output = sender as IOutput;
			if (output == null)
			{
				response = "You can't subscribe to a feed.";
				return false;
			}
			if (arguments.Count != 1)
			{
				response = "Usage: subscribe <feed>\nAvailable feeds: console/log/all";
				return false;
			}
			if (!sender.CheckPermission(PlayerPermissions.ServerLogLiveFeed, out response))
			{
				return false;
			}
			string a = arguments.At(0).ToLowerInvariant();
			if (!(a == "console"))
			{
				if (!(a == "log"))
				{
					if (!(a == "all"))
					{
						response = "Invalid feed name.";
						return false;
					}
					if (sender is ServerConsoleSender)
					{
						goto IL_B1;
					}
					if (!sender.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
					{
						return false;
					}
					ServerConsole.ConsoleOutputs.TryAdd(output.OutputId, output);
					ServerLogs.LiveLogOutput.TryAdd(output.OutputId, output);
					response = "Subscribed to all feeds.";
					return true;
				}
				else if (!ServerLogs.LiveLogOutput.ContainsKey(output.OutputId))
				{
					ServerLogs.LiveLogOutput.TryAdd(output.OutputId, output);
					response = "Subscribed to log feed.";
					return true;
				}
			}
			else if (!ServerConsole.ConsoleOutputs.ContainsKey(output.OutputId))
			{
				if (sender is ServerConsoleSender)
				{
					goto IL_B1;
				}
				if (!sender.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
				{
					return false;
				}
				ServerConsole.ConsoleOutputs.TryAdd(output.OutputId, output);
				response = "Subscribed to console feed.";
				return true;
			}
			response = "You are already subscribed to this feed.";
			return false;
			IL_B1:
			response = "Console feed is not available for server or client console.";
			return false;
		}
	}
}
