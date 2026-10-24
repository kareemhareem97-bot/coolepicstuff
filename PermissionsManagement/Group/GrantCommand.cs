using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group
{
	// Token: 0x02000A47 RID: 2631
	[CommandHandler(typeof(GroupCommand))]
	public class GrantCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000CF7 RID: 3319
		// (get) Token: 0x06003B71 RID: 15217 RVA: 0x000C3D4E File Offset: 0x000C1F4E
		public string Command { get; } = "grant";

		// Token: 0x17000CF8 RID: 3320
		// (get) Token: 0x06003B72 RID: 15218 RVA: 0x000C3D56 File Offset: 0x000C1F56
		public string[] Aliases { get; }

		// Token: 0x17000CF9 RID: 3321
		// (get) Token: 0x06003B73 RID: 15219 RVA: 0x000C3D5E File Offset: 0x000C1F5E
		public string Description { get; } = "Grants a permission to a group.";

		// Token: 0x17000CFA RID: 3322
		// (get) Token: 0x06003B74 RID: 15220 RVA: 0x000C3D66 File Offset: 0x000C1F66
		public string[] Usage { get; } = new string[]
		{
			"Group Name",
			"Permission Name"
		};

		// Token: 0x06003B75 RID: 15221 RVA: 0x000C3D70 File Offset: 0x000C1F70
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
			if (list.Contains(text2))
			{
				response = "Group already has that permission.";
				return false;
			}
			list.Add(text2);
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
				" granted permission ",
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
