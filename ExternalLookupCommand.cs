using System;
using System.Collections.Generic;
using MEC;
using RemoteAdmin;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009EB RID: 2539
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class ExternalLookupCommand : ICommand, IHiddenCommand
	{
		// Token: 0x17000BD5 RID: 3029
		// (get) Token: 0x06003954 RID: 14676 RVA: 0x000BB965 File Offset: 0x000B9B65
		public string Command { get; } = "externallookup";

		// Token: 0x17000BD6 RID: 3030
		// (get) Token: 0x06003955 RID: 14677 RVA: 0x000BB96D File Offset: 0x000B9B6D
		public string[] Aliases { get; }

		// Token: 0x17000BD7 RID: 3031
		// (get) Token: 0x06003956 RID: 14678 RVA: 0x000BB975 File Offset: 0x000B9B75
		public string Description { get; } = "Internal command for lookups in RA";

		// Token: 0x06003957 RID: 14679 RVA: 0x000BB980 File Offset: 0x000B9B80
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.BanningUpToDay | PlayerPermissions.LongTermBanning | PlayerPermissions.SetGroup | PlayerPermissions.PlayersManagement | PlayerPermissions.PermissionsManagement | PlayerPermissions.ViewHiddenBadges | PlayerPermissions.PlayerSensitiveDataAccess | PlayerPermissions.ViewHiddenGlobalBadges, out response))
			{
				return false;
			}
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			if (playerCommandSender == null)
			{
				response = "This command can only be executed by players!";
				return false;
			}
			string text = string.Empty;
			if (arguments.Count >= 1)
			{
				int playerId;
				if (!int.TryParse(arguments.At(0), out playerId))
				{
					response = "Invalid player id!";
					return false;
				}
				ReferenceHub referenceHub;
				if (!ReferenceHub.TryGetHub(playerId, out referenceHub))
				{
					response = "Invalid player id!";
					return false;
				}
				text = referenceHub.authManager.UserId;
			}
			string remoteAdminExternalPlayerLookupMode = ServerConfigSynchronizer.Singleton.RemoteAdminExternalPlayerLookupMode;
			if (remoteAdminExternalPlayerLookupMode == "fullauth")
			{
				Timing.RunCoroutine(this.AuthenticateWithExternalServer(playerCommandSender, text));
				response = "Initiated communication with external server.";
				return true;
			}
			if (!(remoteAdminExternalPlayerLookupMode == "urlonly"))
			{
				response = "Invalid mode or command disabled via config.";
				return false;
			}
			playerCommandSender.RaReply("%" + text + "%" + ServerConfigSynchronizer.Singleton.RemoteAdminExternalPlayerLookupURL, true, false, "");
			response = "Lookup success!";
			return true;
		}

		// Token: 0x06003958 RID: 14680 RVA: 0x000BBA72 File Offset: 0x000B9C72
		private IEnumerator<float> AuthenticateWithExternalServer(PlayerCommandSender ply, string tolookup)
		{
			using (HttpCoroutine http = HttpQuery.GetCoroutine(string.Concat(new string[]
			{
				ServerConfigSynchronizer.Singleton.RemoteAdminExternalPlayerLookupURL,
				"?user-id=",
				ply.ReferenceHub.authManager.UserId,
				"&user-ip=",
				ply.ReferenceHub.characterClassManager.connectionToClient.address,
				"&token=",
				ServerConfigSynchronizer.Singleton.RemoteAdminExternalPlayerLookupToken
			})))
			{
				yield return http.Wait();
				try
				{
					http.CheckResult();
					string text = http.GetText();
					if (!text.StartsWith("https://") && !text.StartsWith("http://"))
					{
						ServerConsole.AddLog("Error while performing external player lookup - URL Invalid.", ConsoleColor.Red, false);
						ply.RaReply("Error while processing reply from server - URL Invalid.", false, true, "");
						yield break;
					}
					if (text.Length > 2000)
					{
						ServerConsole.AddLog("Error while performing external player lookup - URL Too Long.", ConsoleColor.Red, false);
						ply.RaReply("Error while processing reply from server - URL Too Long.", false, true, "");
						yield break;
					}
					ply.RaReply("%" + tolookup + "%" + text, true, false, "");
				}
				catch (Exception ex)
				{
					ServerConsole.AddLog("Error while performing external player lookup: " + ex.Message + ".", ConsoleColor.Red, false);
					ply.RaReply("Error while processing reply from server " + ex.Message + ".", false, true, "");
				}
			}
			HttpCoroutine http = null;
			yield break;
			yield break;
		}
	}
}
