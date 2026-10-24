using System;

namespace GameCore
{
	// Token: 0x02000869 RID: 2153
	public static class PlayerPermissionsExtensions
	{
		// Token: 0x0600312D RID: 12589 RVA: 0x000A5158 File Offset: 0x000A3358
		public static PlayerPermissionType GetPermissionType(this PlayerPermissions permissions)
		{
			if (permissions <= PlayerPermissions.PermissionsManagement)
			{
				if (permissions <= PlayerPermissions.WarheadEvents)
				{
					if (permissions <= PlayerPermissions.ForceclassSelf)
					{
						if (permissions - PlayerPermissions.KickingAndShortTermBanning > 1UL && permissions != PlayerPermissions.LongTermBanning && permissions != PlayerPermissions.ForceclassSelf)
						{
							return PlayerPermissionType.Misc;
						}
					}
					else if (permissions <= PlayerPermissions.ForceclassWithoutRestrictions)
					{
						if (permissions != PlayerPermissions.ForceclassToSpectator && permissions != PlayerPermissions.ForceclassWithoutRestrictions)
						{
							return PlayerPermissionType.Misc;
						}
					}
					else if (permissions != PlayerPermissions.GivingItems && permissions != PlayerPermissions.WarheadEvents)
					{
						return PlayerPermissionType.Misc;
					}
				}
				else if (permissions <= PlayerPermissions.GameplayData)
				{
					if (permissions <= PlayerPermissions.RoundEvents)
					{
						if (permissions != PlayerPermissions.RespawnEvents && permissions != PlayerPermissions.RoundEvents)
						{
							return PlayerPermissionType.Misc;
						}
					}
					else if (permissions != PlayerPermissions.SetGroup && permissions != PlayerPermissions.GameplayData)
					{
						return PlayerPermissionType.Misc;
					}
				}
				else if (permissions <= PlayerPermissions.FacilityManagement)
				{
					if (permissions == PlayerPermissions.Overwatch)
					{
						return PlayerPermissionType.Misc;
					}
					if (permissions != PlayerPermissions.FacilityManagement)
					{
						return PlayerPermissionType.Misc;
					}
				}
				else if (permissions != PlayerPermissions.PlayersManagement && permissions != PlayerPermissions.PermissionsManagement)
				{
					return PlayerPermissionType.Misc;
				}
			}
			else if (permissions <= PlayerPermissions.AFKImmunity)
			{
				if (permissions <= PlayerPermissions.ServerConfigs)
				{
					if (permissions != PlayerPermissions.ServerConsoleCommands)
					{
						if (permissions == PlayerPermissions.ViewHiddenBadges)
						{
							return PlayerPermissionType.Misc;
						}
						if (permissions != PlayerPermissions.ServerConfigs)
						{
							return PlayerPermissionType.Misc;
						}
					}
				}
				else if (permissions <= PlayerPermissions.PlayerSensitiveDataAccess)
				{
					if (permissions != PlayerPermissions.Broadcasting && permissions != PlayerPermissions.PlayerSensitiveDataAccess)
					{
						return PlayerPermissionType.Misc;
					}
				}
				else
				{
					if (permissions != PlayerPermissions.Noclip && permissions != PlayerPermissions.AFKImmunity)
					{
						return PlayerPermissionType.Misc;
					}
					return PlayerPermissionType.Misc;
				}
			}
			else if (permissions <= PlayerPermissions.Effects)
			{
				if (permissions <= PlayerPermissions.ViewHiddenGlobalBadges)
				{
					if (permissions != PlayerPermissions.AdminChat && permissions != PlayerPermissions.ViewHiddenGlobalBadges)
					{
						return PlayerPermissionType.Misc;
					}
					return PlayerPermissionType.ViewAccess;
				}
				else if (permissions != PlayerPermissions.Announcer && permissions != PlayerPermissions.Effects)
				{
					return PlayerPermissionType.Misc;
				}
			}
			else if (permissions <= PlayerPermissions.FriendlyFireDetectorTempDisable)
			{
				if (permissions != PlayerPermissions.FriendlyFireDetectorImmunity && permissions != PlayerPermissions.FriendlyFireDetectorTempDisable)
				{
					return PlayerPermissionType.Misc;
				}
				return PlayerPermissionType.Misc;
			}
			else if (permissions != PlayerPermissions.ServerLogLiveFeed && permissions != PlayerPermissions.ExecuteAs)
			{
				return PlayerPermissionType.Misc;
			}
			return PlayerPermissionType.RaAccess;
		}

		// Token: 0x0600312E RID: 12590 RVA: 0x000A534C File Offset: 0x000A354C
		public static string GetPermissionTypeColor(this PlayerPermissionType type)
		{
			switch (type)
			{
			case PlayerPermissionType.RaAccess:
				return "#F54842";
			case PlayerPermissionType.ViewAccess:
				return "#BE83F2";
			case PlayerPermissionType.Misc:
				return "#7FDB88";
			default:
				return "#7FDB88";
			}
		}

		// Token: 0x04002643 RID: 9795
		private const string RAPermissionColor = "#F54842";

		// Token: 0x04002644 RID: 9796
		private const string ViewAccessPermissionColor = "#BE83F2";

		// Token: 0x04002645 RID: 9797
		private const string MiscPermissionsColor = "#7FDB88";
	}
}
