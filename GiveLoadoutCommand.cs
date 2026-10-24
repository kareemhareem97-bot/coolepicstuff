using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using InventorySystem;
using InventorySystem.Configs;
using PlayerRoles;
using Utils;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009F1 RID: 2545
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class GiveLoadoutCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000BE6 RID: 3046
		// (get) Token: 0x06003979 RID: 14713 RVA: 0x000BC6F9 File Offset: 0x000BA8F9
		public string Command { get; } = "giveloadout";

		// Token: 0x17000BE7 RID: 3047
		// (get) Token: 0x0600397A RID: 14714 RVA: 0x000BC701 File Offset: 0x000BA901
		public string[] Aliases { get; } = new string[]
		{
			"sendloadout",
			"giveinventory",
			"grantloadout"
		};

		// Token: 0x17000BE8 RID: 3048
		// (get) Token: 0x0600397B RID: 14715 RVA: 0x000BC709 File Offset: 0x000BA909
		public string Description { get; } = "Grant target(s) the specified role's loadout.";

		// Token: 0x17000BE9 RID: 3049
		// (get) Token: 0x0600397C RID: 14716 RVA: 0x000BC711 File Offset: 0x000BA911
		public string[] Usage { get; } = new string[]
		{
			"%player%",
			"%role%"
		};

		// Token: 0x0600397D RID: 14717 RVA: 0x000BC71C File Offset: 0x000BA91C
		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
			ReferenceHub referenceHub;
			if (!ReferenceHub.TryGetHostHub(out referenceHub))
			{
				response = "You are not connected to a server.";
				return false;
			}
			if (arguments.Count < 2)
			{
				response = "To execute this command provide at least 2 arguments!\nUsage: " + arguments.Array[0] + " " + this.DisplayCommandUsage();
				return false;
			}
			if (!sender.CheckPermission(PlayerPermissions.GivingItems, out response))
			{
				return false;
			}
			CharacterClassManager characterClassManager = referenceHub.characterClassManager;
			if (characterClassManager == null || !characterClassManager.isLocalPlayer || !characterClassManager.isServer || !characterClassManager.RoundStarted)
			{
				response = "Please start round before using this command.";
				return false;
			}
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			PlayerRoleBase playerRoleBase;
			if (!this.TryParseRole(array[0], out playerRoleBase))
			{
				response = "Invalid role ID / name.";
				return false;
			}
			if (!StartingInventories.DefinedInventories.ContainsKey(playerRoleBase.RoleTypeId))
			{
				response = "Specified role does not have a defined inventory.";
				return false;
			}
			RoleSpawnFlags roleSpawnFlags;
			this.ProvideRoleFlag(array, out roleSpawnFlags);
			bool resetInventory = roleSpawnFlags.HasFlag(RoleSpawnFlags.AssignInventory);
			int num = 0;
			foreach (ReferenceHub referenceHub2 in list)
			{
				if (!(referenceHub2 == null))
				{
					InventoryItemProvider.ServerGrantLoadout(referenceHub2, playerRoleBase, resetInventory);
					ServerLogs.AddLog(ServerLogs.Modules.ClassChange, string.Format("{0} gave {1} the loadout of {2}.", sender.LogName, referenceHub2.LoggedNameFromRefHub(), playerRoleBase.RoleTypeId), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
					num++;
				}
			}
			response = string.Format("Done! Given {0}'s loadout to {1} player{2}!", playerRoleBase.RoleName, num, (num == 1) ? "" : "s");
			return true;
		}

		// Token: 0x0600397E RID: 14718 RVA: 0x000BC8B4 File Offset: 0x000BAAB4
		private void ProvideRoleFlag(string[] arguments, out RoleSpawnFlags spawnFlags)
		{
			byte b;
			if (arguments.Length > 1 && byte.TryParse(arguments[1], out b))
			{
				spawnFlags = (RoleSpawnFlags)b;
				return;
			}
			spawnFlags = RoleSpawnFlags.All;
		}

		// Token: 0x0600397F RID: 14719 RVA: 0x000BC8DC File Offset: 0x000BAADC
		private bool TryParseRole(string s, out PlayerRoleBase prb)
		{
			RoleTypeId roleType;
			if (Enum.TryParse<RoleTypeId>(s, true, out roleType))
			{
				return roleType.TryGetRoleTemplate(out prb);
			}
			foreach (PlayerRoleBase playerRoleBase in PlayerRoleLoader.AllRoles.Values)
			{
				if (!string.Equals(Regex.Replace(playerRoleBase.RoleName, "\\s+", ""), s, StringComparison.InvariantCultureIgnoreCase))
				{
					prb = playerRoleBase;
					return true;
				}
			}
			prb = null;
			return false;
		}
	}
}
