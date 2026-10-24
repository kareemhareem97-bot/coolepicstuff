using System;
using System.Text;
using Mirror;
using NorthwoodLib.Pools;
using RemoteAdmin;

namespace CommandSystem.Commands.Shared
{
	// Token: 0x020009BA RID: 2490
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class PermCommand : ICommand
	{
		// Token: 0x17000B41 RID: 2881
		// (get) Token: 0x0600384A RID: 14410 RVA: 0x000B6ED7 File Offset: 0x000B50D7
		public string Command { get; } = "perm";

		// Token: 0x17000B42 RID: 2882
		// (get) Token: 0x0600384B RID: 14411 RVA: 0x000B6EDF File Offset: 0x000B50DF
		public string[] Aliases { get; }

		// Token: 0x17000B43 RID: 2883
		// (get) Token: 0x0600384C RID: 14412 RVA: 0x000B6EE7 File Offset: 0x000B50E7
		public string Description { get; } = "Lists your permissions.";

		// Token: 0x0600384D RID: 14413 RVA: 0x000B6EF0 File Offset: 0x000B50F0
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			CommandSender commandSender = sender as CommandSender;
			if (commandSender == null)
			{
				response = "You can't use this command (you don't have any permissions set)";
				return false;
			}
			if (!NetworkServer.active)
			{
				response = "This command can only be used on a server.";
				return false;
			}
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			string arg;
			if (playerCommandSender != null)
			{
				arg = ((playerCommandSender.ReferenceHub.serverRoles.Group == null) ? "0" : playerCommandSender.ReferenceHub.serverRoles.Group.RequiredKickPower.ToString());
			}
			else
			{
				arg = "N/A";
			}
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			bool result;
			try
			{
				ulong permissions = commandSender.Permissions;
				stringBuilder.AppendFormat("Your kick power: {0}\nKick power required to kick you: {1}\nYour permissions:", commandSender.KickPower, arg);
				foreach (string text in ServerStatic.PermissionsHandler.GetAllPermissions())
				{
					string text2 = ServerStatic.PermissionsHandler.IsRaPermitted(ServerStatic.PermissionsHandler.GetPermissionValue(text)) ? "*" : "";
					stringBuilder.AppendFormat("\n{0}{1} ({2}): {3}", new object[]
					{
						text,
						text2,
						ServerStatic.PermissionsHandler.GetPermissionValue(text),
						ServerStatic.PermissionsHandler.IsPermitted(permissions, text) ? "YES" : "NO"
					});
				}
				response = stringBuilder.ToString();
				result = true;
			}
			finally
			{
				StringBuilderPool.Shared.Return(stringBuilder);
			}
			return result;
		}
	}
}
