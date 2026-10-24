using System;
using System.Collections.Generic;
using RemoteAdmin;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009F8 RID: 2552
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class KeyCommand : ICommand, IHiddenCommand
	{
		// Token: 0x17000C02 RID: 3074
		// (get) Token: 0x060039A5 RID: 14757 RVA: 0x000BD67D File Offset: 0x000BB87D
		public string Command { get; } = "enkey";

		// Token: 0x17000C03 RID: 3075
		// (get) Token: 0x060039A6 RID: 14758 RVA: 0x000BD685 File Offset: 0x000BB885
		public string[] Aliases { get; }

		// Token: 0x17000C04 RID: 3076
		// (get) Token: 0x060039A7 RID: 14759 RVA: 0x000BD68D File Offset: 0x000BB88D
		public string Description { get; } = "Displays an encryption key of a selected player(s).";

		// Token: 0x17000C05 RID: 3077
		// (get) Token: 0x060039A8 RID: 14760 RVA: 0x000BD695 File Offset: 0x000BB895
		public string[] Usage { get; } = new string[]
		{
			"%player%"
		};

		// Token: 0x060039A9 RID: 14761 RVA: 0x000BD6A0 File Offset: 0x000BB8A0
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.ServerConsoleCommands, out response))
			{
				return false;
			}
			if (!sender.CheckPermission(PlayerPermissions.PermissionsManagement, out response))
			{
				return false;
			}
			if (!sender.CheckPermission(PlayerPermissions.PlayerSensitiveDataAccess, out response))
			{
				return false;
			}
			if (!sender.CheckPermission(PlayerPermissions.ServerConfigs, out response))
			{
				return false;
			}
			if (!EncryptedChannelManager.CryptographyDebug)
			{
				response = "This command is disabled on this server!\nDO NOT ENABLE IT if you don't know what you are doing!\nEnabling this command is a SECURITY RISK. This is meant only for debugging purposes!";
				return false;
			}
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			List<ReferenceHub> list;
			if (playerCommandSender != null && (arguments.Count == 0 || (arguments.Count == 1 && !arguments.At(0).Contains(".") && !arguments.At(0).Contains("@"))))
			{
				list = new List<ReferenceHub>
				{
					playerCommandSender.ReferenceHub
				};
			}
			else
			{
				string[] array;
				list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			}
			if (list == null || list.Count == 0)
			{
				response = "No players specified!";
				return false;
			}
			response = "Encryption keys:\n";
			foreach (ReferenceHub referenceHub in list)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " displayed encryption key of " + referenceHub.LoggedNameFromRefHub() + ". THIS SHOULD NOT HAPPEN IF IT'S NOT A TEST! THIS IS A SECURITY RISK!", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
				response = string.Concat(new string[]
				{
					response,
					referenceHub.LoggedNameFromRefHub(),
					": ",
					(referenceHub.encryptedChannelManager.EncryptionKey == null) ? "(null)" : BitConverter.ToString(referenceHub.encryptedChannelManager.EncryptionKey),
					"\n"
				});
			}
			return true;
		}
	}
}
