using System;
using InventorySystem.Items.Firearms.Attachments;
using Mirror;
using UnityEngine;

namespace CustomPlayerEffects
{
	// Token: 0x02000898 RID: 2200
	public class AmnesiaItems : StatusEffectBase, IUsableItemModifierEffect, IWeaponModifierPlayerEffect, IPulseEffect
	{
		// Token: 0x17000944 RID: 2372
		// (get) Token: 0x060031FE RID: 12798 RVA: 0x000A8028 File Offset: 0x000A6228
		public bool ParamsActive
		{
			get
			{
				return base.IsEnabled && this._activeTime >= this._blockDelay;
			}
		}

		// Token: 0x060031FF RID: 12799 RVA: 0x000A8045 File Offset: 0x000A6245
		protected override void Update()
		{
			base.Update();
			if (base.IsEnabled)
			{
				this._activeTime += Time.deltaTime;
			}
		}

		// Token: 0x06003200 RID: 12800 RVA: 0x000A8067 File Offset: 0x000A6267
		protected override void Enabled()
		{
			base.Enabled();
			this._activeTime = 0f;
		}

		// Token: 0x06003201 RID: 12801 RVA: 0x000A807A File Offset: 0x000A627A
		public bool TryGetSpeed(ItemType item, out float speed)
		{
			speed = 0f;
			if (!NetworkServer.active || !this._blockedUsableItems.Contains(item) || this._activeTime < this._blockDelay)
			{
				return false;
			}
			this.ServerSendPulse();
			return true;
		}

		// Token: 0x06003202 RID: 12802 RVA: 0x000A80AF File Offset: 0x000A62AF
		public bool TryGetWeaponParam(AttachmentParam param, out float val)
		{
			val = 1f;
			if (!NetworkServer.active || param != AttachmentParam.PreventReload || this._activeTime < this._blockDelay)
			{
				return false;
			}
			this.ServerSendPulse();
			return true;
		}

		// Token: 0x06003203 RID: 12803 RVA: 0x00002E9A File Offset: 0x0000109A
		public void ExecutePulse()
		{
		}

		// Token: 0x06003204 RID: 12804 RVA: 0x000A80DB File Offset: 0x000A62DB
		private void ServerSendPulse()
		{
			base.Hub.playerEffectsController.ServerSendPulse<AmnesiaItems>();
		}

		// Token: 0x04002718 RID: 10008
		private float _activeTime;

		// Token: 0x04002719 RID: 10009
		[SerializeField]
		private ItemType[] _blockedUsableItems;

		// Token: 0x0400271A RID: 10010
		[SerializeField]
		private float _blockDelay;
	}
}
