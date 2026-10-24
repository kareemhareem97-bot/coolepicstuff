using System;
using AudioPooling;
using InventorySystem.Items.Usables.Scp1344;
using Mirror;
using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers.Wearables;
using PlayerRoles.Visibility;
using RemoteAdmin.Interfaces;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008D0 RID: 2256
	public class Scp1344 : StatusEffectBase, ICustomDisplayName
	{
		// Token: 0x170009D6 RID: 2518
		// (get) Token: 0x06003370 RID: 13168 RVA: 0x00034727 File Offset: 0x00032927
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Mixed;
			}
		}

		// Token: 0x170009D7 RID: 2519
		// (get) Token: 0x06003371 RID: 13169 RVA: 0x000AA991 File Offset: 0x000A8B91
		public string DisplayName
		{
			get
			{
				return "SCP-1344";
			}
		}

		// Token: 0x170009D8 RID: 2520
		// (get) Token: 0x06003372 RID: 13170 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public bool CanBeDisplayed
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06003373 RID: 13171 RVA: 0x000AA998 File Offset: 0x000A8B98
		protected override void Start()
		{
			this.DisableEffect();
		}

		// Token: 0x06003374 RID: 13172 RVA: 0x000AA9A0 File Offset: 0x000A8BA0
		protected override void Awake()
		{
			base.Awake();
			Scp1344XrayProviderBase[] xrayProviders = this._xrayProviders;
			for (int i = 0; i < xrayProviders.Length; i++)
			{
				xrayProviders[i].OnInit(this);
			}
		}

		// Token: 0x06003375 RID: 13173 RVA: 0x000AA9D1 File Offset: 0x000A8BD1
		protected override void Enabled()
		{
			if (NetworkServer.active)
			{
				base.Hub.EnableWearables(WearableElements.Scp1344Goggles);
			}
			this.PlaySound();
		}

		// Token: 0x06003376 RID: 13174 RVA: 0x000AA9EC File Offset: 0x000A8BEC
		protected override void Disabled()
		{
			if (NetworkServer.active)
			{
				base.Hub.DisableWearables(WearableElements.Scp1344Goggles);
			}
			this.UpdateVision(false);
		}

		// Token: 0x06003377 RID: 13175 RVA: 0x000AAA08 File Offset: 0x000A8C08
		protected override void OnEffectUpdate()
		{
			base.OnEffectUpdate();
			this.UpdateVision(base.IsPOV || NetworkServer.active);
		}

		// Token: 0x06003378 RID: 13176 RVA: 0x000AAA26 File Offset: 0x000A8C26
		public override void OnStopSpectating()
		{
			base.OnStopSpectating();
			this.CloseSoundSessions();
			this.UpdateVision(false);
		}

		// Token: 0x06003379 RID: 13177 RVA: 0x000AAA3B File Offset: 0x000A8C3B
		public void PlayBuildupSound()
		{
			AudioSourcePoolManager.PlayOnTransform(this._sfxBuildupDiegetic, base.Hub.transform, 10f, 1f, FalloffType.Exponential, MixerChannel.DefaultSfx, 1f);
		}

		// Token: 0x0600337A RID: 13178 RVA: 0x000AAA65 File Offset: 0x000A8C65
		private void CloseSoundSessions()
		{
			if (!this._enableSoundSession.SameSession)
			{
				return;
			}
			this._enableSoundSession.Source.Stop();
		}

		// Token: 0x0600337B RID: 13179 RVA: 0x000AAA88 File Offset: 0x000A8C88
		private void PlaySound()
		{
			AudioSourcePoolManager.PlayOnTransform(this._sfxEnable, base.Hub.transform, 10f, 1f, FalloffType.Exponential, MixerChannel.DefaultSfx, 1f);
			if (!base.IsLocalPlayer && !base.IsSpectated)
			{
				return;
			}
			this._enableSoundSession = new AudioPoolSession(AudioSourcePoolManager.Play2D(this._sfxEnableNonDiegetic, 1f, MixerChannel.NoDucking, 1f));
		}

		// Token: 0x0600337C RID: 13180 RVA: 0x000AAAF0 File Offset: 0x000A8CF0
		private void UpdateVision(bool isVisionActive)
		{
			if (isVisionActive != this._prevVisionActive)
			{
				if (isVisionActive)
				{
					this._xrayProviders.ForEach(delegate(Scp1344XrayProviderBase x)
					{
						x.OnVisionEnabled();
					});
				}
				else
				{
					this._xrayProviders.ForEach(delegate(Scp1344XrayProviderBase x)
					{
						x.OnVisionDisabled();
					});
				}
				this._prevVisionActive = isVisionActive;
			}
			if (isVisionActive)
			{
				this._xrayProviders.ForEach(delegate(Scp1344XrayProviderBase x)
				{
					x.OnUpdate();
				});
			}
		}

		// Token: 0x040027B5 RID: 10165
		public const InvisibilityFlags BypassFlags = (InvisibilityFlags)3U;

		// Token: 0x040027B6 RID: 10166
		[SerializeField]
		private AudioClip _sfxEnable;

		// Token: 0x040027B7 RID: 10167
		[SerializeField]
		private AudioClip _sfxEnableNonDiegetic;

		// Token: 0x040027B8 RID: 10168
		[SerializeField]
		private AudioClip _sfxBuildupDiegetic;

		// Token: 0x040027B9 RID: 10169
		[SerializeField]
		private Scp1344XrayProviderBase[] _xrayProviders;

		// Token: 0x040027BA RID: 10170
		private AudioPoolSession _enableSoundSession;

		// Token: 0x040027BB RID: 10171
		private bool _prevVisionActive;
	}
}
