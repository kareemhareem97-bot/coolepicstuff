using System;

namespace CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group
{
	// Token: 0x02000A4D RID: 2637
	[CommandHandler(typeof(GroupCommand))]
	public class SetTagCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000D0A RID: 3338
		// (get) Token: 0x06003B92 RID: 15250 RVA: 0x000C45CE File Offset: 0x000C27CE
		public string Command { get; } = "settag";

		// Token: 0x17000D0B RID: 3339
		// (get) Token: 0x06003B93 RID: 15251 RVA: 0x000C45D6 File Offset: 0x000C27D6
		public string[] Aliases { get; } = new string[]
		{
			"setag"
		};

		// Token: 0x17000D0C RID: 3340
		// (get) Token: 0x06003B94 RID: 15252 RVA: 0x000C45DE File Offset: 0x000C27DE
		public string Description { get; } = "Sets the badge text for a group.";

		// Token: 0x17000D0D RID: 3341
		// (get) Token: 0x06003B95 RID: 15253 RVA: 0x000C45E6 File Offset: 0x000C27E6
		public string[] Usage { get; } = new string[]
		{
			"Group Name",
			"Tag"
		};

		// Token: 0x06003B96 RID: 15254 RVA: 0x000C45F0 File Offset: 0x000C27F0
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
			string value = arguments.At(1);
			if (ServerStatic.PermissionsHandler.GetGroup(text) == null)
			{
				response = "Group can't be found.";
				return false;
			}
			ServerStatic.RolesConfig.SetString(text + "_badge", value);
			ServerStatic.PermissionsHandler = new PermissionsHandler(ref ServerStatic.RolesConfig, ref ServerStatic.SharedGroupsConfig, ref ServerStatic.SharedGroupsMembersConfig);
			response = "Group tag updated.";
			return true;
		}
	}
}
