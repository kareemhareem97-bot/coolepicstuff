using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009E0 RID: 2528
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class DisplayNameCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000BB6 RID: 2998
		// (get) Token: 0x06003914 RID: 14612 RVA: 0x000BA3AD File Offset: 0x000B85AD
		public string Command { get; } = "name";

		// Token: 0x17000BB7 RID: 2999
		// (get) Token: 0x06003915 RID: 14613 RVA: 0x000BA3B5 File Offset: 0x000B85B5
		public string[] Aliases { get; } = new string[]
		{
			"nickname",
			"nick"
		};

		// Token: 0x17000BB8 RID: 3000
		// (get) Token: 0x06003916 RID: 14614 RVA: 0x000BA3BD File Offset: 0x000B85BD
		public string Description { get; } = "Displays player's name.";

		// Token: 0x17000BB9 RID: 3001
		// (get) Token: 0x06003917 RID: 14615 RVA: 0x000BA3C5 File Offset: 0x000B85C5
		public string[] Usage { get; } = new string[]
		{
			"%player%"
		};

		// Token: 0x06003918 RID: 14616 RVA: 0x000BA3D0 File Offset: 0x000B85D0
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			if (list == null)
			{
				response = "Cannot find player! Try using the player ID!";
				return false;
			}
			if (list.Count == 1)
			{
				response = "Player " + arguments.At(0) + "'s name is: " + list[0].nicknameSync.CombinedName;
				return true;
			}
			response = list.Aggregate("Display names of specified players:\n", (string current, ReferenceHub hub) => current + " - " + hub.nicknameSync.CombinedName + "\"");
			return true;
		}
	}
}
