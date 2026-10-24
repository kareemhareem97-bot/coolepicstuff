using System;
using CustomPlayerEffects;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using PlayerRoles;

namespace Hazards
{
	// Token: 0x02000850 RID: 2128
	public class SinkholeEnvironmentalHazard : EnvironmentalHazard
	{
		// Token: 0x06003087 RID: 12423 RVA: 0x000A1E69 File Offset: 0x000A0069
		public override bool OnEnter(ReferenceHub player)
		{
			if (!this.IsActive || player.IsSCP(true))
			{
				return false;
			}
			if (!base.OnEnter(player))
			{
				return false;
			}
			player.playerEffectsController.EnableEffect<Sinkhole>(1f, false);
			PlayerEvents.OnEnteredHazard(new PlayerEnteredHazardEventArgs(player, this));
			return true;
		}

		// Token: 0x06003088 RID: 12424 RVA: 0x000A1EA8 File Offset: 0x000A00A8
		public override void OnStay(ReferenceHub player)
		{
			player.playerEffectsController.EnableEffect<Sinkhole>(1f, false);
		}

		// Token: 0x06003089 RID: 12425 RVA: 0x000A1EBC File Offset: 0x000A00BC
		public override bool OnExit(ReferenceHub player)
		{
			if (!base.OnExit(player))
			{
				return false;
			}
			player.playerEffectsController.EnableEffect<Sinkhole>(1f, false);
			PlayerEvents.OnLeftHazard(new PlayerLeftHazardEventArgs(player, this));
			return true;
		}

		// Token: 0x0600308A RID: 12426 RVA: 0x000A1EE8 File Offset: 0x000A00E8
		protected override void Start()
		{
			base.Start();
			this.ClientApplyDecalSize();
		}

		// Token: 0x0600308B RID: 12427 RVA: 0x00002E9A File Offset: 0x0000109A
		private void ClientApplyDecalSize()
		{
		}

		// Token: 0x0600308D RID: 12429 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Weaved()
		{
			return true;
		}
	}
}
