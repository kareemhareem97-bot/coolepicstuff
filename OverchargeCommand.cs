using System;
using MapGeneration;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009FF RID: 2559
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class OverchargeCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000C1B RID: 3099
		// (get) Token: 0x060039CC RID: 14796 RVA: 0x000BE2F6 File Offset: 0x000BC4F6
		public string Command { get; } = "overcharge";

		// Token: 0x17000C1C RID: 3100
		// (get) Token: 0x060039CD RID: 14797 RVA: 0x000BE2FE File Offset: 0x000BC4FE
		public string[] Aliases { get; } = new string[]
		{
			"ocharge",
			"flicker",
			"blackout"
		};

		// Token: 0x17000C1D RID: 3101
		// (get) Token: 0x060039CE RID: 14798 RVA: 0x000BE306 File Offset: 0x000BC506
		public string Description { get; } = "Turns lights off in heavy and optionally heavy and light.";

		// Token: 0x17000C1E RID: 3102
		// (get) Token: 0x060039CF RID: 14799 RVA: 0x000BE30E File Offset: 0x000BC50E
		public string[] Usage { get; } = new string[]
		{
			"Zone ID",
			"Duration"
		};

		// Token: 0x060039D0 RID: 14800 RVA: 0x000BE318 File Offset: 0x000BC518
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.RoundEvents, out response))
			{
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "To use this, type at least 1 argument(s)! (some parameters are missing)\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			bool flag = arguments.Count >= 2;
			FacilityZone facilityZone = FacilityZone.None;
			float num;
			if (!float.TryParse(arguments.At(flag ? 1 : 0), out num))
			{
				response = "Specified duration is invalid.";
				return false;
			}
			if (flag && !Enum.TryParse<FacilityZone>(arguments.At(0), true, out facilityZone))
			{
				response = "Specified zone is invalid.";
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} has turned off lights in {1} for {2} seconds.", sender.LogName, (facilityZone == FacilityZone.None) ? "the facility" : facilityZone.ToString(), num), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			this.Overcharge(facilityZone, num, out response);
			return true;
		}

		// Token: 0x060039D1 RID: 14801 RVA: 0x000BE3F0 File Offset: 0x000BC5F0
		private void Overcharge(FacilityZone zoneToAffect, float duration, out string commandResponse)
		{
			bool flag = zoneToAffect > FacilityZone.None;
			foreach (RoomLightController roomLightController in RoomLightController.Instances)
			{
				if (!flag || roomLightController.Room.Zone == zoneToAffect)
				{
					roomLightController.ServerFlickerLights(duration);
				}
			}
			commandResponse = ((zoneToAffect == FacilityZone.None) ? string.Format("Turned off lights in the facility for {0} seconds.", duration) : string.Format("Turned off lights in {0} for {1} seconds.", zoneToAffect, duration));
		}
	}
}
