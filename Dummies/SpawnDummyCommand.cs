using System;
using NetworkManagerUtils.Dummies;

namespace CommandSystem.Commands.RemoteAdmin.Dummies
{
	// Token: 0x02000A5F RID: 2655
	[CommandHandler(typeof(DummiesCommand))]
	public class SpawnDummyCommand : ICommand
	{
		// Token: 0x17000D44 RID: 3396
		// (get) Token: 0x06003C03 RID: 15363 RVA: 0x000C62F6 File Offset: 0x000C44F6
		public string Command { get; } = "spawn";

		// Token: 0x17000D45 RID: 3397
		// (get) Token: 0x06003C04 RID: 15364 RVA: 0x000C62FE File Offset: 0x000C44FE
		public string[] Aliases { get; } = new string[]
		{
			"s",
			"create"
		};

		// Token: 0x17000D46 RID: 3398
		// (get) Token: 0x06003C05 RID: 15365 RVA: 0x000C6306 File Offset: 0x000C4506
		public string Description { get; } = "Spawns a dummy on the map.";

		// Token: 0x06003C06 RID: 15366 RVA: 0x000C6310 File Offset: 0x000C4510
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			string text = (arguments.Count > 0) ? string.Join<string>(' ', arguments) : "Dummy";
			DummyUtils.SpawnDummy(text);
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " has spawned a dummy with the nickname " + text + ".", ServerLogs.ServerLogType.RemoteAdminActivity_Misc, false);
			response = "A dummy has been spawned.";
			return true;
		}
	}
}
