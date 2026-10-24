using System;
using System.Collections.Generic;
using InventorySystem.Items.Firearms.Modules;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;

namespace Achievements.Handlers
{
	// Token: 0x02000B04 RID: 2820
	public class LMGGHandler : AchievementHandlerBase
	{
		// Token: 0x06003FD3 RID: 16339 RVA: 0x000D3FAF File Offset: 0x000D21AF
		internal override void OnInitialize()
		{
			PlayerStats.OnAnyPlayerDied += this.OnAnyPlayerDied;
		}

		// Token: 0x06003FD4 RID: 16340 RVA: 0x000D3FC2 File Offset: 0x000D21C2
		internal override void OnRoundStarted()
		{
			LMGGHandler.LastHeldTime.Clear();
			LMGGHandler.KillsSinceHeld.Clear();
		}

		// Token: 0x06003FD5 RID: 16341 RVA: 0x000D3FD8 File Offset: 0x000D21D8
		private void OnAnyPlayerDied(ReferenceHub victim, DamageHandlerBase handler)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			FirearmDamageHandler firearmDamageHandler = handler as FirearmDamageHandler;
			if (firearmDamageHandler == null)
			{
				return;
			}
			ItemType weaponType = firearmDamageHandler.WeaponType;
			if (weaponType != ItemType.GunFRMG0 && weaponType != ItemType.GunLogicer)
			{
				return;
			}
			ReferenceHub hub = firearmDamageHandler.Attacker.Hub;
			if (!hub.IsHuman())
			{
				return;
			}
			if (!HitboxIdentity.IsEnemy(firearmDamageHandler.Attacker.Role, victim.GetRoleId()))
			{
				return;
			}
			SimpleTriggerModule.ReceivedData data = SimpleTriggerModule.GetData(firearmDamageHandler.Firearm.ItemSerial);
			double pressTime;
			if (!LMGGHandler.LastHeldTime.TryGetValue(hub, out pressTime))
			{
				pressTime = data.PressTime;
				LMGGHandler.LastHeldTime[hub] = pressTime;
			}
			if (Math.Abs(data.PressTime - pressTime) > 1E-09)
			{
				LMGGHandler.LastHeldTime[hub] = data.PressTime;
				LMGGHandler.KillsSinceHeld[hub] = 1;
				return;
			}
			int num;
			if (!LMGGHandler.KillsSinceHeld.TryGetValue(hub, out num))
			{
				num = 0;
			}
			num = (LMGGHandler.KillsSinceHeld[hub] = num + 1);
			if (num < 3)
			{
				return;
			}
			AchievementHandlerBase.ServerAchieve(hub.connectionToClient, AchievementName.LMGG);
		}

		// Token: 0x04002E8B RID: 11915
		private const int KillsNeeded = 3;

		// Token: 0x04002E8C RID: 11916
		private static readonly Dictionary<ReferenceHub, double> LastHeldTime = new Dictionary<ReferenceHub, double>();

		// Token: 0x04002E8D RID: 11917
		private static readonly Dictionary<ReferenceHub, int> KillsSinceHeld = new Dictionary<ReferenceHub, int>();
	}
}
