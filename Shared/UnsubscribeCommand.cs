using System;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009C9 RID: 2505
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class UnsubscribeCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000B6D RID: 2925
		// (get) Token: 0x06003899 RID: 14489 RVA: 0x000B809D File Offset: 0x000B629D
		public string Command { get; } = "unsubscribe";

		// Token: 0x17000B6E RID: 2926
		// (get) Token: 0x0600389A RID: 14490 RVA: 0x000B80A5 File Offset: 0x000B62A5
		public string[] Aliases { get; } = new string[]
		{
			"unsub"
		};

		// Token: 0x17000B6F RID: 2927
		// (get) Token: 0x0600389B RID: 14491 RVA: 0x000B80AD File Offset: 0x000B62AD
		public string Description { get; } = "Unsubscribes you from a data feed.";

		// Token: 0x17000B70 RID: 2928
		// (get) Token: 0x0600389C RID: 14492 RVA: 0x000B80B5 File Offset: 0x000B62B5
		public string[] Usage { get; } = new string[]
		{
			"console/log/all"
		};

		// Token: 0x0600389D RID: 14493 RVA: 0x000B80C0 File Offset: 0x000B62C0
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			IOutput output = sender as IOutput;
			if (output == null)
			{
				response = "You can't unsubscribe from a feed.";
				return false;
			}
			if (arguments.Count != 1)
			{
				response = "Usage: unsubscribe <feed>\nAvailable feeds: console/log/all";
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
					IOutput output2;
					ServerConsole.ConsoleOutputs.TryRemove(output.OutputId, out output2);
					ServerLogs.LiveLogOutput.TryRemove(output.OutputId, out output2);
					response = "Unsubscribed from all feeds.";
					return true;
				}
				else if (ServerLogs.LiveLogOutput.ContainsKey(output.OutputId))
				{
					IOutput output2;
					ServerLogs.LiveLogOutput.TryRemove(output.OutputId, out output2);
					response = "Unsubscribed from log feed.";
					return true;
				}
			}
			else if (ServerConsole.ConsoleOutputs.ContainsKey(output.OutputId))
			{
				IOutput output2;
				ServerConsole.ConsoleOutputs.TryRemove(output.OutputId, out output2);
				response = "Unsubscribed from console feed.";
				return true;
			}
			response = "You are not subscribed to that feed.";
			return false;
		}
	}
}
