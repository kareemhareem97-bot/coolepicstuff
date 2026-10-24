using System;
using System.Collections.Generic;
using System.Linq;
using PlayerRoles.FirstPersonControl;
using UnityEngine;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A14 RID: 2580
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class ScaleCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000C5D RID: 3165
		// (get) Token: 0x06003A4D RID: 14925 RVA: 0x000C052F File Offset: 0x000BE72F
		public string Command { get; } = "scalechange";

		// Token: 0x17000C5E RID: 3166
		// (get) Token: 0x06003A4E RID: 14926 RVA: 0x000C0537 File Offset: 0x000BE737
		public string[] Aliases { get; } = new string[]
		{
			"changescale"
		};

		// Token: 0x17000C5F RID: 3167
		// (get) Token: 0x06003A4F RID: 14927 RVA: 0x000C053F File Offset: 0x000BE73F
		public string Description { get; } = "Changes scale of specified player(s). Affects hitboxes.";

		// Token: 0x17000C60 RID: 3168
		// (get) Token: 0x06003A50 RID: 14928 RVA: 0x000C0547 File Offset: 0x000BE747
		public string[] Usage { get; } = new string[]
		{
			"%player%",
			"X",
			"Y",
			"Z"
		};

		// Token: 0x06003A51 RID: 14929 RVA: 0x000C0550 File Offset: 0x000BE750
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (arguments.Count < 4)
			{
				response = "Please enter 4 arguments.";
				return true;
			}
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			if (list.Count == 0)
			{
				response = "No player(s) found";
				return false;
			}
			float x;
			if (!float.TryParse(arguments.ElementAt(1), out x))
			{
				response = "Unable to parse " + arguments.ElementAt(1) + " for X scale";
				return false;
			}
			float y;
			if (!float.TryParse(arguments.ElementAt(2), out y))
			{
				response = "Unable to parse " + arguments.ElementAt(2) + " for Y scale";
				return false;
			}
			float z;
			if (!float.TryParse(arguments.ElementAt(3), out z))
			{
				response = "Unable to parse " + arguments.ElementAt(3) + " for Z scale";
				return false;
			}
			Vector3 scale = new Vector3(x, y, z);
			int num = 0;
			foreach (ReferenceHub referenceHub in list)
			{
				IFpcRole fpcRole = referenceHub.roleManager.CurrentRole as IFpcRole;
				if (fpcRole != null)
				{
					fpcRole.FpcModule.Motor.ScaleController.Scale = scale;
					num++;
				}
			}
			response = string.Format("Sucessfully scaled {0}/{1} player{2}", num, list.Count, (list.Count > 1) ? "s" : string.Empty);
			return true;
		}
	}
}
