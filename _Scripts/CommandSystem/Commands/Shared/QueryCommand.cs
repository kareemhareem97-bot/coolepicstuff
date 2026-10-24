using System;
using System.Text;
using CommandSystem;
using NorthwoodLib.Pools;

namespace _Scripts.CommandSystem.Commands.Shared
{
	// Token: 0x020009A1 RID: 2465
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class QueryCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000AFC RID: 2812
		// (get) Token: 0x060037CD RID: 14285 RVA: 0x000B5018 File Offset: 0x000B3218
		public string Command { get; } = "query";

		// Token: 0x17000AFD RID: 2813
		// (get) Token: 0x060037CE RID: 14286 RVA: 0x000B5020 File Offset: 0x000B3220
		public string[] Aliases { get; }

		// Token: 0x17000AFE RID: 2814
		// (get) Token: 0x060037CF RID: 14287 RVA: 0x000B5028 File Offset: 0x000B3228
		public string Description { get; } = "Manages query client connections.";

		// Token: 0x17000AFF RID: 2815
		// (get) Token: 0x060037D0 RID: 14288 RVA: 0x000B5030 File Offset: 0x000B3230
		public string[] Usage { get; } = new string[]
		{
			"kickall (optional)"
		};

		// Token: 0x060037D1 RID: 14289 RVA: 0x000B5038 File Offset: 0x000B3238
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
			{
				return false;
			}
			if (CustomNetworkManager.QueryServer == null)
			{
				response = "Query server is not initialized.";
				return false;
			}
			if (arguments.Count == 0)
			{
				if (CustomNetworkManager.QueryServer.Users.Count == 0)
				{
					response = "There are no query clients connected.";
					return true;
				}
				StringBuilder stringBuilder = StringBuilderPool.Shared.Rent("Connected query clients:\n");
				try
				{
					for (int i = 0; i < CustomNetworkManager.QueryServer.Users.Count; i++)
					{
						stringBuilder.AppendFormat(" - {0}\n", CustomNetworkManager.QueryServer.Users[i]);
					}
				}
				catch (Exception ex)
				{
					stringBuilder.AppendFormat("\n\nAn exception occured while listing query clients: {0}\n{1}", ex.Message, ex.StackTrace);
				}
				finally
				{
					response = StringBuilderPool.Shared.ToStringReturn(stringBuilder);
				}
				return true;
			}
			else
			{
				string a = arguments.At(0).ToLowerInvariant();
				if (a == "kickall" || a == "ka")
				{
					ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " disconnected all query clients.", ServerLogs.ServerLogType.RemoteAdminActivity_Misc, false);
					CustomNetworkManager.QueryServer.DisconnectAllClients();
					response = "All query clients have been disconnected.";
					return true;
				}
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
		}
	}
}
