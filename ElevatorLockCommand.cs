using System;
using System.Collections.Generic;
using System.Text;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using NorthwoodLib.Pools;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009E6 RID: 2534
	[CommandHandler(typeof(ElevatorCommand))]
	public class ElevatorLockCommand : ICommand
	{
		// Token: 0x17000BC5 RID: 3013
		// (get) Token: 0x06003935 RID: 14645 RVA: 0x000BABF9 File Offset: 0x000B8DF9
		public string Command { get; } = "lock";

		// Token: 0x17000BC6 RID: 3014
		// (get) Token: 0x06003936 RID: 14646 RVA: 0x000BAC01 File Offset: 0x000B8E01
		public string[] Aliases { get; } = new string[]
		{
			"l",
			"lck"
		};

		// Token: 0x17000BC7 RID: 3015
		// (get) Token: 0x06003937 RID: 14647 RVA: 0x000BAC09 File Offset: 0x000B8E09
		public string Description { get; } = "Locks an elevator.";

		// Token: 0x06003938 RID: 14648 RVA: 0x000BAC14 File Offset: 0x000B8E14
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			int count = arguments.Count;
			if (count != 1)
			{
				if (count == 2)
				{
					if (arguments.At(1).Equals("d", StringComparison.OrdinalIgnoreCase) || arguments.At(1).Equals("dynamic", StringComparison.OrdinalIgnoreCase))
					{
						return ElevatorLockCommand.TrySetLock(arguments.At(0), true, out response, sender, -2);
					}
					int num;
					if (int.TryParse(arguments.At(1), out num) && num >= 0)
					{
						return ElevatorLockCommand.TrySetLock(arguments.At(0), true, out response, sender, num);
					}
				}
				response = "Syntax error: elevator lock <Elevator ID / \"all\"> <level / \"dynamic\" (optional)>";
				return false;
			}
			return ElevatorLockCommand.TrySetLock(arguments.At(0), true, out response, sender, -1);
		}

		// Token: 0x06003939 RID: 14649 RVA: 0x000BACBC File Offset: 0x000B8EBC
		internal static bool TrySetLock(string elevatorId, bool locked, out string response, ICommandSender sender, int level)
		{
			if (elevatorId.Equals("all", StringComparison.OrdinalIgnoreCase) || elevatorId.Equals("*", StringComparison.Ordinal))
			{
				StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
				bool flag = true;
				bool result;
				try
				{
					foreach (ElevatorGroup elevatorGroup in EnumUtils<ElevatorGroup>.Values)
					{
						if (ElevatorLockCommand.SetLock(locked, sender, elevatorGroup, level))
						{
							stringBuilder.AppendFormat("Elevator \"{0}\" has been {1}.\n", elevatorGroup, locked ? "locked" : "unlocked");
						}
						else
						{
							flag = false;
							stringBuilder.AppendFormat("Could not update lock status for elevator \"{0}\".\n", elevatorGroup);
						}
					}
					response = stringBuilder.ToString();
					result = flag;
				}
				finally
				{
					StringBuilderPool.Shared.Return(stringBuilder);
				}
				return result;
			}
			ElevatorGroup elevatorGroup2;
			if (!ElevatorCommand.TryParseGroup(elevatorId, out elevatorGroup2))
			{
				response = "Elevator \"" + elevatorId + "\" not found.";
				return false;
			}
			if (!ElevatorLockCommand.SetLock(locked, sender, elevatorGroup2, level))
			{
				response = string.Format("Could not update lock status for elevator \"{0}\".", elevatorGroup2);
				return false;
			}
			response = string.Format("Elevator \"{0}\" has been {1}.", elevatorGroup2, locked ? "locked" : "unlocked");
			return true;
		}

		// Token: 0x0600393A RID: 14650 RVA: 0x000BADE8 File Offset: 0x000B8FE8
		private static bool SetLock(bool forceLock, ICommandSender sender, ElevatorGroup elevatorGroup, int level)
		{
			if (level < -2)
			{
				return false;
			}
			ElevatorChamber elevatorChamber;
			if (!ElevatorChamber.TryGetChamber(elevatorGroup, out elevatorChamber))
			{
				return false;
			}
			List<ElevatorDoor> doorsForGroup = ElevatorDoor.GetDoorsForGroup(elevatorGroup);
			if (level == -1)
			{
				return ElevatorLockCommand.SetLockAll(doorsForGroup, forceLock, sender, elevatorGroup, elevatorChamber);
			}
			if (level == -2)
			{
				if (elevatorChamber.DynamicAdminLock)
				{
					return false;
				}
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} enabled dynamic lock for elevator {1}.", sender.LogName, elevatorGroup), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
				elevatorChamber.DynamicAdminLock = true;
				return true;
			}
			else
			{
				if (doorsForGroup.Count <= level)
				{
					return false;
				}
				ElevatorDoor elevatorDoor = doorsForGroup[level];
				if (((DoorLockReason)elevatorDoor.ActiveLocks).HasFlagFast(DoorLockReason.AdminCommand) == forceLock)
				{
					return true;
				}
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} {1} level {2} of elevator {3}.", new object[]
				{
					sender.LogName,
					forceLock ? "locked" : "unlocked",
					level,
					elevatorGroup
				}), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
				if (elevatorChamber.DynamicAdminLock)
				{
					ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} disabled dynamic lock for elevator {1}.", sender.LogName, elevatorGroup), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
					elevatorChamber.DynamicAdminLock = false;
				}
				if (forceLock)
				{
					ElevatorDoor elevatorDoor2 = elevatorDoor;
					elevatorDoor2.NetworkActiveLocks = (elevatorDoor2.ActiveLocks | 8);
				}
				else
				{
					ElevatorDoor elevatorDoor3 = elevatorDoor;
					elevatorDoor3.NetworkActiveLocks = (elevatorDoor3.ActiveLocks & 65527);
				}
				return true;
			}
		}

		// Token: 0x0600393B RID: 14651 RVA: 0x000BAF14 File Offset: 0x000B9114
		private static bool SetLockAll(List<ElevatorDoor> list, bool isLocking, ICommandSender sender, ElevatorGroup elevatorGroup, ElevatorChamber targetChamber)
		{
			if (targetChamber.DynamicAdminLock)
			{
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} disabled dynamic lock for elevator {1}.", sender.LogName, elevatorGroup), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
				targetChamber.DynamicAdminLock = false;
				if (!isLocking)
				{
					return true;
				}
			}
			if (isLocking && targetChamber.ActiveLocksAllDoors.HasFlagFast(DoorLockReason.AdminCommand))
			{
				return true;
			}
			if (!isLocking && !targetChamber.ActiveLocksAnyDoors.HasFlagFast(DoorLockReason.AdminCommand))
			{
				return true;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} {1} elevator {2}.", sender.LogName, isLocking ? "locked" : "unlocked", elevatorGroup), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			foreach (ElevatorDoor elevatorDoor in list)
			{
				if (isLocking)
				{
					ElevatorDoor elevatorDoor2 = elevatorDoor;
					elevatorDoor2.NetworkActiveLocks = (elevatorDoor2.ActiveLocks | 8);
				}
				else
				{
					ElevatorDoor elevatorDoor3 = elevatorDoor;
					elevatorDoor3.NetworkActiveLocks = (elevatorDoor3.ActiveLocks & 65527);
				}
			}
			return true;
		}

		// Token: 0x04002A95 RID: 10901
		private const int AllLevels = -1;

		// Token: 0x04002A96 RID: 10902
		private const int DynamicLock = -2;
	}
}
