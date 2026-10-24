using System;
using System.Collections.Generic;
using InventorySystem.Items.ThrowableProjectiles;
using MapGeneration;
using PlayerRoles.PlayableScps.Scp079.Cameras;

namespace Achievements.Handlers
{
	// Token: 0x02000B15 RID: 2837
	public class SignalLostHandler : AchievementHandlerBase
	{
		// Token: 0x06003FFC RID: 16380 RVA: 0x000D4702 File Offset: 0x000D2902
		internal override void OnInitialize()
		{
			Scp2176Projectile.OnServerShattered += this.HandleSignalLostAchievment;
		}

		// Token: 0x06003FFD RID: 16381 RVA: 0x000D4718 File Offset: 0x000D2918
		private void HandleSignalLostAchievment(Scp2176Projectile projectile, RoomIdentifier identifier)
		{
			if (!projectile.PreviousOwner.IsSet)
			{
				return;
			}
			ReferenceHub hub = projectile.PreviousOwner.Hub;
			bool flag = false;
			using (List<Scp079Camera>.Enumerator enumerator = Scp079Camera.GetRoomCameras(identifier).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsActive)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				AchievementHandlerBase.ServerAchieve(hub.connectionToClient, AchievementName.SignalLost);
			}
		}
	}
}
