using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group
{
	// Token: 0x02000A4A RID: 2634
	[CommandHandler(typeof(GroupCommand))]
	public class RevokeCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000D02 RID: 3330
		// (get) Token: 0x06003B84 RID: 15236 RVA: 0x000C41B5 File Offset: 0x000C23B5
		public string Command { get; } = "revoke";

		// Token: 0x17000D03 RID: 3331
		// (get) Token: 0x06003B85 RID: 15237 RVA: 0x000C41BD File Offset: 0x000C23BD
		public string[] Aliases { get; }

		// Token: 0x17000D04 RID: 3332
		// (get) Token: 0x06003B86 RID: 15238 RVA: 0x000C41C5 File Offset: 0x000C23C5
		public string Description { get; } = "Revokes a permission group from a group.";

		// Token: 0x17000D05 RID: 3333
		// (get) Token: 0x06003B87 RID: 15239 RVA: 0x000C41CD File Offset: 0x000C23CD
		public string[] Usage { get; } = new string[]
		{
			"Group Name",
			"Permission Name"
		};

		// Token: 0x06003B88 RID: 15240 RVA: 0x000C41D8 File Offset: 0x000C23D8
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
			string text2 = arguments.At(1);
			if (ServerStatic.PermissionsHandler.GetGroup(text) == null)
			{
				response = "Group can't be found.";
				return false;
			}
			if (!ServerStatic.PermissionsHandler.GetAllPermissions().Contains(text2))
			{
				response = "Permission can't be found.";
				return false;
			}
			Dictionary<string, string> stringDictionary = ServerStatic.RolesConfig.GetStringDictionary("Permissions");
			List<string> list = null;
			foreach (string text3 in stringDictionary.Keys)
			{
				if (!(text3 != text2))
				{
					list = YamlConfig.ParseCommaSeparatedString(stringDictionary[text3]).ToList<string>();
				}
			}
			if (list == null)
			{
				response = "Permissions can't be found in the config.";
				return false;
			}
			if (!list.Contains(text2))
			{
				response = "Group does not have that permission.";
				return false;
			}
			list.Remove(text2);
			list.Sort();
			string text4 = "[";
			foreach (string str in list)
			{
				if (text4 != "[")
				{
					text4 += ", ";
				}
				text4 += str;
			}
			text4 += "]";
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Concat(new string[]
			{
				sender.LogName,
				" revoked permission ",
				text2,
				" to group ",
				text,
				"."
			}), ServerLogs.ServerLogType.RemoteAdminActivity_Misc, false);
			ServerStatic.RolesConfig.SetStringDictionaryItem("Permissions", text2, text4);
			ServerStatic.PermissionsHandler = new PermissionsHandler(ref ServerStatic.RolesConfig, ref ServerStatic.SharedGroupsConfig, ref ServerStatic.SharedGroupsMembersConfig);
			response = "Permissions updated.";
			return true;
		}
	}
}
