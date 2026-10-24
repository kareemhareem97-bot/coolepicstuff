using System;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Spectating;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x0200089B RID: 2203
	public class AnomalousTarget : StatusEffectBase
	{
		// Token: 0x0600320E RID: 12814 RVA: 0x000A8164 File Offset: 0x000A6364
		protected override void Enabled()
		{
			base.Enabled();
			this._effectInstance = UnityEngine.Object.Instantiate<GameObject>(this._effectPrefab, base.Hub.transform);
			this._effectInstance.SetActive(this.GetParticleVisibilityState(false));
			PlayerRoleManager.OnRoleChanged += this.OnRoleChanged;
			SpectatorTargetTracker.OnTargetChanged += this.OnSpectatorChange;
		}

		// Token: 0x0600320F RID: 12815 RVA: 0x000A81C7 File Offset: 0x000A63C7
		protected override void Disabled()
		{
			base.Disabled();
			SpectatorTargetTracker.OnTargetChanged -= this.OnSpectatorChange;
			PlayerRoleManager.OnRoleChanged -= this.OnRoleChanged;
			UnityEngine.Object.Destroy(this._effectInstance);
		}

		// Token: 0x06003210 RID: 12816 RVA: 0x000A81FC File Offset: 0x000A63FC
		protected override void OnEffectUpdate()
		{
			base.OnEffectUpdate();
			bool particleVisibilityState = this.GetParticleVisibilityState(true);
			this._effectInstance.SetActive(particleVisibilityState);
		}

		// Token: 0x06003211 RID: 12817 RVA: 0x000A8224 File Offset: 0x000A6424
		private bool GetParticleVisibilityState(bool skipSpectatorCheck = false)
		{
			if (!base.IsEnabled || base.Hub.isLocalPlayer)
			{
				return false;
			}
			ReferenceHub referenceHub;
			if (!ReferenceHub.TryGetLocalHub(out referenceHub))
			{
				return false;
			}
			PlayerRoleBase currentRole = referenceHub.roleManager.CurrentRole;
			if (currentRole.Team == Team.SCPs)
			{
				IFpcRole fpcRole = currentRole as IFpcRole;
				return fpcRole != null && fpcRole.SqrDistanceTo(base.Hub) <= 10000f;
			}
			if (!(referenceHub.roleManager.CurrentRole is SpectatorRole))
			{
				return false;
			}
			SpectatableModuleBase currentTarget = SpectatorTargetTracker.CurrentTarget;
			return !(currentTarget == null) && currentTarget.MainRole.Team != Team.SCPs;
		}

		// Token: 0x06003212 RID: 12818 RVA: 0x000A82C1 File Offset: 0x000A64C1
		private void OnRoleChanged(ReferenceHub userHub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
		{
			if (this._effectInstance == null)
			{
				return;
			}
			this._effectInstance.SetActive(this.GetParticleVisibilityState(false));
		}

		// Token: 0x06003213 RID: 12819 RVA: 0x000A82C1 File Offset: 0x000A64C1
		private void OnSpectatorChange()
		{
			if (this._effectInstance == null)
			{
				return;
			}
			this._effectInstance.SetActive(this.GetParticleVisibilityState(false));
		}

		// Token: 0x0400271C RID: 10012
		private const float ParticleVisibilitySqrDistance = 10000f;

		// Token: 0x0400271D RID: 10013
		[SerializeField]
		private GameObject _effectPrefab;

		// Token: 0x0400271E RID: 10014
		private GameObject _effectInstance;
	}
}
