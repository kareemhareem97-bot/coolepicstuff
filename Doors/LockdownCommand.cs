using System;
using Interactables.Interobjects.DoorUtils;
using MapGeneration;
using MEC;

namespace CommandSystem.Commands.RemoteAdmin.Doors
{
	// Token: 0x02000A68 RID: 2664
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class LockdownCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000D5D RID: 3421
		// (get) Token: 0x06003C33 RID: 15411 RVA: 0x000C6C35 File Offset: 0x000C4E35
		public string Command { get; } = "lockdown";

		// Token: 0x17000D5E RID: 3422
		// (get) Token: 0x06003C34 RID: 15412 RVA: 0x000C6C3D File Offset: 0x000C4E3D
		public string[] Aliases { get; } = new string[]
		{
			"ld"
		};

		// Token: 0x17000D5F RID: 3423
		// (get) Token: 0x06003C35 RID: 15413 RVA: 0x000C6C45 File Offset: 0x000C4E45
		public string Description { get; } = "Locks all the doors in the facility.";

		// Token: 0x17000D60 RID: 3424
		// (get) Token: 0x06003C36 RID: 15414 RVA: 0x000C6C4D File Offset: 0x000C4E4D
		public string[] Usage { get; } = new string[]
		{
			"ZoneID Filter",
			"Duration (Optional)"
		};

		// Token: 0x06003C37 RID: 15415 RVA: 0x000C6C58 File Offset: 0x000C4E58
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			FacilityZone facilityZone = FacilityZone.None;
			float num = 0f;
			if (arguments.Count != 0 && !Enum.TryParse<FacilityZone>(arguments.At(0), true, out facilityZone))
			{
				response = "Specified zone is invalid.";
				return false;
			}
			if (arguments.Count > 1 && !float.TryParse(arguments.At(1), out num))
			{
				response = "Specified duration is invalid.";
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} has locked down the facility (Filter: {1}){2}", sender.LogName, facilityZone, (num == 0f) ? string.Empty : string.Format("for {0} seconds.", num)), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			this.Lockdown(facilityZone, num, num != -1f, out response);
			return true;
		}

		// Token: 0x06003C38 RID: 15416 RVA: 0x000C6D18 File Offset: 0x000C4F18
		private void Lockdown(FacilityZone zoneToAffect, float duration, bool doLock, out string commandResponse)
		{
			if (RoundSummary.SummaryActive)
			{
				commandResponse = string.Empty;
				return;
			}
			bool flag = zoneToAffect > FacilityZone.None;
			foreach (DoorVariant doorVariant in DoorVariant.AllDoors)
			{
				if (!flag || doorVariant.IsInZone(zoneToAffect))
				{
					doorVariant.ServerChangeLock(DoorLockReason.AdminCommand, doLock);
				}
			}
			Timing.KillCoroutines(new CoroutineHandle[]
			{
				this._lockdownHandle
			});
			if (duration != 0f)
			{
				this._lockdownHandle = Timing.CallDelayed(duration, delegate()
				{
					string text;
					this.Lockdown(zoneToAffect, 0f, false, out text);
				});
			}
			commandResponse = ((zoneToAffect == FacilityZone.None) ? ("Locked down the facility" + ((duration == 0f) ? "." : string.Format(" for {0} seconds.", duration))) : string.Format("Locked down {0}{1}", zoneToAffect, (duration == 0f) ? "." : string.Format(" for {0} seconds.", duration)));
		}

		// Token: 0x04002C44 RID: 11332
		private CoroutineHandle _lockdownHandle;
	}
}
