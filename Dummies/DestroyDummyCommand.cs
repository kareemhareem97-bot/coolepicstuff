using System;
using System.Collections.Generic;
using Mirror;
using NetworkManagerUtils.Dummies;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin.Dummies
{
	// Token: 0x02000A5B RID: 2651
	[CommandHandler(typeof(DummiesCommand))]
	public class DestroyDummyCommand : ICommand
	{
		// Token: 0x17000D3A RID: 3386
		// (get) Token: 0x06003BEE RID: 15342 RVA: 0x000C5DE5 File Offset: 0x000C3FE5
		public string Command { get; } = "destroy";

		// Token: 0x17000D3B RID: 3387
		// (get) Token: 0x06003BEF RID: 15343 RVA: 0x000C5DED File Offset: 0x000C3FED
		public string[] Aliases { get; } = new string[]
		{
			"d",
			"kill",
			"remove"
		};

		// Token: 0x17000D3C RID: 3388
		// (get) Token: 0x06003BF0 RID: 15344 RVA: 0x000C5DF5 File Offset: 0x000C3FF5
		public string Description { get; } = "Spawns a dummy on the map.";

		// Token: 0x06003BF1 RID: 15345 RVA: 0x000C5E00 File Offset: 0x000C4000
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			if (arguments.Count < 1)
			{
				response = "You must specify a dummy to destroy.";
				return false;
			}
			if (arguments.At(0).Equals("all", StringComparison.OrdinalIgnoreCase))
			{
				DummyUtils.DestroyAllDummies();
				ServerLogs.AddLog(ServerLogs.Modules.Administrative, sender.LogName + " has destroyed all dummies.", ServerLogs.ServerLogType.RemoteAdminActivity_Misc, false);
				response = "All dummies have been destroyed.";
				return true;
			}
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			if (list == null)
			{
				response = "An unexpected problem has occurred during PlayerId or name array processing.";
				return false;
			}
			int num = 0;
			foreach (ReferenceHub referenceHub in list)
			{
				if (!(referenceHub == null) && referenceHub.IsDummy)
				{
					NetworkServer.Destroy(referenceHub.gameObject);
					num++;
				}
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} administratively destroyed {1} dummies.", sender.LogName, num), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			response = string.Format("Done! The request affected {0} dumm{1}", num, (num == 1) ? "y!" : "ies!");
			return true;
		}
	}
}
