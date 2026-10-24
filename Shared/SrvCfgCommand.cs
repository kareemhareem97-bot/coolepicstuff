using System;
using System.Text;
using CentralAuth;
using GameCore;
using Mirror.LiteNetLib4Mirror;
using NorthwoodLib.Pools;
using RemoteAdmin;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009C5 RID: 2501
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(ClientCommandHandler))]
	public class SrvCfgCommand : ICommand
	{
		// Token: 0x17000B5F RID: 2911
		// (get) Token: 0x06003883 RID: 14467 RVA: 0x000B7986 File Offset: 0x000B5B86
		public string Command { get; } = "srvcfg";

		// Token: 0x17000B60 RID: 2912
		// (get) Token: 0x06003884 RID: 14468 RVA: 0x000B798E File Offset: 0x000B5B8E
		public string[] Aliases { get; }

		// Token: 0x17000B61 RID: 2913
		// (get) Token: 0x06003885 RID: 14469 RVA: 0x000B7996 File Offset: 0x000B5B96
		public string Description { get; } = "Displays the server config.";

		// Token: 0x06003886 RID: 14470 RVA: 0x000B79A0 File Offset: 0x000B5BA0
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			bool result;
			try
			{
				YamlConfig serverConfig = ConfigFile.ServerConfig;
				PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
				if (playerCommandSender == null || playerCommandSender.ReferenceHub.authManager.BypassBansFlagSet || playerCommandSender.ReferenceHub.isLocalPlayer || PermissionsHandler.IsPermitted(playerCommandSender.ReferenceHub.serverRoles.Permissions, PlayerPermissions.ServerConsoleCommands | PlayerPermissions.ServerConfigs))
				{
					stringBuilder.AppendLine("Extended server configuration:");
					stringBuilder.AppendFormat("Server name: {0}\n", serverConfig.GetString("server_name", ""));
					stringBuilder.AppendFormat("Server IP: {0}\n", serverConfig.GetString("server_ip", ""));
					stringBuilder.AppendFormat("Current Server IP: {0}\n", ServerConsole.Ip);
					stringBuilder.AppendFormat("Server port: {0}\n", LiteNetLib4MirrorTransport.Singleton.port);
					stringBuilder.AppendFormat("Server pastebin ID: {0}\n", serverConfig.GetString("serverinfo_pastebin_id", ""));
					stringBuilder.AppendFormat("Server max players: {0}\n", serverConfig.GetInt("max_players", 0));
					stringBuilder.AppendFormat("Online mode: {0}\n", PlayerAuthenticationManager.OnlineMode);
					stringBuilder.AppendFormat("RA password authentication: {0}\n", ReferenceHub.HostHub.queryProcessor.OverridePasswordEnabled);
					stringBuilder.AppendFormat("IP banning: {0}\n", serverConfig.GetBool("ip_banning", false));
					stringBuilder.AppendFormat("Whitelist: {0}\n", serverConfig.GetBool("enable_whitelist", false));
					stringBuilder.AppendFormat("Query status: {0} with port shift {1}\n", serverConfig.GetBool("enable_query", false), serverConfig.GetInt("query_port_shift", 0));
					stringBuilder.AppendFormat("Friendly fire: {0}\n", ServerConsole.FriendlyFire);
					stringBuilder.AppendFormat("Map seed: {0}\n", serverConfig.GetInt("map_seed", 0));
				}
				else
				{
					stringBuilder.AppendLine("Basic server configuration:");
					stringBuilder.AppendFormat("Server name: {0}\n", serverConfig.GetString("server_name", ""));
					stringBuilder.AppendFormat("Server pastebin ID: {0}\n", serverConfig.GetString("serverinfo_pastebin_id", ""));
					stringBuilder.AppendFormat("Server max players: {0}\n", serverConfig.GetInt("max_players", 0));
					stringBuilder.AppendFormat("RA password authentication: {0}\n", ReferenceHub.HostHub.queryProcessor.OverridePasswordEnabled);
					stringBuilder.AppendFormat("Online mode: {0}\n", PlayerAuthenticationManager.OnlineMode);
					stringBuilder.AppendFormat("Whitelist: {0}\n", serverConfig.GetBool("enable_whitelist", false));
					stringBuilder.AppendFormat("Friendly fire: {0}\n", ServerConsole.FriendlyFire);
				}
				result = true;
			}
			finally
			{
				response = StringBuilderPool.Shared.ToStringReturn(stringBuilder);
			}
			return result;
		}
	}
}
