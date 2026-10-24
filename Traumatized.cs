using System;
using PlayerRoles;
using UnityEngine.Rendering;
using Utils.NonAllocLINQ;

namespace CustomPlayerEffects
{
	// Token: 0x020008E8 RID: 2280
	public class Traumatized : StatusEffectBase
	{
		// Token: 0x17000A40 RID: 2624
		// (get) Token: 0x06003459 RID: 13401 RVA: 0x000A84BF File Offset: 0x000A66BF
		public override bool AllowEnabling
		{
			get
			{
				return !SpawnProtected.CheckPlayer(base.Hub);
			}
		}

		// Token: 0x0600345A RID: 13402 RVA: 0x000AC789 File Offset: 0x000AA989
		protected override void Start()
		{
			base.Start();
			PlayerRoleManager.OnServerRoleSet += this.OnServerRoleChanged;
		}

		// Token: 0x0600345B RID: 13403 RVA: 0x000AC7A2 File Offset: 0x000AA9A2
		private void OnDestroy()
		{
			PlayerRoleManager.OnServerRoleSet -= this.OnServerRoleChanged;
		}

		// Token: 0x0600345C RID: 13404 RVA: 0x000AC7B8 File Offset: 0x000AA9B8
		private void OnServerRoleChanged(ReferenceHub hub, RoleTypeId newRole, RoleChangeReason reason)
		{
			if (reason != RoleChangeReason.Died || newRole != RoleTypeId.Spectator || hub.GetRoleId() != RoleTypeId.Scp106)
			{
				return;
			}
			if (ReferenceHub.AllHubs.Any((ReferenceHub x) => x != hub && x.GetRoleId() == RoleTypeId.Scp106))
			{
				return;
			}
			base.ServerSetState(0, 0f, false);
		}

		// Token: 0x0600345D RID: 13405 RVA: 0x000AC810 File Offset: 0x000AAA10
		protected override void Enabled()
		{
			base.Enabled();
			if (ReferenceHub.AllHubs.Any((ReferenceHub x) => x.GetRoleId() == RoleTypeId.Scp106))
			{
				return;
			}
			base.ServerSetState(0, 0f, false);
		}

		// Token: 0x04002828 RID: 10280
		public Volume PPVolume;
	}
}
