using System;

namespace CommandSystem.Commands.RemoteAdmin.Dms
{
	// Token: 0x02000A6F RID: 2671
	[CommandHandler(typeof(DmsCommand))]
	public class GetCommand : ICommand
	{
		// Token: 0x17000D72 RID: 3442
		// (get) Token: 0x06003C59 RID: 15449 RVA: 0x000C70CF File Offset: 0x000C52CF
		public string Command
		{
			get
			{
				return "get";
			}
		}

		// Token: 0x17000D73 RID: 3443
		// (get) Token: 0x06003C5A RID: 15450 RVA: 0x000C70D6 File Offset: 0x000C52D6
		public string[] Aliases
		{
			get
			{
				return new string[]
				{
					"g"
				};
			}
		}

		// Token: 0x17000D74 RID: 3444
		// (get) Token: 0x06003C5B RID: 15451 RVA: 0x000C70E6 File Offset: 0x000C52E6
		public string Description
		{
			get
			{
				return "Gets the current value of the Deadman's Switch timer.";
			}
		}

		// Token: 0x06003C5C RID: 15452 RVA: 0x000C70ED File Offset: 0x000C52ED
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
			response = string.Format("{0:F2}s till DMS triggers.", DeadmanSwitch.CountdownTimeLeft);
			return true;
		}
	}
}
