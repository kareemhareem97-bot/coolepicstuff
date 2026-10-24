using System;
using System.Collections.Generic;
using System.Linq;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;

namespace CommandSystem.Commands.RemoteAdmin.Doors
{
	// Token: 0x02000A64 RID: 2660
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class DoorsListCommand : ICommand
	{
		// Token: 0x17000D53 RID: 3411
		// (get) Token: 0x06003C1D RID: 15389 RVA: 0x000C675E File Offset: 0x000C495E
		public string Command { get; } = "doorslist";

		// Token: 0x17000D54 RID: 3412
		// (get) Token: 0x06003C1E RID: 15390 RVA: 0x000C6766 File Offset: 0x000C4966
		public string[] Aliases { get; } = new string[]
		{
			"doors",
			"dl"
		};

		// Token: 0x17000D55 RID: 3413
		// (get) Token: 0x06003C1F RID: 15391 RVA: 0x000C676E File Offset: 0x000C496E
		public string Description { get; } = "Lists all valid door names.";

		// Token: 0x06003C20 RID: 15392 RVA: 0x000C6778 File Offset: 0x000C4978
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			string text = "List of named doors in the facility:\n";
			List<string> list = (from item in DoorNametagExtension.NamedDoors.Values.ToArray<DoorNametagExtension>()
			where !string.IsNullOrEmpty(item.GetName)
			select item).Select(delegate(DoorNametagExtension item)
			{
				string[] array = new string[5];
				array[0] = item.GetName;
				array[1] = " - ";
				array[2] = (item.TargetDoor.TargetState ? "<color=green>OPENED</color>" : "<color=orange>CLOSED</color>");
				array[3] = ((item.TargetDoor.ActiveLocks > 0) ? " <color=red>[LOCKED]</color>" : "");
				int num = 4;
				BasicDoor basicDoor = item.TargetDoor as BasicDoor;
				string text2;
				if (basicDoor == null || basicDoor.RequiredPermissions.RequiredPermissions <= DoorPermissionFlags.None)
				{
					CheckpointDoor checkpointDoor = item.TargetDoor as CheckpointDoor;
					if (checkpointDoor == null || checkpointDoor.RequiredPermissions.RequiredPermissions <= DoorPermissionFlags.None)
					{
						text2 = "";
						goto IL_98;
					}
				}
				text2 = " <color=blue>[CARD REQUIRED]</color>";
				IL_98:
				array[num] = text2;
				return string.Concat(array);
			}).ToList<string>();
			list.Sort();
			text += list.Aggregate((string current, string adding) => current + "\n" + adding);
			response = text;
			return true;
		}
	}
}
