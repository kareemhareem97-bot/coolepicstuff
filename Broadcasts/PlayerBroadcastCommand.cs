using System;
using System.Collections.Generic;
using System.Text;
using NorthwoodLib.Pools;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin.Broadcasts
{
	// Token: 0x02000A84 RID: 2692
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class PlayerBroadcastCommand : BroadcastCommandBase
	{
		// Token: 0x17000DA7 RID: 3495
		// (get) Token: 0x06003CC2 RID: 15554 RVA: 0x000C7E15 File Offset: 0x000C6015
		public override string Command { get; } = "playerbroadcast";

		// Token: 0x17000DA8 RID: 3496
		// (get) Token: 0x06003CC3 RID: 15555 RVA: 0x000C7E1D File Offset: 0x000C601D
		public override string[] Aliases { get; } = new string[]
		{
			"pbc"
		};

		// Token: 0x17000DA9 RID: 3497
		// (get) Token: 0x06003CC4 RID: 15556 RVA: 0x000C7E25 File Offset: 0x000C6025
		public override string Description { get; } = "Sends an administrative broadcast to specific player(s).";

		// Token: 0x17000DAA RID: 3498
		// (get) Token: 0x06003CC5 RID: 15557 RVA: 0x000C7E2D File Offset: 0x000C602D
		public override string[] Usage { get; } = new string[]
		{
			"%player%",
			"Duration",
			"BroadcastFlag (Optional)",
			"Message"
		};

		// Token: 0x06003CC6 RID: 15558 RVA: 0x000C7E38 File Offset: 0x000C6038
		public override bool OnExecute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			if (array == null || array.Length < this.MinimumArguments)
			{
				response = "To execute this command provide at least 3 arguments!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			string text = array[0];
			ushort num;
			if (!base.IsValidDuration(text, out num))
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
			bool flag = base.HasInputFlag(array[1], out broadcastFlags, array.Length);
			string text2 = RAUtils.FormatArguments(array.Segment(1), flag ? 1 : 0, null);
			StringBuilder stringBuilder = StringBuilderPool.Shared.Rent();
			Broadcast singleton = Broadcast.Singleton;
			int num2 = 0;
			foreach (ReferenceHub referenceHub in list)
			{
				if (num2++ != 0)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(referenceHub.LoggedNameFromRefHub());
				singleton.TargetAddElement(referenceHub.connectionToClient, text2, num, broadcastFlags);
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} broadcast text \"{1}\" to {2} players. Duration: {3} seconds. Affected players: {4}. Broadcast Flag: {5}.", new object[]
			{
				sender.LogName,
				text2,
				num2,
				num,
				stringBuilder,
				broadcastFlags
			}), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			StringBuilderPool.Shared.Return(stringBuilder);
			response = string.Format("Broadcast sent to {0} players.", num2);
			return true;
		}
	}
}
