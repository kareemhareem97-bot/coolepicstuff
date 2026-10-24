using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Mirror;
using NorthwoodLib.Pools;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace AdminToys
{
	// Token: 0x02000AD5 RID: 2773
	public class CapybaraToy : AdminToyBase
	{
		// Token: 0x17000E27 RID: 3623
		// (get) Token: 0x06003E88 RID: 16008 RVA: 0x000CEF2F File Offset: 0x000CD12F
		public override string CommandName
		{
			get
			{
				return "Capybara";
			}
		}

		// Token: 0x06003E89 RID: 16009 RVA: 0x000CEF38 File Offset: 0x000CD138
		public override void OnSpawned(ReferenceHub admin, ArraySegment<string> arguments)
		{
			base.OnSpawned(admin, arguments);
			IFpcRole fpcRole = admin.roleManager.CurrentRole as IFpcRole;
			Vector3 position;
			Quaternion quaternion;
			if (fpcRole != null)
			{
				RaycastHit raycastHit;
				if (Physics.Raycast(fpcRole.FpcModule.Position, Vector3.down, out raycastHit, 2f))
				{
					position = raycastHit.point;
				}
				else
				{
					position = fpcRole.FpcModule.Position;
				}
				quaternion = Quaternion.Euler(0f, fpcRole.FpcModule.MouseLook.CurrentHorizontal, 0f);
			}
			else
			{
				admin.transform.GetPositionAndRotation(out position, out quaternion);
			}
			base.transform.SetPositionAndRotation(position, Quaternion.Euler(0f, quaternion.eulerAngles.y, 0f));
			base.transform.localScale = Vector3.one;
			bool networkCollisionsEnabled;
			if (arguments.Count > 1 && bool.TryParse(arguments.ElementAt(1), out networkCollisionsEnabled))
			{
				this.NetworkCollisionsEnabled = networkCollisionsEnabled;
			}
		}

		// Token: 0x06003E8A RID: 16010 RVA: 0x000CF024 File Offset: 0x000CD224
		private void SetCollisionsEnabled(bool _, bool newValue)
		{
			foreach (Collider collider in this._colliders)
			{
				collider.enabled = newValue;
			}
		}

		// Token: 0x06003E8B RID: 16011 RVA: 0x000CF078 File Offset: 0x000CD278
		private void Awake()
		{
			this._colliders = ListPool<Collider>.Shared.Rent();
			base.GetComponentsInChildren<Collider>(true, this._colliders);
			this.SetCollisionsEnabled(true, this.CollisionsEnabled);
		}

		// Token: 0x06003E8C RID: 16012 RVA: 0x000CF0A4 File Offset: 0x000CD2A4
		protected override void OnDestroy()
		{
			base.OnDestroy();
			ListPool<Collider>.Shared.Return(this._colliders);
		}

		// Token: 0x06003E8E RID: 16014 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Weaved()
		{
			return true;
		}

		// Token: 0x17000E28 RID: 3624
		// (get) Token: 0x06003E8F RID: 16015 RVA: 0x000CF0CC File Offset: 0x000CD2CC
		// (set) Token: 0x06003E90 RID: 16016 RVA: 0x000CF0DF File Offset: 0x000CD2DF
		public bool NetworkCollisionsEnabled
		{
			get
			{
				return this.CollisionsEnabled;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<bool>(value, ref this.CollisionsEnabled, 32UL, new Action<bool, bool>(this.SetCollisionsEnabled));
			}
		}

		// Token: 0x06003E91 RID: 16017 RVA: 0x000CF104 File Offset: 0x000CD304
		public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
		{
			base.SerializeSyncVars(writer, forceAll);
			if (forceAll)
			{
				writer.WriteBool(this.CollisionsEnabled);
				return;
			}
			writer.WriteULong(base.syncVarDirtyBits);
			if ((base.syncVarDirtyBits & 32UL) != 0UL)
			{
				writer.WriteBool(this.CollisionsEnabled);
			}
		}

		// Token: 0x06003E92 RID: 16018 RVA: 0x000CF15C File Offset: 0x000CD35C
		public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
		{
			base.DeserializeSyncVars(reader, initialState);
			if (initialState)
			{
				base.GeneratedSyncVarDeserialize<bool>(ref this.CollisionsEnabled, new Action<bool, bool>(this.SetCollisionsEnabled), reader.ReadBool());
				return;
			}
			long num = (long)reader.ReadULong();
			if ((num & 32L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<bool>(ref this.CollisionsEnabled, new Action<bool, bool>(this.SetCollisionsEnabled), reader.ReadBool());
			}
		}

		// Token: 0x04002DC4 RID: 11716
		[SyncVar(hook = "SetCollisionsEnabled")]
		public bool CollisionsEnabled = true;

		// Token: 0x04002DC5 RID: 11717
		private List<Collider> _colliders;
	}
}
