using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mirror;
using PlayerRoles.FirstPersonControl;
using TMPro;
using UnityEngine;

namespace AdminToys
{
	// Token: 0x02000AE4 RID: 2788
	public class TextToy : AdminToyBase
	{
		// Token: 0x17000E55 RID: 3669
		// (get) Token: 0x06003F57 RID: 16215 RVA: 0x000D20D1 File Offset: 0x000D02D1
		public static Vector2 DefaultDisplaySize
		{
			get
			{
				return new Vector2(200f, 50f);
			}
		}

		// Token: 0x17000E56 RID: 3670
		// (get) Token: 0x06003F58 RID: 16216 RVA: 0x000D20E2 File Offset: 0x000D02E2
		public override string CommandName
		{
			get
			{
				return "Text";
			}
		}

		// Token: 0x17000E57 RID: 3671
		// (get) Token: 0x06003F59 RID: 16217 RVA: 0x000D20E9 File Offset: 0x000D02E9
		// (set) Token: 0x06003F5A RID: 16218 RVA: 0x000D20F1 File Offset: 0x000D02F1
		public Vector2 DisplaySize
		{
			get
			{
				return this._displaySize;
			}
			set
			{
				this.Network_displaySize = value;
			}
		}

		// Token: 0x17000E58 RID: 3672
		// (get) Token: 0x06003F5B RID: 16219 RVA: 0x000D20FA File Offset: 0x000D02FA
		// (set) Token: 0x06003F5C RID: 16220 RVA: 0x000D2102 File Offset: 0x000D0302
		public string TextFormat
		{
			get
			{
				return this._textFormat;
			}
			set
			{
				this.Network_textFormat = value;
			}
		}

		// Token: 0x06003F5D RID: 16221 RVA: 0x000D210C File Offset: 0x000D030C
		public override void OnSpawned(ReferenceHub admin, ArraySegment<string> arguments)
		{
			base.OnSpawned(admin, arguments);
			IFpcRole fpcRole = admin.roleManager.CurrentRole as IFpcRole;
			Vector3 position;
			Quaternion rotation;
			if (fpcRole != null)
			{
				position = fpcRole.FpcModule.Position;
				rotation = Quaternion.Euler(0f, fpcRole.FpcModule.MouseLook.CurrentHorizontal, 0f);
			}
			else
			{
				position = admin.transform.position;
				rotation = Quaternion.Euler(0f, admin.transform.rotation.eulerAngles.y, 0f);
			}
			base.transform.SetPositionAndRotation(position, rotation);
			if (arguments.Array.Length > 2)
			{
				this.TextFormat = string.Join(' ', RuntimeHelpers.GetSubArray<string>(arguments.Array, Range.StartAt(2)));
			}
		}

		// Token: 0x06003F5E RID: 16222 RVA: 0x000D21D4 File Offset: 0x000D03D4
		public override void OnStartClient()
		{
			base.OnStartClient();
			this.Arguments.OnModified += this.RefreshText;
		}

		// Token: 0x06003F5F RID: 16223 RVA: 0x000D21F4 File Offset: 0x000D03F4
		private void RefreshText()
		{
			if (this._textMesh == null)
			{
				return;
			}
			if (this.Arguments.Count > 0 && !string.IsNullOrEmpty(this.TextFormat))
			{
				TMP_Text textMesh = this._textMesh;
				string textFormat = this.TextFormat;
				object[] args = this.Arguments.ToArray<string>();
				textMesh.SetText(Misc.FormatAvailable(textFormat, args));
				return;
			}
			this._textMesh.SetText(this.TextFormat);
		}

		// Token: 0x06003F60 RID: 16224 RVA: 0x000D2260 File Offset: 0x000D0460
		private void Awake()
		{
			if (NetworkServer.active)
			{
				return;
			}
			this._textMesh = base.GetComponentInChildren<TMP_Text>();
			this._rectTransform = this._textMesh.gameObject.GetComponent<RectTransform>();
		}

		// Token: 0x06003F61 RID: 16225 RVA: 0x000D228C File Offset: 0x000D048C
		private void OnDisplaySizeChanged(Vector2 oldSize, Vector2 newSize)
		{
			if (this._rectTransform == null)
			{
				return;
			}
			this._rectTransform.sizeDelta = newSize;
		}

		// Token: 0x06003F62 RID: 16226 RVA: 0x000D22A9 File Offset: 0x000D04A9
		private void OnTextFormatedChanged(string oldValue, string newValue)
		{
			this.RefreshText();
		}

		// Token: 0x06003F63 RID: 16227 RVA: 0x000D22B1 File Offset: 0x000D04B1
		public TextToy()
		{
			base.InitSyncObject(this.Arguments);
		}

		// Token: 0x06003F64 RID: 16228 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Weaved()
		{
			return true;
		}

		// Token: 0x17000E59 RID: 3673
		// (get) Token: 0x06003F65 RID: 16229 RVA: 0x000D22DC File Offset: 0x000D04DC
		// (set) Token: 0x06003F66 RID: 16230 RVA: 0x000D22EF File Offset: 0x000D04EF
		public Vector2 Network_displaySize
		{
			get
			{
				return this._displaySize;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<Vector2>(value, ref this._displaySize, 32UL, new Action<Vector2, Vector2>(this.OnDisplaySizeChanged));
			}
		}

		// Token: 0x17000E5A RID: 3674
		// (get) Token: 0x06003F67 RID: 16231 RVA: 0x000D2314 File Offset: 0x000D0514
		// (set) Token: 0x06003F68 RID: 16232 RVA: 0x000D2327 File Offset: 0x000D0527
		public string Network_textFormat
		{
			get
			{
				return this._textFormat;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<string>(value, ref this._textFormat, 64UL, new Action<string, string>(this.OnTextFormatedChanged));
			}
		}

		// Token: 0x06003F69 RID: 16233 RVA: 0x000D234C File Offset: 0x000D054C
		public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
		{
			base.SerializeSyncVars(writer, forceAll);
			if (forceAll)
			{
				writer.WriteVector2(this._displaySize);
				writer.WriteString(this._textFormat);
				return;
			}
			writer.WriteULong(base.syncVarDirtyBits);
			if ((base.syncVarDirtyBits & 32UL) != 0UL)
			{
				writer.WriteVector2(this._displaySize);
			}
			if ((base.syncVarDirtyBits & 64UL) != 0UL)
			{
				writer.WriteString(this._textFormat);
			}
		}

		// Token: 0x06003F6A RID: 16234 RVA: 0x000D23D4 File Offset: 0x000D05D4
		public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
		{
			base.DeserializeSyncVars(reader, initialState);
			if (initialState)
			{
				base.GeneratedSyncVarDeserialize<Vector2>(ref this._displaySize, new Action<Vector2, Vector2>(this.OnDisplaySizeChanged), reader.ReadVector2());
				base.GeneratedSyncVarDeserialize<string>(ref this._textFormat, new Action<string, string>(this.OnTextFormatedChanged), reader.ReadString());
				return;
			}
			long num = (long)reader.ReadULong();
			if ((num & 32L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<Vector2>(ref this._displaySize, new Action<Vector2, Vector2>(this.OnDisplaySizeChanged), reader.ReadVector2());
			}
			if ((num & 64L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<string>(ref this._textFormat, new Action<string, string>(this.OnTextFormatedChanged), reader.ReadString());
			}
		}

		// Token: 0x04002E24 RID: 11812
		public readonly SyncList<string> Arguments = new SyncList<string>();

		// Token: 0x04002E25 RID: 11813
		[SyncVar(hook = "OnDisplaySizeChanged")]
		private Vector2 _displaySize = TextToy.DefaultDisplaySize;

		// Token: 0x04002E26 RID: 11814
		[SyncVar(hook = "OnTextFormatedChanged")]
		private string _textFormat;

		// Token: 0x04002E27 RID: 11815
		private TMP_Text _textMesh;

		// Token: 0x04002E28 RID: 11816
		private RectTransform _rectTransform;
	}
}
