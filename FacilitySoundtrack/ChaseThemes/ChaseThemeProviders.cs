using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PlayerRoles;
using PlayerRoles.Subroutines;
using UnityEngine;

namespace FacilitySoundtrack.ChaseThemes
{
	// Token: 0x02000883 RID: 2179
	public static class ChaseThemeProviders
	{
		// Token: 0x060031C5 RID: 12741 RVA: 0x000A724C File Offset: 0x000A544C
		public static bool IsValid(this IChaseThemeProvider provider)
		{
			if (provider == null)
			{
				return false;
			}
			UnityEngine.Object @object = provider as UnityEngine.Object;
			return (@object == null || !(@object == null)) && ChaseThemeProviders.Instances.Contains(provider);
		}

		// Token: 0x060031C6 RID: 12742 RVA: 0x000A7283 File Offset: 0x000A5483
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			PlayerRoleManager.RoleChanged value;
			if ((value = ChaseThemeProviders.<>O.<0>__OnRoleChanged) == null)
			{
				value = (ChaseThemeProviders.<>O.<0>__OnRoleChanged = new PlayerRoleManager.RoleChanged(ChaseThemeProviders.OnRoleChanged));
			}
			PlayerRoleManager.OnRoleChanged += value;
		}

		// Token: 0x060031C7 RID: 12743 RVA: 0x000A72A8 File Offset: 0x000A54A8
		private static void OnRoleChanged(ReferenceHub userHub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
		{
			IChaseThemeProvider item;
			if (ChaseThemeProviders.TryFindProvider(prevRole, out item))
			{
				ChaseThemeProviders.Instances.Remove(item);
			}
			IChaseThemeProvider item2;
			if (ChaseThemeProviders.TryFindProvider(newRole, out item2))
			{
				ChaseThemeProviders.Instances.Add(item2);
			}
		}

		// Token: 0x060031C8 RID: 12744 RVA: 0x000A72E0 File Offset: 0x000A54E0
		private static bool TryFindProvider(PlayerRoleBase prb, out IChaseThemeProvider provider)
		{
			ISubroutinedRole subroutinedRole = prb as ISubroutinedRole;
			if (subroutinedRole != null)
			{
				SubroutineBase[] allSubroutines = subroutinedRole.SubroutineModule.AllSubroutines;
				for (int i = 0; i < allSubroutines.Length; i++)
				{
					IChaseThemeProvider chaseThemeProvider = allSubroutines[i] as IChaseThemeProvider;
					if (chaseThemeProvider != null)
					{
						provider = chaseThemeProvider;
						return true;
					}
				}
			}
			provider = null;
			return false;
		}

		// Token: 0x040026DE RID: 9950
		public static readonly List<IChaseThemeProvider> Instances = new List<IChaseThemeProvider>();

		// Token: 0x02000884 RID: 2180
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040026DF RID: 9951
			public static PlayerRoleManager.RoleChanged <0>__OnRoleChanged;
		}
	}
}
