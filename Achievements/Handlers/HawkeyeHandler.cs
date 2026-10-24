using System;
using CustomPlayerEffects;
using InventorySystem.Items.Usables.Scp1344;

namespace Achievements.Handlers
{
	// Token: 0x02000AFD RID: 2813
	public class HawkeyeHandler : AchievementHandlerBase
	{
		// Token: 0x06003FBD RID: 16317 RVA: 0x000D3C8D File Offset: 0x000D1E8D
		internal override void OnInitialize()
		{
			Scp1344HumanXrayProvider.OnPlayerSeen += this.OnPlayerSeen;
		}

		// Token: 0x06003FBE RID: 16318 RVA: 0x000D3CA0 File Offset: 0x000D1EA0
		private void OnPlayerSeen(ReferenceHub owner, ReferenceHub seenPlayer)
		{
			if (!owner.isLocalPlayer)
			{
				return;
			}
			Invisible invisible;
			if (!seenPlayer.playerEffectsController.TryGetEffect<Invisible>(out invisible))
			{
				return;
			}
			if (!invisible.IsEnabled)
			{
				return;
			}
			if (!HitboxIdentity.IsEnemy(owner, seenPlayer))
			{
				return;
			}
			AchievementHandlerBase.ClientAchieve(AchievementName.Hawkeye);
		}
	}
}
