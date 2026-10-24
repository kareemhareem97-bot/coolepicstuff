using System;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008CF RID: 2255
	public class Scanned : StatusEffectBase, ISoundtrackMutingEffect
	{
		// Token: 0x170009D3 RID: 2515
		// (get) Token: 0x06003368 RID: 13160 RVA: 0x00004070 File Offset: 0x00002270
		public bool MuteSoundtrack
		{
			get
			{
				return base.IsEnabled;
			}
		}

		// Token: 0x170009D4 RID: 2516
		// (get) Token: 0x06003369 RID: 13161 RVA: 0x000A84BF File Offset: 0x000A66BF
		public override bool AllowEnabling
		{
			get
			{
				return !SpawnProtected.CheckPlayer(base.Hub);
			}
		}

		// Token: 0x170009D5 RID: 2517
		// (get) Token: 0x0600336A RID: 13162 RVA: 0x00004094 File Offset: 0x00002294
		public override StatusEffectBase.EffectClassification Classification
		{
			get
			{
				return StatusEffectBase.EffectClassification.Technical;
			}
		}

		// Token: 0x0600336B RID: 13163 RVA: 0x000AA935 File Offset: 0x000A8B35
		protected override void Enabled()
		{
			base.Enabled();
			this.UpdateSourceMute();
			this._soundSource.Play();
		}

		// Token: 0x0600336C RID: 13164 RVA: 0x000AA94E File Offset: 0x000A8B4E
		protected override void Disabled()
		{
			base.Disabled();
			this._soundSource.mute = true;
		}

		// Token: 0x0600336D RID: 13165 RVA: 0x000AA962 File Offset: 0x000A8B62
		protected override void OnEffectUpdate()
		{
			base.OnEffectUpdate();
			this.UpdateSourceMute();
		}

		// Token: 0x0600336E RID: 13166 RVA: 0x000AA970 File Offset: 0x000A8B70
		private void UpdateSourceMute()
		{
			this._soundSource.mute = (!base.IsLocalPlayer && !base.IsSpectated);
		}

		// Token: 0x040027B4 RID: 10164
		[SerializeField]
		private AudioSource _soundSource;
	}
}
