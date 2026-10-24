using System;
using System.Linq;

namespace CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group
{
	// Token: 0x02000A4B RID: 2635
	[CommandHandler(typeof(GroupCommand))]
	public class SetColorCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000D06 RID: 3334
		// (get) Token: 0x06003B8A RID: 15242 RVA: 0x000C441E File Offset: 0x000C261E
		public string Command { get; } = "setcolor";

		// Token: 0x17000D07 RID: 3335
		// (get) Token: 0x06003B8B RID: 15243 RVA: 0x000C4426 File Offset: 0x000C2626
		public string[] Aliases { get; }

		// Token: 0x17000D08 RID: 3336
		// (get) Token: 0x06003B8C RID: 15244 RVA: 0x000C442E File Offset: 0x000C262E
		public string Description { get; } = "Sets the badge color for a group.";

		// Token: 0x17000D09 RID: 3337
		// (get) Token: 0x06003B8D RID: 15245 RVA: 0x000C4436 File Offset: 0x000C2636
		public string[] Usage { get; } = new string[]
		{
			"Group Name",
			"Color"
		};

		// Token: 0x06003B8E RID: 15246 RVA: 0x000C4440 File Offset: 0x000C2640
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PermissionsManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 2)
			{
				response = "To execute this command provide at least 2 arguments!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			string text = arguments.At(0);
			string colorName = arguments.At(1);
			if (ServerStatic.PermissionsHandler.GetGroup(text) == null)
			{
				response = "Group can't be found.";
				return false;
			}
			ServerRoles.NamedColor namedColor = ReferenceHub.LocalHub.serverRoles.NamedColors.SingleOrDefault((ServerRoles.NamedColor c) => c.Name == colorName);
			if (namedColor == null)
			{
				response = "The entered color was invalid! Type \"colors\" for a list of available colors.";
				return false;
			}
			if (namedColor.Restricted)
			{
				response = "The color you have chosen is restricted to global badges only.";
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Concat(new string[]
			{
				sender.LogName,
				" set color of group ",
				text,
				" to ",
				colorName,
				"."
			}), ServerLogs.ServerLogType.RemoteAdminActivity_Misc, false);
			ServerStatic.RolesConfig.SetString(text + "_color", colorName);
			ServerStatic.PermissionsHandler = new PermissionsHandler(ref ServerStatic.RolesConfig, ref ServerStatic.SharedGroupsConfig, ref ServerStatic.SharedGroupsMembersConfig);
			response = "Group color updated to: " + colorName;
			return true;
		}
	}
}
