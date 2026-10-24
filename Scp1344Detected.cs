using System;
using System.Collections.Generic;
using AudioPooling;
using InventorySystem.Items.Usables.Scp1344;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using Mirror;
using RemoteAdmin.Interfaces;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x020008D2 RID: 2258
	public class Scp1344Detected : StatusEffectBase, ICustomDisplayName
	{
		// Token: 0x170009D9 RID: 2521
		// (get) Token: 0x06003383 RID: 13187 RVA: 0x000042EB File Offset: 0x000024EB
		public string DisplayName
		{
			get
			{
				return null;
			}
		}

		// Token: 0x170009DA RID: 2522
		// (get) Token: 0x06003384 RID: 13188 RVA: 0x00004094 File Offset: 0x00002294
		public bool CanBeDisplayed
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06003385 RID: 13189 RVA: 0x000AABB8 File Offset: 0x000A8DB8
		public void ServerRegisterObserver(Scp1344HumanXrayProvider source)
		{
			if (this._seenBy.Contains(source))
			{
				return;
			}
			if (!this.ValidateObserver(source))
			{
				return;
			}
			this._seenBy.Add(source);
			this.UpdateStatus();
			PlayerEvents.OnDetectedByScp1344(new PlayerDetectedByScp1344EventArgs(source.Hub, base.Hub));
		}

		// Token: 0x06003386 RID: 13190 RVA: 0x000AAC06 File Offset: 0x000A8E06
		protected override void Enabled()
		{
			base.Enabled();
			if (!base.IsPOV || this._remainingReEnableCooldown > 0f)
			{
				return;
			}
			AudioSourcePoolManager.Play2D(this._detectedClip, 1f, MixerChannel.NoDucking, 1f);
		}

		// Token: 0x06003387 RID: 13191 RVA: 0x000AAC3B File Offset: 0x000A8E3B
		protected override void Disabled()
		{
			base.Disabled();
			this._seenBy.Clear();
			this._remainingReEnableCooldown = this._reEnableCooldown;
		}

		// Token: 0x06003388 RID: 13192 RVA: 0x000AAC5A File Offset: 0x000A8E5A
		protected override void OnEffectUpdate()
		{
			base.OnEffectUpdate();
			this.UpdateStatus();
		}

		// Token: 0x06003389 RID: 13193 RVA: 0x000AAC68 File Offset: 0x000A8E68
		protected override void Update()
		{
			base.Update();
			if (this._remainingReEnableCooldown > 0f)
			{
				this._remainingReEnableCooldown -= Time.deltaTime;
			}
		}

		// Token: 0x0600338A RID: 13194 RVA: 0x000AAC8F File Offset: 0x000A8E8F
		private bool ValidateObserver(Scp1344HumanXrayProvider source)
		{
			return source != null && HitboxIdentity.IsEnemy(source.Hub, base.Hub) && source.GetVisibilityForTarget(base.Hub);
		}

		// Token: 0x0600338B RID: 13195 RVA: 0x000AACBC File Offset: 0x000A8EBC
		private void UpdateStatus()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			for (int i = this._seenBy.Count - 1; i >= 0; i--)
			{
				if (!this.ValidateObserver(this._seenBy[i]))
				{
					this._seenBy.RemoveAt(i);
				}
			}
			base.IsEnabled = (this._seenBy.Count > 0);
		}

		// Token: 0x040027C0 RID: 10176
		private readonly List<Scp1344HumanXrayProvider> _seenBy = new List<Scp1344HumanXrayProvider>();

		// Token: 0x040027C1 RID: 10177
		private float _remainingReEnableCooldown;

		// Token: 0x040027C2 RID: 10178
		[SerializeField]
		private AudioClip _detectedClip;

		// Token: 0x040027C3 RID: 10179
		[SerializeField]
		private float _reEnableCooldown;
	}
}
