using System;
using System.Collections.Generic;
using System.Text;
using Interactables.Interobjects;
using NorthwoodLib.Pools;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009E7 RID: 2535
	[CommandHandler(typeof(ElevatorCommand))]
	public class ElevatorSendCommand : ICommand
	{
		// Token: 0x17000BC8 RID: 3016
		// (get) Token: 0x0600393D RID: 14653 RVA: 0x000BB046 File Offset: 0x000B9246
		public string Command { get; } = "send";

		// Token: 0x17000BC9 RID: 3017
		// (get) Token: 0x0600393E RID: 14654 RVA: 0x000BB04E File Offset: 0x000B924E
		public string[] Aliases { get; } = new string[]
		{
			"s",
			"snd"
		};

		// Token: 0x17000BCA RID: 3018
		// (get) Token: 0x0600393F RID: 14655 RVA: 0x000BB056 File Offset: 0x000B9256
		public string Description { get; } = "Sends an elevator.";

		// Token: 0x06003940 RID: 14656 RVA: 0x000BB060 File Offset: 0x000B9260
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 1 || arguments.Count > 2)
			{
				response = "Syntax error: elevator send <Elevator ID / \"all\"> [level]";
				return false;
			}
			string text = arguments.At(0);
			bool flag = text.Equals("all", StringComparison.OrdinalIgnoreCase) || text.Equals("*", StringComparison.OrdinalIgnoreCase);
			int num = -1;
			if (arguments.Count > 1 && (!int.TryParse(arguments.At(1), out num) || num < 0))
			{
				response = "Level must be a nonnegative integer.";
				return false;
			}
			if (flag)
			{
				StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
				bool flag2 = true;
				bool result;
				try
				{
					ElevatorGroup[] values = EnumUtils<ElevatorGroup>.Values;
					for (int i = 0; i < values.Length; i++)
					{
						string arg;
						if (!ElevatorSendCommand.SendElevator(values[i], -1, out arg, sender))
						{
							flag2 = false;
						}
						stringBuilder.AppendFormat("{0}\n", arg);
					}
					response = stringBuilder.ToString();
					result = flag2;
				}
				finally
				{
					StringBuilderPool.Shared.Return(stringBuilder);
				}
				return result;
			}
			ElevatorGroup group;
			if (!ElevatorCommand.TryParseGroup(text, out group))
			{
				response = "Elevator \"" + text + "\" not found.";
				return false;
			}
			return ElevatorSendCommand.SendElevator(group, num, out response, sender);
		}

		// Token: 0x06003941 RID: 14657 RVA: 0x000BB18C File Offset: 0x000B938C
		private static bool SendElevator(ElevatorGroup group, int level, out string response, ICommandSender sender)
		{
			List<ElevatorDoor> doorsForGroup = ElevatorDoor.GetDoorsForGroup(group);
			if (doorsForGroup.Count == 0)
			{
				response = string.Format("No doors for \"{0}\" could not be found in the Facility.", group);
				return false;
			}
			ElevatorChamber elevatorChamber;
			if (!ElevatorChamber.TryGetChamber(group, out elevatorChamber))
			{
				response = string.Format("Chamber for elevator \"{0}\" is not spawned.", group);
				return false;
			}
			if (level == -1)
			{
				level = elevatorChamber.NextLevel;
			}
			else if (level >= doorsForGroup.Count)
			{
				response = string.Format("Elevator \"{0}\" does not have a level {1}.", group, level);
				return false;
			}
			elevatorChamber.ServerSetDestination(level, true);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} sent elevator {1} to level {2}.", sender.LogName, group, level), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			response = string.Format("Elevator \"{0}\" has been sent to level {1}.", group, level);
			return true;
		}
	}
}
