using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandSystem.Commands.RemoteAdmin;
using GameCore;
using NorthwoodLib.Pools;
using RemoteAdmin;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009B3 RID: 2483
	[CommandHandler(typeof(ClientCommandHandler))]
	[CommandHandler(typeof(PermissionsManagementCommand))]
	public class GroupsCommand : ICommand
	{
		// Token: 0x17000B2E RID: 2862
		// (get) Token: 0x06003828 RID: 14376 RVA: 0x000B6711 File Offset: 0x000B4911
		public string Command { get; } = "groups";

		// Token: 0x17000B2F RID: 2863
		// (get) Token: 0x06003829 RID: 14377 RVA: 0x000B6719 File Offset: 0x000B4919
		public string[] Aliases { get; }

		// Token: 0x17000B30 RID: 2864
		// (get) Token: 0x0600382A RID: 14378 RVA: 0x000B6721 File Offset: 0x000B4921
		public string Description { get; } = "Lists all defined server groups.";

		// Token: 0x0600382B RID: 14379 RVA: 0x000B672C File Offset: 0x000B492C
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			if (playerCommandSender != null && !playerCommandSender.ReferenceHub.authManager.BypassBansFlagSet && !playerCommandSender.ReferenceHub.isLocalPlayer && !sender.CheckPermission(PlayerPermissions.PermissionsManagement, out response))
			{
				response = "You don't have permissions to execute this command.";
				return false;
			}
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent("Groups defined on this server:");
			try
			{
				Dictionary<string, UserGroup> allGroups = ServerStatic.PermissionsHandler.GetAllGroups();
				ServerRoles.NamedColor[] namedColors = ReferenceHub.LocalHub.serverRoles.NamedColors;
				foreach (KeyValuePair<string, UserGroup> keyValuePair in allGroups)
				{
					string key = keyValuePair.Key;
					UserGroup group = keyValuePair.Value;
					try
					{
						ServerRoles.NamedColor namedColor = namedColors.FirstOrDefault((ServerRoles.NamedColor x) => x.Name == group.BadgeColor);
						string text = (namedColor != null) ? namedColor.ColorHex : null;
						stringBuilder.AppendFormat("\n{0} ({1}) - <color=#{2}>{3}</color> in color {4}", new object[]
						{
							key,
							group.Permissions,
							text,
							group.BadgeText,
							group.BadgeColor
						});
					}
					catch
					{
						stringBuilder.AppendFormat("\n{0} ({1}) - {2} in color {3}", new object[]
						{
							key,
							group.Permissions,
							group.BadgeText,
							group.BadgeColor
						});
					}
					foreach (KeyValuePair<PlayerPermissions, string> keyValuePair2 in PermissionsHandler.PermissionCodes)
					{
						if (PermissionsHandler.IsPermitted(keyValuePair.Value.Permissions, keyValuePair2.Key))
						{
							stringBuilder.AppendFormat("<color={0}> {1}</color>", keyValuePair2.Key.GetPermissionType().GetPermissionTypeColor(), keyValuePair2.Value);
						}
					}
				}
				response = stringBuilder.ToString();
			}
			finally
			{
				StringBuilderPool.Shared.Return(stringBuilder);
			}
			return true;
		}
	}
}
