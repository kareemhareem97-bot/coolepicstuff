using System;
using System.Collections.Generic;
using System.Text;
using NorthwoodLib.Pools;

namespace CommandSystem.Commands.RemoteAdmin.PermissionsManagement
{
	// Token: 0x02000A44 RID: 2628
	[CommandHandler(typeof(PermissionsManagementCommand))]
	public class UsersCommand : ICommand
	{
		// Token: 0x17000CEC RID: 3308
		// (get) Token: 0x06003B60 RID: 15200 RVA: 0x000C3995 File Offset: 0x000C1B95
		public string Command { get; } = "users";

		// Token: 0x17000CED RID: 3309
		// (get) Token: 0x06003B61 RID: 15201 RVA: 0x000C399D File Offset: 0x000C1B9D
		public string[] Aliases { get; }

		// Token: 0x17000CEE RID: 3310
		// (get) Token: 0x06003B62 RID: 15202 RVA: 0x000C39A5 File Offset: 0x000C1BA5
		public string Description { get; } = "List all the users that are assigned to any group.";

		// Token: 0x06003B63 RID: 15203 RVA: 0x000C39B0 File Offset: 0x000C1BB0
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PermissionsManagement, out response))
			{
				return false;
			}
			Dictionary<string, string> stringDictionary = ServerStatic.RolesConfig.GetStringDictionary("Members");
			YamlConfig sharedGroupsMembersConfig = ServerStatic.SharedGroupsMembersConfig;
			Dictionary<string, string> dictionary = (sharedGroupsMembersConfig != null) ? sharedGroupsMembersConfig.GetStringDictionary("SharedMembers") : null;
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent("Players with assigned groups:");
			foreach (KeyValuePair<string, string> keyValuePair in stringDictionary)
			{
				stringBuilder.Append("\n" + keyValuePair.Key + " - " + keyValuePair.Value);
			}
			if (dictionary != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair2 in dictionary)
				{
					stringBuilder.Append(string.Concat(new string[]
					{
						"\n",
						keyValuePair2.Key,
						" - ",
						keyValuePair2.Value,
						" <color=#FFD700>[SHARED MEMBERSHIP]</color>"
					}));
				}
			}
			response = stringBuilder.ToString();
			StringBuilderPool.Shared.Return(stringBuilder);
			return true;
		}
	}
}
