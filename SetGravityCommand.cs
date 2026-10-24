using System;
using System.Collections.Generic;
using System.Linq;
using PlayerRoles.FirstPersonControl;
using UnityEngine;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A1A RID: 2586
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class SetGravityCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000C75 RID: 3189
		// (get) Token: 0x06003A74 RID: 14964 RVA: 0x000C0D25 File Offset: 0x000BEF25
		public string Command
		{
			get
			{
				return "setgravity";
			}
		}

		// Token: 0x17000C76 RID: 3190
		// (get) Token: 0x06003A75 RID: 14965 RVA: 0x000B8060 File Offset: 0x000B6260
		public string[] Aliases
		{
			get
			{
				return Array.Empty<string>();
			}
		}

		// Token: 0x17000C77 RID: 3191
		// (get) Token: 0x06003A76 RID: 14966 RVA: 0x000C0D2C File Offset: 0x000BEF2C
		public string Description
		{
			get
			{
				return "Sets player(s) gravity to specified value";
			}
		}

		// Token: 0x17000C78 RID: 3192
		// (get) Token: 0x06003A77 RID: 14967 RVA: 0x000C0D33 File Offset: 0x000BEF33
		public string[] Usage
		{
			get
			{
				return new string[]
				{
					"%player%",
					"X",
					"Y",
					"Z"
				};
			}
		}

		// Token: 0x06003A78 RID: 14968 RVA: 0x000C0D5C File Offset: 0x000BEF5C
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (arguments.Count < 4)
			{
				response = "To execute this command provide 4 arguments!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			if (list.Count == 0)
			{
				response = "To execute this command provide 4 arguments!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			float x;
			if (!float.TryParse(arguments.ElementAt(1), out x))
			{
				response = "Unable to parse " + arguments.ElementAt(1) + " to a decimal number for X value";
				return false;
			}
			float y;
			if (!float.TryParse(arguments.ElementAt(2), out y))
			{
				response = "Unable to parse " + arguments.ElementAt(2) + " to a decimal number for Y value";
				return false;
			}
			float z;
			if (!float.TryParse(arguments.ElementAt(3), out z))
			{
				response = "Unable to parse " + arguments.ElementAt(3) + " to a decimal number for Z value";
				return false;
			}
			Vector3 vector = new Vector3(x, y, z);
			foreach (ReferenceHub hub in list)
			{
				FpcGravityController.ServerSetGravity(hub, vector);
			}
			response = string.Format("Set gravity of {0} player{1} to {2}", list.Count, (list.Count != 1) ? "s" : "", vector);
			return true;
		}
	}
}
