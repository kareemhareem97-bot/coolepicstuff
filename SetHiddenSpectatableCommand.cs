using System;
using System.Collections.Generic;
using System.Linq;
using PlayerRoles.Spectating;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A1D RID: 2589
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class SetHiddenSpectatableCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000C81 RID: 3201
		// (get) Token: 0x06003A86 RID: 14982 RVA: 0x000C133D File Offset: 0x000BF53D
		public string Command
		{
			get
			{
				return "setspectatable";
			}
		}

		// Token: 0x17000C82 RID: 3202
		// (get) Token: 0x06003A87 RID: 14983 RVA: 0x000C1344 File Offset: 0x000BF544
		public string[] Aliases
		{
			get
			{
				return new string[]
				{
					"setspectvis",
					"hidespectator",
					"hidespec"
				};
			}
		}

		// Token: 0x17000C83 RID: 3203
		// (get) Token: 0x06003A88 RID: 14984 RVA: 0x000C1364 File Offset: 0x000BF564
		public string Description
		{
			get
			{
				return "Forcefully hides a player from a spectator list";
			}
		}

		// Token: 0x17000C84 RID: 3204
		// (get) Token: 0x06003A89 RID: 14985 RVA: 0x000C136B File Offset: 0x000BF56B
		public string[] Usage
		{
			get
			{
				return new string[]
				{
					"%player%",
					"Hidden (True/False)"
				};
			}
		}

		// Token: 0x06003A8A RID: 14986 RVA: 0x000C1384 File Offset: 0x000BF584
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission((PlayerPermissions)(-2147483648)))
			{
				response = "You don't have enough permissions to run this command!";
				return false;
			}
			if (arguments.Count != 2)
			{
				response = "Please enter all required arguments!";
				return false;
			}
			bool flag;
			if (!bool.TryParse(arguments.ElementAt(1), out flag))
			{
				response = "Please provide visibility option as true/false";
				return false;
			}
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			if (list == null)
			{
				response = "An unexpected problem has occurred during PlayerId/Name array processing.";
				return false;
			}
			if (array == null)
			{
				response = "An error occured while processing this command.\nUsage: " + this.DisplayCommandUsage();
				return false;
			}
			foreach (ReferenceHub hub in list)
			{
				SpectatableVisibilityManager.SetHidden(hub, flag);
			}
			response = string.Format("Set new hidden state of {0} players to {1}", list.Count, flag);
			return true;
		}
	}
}
