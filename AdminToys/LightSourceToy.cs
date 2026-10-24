using System;
using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;
using UserSettings;
using UserSettings.VideoSettings;

namespace AdminToys
{
	// Token: 0x02000ADA RID: 2778
	public class LightSourceToy : AdminToyBase
	{
		// Token: 0x17000E30 RID: 3632
		// (get) Token: 0x06003EBC RID: 16060 RVA: 0x000CF9AA File Offset: 0x000CDBAA
		public override string CommandName
		{
			get
			{
				return "LightSource";
			}
		}

		// Token: 0x06003EBD RID: 16061 RVA: 0x000CF9B1 File Offset: 0x000CDBB1
		public override void OnSpawned(ReferenceHub admin, ArraySegment<string> arguments)
		{
			base.OnSpawned(admin, arguments);
			base.transform.position = admin.PlayerCameraReference.position;
			base.transform.localScale = Vector3.one;
		}

		// Token: 0x06003EBE RID: 16062 RVA: 0x000CF9E1 File Offset: 0x000CDBE1
		protected override void Start()
		{
			base.Start();
			if (!UserSetting<bool>.Get<LightingVideoSetting>(LightingVideoSetting.RenderShadows))
			{
				this._light.shadows = LightShadows.None;
			}
		}

		// Token: 0x06003EBF RID: 16063 RVA: 0x000CF9FD File Offset: 0x000CDBFD
		private void SetIntensity(float oldIntensity, float newIntensity)
		{
			this._light.intensity = newIntensity;
		}

		// Token: 0x06003EC0 RID: 16064 RVA: 0x000CFA0B File Offset: 0x000CDC0B
		private void SetRange(float oldRange, float newRange)
		{
			this._light.range = newRange;
		}

		// Token: 0x06003EC1 RID: 16065 RVA: 0x000CFA19 File Offset: 0x000CDC19
		private void SetColor(Color oldColor, Color newColor)
		{
			this._light.color = newColor;
		}

		// Token: 0x06003EC2 RID: 16066 RVA: 0x000CFA27 File Offset: 0x000CDC27
		private void SetShadows(LightShadows oldShadows, LightShadows newShadows)
		{
			if (!UserSetting<bool>.Get<LightingVideoSetting>(LightingVideoSetting.RenderShadows))
			{
				this._light.shadows = LightShadows.None;
				return;
			}
			this._light.shadows = ((this.LightType == LightType.Directional) ? LightShadows.None : newShadows);
		}

		// Token: 0x06003EC3 RID: 16067 RVA: 0x000CFA56 File Offset: 0x000CDC56
		private void SetShadowStrength(float oldStrength, float newStrength)
		{
			this._light.shadowStrength = newStrength;
		}

		// Token: 0x06003EC4 RID: 16068 RVA: 0x000CFA64 File Offset: 0x000CDC64
		private void SetType(LightType oldType, LightType newType)
		{
			if (newType == LightType.Directional)
			{
				this.NetworkShadowType = LightShadows.None;
			}
			if (newType > LightType.Point)
			{
				this.NetworkLightType = LightType.Point;
				return;
			}
			this._light.type = newType;
		}

		// Token: 0x06003EC5 RID: 16069 RVA: 0x000CFA89 File Offset: 0x000CDC89
		private void SetShape(LightShape oldShape, LightShape newShape)
		{
			this._light.shape = newShape;
		}

		// Token: 0x06003EC6 RID: 16070 RVA: 0x000CFA97 File Offset: 0x000CDC97
		private void SetSpotAngle(float oldAngle, float newAngle)
		{
			this._light.spotAngle = newAngle;
		}

		// Token: 0x06003EC7 RID: 16071 RVA: 0x000CFAA5 File Offset: 0x000CDCA5
		private void SetInnerSpotAngle(float oldAngle, float newAngle)
		{
			this._light.innerSpotAngle = newAngle;
		}

		// Token: 0x06003EC9 RID: 16073 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Weaved()
		{
			return true;
		}

		// Token: 0x17000E31 RID: 3633
		// (get) Token: 0x06003ECA RID: 16074 RVA: 0x000CFAB4 File Offset: 0x000CDCB4
		// (set) Token: 0x06003ECB RID: 16075 RVA: 0x000CFAC7 File Offset: 0x000CDCC7
		public float NetworkLightIntensity
		{
			get
			{
				return this.LightIntensity;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<float>(value, ref this.LightIntensity, 32UL, new Action<float, float>(this.SetIntensity));
			}
		}

		// Token: 0x17000E32 RID: 3634
		// (get) Token: 0x06003ECC RID: 16076 RVA: 0x000CFAEC File Offset: 0x000CDCEC
		// (set) Token: 0x06003ECD RID: 16077 RVA: 0x000CFAFF File Offset: 0x000CDCFF
		public float NetworkLightRange
		{
			get
			{
				return this.LightRange;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<float>(value, ref this.LightRange, 64UL, new Action<float, float>(this.SetRange));
			}
		}

		// Token: 0x17000E33 RID: 3635
		// (get) Token: 0x06003ECE RID: 16078 RVA: 0x000CFB24 File Offset: 0x000CDD24
		// (set) Token: 0x06003ECF RID: 16079 RVA: 0x000CFB37 File Offset: 0x000CDD37
		public Color NetworkLightColor
		{
			get
			{
				return this.LightColor;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<Color>(value, ref this.LightColor, 128UL, new Action<Color, Color>(this.SetColor));
			}
		}

		// Token: 0x17000E34 RID: 3636
		// (get) Token: 0x06003ED0 RID: 16080 RVA: 0x000CFB5C File Offset: 0x000CDD5C
		// (set) Token: 0x06003ED1 RID: 16081 RVA: 0x000CFB6F File Offset: 0x000CDD6F
		public LightShadows NetworkShadowType
		{
			get
			{
				return this.ShadowType;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<LightShadows>(value, ref this.ShadowType, 256UL, new Action<LightShadows, LightShadows>(this.SetShadows));
			}
		}

		// Token: 0x17000E35 RID: 3637
		// (get) Token: 0x06003ED2 RID: 16082 RVA: 0x000CFB94 File Offset: 0x000CDD94
		// (set) Token: 0x06003ED3 RID: 16083 RVA: 0x000CFBA7 File Offset: 0x000CDDA7
		public float NetworkShadowStrength
		{
			get
			{
				return this.ShadowStrength;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<float>(value, ref this.ShadowStrength, 512UL, new Action<float, float>(this.SetShadowStrength));
			}
		}

		// Token: 0x17000E36 RID: 3638
		// (get) Token: 0x06003ED4 RID: 16084 RVA: 0x000CFBCC File Offset: 0x000CDDCC
		// (set) Token: 0x06003ED5 RID: 16085 RVA: 0x000CFBDF File Offset: 0x000CDDDF
		public LightType NetworkLightType
		{
			get
			{
				return this.LightType;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<LightType>(value, ref this.LightType, 1024UL, new Action<LightType, LightType>(this.SetType));
			}
		}

		// Token: 0x17000E37 RID: 3639
		// (get) Token: 0x06003ED6 RID: 16086 RVA: 0x000CFC04 File Offset: 0x000CDE04
		// (set) Token: 0x06003ED7 RID: 16087 RVA: 0x000CFC17 File Offset: 0x000CDE17
		public LightShape NetworkLightShape
		{
			get
			{
				return this.LightShape;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<LightShape>(value, ref this.LightShape, 2048UL, new Action<LightShape, LightShape>(this.SetShape));
			}
		}

		// Token: 0x17000E38 RID: 3640
		// (get) Token: 0x06003ED8 RID: 16088 RVA: 0x000CFC3C File Offset: 0x000CDE3C
		// (set) Token: 0x06003ED9 RID: 16089 RVA: 0x000CFC4F File Offset: 0x000CDE4F
		public float NetworkSpotAngle
		{
			get
			{
				return this.SpotAngle;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<float>(value, ref this.SpotAngle, 4096UL, new Action<float, float>(this.SetSpotAngle));
			}
		}

		// Token: 0x17000E39 RID: 3641
		// (get) Token: 0x06003EDA RID: 16090 RVA: 0x000CFC74 File Offset: 0x000CDE74
		// (set) Token: 0x06003EDB RID: 16091 RVA: 0x000CFC87 File Offset: 0x000CDE87
		public float NetworkInnerSpotAngle
		{
			get
			{
				return this.InnerSpotAngle;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<float>(value, ref this.InnerSpotAngle, 8192UL, new Action<float, float>(this.SetInnerSpotAngle));
			}
		}

		// Token: 0x06003EDC RID: 16092 RVA: 0x000CFCAC File Offset: 0x000CDEAC
		public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
		{
			base.SerializeSyncVars(writer, forceAll);
			if (forceAll)
			{
				writer.WriteFloat(this.LightIntensity);
				writer.WriteFloat(this.LightRange);
				writer.WriteColor(this.LightColor);
				Mirror.GeneratedNetworkCode._Write_UnityEngine.LightShadows(writer, this.ShadowType);
				writer.WriteFloat(this.ShadowStrength);
				Mirror.GeneratedNetworkCode._Write_UnityEngine.LightType(writer, this.LightType);
				Mirror.GeneratedNetworkCode._Write_UnityEngine.LightShape(writer, this.LightShape);
				writer.WriteFloat(this.SpotAngle);
				writer.WriteFloat(this.InnerSpotAngle);
				return;
			}
			writer.WriteULong(base.syncVarDirtyBits);
			if ((base.syncVarDirtyBits & 32UL) != 0UL)
			{
				writer.WriteFloat(this.LightIntensity);
			}
			if ((base.syncVarDirtyBits & 64UL) != 0UL)
			{
				writer.WriteFloat(this.LightRange);
			}
			if ((base.syncVarDirtyBits & 128UL) != 0UL)
			{
				writer.WriteColor(this.LightColor);
			}
			if ((base.syncVarDirtyBits & 256UL) != 0UL)
			{
				Mirror.GeneratedNetworkCode._Write_UnityEngine.LightShadows(writer, this.ShadowType);
			}
			if ((base.syncVarDirtyBits & 512UL) != 0UL)
			{
				writer.WriteFloat(this.ShadowStrength);
			}
			if ((base.syncVarDirtyBits & 1024UL) != 0UL)
			{
				Mirror.GeneratedNetworkCode._Write_UnityEngine.LightType(writer, this.LightType);
			}
			if ((base.syncVarDirtyBits & 2048UL) != 0UL)
			{
				Mirror.GeneratedNetworkCode._Write_UnityEngine.LightShape(writer, this.LightShape);
			}
			if ((base.syncVarDirtyBits & 4096UL) != 0UL)
			{
				writer.WriteFloat(this.SpotAngle);
			}
			if ((base.syncVarDirtyBits & 8192UL) != 0UL)
			{
				writer.WriteFloat(this.InnerSpotAngle);
			}
		}

		// Token: 0x06003EDD RID: 16093 RVA: 0x000CFE74 File Offset: 0x000CE074
		public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
		{
			base.DeserializeSyncVars(reader, initialState);
			if (initialState)
			{
				base.GeneratedSyncVarDeserialize<float>(ref this.LightIntensity, new Action<float, float>(this.SetIntensity), reader.ReadFloat());
				base.GeneratedSyncVarDeserialize<float>(ref this.LightRange, new Action<float, float>(this.SetRange), reader.ReadFloat());
				base.GeneratedSyncVarDeserialize<Color>(ref this.LightColor, new Action<Color, Color>(this.SetColor), reader.ReadColor());
				base.GeneratedSyncVarDeserialize<LightShadows>(ref this.ShadowType, new Action<LightShadows, LightShadows>(this.SetShadows), Mirror.GeneratedNetworkCode._Read_UnityEngine.LightShadows(reader));
				base.GeneratedSyncVarDeserialize<float>(ref this.ShadowStrength, new Action<float, float>(this.SetShadowStrength), reader.ReadFloat());
				base.GeneratedSyncVarDeserialize<LightType>(ref this.LightType, new Action<LightType, LightType>(this.SetType), Mirror.GeneratedNetworkCode._Read_UnityEngine.LightType(reader));
				base.GeneratedSyncVarDeserialize<LightShape>(ref this.LightShape, new Action<LightShape, LightShape>(this.SetShape), Mirror.GeneratedNetworkCode._Read_UnityEngine.LightShape(reader));
				base.GeneratedSyncVarDeserialize<float>(ref this.SpotAngle, new Action<float, float>(this.SetSpotAngle), reader.ReadFloat());
				base.GeneratedSyncVarDeserialize<float>(ref this.InnerSpotAngle, new Action<float, float>(this.SetInnerSpotAngle), reader.ReadFloat());
				return;
			}
			long num = (long)reader.ReadULong();
			if ((num & 32L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<float>(ref this.LightIntensity, new Action<float, float>(this.SetIntensity), reader.ReadFloat());
			}
			if ((num & 64L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<float>(ref this.LightRange, new Action<float, float>(this.SetRange), reader.ReadFloat());
			}
			if ((num & 128L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<Color>(ref this.LightColor, new Action<Color, Color>(this.SetColor), reader.ReadColor());
			}
			if ((num & 256L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<LightShadows>(ref this.ShadowType, new Action<LightShadows, LightShadows>(this.SetShadows), Mirror.GeneratedNetworkCode._Read_UnityEngine.LightShadows(reader));
			}
			if ((num & 512L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<float>(ref this.ShadowStrength, new Action<float, float>(this.SetShadowStrength), reader.ReadFloat());
			}
			if ((num & 1024L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<LightType>(ref this.LightType, new Action<LightType, LightType>(this.SetType), Mirror.GeneratedNetworkCode._Read_UnityEngine.LightType(reader));
			}
			if ((num & 2048L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<LightShape>(ref this.LightShape, new Action<LightShape, LightShape>(this.SetShape), Mirror.GeneratedNetworkCode._Read_UnityEngine.LightShape(reader));
			}
			if ((num & 4096L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<float>(ref this.SpotAngle, new Action<float, float>(this.SetSpotAngle), reader.ReadFloat());
			}
			if ((num & 8192L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<float>(ref this.InnerSpotAngle, new Action<float, float>(this.SetInnerSpotAngle), reader.ReadFloat());
			}
		}

		// Token: 0x04002DD6 RID: 11734
		[SerializeField]
		private Light _light;

		// Token: 0x04002DD7 RID: 11735
		[SyncVar(hook = "SetIntensity")]
		public float LightIntensity;

		// Token: 0x04002DD8 RID: 11736
		[SyncVar(hook = "SetRange")]
		public float LightRange;

		// Token: 0x04002DD9 RID: 11737
		[SyncVar(hook = "SetColor")]
		public Color LightColor;

		// Token: 0x04002DDA RID: 11738
		[SyncVar(hook = "SetShadows")]
		public LightShadows ShadowType;

		// Token: 0x04002DDB RID: 11739
		[SyncVar(hook = "SetShadowStrength")]
		public float ShadowStrength;

		// Token: 0x04002DDC RID: 11740
		[SyncVar(hook = "SetType")]
		public LightType LightType;

		// Token: 0x04002DDD RID: 11741
		[SyncVar(hook = "SetShape")]
		public LightShape LightShape;

		// Token: 0x04002DDE RID: 11742
		[SyncVar(hook = "SetSpotAngle")]
		public float SpotAngle;

		// Token: 0x04002DDF RID: 11743
		[SyncVar(hook = "SetInnerSpotAngle")]
		public float InnerSpotAngle;
	}
}
