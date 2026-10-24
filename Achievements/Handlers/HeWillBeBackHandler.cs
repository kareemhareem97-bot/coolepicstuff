using System;

namespace Achievements.Handlers
{
	// Token: 0x02000AFE RID: 2814
	public class HeWillBeBackHandler : AchievementHandlerBase
	{
		// Token: 0x06003FC0 RID: 16320 RVA: 0x000D3CE0 File Offset: 0x000D1EE0
		internal override void OnInitialize()
		{
			PocketDimensionTeleport.OnPlayerEscapePocketDimension += this.OnPlayerEscapePocketDimension;
		}

		// Token: 0x06003FC1 RID: 16321 RVA: 0x000D3CF3 File Offset: 0x000D1EF3
		private void OnPlayerEscapePocketDimension(ReferenceHub hub)
		{
			AchievementHandlerBase.ServerAchieve(hub.connectionToClient, AchievementName.HeWillBeBack);
		}
	}
}
