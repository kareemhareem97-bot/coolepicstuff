using System;
using System.Collections.Generic;
using System.Text;
using PlayerRoles;

namespace CommandSystem.Commands.Console
{
	// Token: 0x02000AA0 RID: 2720
	[CommandHandler(typeof(GameConsoleCommandHandler))]
	public class RoleListCommand : ICommand
	{
		// Token: 0x17000DEC RID: 3564
		// (get) Token: 0x06003D4D RID: 15693 RVA: 0x000C96E5 File Offset: 0x000C78E5
		public string Command { get; } = "rolelist";

		// Token: 0x17000DED RID: 3565
		// (get) Token: 0x06003D4E RID: 15694 RVA: 0x000C96ED File Offset: 0x000C78ED
		public string[] Aliases { get; }

		// Token: 0x17000DEE RID: 3566
		// (get) Token: 0x06003D4F RID: 15695 RVA: 0x000C96F5 File Offset: 0x000C78F5
		public string Description { get; } = "Display a list of roles.";

		// Token: 0x06003D50 RID: 15696 RVA: 0x000C9700 File Offset: 0x000C7900
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("<size=25>List of role:</size>");
			foreach (KeyValuePair<RoleTypeId, PlayerRoleBase> keyValuePair in PlayerRoleLoader.AllRoles)
			{
				PlayerRoleBase value = keyValuePair.Value;
				IHiddenRole hiddenRole = value as IHiddenRole;
				if (hiddenRole == null || !hiddenRole.IsHidden)
				{
					stringBuilder.AppendLine(string.Format("Role #{0:000} : {1} - \"{2}\"", (int)keyValuePair.Key, keyValuePair.Key, value.RoleName));
				}
			}
			response = stringBuilder.ToString();
			return true;
		}
	}
}
