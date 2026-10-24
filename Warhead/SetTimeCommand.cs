using System;

namespace CommandSystem.Commands.RemoteAdmin.Warhead
{
	// Token: 0x02000A2D RID: 2605
	[CommandHandler(typeof(WarheadCommand))]
	public class SetTimeCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000CB7 RID: 3255
		// (get) Token: 0x06003AED RID: 15085 RVA: 0x000C2632 File Offset: 0x000C0832
		public string Command { get; } = "settime";

		// Token: 0x17000CB8 RID: 3256
		// (get) Token: 0x06003AEE RID: 15086 RVA: 0x000C263A File Offset: 0x000C083A
		public string[] Aliases { get; } = new string[]
		{
			"time",
			"st"
		};

		// Token: 0x17000CB9 RID: 3257
		// (get) Token: 0x06003AEF RID: 15087 RVA: 0x000C2642 File Offset: 0x000C0842
		public string Description { get; } = "Sets the remaining time to detonation.";

		// Token: 0x17000CBA RID: 3258
		// (get) Token: 0x06003AF0 RID: 15088 RVA: 0x000C264A File Offset: 0x000C084A
		public string[] Usage { get; } = new string[]
		{
			"time (seconds)"
		};

		// Token: 0x06003AF1 RID: 15089 RVA: 0x000C2654 File Offset: 0x000C0854
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "To execute this command provide at least 1 argument!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			float num;
			if (!float.TryParse(arguments.At(0), out num))
			{
				response = "The specified time must be a valid number.";
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " set remaining time to warhead detonation to " + num.ToString() + ((Math.Abs(num - 1f) < 0.0001f) ? " second." : " seconds."), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			AlphaWarheadController.Singleton.ForceTime(num);
			response = "Time remaining to detonation set to " + num.ToString() + ((Math.Abs(num - 1f) < 0.0001f) ? " second." : " seconds.");
			return true;
		}
	}
}
