using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AudioPooling;
using GameCore;
using Interactables;
using Interactables.Verification;
using MapGeneration;
using Mirror;
using PlayerRoles;
using UnityEngine;

// Token: 0x02000025 RID: 37
public class Scp559Cake : NetworkBehaviour, IServerInteractable, IInteractable
{
	// Token: 0x1700001C RID: 28
	// (get) Token: 0x06000093 RID: 147 RVA: 0x00003897 File Offset: 0x00001A97
	public IVerificationRule VerificationRule
	{
		get
		{
			return StandardDistanceVerification.Default;
		}
	}

	// Token: 0x1700001D RID: 29
	// (get) Token: 0x06000094 RID: 148 RVA: 0x0000389E File Offset: 0x00001A9E
	public bool CanBeUsed
	{
		get
		{
			return this._remainingSlices > 0;
		}
	}

	// Token: 0x06000095 RID: 149 RVA: 0x000038AC File Offset: 0x00001AAC
	public void ServerInteract(ReferenceHub ply, byte colliderId)
	{
		if (!this._isSpawned || !ply.IsHuman() || this._remainingSlices == 0)
		{
			return;
		}
		Scp559Effect effect = ply.playerEffectsController.GetEffect<Scp559Effect>();
		if (effect.IsEnabled)
		{
			return;
		}
		this.Network_remainingSlices = this._remainingSlices - 1;
		effect.IsEnabled = true;
	}

	// Token: 0x06000096 RID: 150 RVA: 0x000038FD File Offset: 0x00001AFD
	private void Update()
	{
		if (NetworkServer.active)
		{
			this.UpdateServer();
		}
		this.UpdateVisual();
	}

	// Token: 0x06000097 RID: 151 RVA: 0x00003912 File Offset: 0x00001B12
	private void Start()
	{
		this.SetPosition(this._position, this._position);
	}

	// Token: 0x06000098 RID: 152 RVA: 0x00003926 File Offset: 0x00001B26
	private void SetPosition(Vector3 prev, Vector3 cur)
	{
		base.transform.position = cur;
	}

	// Token: 0x06000099 RID: 153 RVA: 0x00003934 File Offset: 0x00001B34
	private void UpdateServer()
	{
		this._remainingTime -= Time.deltaTime;
		if (this._remainingTime > 0f)
		{
			return;
		}
		if (this._isSpawned)
		{
			this.Network_isSpawned = false;
			this._remainingTime = this._respawnTime;
			return;
		}
		if (!RoundStart.RoundStarted)
		{
			return;
		}
		if (RoundStart.RoundLength.TotalSeconds < (double)this._respawnTime)
		{
			return;
		}
		Vector3 network_position;
		bool network_hasPedestal;
		if (!this.TryGetSpawnPoint(out network_position, out network_hasPedestal))
		{
			return;
		}
		this.Network_position = network_position;
		this.Network_isSpawned = true;
		this.Network_hasPedestal = network_hasPedestal;
		this.Network_remainingSlices = this._maxSlices;
		this._remainingTime = this._spawnedDuration;
	}

	// Token: 0x0600009A RID: 154 RVA: 0x000039D8 File Offset: 0x00001BD8
	private void UpdateVisual()
	{
		this._pedestal.SetActive(this._hasPedestal);
		float num = Mathf.Clamp01(this._fade + this._dissolveSpeed * Time.deltaTime * (float)(this._isSpawned ? 1 : -1));
		if (num != this._fade)
		{
			if (this._fade <= 0f)
			{
				AudioSourcePoolManager.PlayOnTransform(this._appearSound, base.transform, 25f, 1f, FalloffType.Exponential, MixerChannel.DefaultSfx, 1f);
			}
			if (this._fade >= 1f)
			{
				AudioSourcePoolManager.PlayOnTransform(this._disappearClip, base.transform, 15f, 1f, FalloffType.Exponential, MixerChannel.DefaultSfx, 1f);
			}
			this._fade = num;
			this._dissolveMat.SetFloat(Scp559Cake.ShaderDissolveProperty, 1f - this._fade);
			this._visuals.SetActive(this._fade > 0f);
			this._prevSlices = -1;
			this._slices.ForEach(delegate(GameObject x)
			{
				x.SetActive(false);
			});
			return;
		}
		if (this._prevSlices != (int)this._remainingSlices)
		{
			for (int i = 0; i < this._slices.Length; i++)
			{
				this._slices[i].SetActive(i < (int)this._remainingSlices);
			}
			if (this._prevSlices > (int)this._remainingSlices)
			{
				AudioSourcePoolManager.PlayOnTransform(this._useSound, base.transform, 9f, 1f, FalloffType.Exponential, MixerChannel.DefaultSfx, 1f);
			}
			this._prevSlices = (int)this._remainingSlices;
		}
	}

	// Token: 0x0600009B RID: 155 RVA: 0x00003B6C File Offset: 0x00001D6C
	private bool TryGetSpawnPoint(out Vector3 pos, out bool pedestal)
	{
		Scp559Cake.PossibleSpawnpoints.Clear();
		Scp559Cake.PopulatedRooms.Clear();
		int num = 0;
		Dictionary<RoomIdentifier, Scp559Spawnpoint> instancesPerRoom = Scp559Spawnpoint.InstancesPerRoom;
		foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
		{
			RoomIdentifier key;
			if (referenceHub.roleManager.CurrentRole is HumanRole && referenceHub.TryGetCurrentRoom(out key) && instancesPerRoom.ContainsKey(key))
			{
				int num2;
				Scp559Cake.PopulatedRooms.TryGetValue(key, out num2);
				if (!referenceHub.playerEffectsController.GetEffect<Scp559Effect>().IsEnabled)
				{
					num2++;
				}
				Scp559Cake.PopulatedRooms[key] = num2;
				num = Mathf.Max(num2, num);
			}
		}
		foreach (KeyValuePair<RoomIdentifier, int> keyValuePair in Scp559Cake.PopulatedRooms)
		{
			if (keyValuePair.Value >= num)
			{
				Scp559Spawnpoint scp559Spawnpoint = instancesPerRoom[keyValuePair.Key];
				if (!Physics.CheckSphere(scp559Spawnpoint.transform.position, this._cakeRadius, Scp559Cake.CheckerMask))
				{
					Scp559Cake.PossibleSpawnpoints.Add(scp559Spawnpoint);
				}
			}
		}
		if (Scp559Cake.PossibleSpawnpoints.Count == 0)
		{
			pos = Vector3.zero;
			pedestal = false;
			return false;
		}
		Scp559Spawnpoint scp559Spawnpoint2 = Scp559Cake.PossibleSpawnpoints.RandomItem<Scp559Spawnpoint>();
		pos = scp559Spawnpoint2.transform.position;
		if (scp559Spawnpoint2.HidePedestal)
		{
			pedestal = false;
			pos += Vector3.down * 1.042f;
		}
		else
		{
			pedestal = true;
		}
		return true;
	}

	// Token: 0x0600009E RID: 158 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x1700001E RID: 30
	// (get) Token: 0x0600009F RID: 159 RVA: 0x00003D88 File Offset: 0x00001F88
	// (set) Token: 0x060000A0 RID: 160 RVA: 0x00003D9B File Offset: 0x00001F9B
	public byte Network_remainingSlices
	{
		get
		{
			return this._remainingSlices;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<byte>(value, ref this._remainingSlices, 1UL, null);
		}
	}

	// Token: 0x1700001F RID: 31
	// (get) Token: 0x060000A1 RID: 161 RVA: 0x00003DB8 File Offset: 0x00001FB8
	// (set) Token: 0x060000A2 RID: 162 RVA: 0x00003DCB File Offset: 0x00001FCB
	public bool Network_isSpawned
	{
		get
		{
			return this._isSpawned;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<bool>(value, ref this._isSpawned, 2UL, null);
		}
	}

	// Token: 0x17000020 RID: 32
	// (get) Token: 0x060000A3 RID: 163 RVA: 0x00003DE8 File Offset: 0x00001FE8
	// (set) Token: 0x060000A4 RID: 164 RVA: 0x00003DFB File Offset: 0x00001FFB
	public bool Network_hasPedestal
	{
		get
		{
			return this._hasPedestal;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<bool>(value, ref this._hasPedestal, 4UL, null);
		}
	}

	// Token: 0x17000021 RID: 33
	// (get) Token: 0x060000A5 RID: 165 RVA: 0x00003E18 File Offset: 0x00002018
	// (set) Token: 0x060000A6 RID: 166 RVA: 0x00003E2B File Offset: 0x0000202B
	public Vector3 Network_position
	{
		get
		{
			return this._position;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<Vector3>(value, ref this._position, 8UL, new Action<Vector3, Vector3>(this.SetPosition));
		}
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x00003E50 File Offset: 0x00002050
	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteByte(this._remainingSlices);
			writer.WriteBool(this._isSpawned);
			writer.WriteBool(this._hasPedestal);
			writer.WriteVector3(this._position);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1UL) != 0UL)
		{
			writer.WriteByte(this._remainingSlices);
		}
		if ((base.syncVarDirtyBits & 2UL) != 0UL)
		{
			writer.WriteBool(this._isSpawned);
		}
		if ((base.syncVarDirtyBits & 4UL) != 0UL)
		{
			writer.WriteBool(this._hasPedestal);
		}
		if ((base.syncVarDirtyBits & 8UL) != 0UL)
		{
			writer.WriteVector3(this._position);
		}
	}

	// Token: 0x060000A8 RID: 168 RVA: 0x00003F34 File Offset: 0x00002134
	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			base.GeneratedSyncVarDeserialize<byte>(ref this._remainingSlices, null, reader.ReadByte());
			base.GeneratedSyncVarDeserialize<bool>(ref this._isSpawned, null, reader.ReadBool());
			base.GeneratedSyncVarDeserialize<bool>(ref this._hasPedestal, null, reader.ReadBool());
			base.GeneratedSyncVarDeserialize<Vector3>(ref this._position, new Action<Vector3, Vector3>(this.SetPosition), reader.ReadVector3());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<byte>(ref this._remainingSlices, null, reader.ReadByte());
		}
		if ((num & 2L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<bool>(ref this._isSpawned, null, reader.ReadBool());
		}
		if ((num & 4L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<bool>(ref this._hasPedestal, null, reader.ReadBool());
		}
		if ((num & 8L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<Vector3>(ref this._position, new Action<Vector3, Vector3>(this.SetPosition), reader.ReadVector3());
		}
	}

	// Token: 0x04000092 RID: 146
	private const float SpawnChance = 33f;

	// Token: 0x04000093 RID: 147
	public static readonly int ShaderDissolveProperty = Shader.PropertyToID("_Dissolve");

	// Token: 0x04000094 RID: 148
	private static readonly CachedLayerMask CheckerMask = new CachedLayerMask(new string[]
	{
		"Pickup",
		"Hitbox"
	});

	// Token: 0x04000095 RID: 149
	private static readonly List<Scp559Spawnpoint> PossibleSpawnpoints = new List<Scp559Spawnpoint>();

	// Token: 0x04000096 RID: 150
	private static readonly Dictionary<RoomIdentifier, int> PopulatedRooms = new Dictionary<RoomIdentifier, int>();

	// Token: 0x04000097 RID: 151
	[SerializeField]
	private GameObject _visuals;

	// Token: 0x04000098 RID: 152
	[SerializeField]
	private GameObject _pedestal;

	// Token: 0x04000099 RID: 153
	[SerializeField]
	private Material _dissolveMat;

	// Token: 0x0400009A RID: 154
	[SerializeField]
	private GameObject[] _slices;

	// Token: 0x0400009B RID: 155
	[SerializeField]
	private float _dissolveSpeed;

	// Token: 0x0400009C RID: 156
	[SerializeField]
	private AudioClip _useSound;

	// Token: 0x0400009D RID: 157
	[SerializeField]
	private AudioClip _appearSound;

	// Token: 0x0400009E RID: 158
	[SerializeField]
	private AudioClip _disappearClip;

	// Token: 0x0400009F RID: 159
	[SerializeField]
	private byte _maxSlices;

	// Token: 0x040000A0 RID: 160
	[SyncVar]
	private byte _remainingSlices;

	// Token: 0x040000A1 RID: 161
	[SyncVar]
	private bool _isSpawned;

	// Token: 0x040000A2 RID: 162
	[SyncVar]
	private bool _hasPedestal;

	// Token: 0x040000A3 RID: 163
	[SerializeField]
	private float _spawnedDuration;

	// Token: 0x040000A4 RID: 164
	[SerializeField]
	private float _respawnTime;

	// Token: 0x040000A5 RID: 165
	[SerializeField]
	private float _cakeRadius;

	// Token: 0x040000A6 RID: 166
	[SyncVar(hook = "SetPosition")]
	private Vector3 _position;

	// Token: 0x040000A7 RID: 167
	private float _fade;

	// Token: 0x040000A8 RID: 168
	private float _remainingTime;

	// Token: 0x040000A9 RID: 169
	private int _prevSlices = -1;
}
