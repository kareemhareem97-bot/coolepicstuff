using System;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin.Broadcasts
{
	// Token: 0x02000A81 RID: 2689
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class BroadcastCommand : BroadcastCommandBase
	{
		// Token: 0x17000D9B RID: 3483
		// (get) Token: 0x06003CAD RID: 15533 RVA: 0x000C7B61 File Offset: 0x000C5D61
		public override string Command { get; } = "broadcast";

		// Token: 0x17000D9C RID: 3484
		// (get) Token: 0x06003CAE RID: 15534 RVA: 0x000C7B69 File Offset: 0x000C5D69
		public override string[] Aliases { get; } = new string[]
		{
			"bc",
			"alert"
		};

		// Token: 0x17000D9D RID: 3485
		// (get) Token: 0x06003CAF RID: 15535 RVA: 0x000C7B71 File Offset: 0x000C5D71
		public override string Description { get; } = "Sends an administrative broadcast to all players.";

		// Token: 0x17000D9E RID: 3486
		// (get) Token: 0x06003CB0 RID: 15536 RVA: 0x000C7B79 File Offset: 0x000C5D79
		public override string[] Usage { get; } = new string[]
		{
			"Duration",
			"BroadcastFlag (Optional)",
			"Message"
		};

		// Token: 0x06003CB1 RID: 15537 RVA: 0x000C7B84 File Offset: 0x000C5D84
		public override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			string text = arguments.At(0);
			ushort time;
			if (!base.IsValidDuration(text, out time))
			{
				response = string.Concat(new string[]
				{
					"Invalid argument for duration: ",
					text,
					" Usage: ",
					arguments.Array[0],
					" ",
					this.DisplayCommandUsage()
				});
				return false;
			}
			Broadcast.BroadcastFlags broadcastFlags;
			bool flag = base.HasInputFlag(arguments.At(1), out broadcastFlags, arguments.Count);
			string text2 = RAUtils.FormatArguments(arguments, flag ? 2 : 1, null);
			Broadcast.Singleton.RpcAddElement(text2, time, broadcastFlags);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} broadcast text \"{1}\". Duration: {2} seconds. Broadcast Flag: {3}.", new object[]
			{
				sender.LogName,
				text2,
				text,
				broadcastFlags
			}), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			response = "Global broadcast sent.";
			return true;
		}
	}
}
