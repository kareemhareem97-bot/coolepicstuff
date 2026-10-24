using System;
using PlayerRoles.Voice;

namespace Achievements.Handlers
{
	// Token: 0x02000B00 RID: 2816
	public class IntercomHandler : AchievementHandlerBase
	{
		// Token: 0x06003FC8 RID: 16328 RVA: 0x000D3DD9 File Offset: 0x000D1FD9
		internal override void OnInitialize()
		{
			Intercom.OnServerBeginUsage += this.OnBeginUsage;
		}

		// Token: 0x06003FC9 RID: 16329 RVA: 0x000D3DEC File Offset: 0x000D1FEC
		private void OnBeginUsage(ReferenceHub hub)
		{
			if (hub == null)
			{
				return;
			}
			AchievementHandlerBase.ServerAchieve(hub.connectionToClient, AchievementName.IsThisThingOn);
		}
	}
}
