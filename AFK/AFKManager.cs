using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using GameCore;
using Mirror;
using PlayerRoles;
using UnityEngine;

namespace AFK
{
	// Token: 0x0200075F RID: 1887
	public static class AFKManager
	{
		// Token: 0x1400009B RID: 155
		// (add) Token: 0x06002C7C RID: 11388 RVA: 0x00096AD8 File Offset: 0x00094CD8
		// (remove) Token: 0x06002C7D RID: 11389 RVA: 0x00096B0C File Offset: 0x00094D0C
		public static event AFKManager.AFKKick OnAFKKick;

		// Token: 0x06002C7E RID: 11390 RVA: 0x00096B3F File Offset: 0x00094D3F
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void Init()
		{
			Delegate onConfigReloaded = ConfigFile.OnConfigReloaded;
			Action b;
			if ((b = AFKManager.<>O.<0>__ConfigReloaded) == null)
			{
				b = (AFKManager.<>O.<0>__ConfigReloaded = new Action(AFKManager.ConfigReloaded));
			}
			ConfigFile.OnConfigReloaded = (Action)Delegate.Combine(onConfigReloaded, b);
		}

		// Token: 0x06002C7F RID: 11391 RVA: 0x00096B70 File Offset: 0x00094D70
		private static void ConfigReloaded()
		{
			AFKManager._constantlyCheck = ConfigFile.ServerConfig.GetBool("constantly_check_afk", false);
			AFKManager._kickTime = ConfigFile.ServerConfig.GetFloat("afk_time", 90f);
			AFKManager._kickMessage = ConfigFile.ServerConfig.GetString("afk_kick_message", "AFK");
			if (AFKManager._kickTime > 0f)
			{
				if (!AFKManager._eventsStatus)
				{
					AFKManager._eventsStatus = true;
					Action<ReferenceHub> value;
					if ((value = AFKManager.<>O.<1>__AddPlayer) == null)
					{
						value = (AFKManager.<>O.<1>__AddPlayer = new Action<ReferenceHub>(AFKManager.AddPlayer));
					}
					ReferenceHub.OnPlayerAdded += value;
					Action<ReferenceHub> value2;
					if ((value2 = AFKManager.<>O.<2>__RemovePlayer) == null)
					{
						value2 = (AFKManager.<>O.<2>__RemovePlayer = new Action<ReferenceHub>(AFKManager.RemovePlayer));
					}
					ReferenceHub.OnPlayerRemoved += value2;
					Action value3;
					if ((value3 = AFKManager.<>O.<3>__OnUpdate) == null)
					{
						value3 = (AFKManager.<>O.<3>__OnUpdate = new Action(AFKManager.OnUpdate));
					}
					StaticUnityMethods.OnUpdate += value3;
					PlayerRoleManager.RoleChanged value4;
					if ((value4 = AFKManager.<>O.<4>__RoleChange) == null)
					{
						value4 = (AFKManager.<>O.<4>__RoleChange = new PlayerRoleManager.RoleChanged(AFKManager.RoleChange));
					}
					PlayerRoleManager.OnRoleChanged += value4;
				}
				return;
			}
			if (!AFKManager._eventsStatus)
			{
				return;
			}
			AFKManager._eventsStatus = false;
			Action<ReferenceHub> value5;
			if ((value5 = AFKManager.<>O.<1>__AddPlayer) == null)
			{
				value5 = (AFKManager.<>O.<1>__AddPlayer = new Action<ReferenceHub>(AFKManager.AddPlayer));
			}
			ReferenceHub.OnPlayerAdded -= value5;
			Action<ReferenceHub> value6;
			if ((value6 = AFKManager.<>O.<2>__RemovePlayer) == null)
			{
				value6 = (AFKManager.<>O.<2>__RemovePlayer = new Action<ReferenceHub>(AFKManager.RemovePlayer));
			}
			ReferenceHub.OnPlayerRemoved -= value6;
			Action value7;
			if ((value7 = AFKManager.<>O.<3>__OnUpdate) == null)
			{
				value7 = (AFKManager.<>O.<3>__OnUpdate = new Action(AFKManager.OnUpdate));
			}
			StaticUnityMethods.OnUpdate -= value7;
			PlayerRoleManager.RoleChanged value8;
			if ((value8 = AFKManager.<>O.<4>__RoleChange) == null)
			{
				value8 = (AFKManager.<>O.<4>__RoleChange = new PlayerRoleManager.RoleChanged(AFKManager.RoleChange));
			}
			PlayerRoleManager.OnRoleChanged -= value8;
		}

		// Token: 0x06002C80 RID: 11392 RVA: 0x00096CF4 File Offset: 0x00094EF4
		public static void AddPlayer(ReferenceHub hub)
		{
			if (!NetworkServer.active || hub == ReferenceHub.HostHub || AFKManager.AFKTimers.ContainsKey(hub))
			{
				return;
			}
			if (PermissionsHandler.IsPermitted(hub.serverRoles.Permissions, PlayerPermissions.AFKImmunity))
			{
				return;
			}
			AFKManager.AFKTimers.Add(hub, Stopwatch.StartNew());
		}

		// Token: 0x06002C81 RID: 11393 RVA: 0x00096D4C File Offset: 0x00094F4C
		private static void RemovePlayer(ReferenceHub hub)
		{
			AFKManager.AFKTimers.Remove(hub);
		}

		// Token: 0x06002C82 RID: 11394 RVA: 0x00096D5C File Offset: 0x00094F5C
		private static void RoleChange(ReferenceHub hub, PlayerRoleBase oldRole, PlayerRoleBase newRole)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			Stopwatch stopwatch;
			if (!AFKManager.AFKTimers.TryGetValue(hub, out stopwatch))
			{
				return;
			}
			if (PermissionsHandler.IsPermitted(hub.serverRoles.Permissions, PlayerPermissions.AFKImmunity) || hub == ReferenceHub.HostHub)
			{
				AFKManager.AFKTimers.Remove(hub);
				return;
			}
			stopwatch.Restart();
		}

		// Token: 0x06002C83 RID: 11395 RVA: 0x00096DBC File Offset: 0x00094FBC
		private static void OnUpdate()
		{
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				Stopwatch stopwatch;
				if (AFKManager.AFKTimers.TryGetValue(referenceHub, out stopwatch) && stopwatch.IsRunning)
				{
					IAFKRole iafkrole = referenceHub.roleManager.CurrentRole as IAFKRole;
					if (iafkrole != null)
					{
						if (!iafkrole.IsAFK)
						{
							if (AFKManager._constantlyCheck)
							{
								stopwatch.Restart();
							}
							else
							{
								stopwatch.Reset();
							}
						}
						else if (stopwatch.Elapsed.TotalSeconds >= (double)AFKManager._kickTime)
						{
							stopwatch.Reset();
							AFKManager.AFKKick onAFKKick = AFKManager.OnAFKKick;
							if (onAFKKick != null)
							{
								onAFKKick(referenceHub);
							}
							BanPlayer.KickUser(referenceHub, AFKManager._kickMessage);
						}
					}
				}
			}
		}

		// Token: 0x040022BA RID: 8890
		private static readonly Dictionary<ReferenceHub, Stopwatch> AFKTimers = new Dictionary<ReferenceHub, Stopwatch>();

		// Token: 0x040022BB RID: 8891
		private static float _kickTime;

		// Token: 0x040022BC RID: 8892
		private static bool _constantlyCheck;

		// Token: 0x040022BD RID: 8893
		private static string _kickMessage;

		// Token: 0x040022BE RID: 8894
		private static bool _eventsStatus;

		// Token: 0x02000760 RID: 1888
		// (Invoke) Token: 0x06002C86 RID: 11398
		public delegate void AFKKick(ReferenceHub userHub);

		// Token: 0x02000761 RID: 1889
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040022BF RID: 8895
			public static Action <0>__ConfigReloaded;

			// Token: 0x040022C0 RID: 8896
			public static Action<ReferenceHub> <1>__AddPlayer;

			// Token: 0x040022C1 RID: 8897
			public static Action<ReferenceHub> <2>__RemovePlayer;

			// Token: 0x040022C2 RID: 8898
			public static Action <3>__OnUpdate;

			// Token: 0x040022C3 RID: 8899
			public static PlayerRoleManager.RoleChanged <4>__RoleChange;
		}
	}
}
