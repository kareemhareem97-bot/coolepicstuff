using System;
using System.Runtime.InteropServices;
using Mirror;
using UnityEngine;
using VoiceChat.Playbacks;

namespace AdminToys
{
	// Token: 0x02000AE3 RID: 2787
	public class SpeakerToy : AdminToyBase
	{
		// Token: 0x17000E4F RID: 3663
		// (get) Token: 0x06003F42 RID: 16194 RVA: 0x000D1BBA File Offset: 0x000CFDBA
		public override string CommandName
		{
			get
			{
				return "Speaker";
			}
		}

		// Token: 0x06003F43 RID: 16195 RVA: 0x000D1BC4 File Offset: 0x000CFDC4
		public override void OnSpawned(ReferenceHub admin, ArraySegment<string> arguments)
		{
			base.OnSpawned(admin, arguments);
			base.transform.position = admin.PlayerCameraReference.position;
			base.transform.localScale = Vector3.one;
			this.Playback.ControllerId = this.ControllerId;
			this.Playback.Source.spatialBlend = (this.IsSpatial ? 1f : 0f);
			this.Playback.VolumeScale = this.Volume;
			this.Playback.Source.minDistance = this.MinDistance;
			this.Playback.Source.maxDistance = this.MaxDistance;
		}

		// Token: 0x06003F44 RID: 16196 RVA: 0x000D1C71 File Offset: 0x000CFE71
		private void OnControllerIdChanged(byte _, byte id)
		{
			this.Playback.ControllerId = id;
		}

		// Token: 0x06003F45 RID: 16197 RVA: 0x000D1C7F File Offset: 0x000CFE7F
		private void OnIsSpatialChanged(bool _, bool isSpatial)
		{
			this.Playback.Source.spatialBlend = (isSpatial ? 1f : 0f);
		}

		// Token: 0x06003F46 RID: 16198 RVA: 0x000D1CA0 File Offset: 0x000CFEA0
		private void OnVolumeChanged(float _, float volume)
		{
			this.Playback.VolumeScale = volume;
		}

		// Token: 0x06003F47 RID: 16199 RVA: 0x000D1CAE File Offset: 0x000CFEAE
		private void OnMinDistanceChanged(float _, float distance)
		{
			this.Playback.Source.minDistance = distance;
		}

		// Token: 0x06003F48 RID: 16200 RVA: 0x000D1CC1 File Offset: 0x000CFEC1
		private void OnMaxDistanceChanged(float _, float distance)
		{
			this.Playback.Source.maxDistance = distance;
		}

		// Token: 0x06003F4A RID: 16202 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Weaved()
		{
			return true;
		}

		// Token: 0x17000E50 RID: 3664
		// (get) Token: 0x06003F4B RID: 16203 RVA: 0x000D1D04 File Offset: 0x000CFF04
		// (set) Token: 0x06003F4C RID: 16204 RVA: 0x000D1D17 File Offset: 0x000CFF17
		public byte NetworkControllerId
		{
			get
			{
				return this.ControllerId;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<byte>(value, ref this.ControllerId, 32UL, new Action<byte, byte>(this.OnControllerIdChanged));
			}
		}

		// Token: 0x17000E51 RID: 3665
		// (get) Token: 0x06003F4D RID: 16205 RVA: 0x000D1D3C File Offset: 0x000CFF3C
		// (set) Token: 0x06003F4E RID: 16206 RVA: 0x000D1D4F File Offset: 0x000CFF4F
		public bool NetworkIsSpatial
		{
			get
			{
				return this.IsSpatial;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<bool>(value, ref this.IsSpatial, 64UL, new Action<bool, bool>(this.OnIsSpatialChanged));
			}
		}

		// Token: 0x17000E52 RID: 3666
		// (get) Token: 0x06003F4F RID: 16207 RVA: 0x000D1D74 File Offset: 0x000CFF74
		// (set) Token: 0x06003F50 RID: 16208 RVA: 0x000D1D87 File Offset: 0x000CFF87
		public float NetworkVolume
		{
			get
			{
				return this.Volume;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<float>(value, ref this.Volume, 128UL, new Action<float, float>(this.OnVolumeChanged));
			}
		}

		// Token: 0x17000E53 RID: 3667
		// (get) Token: 0x06003F51 RID: 16209 RVA: 0x000D1DAC File Offset: 0x000CFFAC
		// (set) Token: 0x06003F52 RID: 16210 RVA: 0x000D1DBF File Offset: 0x000CFFBF
		public float NetworkMinDistance
		{
			get
			{
				return this.MinDistance;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<float>(value, ref this.MinDistance, 256UL, new Action<float, float>(this.OnMinDistanceChanged));
			}
		}

		// Token: 0x17000E54 RID: 3668
		// (get) Token: 0x06003F53 RID: 16211 RVA: 0x000D1DE4 File Offset: 0x000CFFE4
		// (set) Token: 0x06003F54 RID: 16212 RVA: 0x000D1DF7 File Offset: 0x000CFFF7
		public float NetworkMaxDistance
		{
			get
			{
				return this.MaxDistance;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<float>(value, ref this.MaxDistance, 512UL, new Action<float, float>(this.OnMaxDistanceChanged));
			}
		}

		// Token: 0x06003F55 RID: 16213 RVA: 0x000D1E1C File Offset: 0x000D001C
		public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
		{
			base.SerializeSyncVars(writer, forceAll);
			if (forceAll)
			{
				writer.WriteByte(this.ControllerId);
				writer.WriteBool(this.IsSpatial);
				writer.WriteFloat(this.Volume);
				writer.WriteFloat(this.MinDistance);
				writer.WriteFloat(this.MaxDistance);
				return;
			}
			writer.WriteULong(base.syncVarDirtyBits);
			if ((base.syncVarDirtyBits & 32UL) != 0UL)
			{
				writer.WriteByte(this.ControllerId);
			}
			if ((base.syncVarDirtyBits & 64UL) != 0UL)
			{
				writer.WriteBool(this.IsSpatial);
			}
			if ((base.syncVarDirtyBits & 128UL) != 0UL)
			{
				writer.WriteFloat(this.Volume);
			}
			if ((base.syncVarDirtyBits & 256UL) != 0UL)
			{
				writer.WriteFloat(this.MinDistance);
			}
			if ((base.syncVarDirtyBits & 512UL) != 0UL)
			{
				writer.WriteFloat(this.MaxDistance);
			}
		}

		// Token: 0x06003F56 RID: 16214 RVA: 0x000D1F2C File Offset: 0x000D012C
		public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
		{
			base.DeserializeSyncVars(reader, initialState);
			if (initialState)
			{
				base.GeneratedSyncVarDeserialize<byte>(ref this.ControllerId, new Action<byte, byte>(this.OnControllerIdChanged), reader.ReadByte());
				base.GeneratedSyncVarDeserialize<bool>(ref this.IsSpatial, new Action<bool, bool>(this.OnIsSpatialChanged), reader.ReadBool());
				base.GeneratedSyncVarDeserialize<float>(ref this.Volume, new Action<float, float>(this.OnVolumeChanged), reader.ReadFloat());
				base.GeneratedSyncVarDeserialize<float>(ref this.MinDistance, new Action<float, float>(this.OnMinDistanceChanged), reader.ReadFloat());
				base.GeneratedSyncVarDeserialize<float>(ref this.MaxDistance, new Action<float, float>(this.OnMaxDistanceChanged), reader.ReadFloat());
				return;
			}
			long num = (long)reader.ReadULong();
			if ((num & 32L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<byte>(ref this.ControllerId, new Action<byte, byte>(this.OnControllerIdChanged), reader.ReadByte());
			}
			if ((num & 64L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<bool>(ref this.IsSpatial, new Action<bool, bool>(this.OnIsSpatialChanged), reader.ReadBool());
			}
			if ((num & 128L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<float>(ref this.Volume, new Action<float, float>(this.OnVolumeChanged), reader.ReadFloat());
			}
			if ((num & 256L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<float>(ref this.MinDistance, new Action<float, float>(this.OnMinDistanceChanged), reader.ReadFloat());
			}
			if ((num & 512L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<float>(ref this.MaxDistance, new Action<float, float>(this.OnMaxDistanceChanged), reader.ReadFloat());
			}
		}

		// Token: 0x04002E1E RID: 11806
		public SpeakerToyPlaybackBase Playback;

		// Token: 0x04002E1F RID: 11807
		[SyncVar(hook = "OnControllerIdChanged")]
		public byte ControllerId;

		// Token: 0x04002E20 RID: 11808
		[SyncVar(hook = "OnIsSpatialChanged")]
		public bool IsSpatial = true;

		// Token: 0x04002E21 RID: 11809
		[SyncVar(hook = "OnVolumeChanged")]
		public float Volume = 1f;

		// Token: 0x04002E22 RID: 11810
		[SyncVar(hook = "OnMinDistanceChanged")]
		public float MinDistance = 1f;

		// Token: 0x04002E23 RID: 11811
		[SyncVar(hook = "OnMaxDistanceChanged")]
		public float MaxDistance = 15f;
	}
}
