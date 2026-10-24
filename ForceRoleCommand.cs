using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PlayerRoles;
using RemoteAdmin;
using Utils;
using Utils.NonAllocLINQ;

namespace CommandSystem.Commands.RemoteAdmin
{
	// Token: 0x020009EE RID: 2542
	[CommandHandler(typeof(RemoteAdminCommandHandler))]
	public class ForceRoleCommand : ICommand, IUsageProvider
	{
		// Token: 0x17000BDE RID: 3038
		// (get) Token: 0x06003967 RID: 14695 RVA: 0x000BC095 File Offset: 0x000BA295
		public string Command { get; } = "forcerole";

		// Token: 0x17000BDF RID: 3039
		// (get) Token: 0x06003968 RID: 14696 RVA: 0x000BC09D File Offset: 0x000BA29D
		public string[] Aliases { get; } = new string[]
		{
			"fc",
			"fr",
			"forceclass"
		};

		// Token: 0x17000BE0 RID: 3040
		// (get) Token: 0x06003969 RID: 14697 RVA: 0x000BC0A5 File Offset: 0x000BA2A5
		public string Description { get; } = "Forces a player to a specified role.";

		// Token: 0x17000BE1 RID: 3041
		// (get) Token: 0x0600396A RID: 14698 RVA: 0x000BC0AD File Offset: 0x000BA2AD
		public string[] Usage { get; } = new string[]
		{
			"%player%",
			"%role%"
		};

		// Token: 0x0600396B RID: 14699 RVA: 0x000BC0B8 File Offset: 0x000BA2B8
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
			string[] array;
			List<ReferenceHub> list = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out array, false);
			bool flag;
			if (list.Count == 1)
			{
				PlayerCommandSender playerCommandSender = sender as PlayerCommandSender;
				if (playerCommandSender != null)
				{
					flag = (playerCommandSender.ReferenceHub == list[0]);
					goto IL_71;
				}
			}
			flag = false;
			IL_71:
			bool self = flag;
			PlayerRoleBase playerRoleBase;
			if (!this.TryParseRole(array[0], out playerRoleBase))
			{
				response = "Invalid role ID / name.";
				return false;
			}
			if (!this.HasPerms(playerRoleBase.RoleTypeId, self, sender, out response))
			{
				return false;
			}
			RoleSpawnFlags spawnFlags;
			this.ProvideRoleFlag(array, out spawnFlags);
			bool flag2 = list.Any((ReferenceHub p) => p.GetRoleId() != RoleTypeId.Overwatch);
			int num = 0;
			foreach (ReferenceHub referenceHub2 in list)
			{
				if (!(referenceHub2 == null) && (!flag2 || referenceHub2.GetRoleId() != RoleTypeId.Overwatch))
				{
					referenceHub2.roleManager.ServerSetRole(playerRoleBase.RoleTypeId, RoleChangeReason.RemoteAdmin, spawnFlags, null);
					ServerLogs.AddLog(ServerLogs.Modules.ClassChange, string.Format("{0} changed role of player {1} to {2}.", sender.LogName, referenceHub2.LoggedNameFromRefHub(), playerRoleBase.RoleTypeId), ServerLogs.ServerLogType.RemoteAdminActivity_GameChanging, false);
					num++;
				}
			}
			response = string.Format("Done! Changed role of {0} player{1} to {2}!", num, (num == 1) ? "" : "s", playerRoleBase.RoleTypeId);
			return true;
		}

		// Token: 0x0600396C RID: 14700 RVA: 0x000BC264 File Offset: 0x000BA464
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

		// Token: 0x0600396D RID: 14701 RVA: 0x000BC28C File Offset: 0x000BA48C
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

		// Token: 0x0600396E RID: 14702 RVA: 0x000BC31C File Offset: 0x000BA51C
		private bool HasPerms(RoleTypeId targetRole, bool self, ICommandSender sender, out string response)
		{
			if (targetRole != RoleTypeId.Spectator)
			{
				if (targetRole != RoleTypeId.Overwatch)
				{
					if (self)
					{
						return sender.CheckPermission(new PlayerPermissions[]
						{
							PlayerPermissions.ForceclassWithoutRestrictions,
							PlayerPermissions.ForceclassSelf
						}, out response);
					}
					return sender.CheckPermission(PlayerPermissions.ForceclassWithoutRestrictions, out response);
				}
				else
				{
					if (self)
					{
						return sender.CheckPermission(PlayerPermissions.Overwatch, out response);
					}
					return sender.CheckPermission(PlayerPermissions.Overwatch, out response) && sender.CheckPermission(new PlayerPermissions[]
					{
						PlayerPermissions.ForceclassWithoutRestrictions,
						PlayerPermissions.ForceclassToSpectator
					}, out response);
				}
			}
			else
			{
				if (self)
				{
					return sender.CheckPermission(new PlayerPermissions[]
					{
						PlayerPermissions.ForceclassWithoutRestrictions,
						PlayerPermissions.ForceclassToSpectator,
						PlayerPermissions.ForceclassSelf,
						PlayerPermissions.Overwatch
					}, out response);
				}
				return sender.CheckPermission(new PlayerPermissions[]
				{
					PlayerPermissions.ForceclassWithoutRestrictions,
					PlayerPermissions.ForceclassToSpectator
				}, out response);
			}
		}
	}
}
