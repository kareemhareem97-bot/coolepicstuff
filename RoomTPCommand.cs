using System;
using System.Collections.Generic;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using NorthwoodLib.Pools;
using PlayerRoles.FirstPersonControl;
using RemoteAdmin;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x02000A10 RID: 2576
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class RoomTPCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000C52 RID: 3154
		// (get) Token: 0x06003A3A RID: 14906 RVA: 0x000C00BD File Offset: 0x000BE2BD
		public string Command { get; } = "roomtp";

		// Token: 0x17000C53 RID: 3155
		// (get) Token: 0x06003A3B RID: 14907 RVA: 0x000C00C5 File Offset: 0x000BE2C5
		public string[] Aliases { get; } = new string[]
		{
			"rtp",
			"ridtp"
		};

		// Token: 0x17000C54 RID: 3156
		// (get) Token: 0x06003A3C RID: 14908 RVA: 0x000C00CD File Offset: 0x000BE2CD
		public string Description { get; } = "Teleports you to a room.";

		// Token: 0x17000C55 RID: 3157
		// (get) Token: 0x06003A3D RID: 14909 RVA: 0x000C00D5 File Offset: 0x000BE2D5
		public string[] Usage { get; } = new string[]
		{
			"RoomID"
		};

		// Token: 0x06003A3E RID: 14910 RVA: 0x000C00E0 File Offset: 0x000BE2E0
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!(sender is PlayerCommandSender))
			{
				response = "Only players can run this command.";
				return false;
			}
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			RoomName roomName;
			if (!Enum.TryParse<RoomName>(arguments.At(0), true, out roomName) || roomName == RoomName.Unnamed)
			{
				response = "Room not defined.";
				return false;
			}
			List<Vector3> list = ListPool<Vector3>.Shared.Rent();
			using (HashSet<RoomIdentifier>.Enumerator enumerator = RoomIdentifier.AllRoomIdentifiers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					RoomIdentifier rid = enumerator.Current;
					if (rid.Name == roomName)
					{
						Vector3 position = rid.transform.position;
						DoorVariant doorVariant;
						if (!DoorVariant.AllDoors.TryGetFirst(delegate(DoorVariant x)
						{
							if (x.Rooms.Contains(rid))
							{
								BreakableDoor breakableDoor = x as BreakableDoor;
								if (breakableDoor != null)
								{
									return !breakableDoor.IgnoreRemoteAdmin;
								}
							}
							return false;
						}, out doorVariant))
						{
							list.Add(position + Vector3.up);
						}
						else
						{
							Vector3 position2 = doorVariant.transform.position;
							Vector3 b = (position - position2).NormalizeIgnoreY();
							list.Add(position2 + b + Vector3.up);
						}
					}
				}
			}
			if (list.Count == 0)
			{
				ListPool<Vector3>.Shared.Return(list);
				response = "Room couldn't be found.";
				return false;
			}
			Vector3 position3 = list[UnityEngine.Random.Range(0, list.Count)];
			ListPool<Vector3>.Shared.Return(list);
			PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
			if (playerCommandSender == null || !playerCommandSender.ReferenceHub.TryOverridePosition(position3))
			{
				response = "Your current character role does not support this operation.";
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " teleported themself to room " + arguments.At(0) + ".", ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			response = "You have been teleported.";
			return true;
		}
	}
}
