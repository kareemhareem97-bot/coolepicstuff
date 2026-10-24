using System;

namespace CommandSystem.Commands.RemoteAdmin.Dms
{
	// Token: 0x02000A70 RID: 2672
	[CommandHandler(typeof(DmsCommand))]
	public class SetCommand : ICommand
	{
		// Token: 0x17000D75 RID: 3445
		// (get) Token: 0x06003C5E RID: 15454 RVA: 0x000C7127 File Offset: 0x000C5327
		public string Command
		{
			get
			{
				return "set";
			}
		}

		// Token: 0x17000D76 RID: 3446
		// (get) Token: 0x06003C5F RID: 15455 RVA: 0x000C712E File Offset: 0x000C532E
		public string[] Aliases
		{
			get
			{
				return new string[]
				{
					"s"
				};
			}
		}

		// Token: 0x17000D77 RID: 3447
		// (get) Token: 0x06003C60 RID: 15456 RVA: 0x000C713E File Offset: 0x000C533E
		public string Description
		{
			get
			{
				return "Sets the current value of the Deadman's Switch timer.";
			}
		}

		// Token: 0x06003C61 RID: 15457 RVA: 0x000C7148 File Offset: 0x000C5348
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.WarheadEvents, out response))
			{
				return false;
			}
			if (DeadmanSwitch.IsSequenceActive)
			{
				response = "DMS is already in progress.";
				return false;
			}
			float countdownTimeLeft;
			if (arguments.Count == 0 || !float.TryParse(arguments.At(0), out countdownTimeLeft))
			{
				response = "You must input a valid number.";
				return false;
			}
			DeadmanSwitch.CountdownTimeLeft = countdownTimeLeft;
			response = string.Format("DMS will now trigger in {0:F2}s.", DeadmanSwitch.CountdownTimeLeft);
			return true;
		}
	}
}
