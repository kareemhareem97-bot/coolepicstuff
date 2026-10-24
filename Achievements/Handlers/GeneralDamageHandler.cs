using System;
using System.Runtime.CompilerServices;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;

namespace Achievements.Handlers
{
	// Token: 0x02000AF6 RID: 2806
	public class GeneralDamageHandler : AchievementHandlerBase
	{
		// Token: 0x06003FAB RID: 16299 RVA: 0x000D3708 File Offset: 0x000D1908
		internal override void OnInitialize()
		{
			Action<ReferenceHub, DamageHandlerBase> value;
			if ((value = GeneralDamageHandler.<>O.<0>__AnyDamage) == null)
			{
				value = (GeneralDamageHandler.<>O.<0>__AnyDamage = new Action<ReferenceHub, DamageHandlerBase>(GeneralDamageHandler.AnyDamage));
			}
			PlayerStats.OnAnyPlayerDamaged += value;
		}

		// Token: 0x06003FAC RID: 16300 RVA: 0x000D372C File Offset: 0x000D192C
		private static void AnyDamage(ReferenceHub ply, DamageHandlerBase handler)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			HealthStat module = ply.playerStats.GetModule<HealthStat>();
			UniversalDamageHandler universalDamageHandler = handler as UniversalDamageHandler;
			if (universalDamageHandler != null && universalDamageHandler.TranslationId == DeathTranslations.Falldown.Id && module.CurValue > 0f)
			{
				float num = module.CurValue + universalDamageHandler.Damage;
				if (num > 0f && module.CurValue / num < 0.5f)
				{
					AchievementHandlerBase.ServerAchieve(ply.connectionToClient, AchievementName.WalkItOff);
				}
			}
			StandardDamageHandler standardDamageHandler = handler as StandardDamageHandler;
			if (standardDamageHandler == null)
			{
				return;
			}
			if (!ply.IsHuman())
			{
				return;
			}
			if (module.CurValue > 0f && module.CurValue - standardDamageHandler.AbsorbedAhpDamage <= 0f)
			{
				AchievementHandlerBase.ServerAchieve(ply.networkIdentity.connectionToClient, AchievementName.DidntEvenFeelThat);
			}
		}

		// Token: 0x04002E80 RID: 11904
		private const float WalkItOffThreshHold = 0.5f;

		// Token: 0x02000AF7 RID: 2807
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04002E81 RID: 11905
			public static Action<ReferenceHub, DamageHandlerBase> <0>__AnyDamage;
		}
	}
}
