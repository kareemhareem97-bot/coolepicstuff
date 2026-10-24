using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GameCore;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;
using RemoteAdmin.Interfaces;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008DE RID: 2270
	public class SpawnProtected : StatusEffectBase, ISpectatorDataPlayerEffect, ICustomDisplayName, IDamageModifierEffect, IPulseEffect
	{
		// Token: 0x17000A08 RID: 2568
		// (get) Token: 0x060033ED RID: 13293 RVA: 0x000AB65A File Offset: 0x000A985A
		public string DisplayName
		{
			get
			{
				return "Spawn Protection";
			}
		}

		// Token: 0x17000A09 RID: 2569
		// (get) Token: 0x060033EE RID: 13294 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public bool CanBeDisplayed
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000A0A RID: 2570
		// (get) Token: 0x060033EF RID: 13295 RVA: 0x00004070 File Offset: 0x00002270
		public bool DamageModifierActive
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x17000A0B RID: 2571
		// (get) Token: 0x060033F0 RID: 13296 RVA: 0x00040288 File Offset: 0x0003E488
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Positive;
			}
		}

		// Token: 0x060033F1 RID: 13297 RVA: 0x000AB661 File Offset: 0x000A9861
		internal override void OnRoleChanged(PlayerRoleBase previousRole, PlayerRoleBase newRole)
		{
			if (!SpawnProtected.TryGiveProtection(base.Hub))
			{
				base.OnRoleChanged(previousRole, newRole);
			}
		}

		// Token: 0x060033F2 RID: 13298 RVA: 0x000AB678 File Offset: 0x000A9878
		public float GetDamageModifier(float baseDamage, DamageHandlerBase handler, HitboxType hitboxType)
		{
			if (!SpawnProtected.IsProtectionEnabled)
			{
				return 1f;
			}
			AttackerDamageHandler attackerDamageHandler = handler as AttackerDamageHandler;
			if (attackerDamageHandler == null)
			{
				if (!SpawnProtected.PreventAllDamage)
				{
					return 1f;
				}
				return this.CancelDamage();
			}
			else
			{
				if (!(attackerDamageHandler.Attacker.Hub == base.Hub))
				{
					return this.CancelDamage();
				}
				return 1f;
			}
		}

		// Token: 0x060033F3 RID: 13299 RVA: 0x000AB6D4 File Offset: 0x000A98D4
		public bool GetSpectatorText(out string display)
		{
			display = "Spawn protected";
			return base.IsEnabled;
		}

		// Token: 0x060033F4 RID: 13300 RVA: 0x00002E9A File Offset: 0x0000109A
		public void ExecutePulse()
		{
		}

		// Token: 0x060033F5 RID: 13301 RVA: 0x000AB6E3 File Offset: 0x000A98E3
		private float CancelDamage()
		{
			base.Hub.playerEffectsController.ServerSendPulse<SpawnProtected>();
			return 0f;
		}

		// Token: 0x060033F6 RID: 13302 RVA: 0x000AB6FA File Offset: 0x000A98FA
		public static bool CheckPlayer(ReferenceHub hub)
		{
			return (!NetworkServer.active || SpawnProtected.IsProtectionEnabled) && hub.playerEffectsController.GetEffect<SpawnProtected>().IsEnabled;
		}

		// Token: 0x060033F7 RID: 13303 RVA: 0x000AB71C File Offset: 0x000A991C
		public static bool TryGiveProtection(ReferenceHub hub)
		{
			if (!NetworkServer.active || !SpawnProtected.IsProtectionEnabled)
			{
				return false;
			}
			PlayerRoleBase currentRole = hub.roleManager.CurrentRole;
			if (!(currentRole is IHealthbarRole))
			{
				return false;
			}
			if (!SpawnProtected.ProtectedTeams.Contains(currentRole.Team))
			{
				return false;
			}
			hub.playerEffectsController.EnableEffect<SpawnProtected>(SpawnProtected.SpawnDuration, false);
			return true;
		}

		// Token: 0x060033F8 RID: 13304 RVA: 0x000AB778 File Offset: 0x000A9978
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			if (!SpawnProtected._eventAssigned)
			{
				Delegate onConfigReloaded = ConfigFile.OnConfigReloaded;
				Action b;
				if ((b = SpawnProtected.<>O.<0>__RefreshConfigs) == null)
				{
					b = (SpawnProtected.<>O.<0>__RefreshConfigs = new Action(SpawnProtected.RefreshConfigs));
				}
				ConfigFile.OnConfigReloaded = (Action)Delegate.Combine(onConfigReloaded, b);
				Delegate onRefreshed = ServerConfigSynchronizer.OnRefreshed;
				Action b2;
				if ((b2 = SpawnProtected.<>O.<0>__RefreshConfigs) == null)
				{
					b2 = (SpawnProtected.<>O.<0>__RefreshConfigs = new Action(SpawnProtected.RefreshConfigs));
				}
				ServerConfigSynchronizer.OnRefreshed = (Action)Delegate.Combine(onRefreshed, b2);
				SpawnProtected._eventAssigned = true;
			}
			SpawnProtected.RefreshConfigs();
		}

		// Token: 0x060033F9 RID: 13305 RVA: 0x000AB7F8 File Offset: 0x000A99F8
		private static void RefreshConfigs()
		{
			SpawnProtected.ProtectedTeams.Clear();
			SpawnProtected.IsProtectionEnabled = ConfigFile.ServerConfig.GetBool("spawn_protect_enabled", false);
			SpawnProtected.CanShoot = ConfigFile.ServerConfig.GetBool("spawn_protect_can_shoot", false);
			SpawnProtected.PreventAllDamage = ConfigFile.ServerConfig.GetBool("spawn_protect_prevent_all", false);
			SpawnProtected.SpawnDuration = ConfigFile.ServerConfig.GetFloat("spawn_protect_time", 8f);
			List<int> intList = ConfigFile.ServerConfig.GetIntList("spawn_protect_team");
			if (intList.Count == 0)
			{
				SpawnProtected.ProtectedTeams.Add(Team.FoundationForces);
				SpawnProtected.ProtectedTeams.Add(Team.ChaosInsurgency);
				return;
			}
			intList.ForEach(delegate(int t)
			{
				SpawnProtected.ProtectedTeams.Add((Team)t);
			});
		}

		// Token: 0x040027EC RID: 10220
		public static bool IsProtectionEnabled;

		// Token: 0x040027ED RID: 10221
		public static bool CanShoot;

		// Token: 0x040027EE RID: 10222
		public static bool PreventAllDamage;

		// Token: 0x040027EF RID: 10223
		public static float SpawnDuration;

		// Token: 0x040027F0 RID: 10224
		public static readonly List<Team> ProtectedTeams = new List<Team>();

		// Token: 0x040027F1 RID: 10225
		private static bool _eventAssigned = false;

		// Token: 0x020008DF RID: 2271
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x040027F2 RID: 10226
			public static Action <0>__RefreshConfigs;
		}
	}
}
