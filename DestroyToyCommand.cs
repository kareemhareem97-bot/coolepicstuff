using System;
using AdminToys;
using Mirror;
using Utils.Networking;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009DE RID: 2526
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class DestroyToyCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000BAE RID: 2990
		// (get) Token: 0x06003908 RID: 14600 RVA: 0x000BA0C0 File Offset: 0x000B82C0
		public string Command { get; } = "destroytoy";

		// Token: 0x17000BAF RID: 2991
		// (get) Token: 0x06003909 RID: 14601 RVA: 0x000BA0C8 File Offset: 0x000B82C8
		public string[] Aliases { get; }

		// Token: 0x17000BB0 RID: 2992
		// (get) Token: 0x0600390A RID: 14602 RVA: 0x000BA0D0 File Offset: 0x000B82D0
		public string Description { get; } = "Despawns a toy placed by an admin.";

		// Token: 0x17000BB1 RID: 2993
		// (get) Token: 0x0600390B RID: 14603 RVA: 0x000BA0D8 File Offset: 0x000B82D8
		public string[] Usage { get; } = new string[]
		{
			"NetID of toy to remove."
		};

		// Token: 0x0600390C RID: 14604 RVA: 0x000BA0E0 File Offset: 0x000B82E0
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			if (!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
			{
				return false;
			}
			uint num;
			if (arguments.Count < 1 || !uint.TryParse(arguments.Array[1], out num))
			{
				response = "Failed to parse NetID of the toy to destroy.";
				return false;
			}
			NetworkIdentity networkIdentity;
			AdminToyBase adminToyBase;
			if (!NetworkUtils.SpawnedNetIds.TryGetValue(num, out networkIdentity) || !networkIdentity.TryGetComponent<AdminToyBase>(out adminToyBase))
			{
				response = string.Format("{0} is not a valid toy NetID.", num);
				return false;
			}
			ServerLogs.AddLog(ServerLogs.Modules.Administrative, string.Format("{0} removed admin toy: {1} ({2}).", sender.LogName, adminToyBase.CommandName, adminToyBase.netId), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
			response = string.Format("Toy {0} successfully removed.", num);
			NetworkServer.Destroy(adminToyBase.gameObject);
			return true;
		}
	}
}
