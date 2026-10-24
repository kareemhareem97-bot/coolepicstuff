using System;

namespace CommandSystem.Commands.RemoteAdmin.PermissionsManagement.Group
{
	// Token: 0x02000A49 RID: 2633
	[CommandHandler(typeof(GroupCommand))]
	public class InfoCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000CFE RID: 3326
		// (get) Token: 0x06003B7E RID: 15230 RVA: 0x000C4074 File Offset: 0x000C2274
		public string Command { get; } = "info";

		// Token: 0x17000CFF RID: 3327
		// (get) Token: 0x06003B7F RID: 15231 RVA: 0x000C407C File Offset: 0x000C227C
		public string[] Aliases { get; }

		// Token: 0x17000D00 RID: 3328
		// (get) Token: 0x06003B80 RID: 15232 RVA: 0x000C4084 File Offset: 0x000C2284
		public string Description { get; } = "Displays group info.";

		// Token: 0x17000D01 RID: 3329
		// (get) Token: 0x06003B81 RID: 15233 RVA: 0x000C408C File Offset: 0x000C228C
		public string[] Usage { get; } = new string[]
		{
			"Group Name"
		};

		// Token: 0x06003B82 RID: 15234 RVA: 0x000C4094 File Offset: 0x000C2294
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PermissionsManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			UserGroup group = ServerStatic.PermissionsHandler.GetGroup(arguments.At(0));
			if (group == null)
			{
				response = "Group can't be found.";
				return false;
			}
			response = string.Format("Details of group {0}\nTag text: {1}\nTag color: {2}\nPermissions: {3}\nCover: {4}\nHidden by default: {5}\nKick power: {6}\nRequired kick power: {7}", new object[]
			{
				arguments.At(0),
				group.BadgeText,
				group.BadgeColor,
				group.Permissions,
				group.Cover ? "YES" : "NO",
				group.HiddenByDefault ? "YES" : "NO",
				group.KickPower,
				group.RequiredKickPower
			});
			return true;
		}
	}
}
