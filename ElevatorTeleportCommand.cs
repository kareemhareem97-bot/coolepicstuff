using System;
using System.Collections.Generic;
using System.Text;
using CommandSystem.Commands.RemoteAdmin.Doors;
using Interactables.Interobjects;
using NorthwoodLib.Pools;
using PlayerRoles.FirstPersonControl;
using UnityEngine;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009E8 RID: 2536
	[CommandHandler(typeof(ElevatorCommand))]
	public class ElevatorTeleportCommand : ICommand
	{
		// Token: 0x17000BCB RID: 3019
		// (get) Token: 0x06003943 RID: 14659 RVA: 0x000BB28A File Offset: 0x000B948A
		public string Command { get; } = "teleport";

		// Token: 0x17000BCC RID: 3020
		// (get) Token: 0x06003944 RID: 14660 RVA: 0x000BB292 File Offset: 0x000B9492
		public string[] Aliases { get; } = new string[]
		{
			"t",
			"tp",
			"goto",
			"tele"
		};

		// Token: 0x17000BCD RID: 3021
		// (get) Token: 0x06003945 RID: 14661 RVA: 0x000BB29A File Offset: 0x000B949A
		public string Description { get; } = "Teleports to an elevator.";

		// Token: 0x06003946 RID: 14662 RVA: 0x000BB2A4 File Offset: 0x000B94A4
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.PlayersManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 2)
			{
				response = "Syntax error: elevator teleport <Elevator ID> <Target Players> [\"inside\"/level ID/\"outside\"]";
				return false;
			}
			ElevatorGroup elevatorGroup;
			if (!ElevatorCommand.TryParseGroup(arguments.At(0), out elevatorGroup))
			{
				response = "Elevator \"" + arguments.At(0) + "\" not found.";
				return false;
			}
			ElevatorChamber elevatorChamber;
			if (!ElevatorChamber.TryGetChamber(elevatorGroup, out elevatorChamber))
			{
				response = string.Format("Elevator \"{0}\" could not be found in the Facility.", elevatorGroup);
				return false;
			}
			List<ElevatorDoor> doorsForGroup = ElevatorDoor.GetDoorsForGroup(elevatorGroup);
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 1, out array, false);
			if (list == null || list.Count == 0)
			{
				response = "No players have been selected.";
				return false;
			}
			Vector3 position = elevatorChamber.transform.position + Vector3.up * ElevatorTeleportCommand.PositionOffset;
			string text = "inside";
			if (array != null && array.Length > 0)
			{
				string text2 = array[0].ToLowerInvariant();
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text2);
				if (num <= 2870621791U)
				{
					if (num != 1094220446U)
					{
						if (num != 2565440279U)
						{
							if (num != 2870621791U)
							{
								goto IL_1DD;
							}
							if (!(text2 == "out"))
							{
								goto IL_1DD;
							}
						}
						else
						{
							if (!(text2 == "ins"))
							{
								goto IL_1DD;
							}
							goto IL_249;
						}
					}
					else
					{
						if (!(text2 == "in"))
						{
							goto IL_1DD;
						}
						goto IL_249;
					}
				}
				else if (num <= 3926667934U)
				{
					if (num != 3280923844U)
					{
						if (num != 3926667934U)
						{
							goto IL_1DD;
						}
						if (!(text2 == "o"))
						{
							goto IL_1DD;
						}
					}
					else if (!(text2 == "outside"))
					{
						goto IL_1DD;
					}
				}
				else if (num != 3960223172U)
				{
					if (num != 4148812653U)
					{
						goto IL_1DD;
					}
					if (!(text2 == "inside"))
					{
						goto IL_1DD;
					}
					goto IL_249;
				}
				else
				{
					if (!(text2 == "i"))
					{
						goto IL_1DD;
					}
					goto IL_249;
				}
				position = DoorTPCommand.EnsurePositionSafety(elevatorChamber.DestinationDoor.transform);
				text = "outside";
				goto IL_249;
				IL_1DD:
				int num2;
				if (!int.TryParse(array[0], out num2))
				{
					response = "Invalid level ID: " + array[0];
					return false;
				}
				if (num2 < 0 || num2 >= doorsForGroup.Count)
				{
					response = "Selected elevator doesn't have level " + array[0] + ".";
					return false;
				}
				position = DoorTPCommand.EnsurePositionSafety(doorsForGroup[num2].transform);
				text = string.Format("level {0}", num2);
			}
			IL_249:
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			int num3 = 0;
			foreach (ReferenceHub referenceHub in list)
			{
				if (referenceHub.TryOverridePosition(position))
				{
					if (num3 != 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(referenceHub.LoggedNameFromRefHub());
					num3++;
				}
			}
			if (num3 > 0)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} teleported player{1}{2} to elevator {3} ({4}).", new object[]
				{
					sender.LogName,
					(num3 == 1) ? " " : "s ",
					stringBuilder,
					elevatorGroup.ToString(),
					text
				}), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			}
			StringBuilderPool.Shared.Return(stringBuilder);
			response = string.Format("Done! The request affected {0} player{1}", num3, (num3 == 1) ? "!" : "s!");
			return true;
		}

		// Token: 0x04002A9D RID: 10909
		private static float PositionOffset = 0.8f;
	}
}
