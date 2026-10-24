using System;
using Mirror;
using PlayerRoles;
using RemoteAdmin.Interfaces;
using UserSettings;
using UserSettings.VideoSettings;

namespace CustomPlayerEffects
{
	// Token: 0x020008BE RID: 2238
	public class InsufficientLighting : StatusEffectBase, ICustomDisplayName
	{
		// Token: 0x1700099B RID: 2459
		// (get) Token: 0x060032D6 RID: 13014 RVA: 0x000A9520 File Offset: 0x000A7720
		private PlayerRoleBase CurRole
		{
			get
			{
				return base.Hub.roleManager.CurrentRole;
			}
		}

		// Token: 0x1700099C RID: 2460
		// (get) Token: 0x060032D7 RID: 13015 RVA: 0x000A9532 File Offset: 0x000A7732
		public string DisplayName { get; }

		// Token: 0x1700099D RID: 2461
		// (get) Token: 0x060032D8 RID: 13016 RVA: 0x000A953A File Offset: 0x000A773A
		public bool CanBeDisplayed { get; }

		// Token: 0x1700099E RID: 2462
		// (get) Token: 0x060032D9 RID: 13017 RVA: 0x00004094 File Offset: 0x00002294
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Technical;
			}
		}

		// Token: 0x1700099F RID: 2463
		// (get) Token: 0x060032DA RID: 13018 RVA: 0x0000548E File Offset: 0x0000368E
		public static float DefaultIntensity
		{
			get
			{
				return 0f;
			}
		}

		// Token: 0x060032DB RID: 13019 RVA: 0x000A9542 File Offset: 0x000A7742
		internal override void OnRoleChanged(PlayerRoleBase previousRole, PlayerRoleBase newRole)
		{
			base.OnRoleChanged(previousRole, newRole);
			this._prevTarget = false;
		}

		// Token: 0x060032DC RID: 13020 RVA: 0x000A9553 File Offset: 0x000A7753
		protected override void Start()
		{
			base.Start();
			StaticUnityMethods.OnUpdate += this.AlwaysUpdate;
		}

		// Token: 0x060032DD RID: 13021 RVA: 0x000A956C File Offset: 0x000A776C
		private void OnDestroy()
		{
			StaticUnityMethods.OnUpdate -= this.AlwaysUpdate;
		}

		// Token: 0x060032DE RID: 13022 RVA: 0x000A957F File Offset: 0x000A777F
		private void AlwaysUpdate()
		{
			if (NetworkServer.active)
			{
				this.UpdateServer();
			}
		}

		// Token: 0x060032DF RID: 13023 RVA: 0x000A9590 File Offset: 0x000A7790
		private void UpdateServer()
		{
			IAmbientLightRole ambientLightRole = this.CurRole as IAmbientLightRole;
			bool flag = ambientLightRole != null && ambientLightRole.InsufficientLight;
			if (flag == this._prevTarget)
			{
				return;
			}
			base.Intensity = (flag ? 1 : 0);
			this._prevTarget = flag;
		}

		// Token: 0x04002767 RID: 10087
		private static readonly CachedUserSetting<bool> RenderLights = new CachedUserSetting<bool>(LightingVideoSetting.RenderLights);

		// Token: 0x04002768 RID: 10088
		private bool _prevTarget;

		// Token: 0x04002769 RID: 10089
		private const float NoLightsAmbient = 0.03f;
	}
}
