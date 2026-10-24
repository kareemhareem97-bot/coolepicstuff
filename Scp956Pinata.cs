using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items;
using MapGeneration;
using Mirror;
using Mirror.RemoteCalls;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using RelativePositioning;
using UnityEngine;
using Utils.NonAllocLINQ;

// Token: 0x0200002B RID: 43
public class Scp956Pinata : NetworkBehaviour
{
	// Token: 0x060000D1 RID: 209 RVA: 0x00004412 File Offset: 0x00002612
	public static bool TryGetInstance(out Scp956Pinata instance)
	{
		instance = Scp956Pinata._instance;
		return instance != null;
	}

	// Token: 0x17000034 RID: 52
	// (get) Token: 0x060000D2 RID: 210 RVA: 0x00004423 File Offset: 0x00002623
	// (set) Token: 0x060000D3 RID: 211 RVA: 0x0000442F File Offset: 0x0000262F
	public static bool IsSpawned
	{
		get
		{
			return Scp956Pinata._instance._spawned;
		}
		set
		{
			Scp956Pinata._instance.Network_spawned = value;
		}
	}

	// Token: 0x17000035 RID: 53
	// (get) Token: 0x060000D4 RID: 212 RVA: 0x0000443C File Offset: 0x0000263C
	private static float RandomSign
	{
		get
		{
			return (float)((UnityEngine.Random.value > 0.5f) ? 1 : -1);
		}
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x00004450 File Offset: 0x00002650
	[ClientRpc]
	private void RpcAttack()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		this.SendRPCInternal("System.Void Scp956Pinata::RpcAttack()", 977535511, writer, 0, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x060000D6 RID: 214 RVA: 0x00004480 File Offset: 0x00002680
	private void Awake()
	{
		this._tr = base.transform;
		Scp956Pinata._instance = this;
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x00004494 File Offset: 0x00002694
	private void Update()
	{
		this.UpdateVisual();
		if (NetworkServer.active)
		{
			this.UpdateAi();
		}
		this.UpdatePos();
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x000044B0 File Offset: 0x000026B0
	private void UpdateVisual()
	{
		this._attackTimer += Time.deltaTime;
		this._attackMover.localPosition = Vector3.forward * this._attackCurve.Evaluate(this._attackTimer);
		float num = Mathf.Clamp01(this._fade + this._appearSpeed * Time.deltaTime * (float)(this._spawned ? 1 : -1));
		bool flag = this._carpincho == 67;
		this._capybara.SetActive(flag && num > 0f);
		this._visuals.SetActive(!flag && num > 0f);
		if (num == this._fade)
		{
			return;
		}
		this._fade = num;
		this._appearMat.SetFloat(Scp559Cake.ShaderDissolveProperty, 1f - this._fade);
		this._tr.rotation = Quaternion.Euler(Vector3.up * this._syncRot);
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x000045A8 File Offset: 0x000027A8
	private void UpdateAi()
	{
		this._respawnTimer -= Time.deltaTime;
		if (!this._spawned && !Scp956Pinata._hasBeenGiftSpawned)
		{
			if (this._respawnTimer > 0f)
			{
				return;
			}
			Vector3 vector;
			if (!this.TryGetSpawnPos(out vector))
			{
				this._respawnTimer = this._changePositionCooldown;
				return;
			}
			this.Network_carpincho = (byte)UnityEngine.Random.Range(0, 255);
			this.Network_syncPos = vector;
			this._spawnPos = vector;
			this.Network_syncRot = UnityEngine.Random.value * 360f;
			this.Network_spawned = true;
			this.Network_flying = false;
			this._respawnTimer = this._huntingTime;
			return;
		}
		else
		{
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				Scp956Target.EffectReason effectReason = this.CheckSpawnCondition(referenceHub);
				if (effectReason == Scp956Target.EffectReason.None || !this.CheckPlayer(referenceHub))
				{
					if (Scp956Pinata.ActiveTargets.Remove(referenceHub))
					{
						referenceHub.playerEffectsController.DisableEffect<Scp956Target>();
					}
				}
				else
				{
					Scp956Pinata.ActiveTargets.Add(referenceHub);
					referenceHub.playerEffectsController.GetEffect<Scp956Target>().Intensity = (byte)effectReason;
					this._respawnTimer = this._huntingTime;
				}
			}
			if (this._respawnTimer < 0f)
			{
				this.TryDespawn();
				return;
			}
			bool flag = this._foundTarget == null;
			if (!flag && (this.CheckSpawnCondition(this._foundTarget.Hub) == Scp956Target.EffectReason.None || !Scp956Pinata.ActiveTargets.Contains(this._foundTarget.Hub)))
			{
				this._foundTarget = null;
				flag = true;
			}
			if (flag)
			{
				float num = float.MaxValue;
				bool flag2 = false;
				foreach (ReferenceHub referenceHub2 in Scp956Pinata.ActiveTargets)
				{
					Scp956Target effect = referenceHub2.playerEffectsController.GetEffect<Scp956Target>();
					if (effect.IsAffected)
					{
						float sqrMagnitude = (effect.Position - this._tr.position).sqrMagnitude;
						if (sqrMagnitude <= num)
						{
							this._foundTarget = effect;
							num = sqrMagnitude;
							flag2 = true;
						}
					}
				}
				if (!flag2)
				{
					return;
				}
				this._sequenceTimer = 0f;
				this._initialRot = this._syncRot;
				this._initialPos = this._syncPos;
				this._attackTriggered = false;
			}
			this._sequenceTimer += Time.deltaTime;
			Vector3 normalized = (this._foundTarget.Position - this._tr.position).normalized;
			float b = Vector3.Angle(normalized, Vector3.forward) * Mathf.Sign(Vector3.Dot(normalized, Vector3.right));
			this.Network_syncRot = Mathf.LerpAngle(this._initialRot, b, (this._sequenceTimer - 1f) / 4f);
			if (this._sequenceTimer < 6f)
			{
				return;
			}
			Vector3 b2 = this._foundTarget.Position - this._tr.forward.NormalizeIgnoreY();
			if (Mathf.Abs(this._spawnPos.y - this._foundTarget.Position.y) < this._heightTolerance)
			{
				b2.y = this._spawnPos.y;
				this.Network_flying = false;
			}
			else
			{
				b2.y = this._foundTarget.Position.y;
				this.Network_flying = true;
			}
			this.Network_syncPos = Vector3.Lerp(this._initialPos, b2, (this._sequenceTimer - 6f) / 7f);
			if (this._sequenceTimer < 13.3f)
			{
				return;
			}
			if (!this._attackTriggered)
			{
				this.RpcAttack();
				this._attackTriggered = true;
			}
			if (this._sequenceTimer < 13.5f)
			{
				return;
			}
			this._foundTarget.Hub.playerStats.DealDamage(new Scp956DamageHandler(normalized));
			this._foundTarget = null;
			return;
		}
	}

	// Token: 0x060000DA RID: 218 RVA: 0x0000498C File Offset: 0x00002B8C
	public void SpawnBehindTarget(ReferenceHub target)
	{
		Scp956Pinata.ActiveTargets.Add(target);
		this._foundTarget = target.playerEffectsController.EnableEffect<Scp956Target>(0f, false);
		this._foundTarget.Intensity = 254;
		this._respawnTimer = this._huntingTime;
		Transform transform = target.transform;
		this._initialPos = (this.Network_syncPos = (this._spawnPos = transform.position - transform.forward * 2f));
		this._initialRot = (this.Network_syncRot = UnityEngine.Random.value * 360f);
		this._respawnTimer = this._huntingTime;
		this._attackTriggered = false;
		Scp956Pinata._hasBeenGiftSpawned = (this.Network_spawned = true);
	}

	// Token: 0x060000DB RID: 219 RVA: 0x00004A50 File Offset: 0x00002C50
	private void TryDespawn()
	{
		Vector3 thisPos = this._tr.position;
		if (ReferenceHub.AllHubs.Any(delegate(ReferenceHub x)
		{
			IFpcRole fpcRole = x.roleManager.CurrentRole as IFpcRole;
			return fpcRole != null && this.CheckVisibility(thisPos, fpcRole.FpcModule.Position);
		}))
		{
			return;
		}
		this.Network_spawned = false;
		this._respawnTimer = this._changePositionCooldown;
	}

	// Token: 0x060000DC RID: 220 RVA: 0x00004AA8 File Offset: 0x00002CA8
	private bool CheckPlayer(ReferenceHub hub)
	{
		Vector3 position = this._tr.position;
		Vector3 position2 = (hub.roleManager.CurrentRole as HumanRole).FpcModule.Position;
		float num = this._targettingRange * this._targettingRange;
		WaypointBase waypointBase;
		return (position - position2).sqrMagnitude <= num && this.CheckVisibility(position, position2) && (!WaypointBase.TryGetWaypoint(new RelativePosition(position2).WaypointId, out waypointBase) || !(waypointBase is ElevatorWaypoint));
	}

	// Token: 0x060000DD RID: 221 RVA: 0x00004B30 File Offset: 0x00002D30
	private bool TryGetSpawnPos(out Vector3 pos)
	{
		pos = default(Vector3);
		Scp956Pinata.ZonesByIntensity.Clear();
		foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
		{
			RoomIdentifier roomIdentifier;
			if (this.CheckSpawnCondition(referenceHub) == Scp956Target.EffectReason.Child && referenceHub.roleManager.CurrentRole is HumanRole && referenceHub.TryGetCurrentRoom(out roomIdentifier) && this._spawnableZones.Contains(roomIdentifier.Zone))
			{
				int num;
				Scp956Pinata.ZonesByIntensity.TryGetValue(roomIdentifier.Zone, out num);
				Scp956Pinata.ZonesByIntensity[roomIdentifier.Zone] = num + 1;
			}
		}
		if (Scp956Pinata.ZonesByIntensity.Count == 0)
		{
			return false;
		}
		FacilityZone facilityZone = FacilityZone.None;
		int num2 = 0;
		foreach (KeyValuePair<FacilityZone, int> keyValuePair in Scp956Pinata.ZonesByIntensity)
		{
			if (keyValuePair.Value >= num2)
			{
				facilityZone = keyValuePair.Key;
				num2 = keyValuePair.Value;
			}
		}
		Scp956Pinata.CompatibleDoors.Clear();
		using (HashSet<DoorVariant>.Enumerator enumerator3 = DoorVariant.AllDoors.GetEnumerator())
		{
			while (enumerator3.MoveNext())
			{
				DoorVariant doorVariant = enumerator3.Current;
				if (doorVariant is BreakableDoor && doorVariant.Rooms != null && doorVariant.Rooms.Length != 0 && doorVariant.Rooms[0].Zone == facilityZone)
				{
					Scp956Pinata.CompatibleDoors.Add(doorVariant);
				}
			}
			goto IL_2B3;
		}
		IL_178:
		int index = UnityEngine.Random.Range(0, Scp956Pinata.CompatibleDoors.Count);
		Transform transform = Scp956Pinata.CompatibleDoors[index].transform;
		Scp956Pinata.CompatibleDoors.RemoveAt(index);
		pos = transform.position + this._doorOffset * Scp956Pinata.RandomSign * transform.forward + Vector3.up * this._raycastHeight;
		RaycastHit raycastHit;
		if (Physics.SphereCast(pos, this._pinataRadius, transform.right * Scp956Pinata.RandomSign, out raycastHit, this._raycastRange, FpcStateProcessor.Mask) && raycastHit.distance >= this._minDistance)
		{
			Vector3 vector = raycastHit.point + raycastHit.normal * this._pinataRadius;
			pos = new Vector3(vector.x, transform.position.y + this._floorHeight, vector.z);
			Vector3 checkPos = pos;
			if (!ReferenceHub.AllHubs.Any(delegate(ReferenceHub x)
			{
				IFpcRole fpcRole = x.roleManager.CurrentRole as IFpcRole;
				return fpcRole != null && this.CheckVisibility(checkPos, fpcRole.FpcModule.Position);
			}))
			{
				return true;
			}
		}
		IL_2B3:
		if (Scp956Pinata.CompatibleDoors.Count <= 0)
		{
			return false;
		}
		goto IL_178;
	}

	// Token: 0x060000DE RID: 222 RVA: 0x00004E2C File Offset: 0x0000302C
	private bool CheckVisibility(Vector3 checkPos, Vector3 humanPos)
	{
		return !Physics.CheckCapsule(checkPos, humanPos, this._targettingThickness, FpcStateProcessor.Mask);
	}

	// Token: 0x060000DF RID: 223 RVA: 0x00004E48 File Offset: 0x00003048
	private Scp956Target.EffectReason CheckSpawnCondition(ReferenceHub hub)
	{
		if (!(hub.roleManager.CurrentRole is HumanRole))
		{
			return Scp956Target.EffectReason.None;
		}
		if (hub.playerEffectsController.GetEffect<Scp559Effect>().IsEnabled)
		{
			return Scp956Target.EffectReason.Child;
		}
		if (!hub.inventory.UserInventory.Items.Any((KeyValuePair<ushort, ItemBase> x) => x.Value.ItemTypeId == ItemType.SCP330))
		{
			return Scp956Target.EffectReason.None;
		}
		return Scp956Target.EffectReason.HasCandy;
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x00004EC0 File Offset: 0x000030C0
	private void UpdatePos()
	{
		Vector3 position = this._tr.position;
		Vector3 syncPos = this._syncPos;
		if (this._flying)
		{
			syncPos.y += Mathf.Sin(Time.timeSinceLevelLoad * this._flyingSinSpeed) * this._flyingSinIntensity;
		}
		if ((position - syncPos).sqrMagnitude > 9f)
		{
			this._tr.position = this._syncPos;
		}
		this._tr.SetPositionAndRotation(Vector3.Lerp(position, syncPos, Time.deltaTime * this._positionLerp), Quaternion.Lerp(this._tr.rotation, Quaternion.Euler(Vector3.up * this._syncRot), Time.deltaTime * this._rotationLerp));
		Scp956Pinata.LastPosition = position;
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x00004F87 File Offset: 0x00003187
	[RuntimeInitializeOnLoadMethod]
	private static void Init()
	{
		ReferenceHub.OnPlayerRemoved += delegate(ReferenceHub hub)
		{
			Scp956Pinata.ActiveTargets.Remove(hub);
		};
	}

	// Token: 0x060000E3 RID: 227 RVA: 0x00004FB0 File Offset: 0x000031B0
	static Scp956Pinata()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(Scp956Pinata), "System.Void Scp956Pinata::RpcAttack()", new RemoteCallDelegate(Scp956Pinata.InvokeUserCode_RpcAttack));
	}

	// Token: 0x060000E4 RID: 228 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x17000036 RID: 54
	// (get) Token: 0x060000E5 RID: 229 RVA: 0x00005004 File Offset: 0x00003204
	// (set) Token: 0x060000E6 RID: 230 RVA: 0x00005017 File Offset: 0x00003217
	public bool Network_spawned
	{
		get
		{
			return this._spawned;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<bool>(value, ref this._spawned, 1UL, null);
		}
	}

	// Token: 0x17000037 RID: 55
	// (get) Token: 0x060000E7 RID: 231 RVA: 0x00005034 File Offset: 0x00003234
	// (set) Token: 0x060000E8 RID: 232 RVA: 0x00005047 File Offset: 0x00003247
	public Vector3 Network_syncPos
	{
		get
		{
			return this._syncPos;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<Vector3>(value, ref this._syncPos, 2UL, null);
		}
	}

	// Token: 0x17000038 RID: 56
	// (get) Token: 0x060000E9 RID: 233 RVA: 0x00005064 File Offset: 0x00003264
	// (set) Token: 0x060000EA RID: 234 RVA: 0x00005077 File Offset: 0x00003277
	public float Network_syncRot
	{
		get
		{
			return this._syncRot;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<float>(value, ref this._syncRot, 4UL, null);
		}
	}

	// Token: 0x17000039 RID: 57
	// (get) Token: 0x060000EB RID: 235 RVA: 0x00005094 File Offset: 0x00003294
	// (set) Token: 0x060000EC RID: 236 RVA: 0x000050A7 File Offset: 0x000032A7
	public bool Network_flying
	{
		get
		{
			return this._flying;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<bool>(value, ref this._flying, 8UL, null);
		}
	}

	// Token: 0x1700003A RID: 58
	// (get) Token: 0x060000ED RID: 237 RVA: 0x000050C4 File Offset: 0x000032C4
	// (set) Token: 0x060000EE RID: 238 RVA: 0x000050D7 File Offset: 0x000032D7
	public byte Network_carpincho
	{
		get
		{
			return this._carpincho;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<byte>(value, ref this._carpincho, 16UL, null);
		}
	}

	// Token: 0x060000EF RID: 239 RVA: 0x000050F1 File Offset: 0x000032F1
	protected void UserCode_RpcAttack()
	{
		this._attackTimer = 0f;
		this._attackSound.Play();
	}

	// Token: 0x060000F0 RID: 240 RVA: 0x00005109 File Offset: 0x00003309
	protected static void InvokeUserCode_RpcAttack(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcAttack called on server.");
			return;
		}
		((Scp956Pinata)obj).UserCode_RpcAttack();
	}

	// Token: 0x060000F1 RID: 241 RVA: 0x0000512C File Offset: 0x0000332C
	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteBool(this._spawned);
			writer.WriteVector3(this._syncPos);
			writer.WriteFloat(this._syncRot);
			writer.WriteBool(this._flying);
			writer.WriteByte(this._carpincho);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1UL) != 0UL)
		{
			writer.WriteBool(this._spawned);
		}
		if ((base.syncVarDirtyBits & 2UL) != 0UL)
		{
			writer.WriteVector3(this._syncPos);
		}
		if ((base.syncVarDirtyBits & 4UL) != 0UL)
		{
			writer.WriteFloat(this._syncRot);
		}
		if ((base.syncVarDirtyBits & 8UL) != 0UL)
		{
			writer.WriteBool(this._flying);
		}
		if ((base.syncVarDirtyBits & 16UL) != 0UL)
		{
			writer.WriteByte(this._carpincho);
		}
	}

	// Token: 0x060000F2 RID: 242 RVA: 0x0000523C File Offset: 0x0000343C
	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			base.GeneratedSyncVarDeserialize<bool>(ref this._spawned, null, reader.ReadBool());
			base.GeneratedSyncVarDeserialize<Vector3>(ref this._syncPos, null, reader.ReadVector3());
			base.GeneratedSyncVarDeserialize<float>(ref this._syncRot, null, reader.ReadFloat());
			base.GeneratedSyncVarDeserialize<bool>(ref this._flying, null, reader.ReadBool());
			base.GeneratedSyncVarDeserialize<byte>(ref this._carpincho, null, reader.ReadByte());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<bool>(ref this._spawned, null, reader.ReadBool());
		}
		if ((num & 2L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<Vector3>(ref this._syncPos, null, reader.ReadVector3());
		}
		if ((num & 4L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<float>(ref this._syncRot, null, reader.ReadFloat());
		}
		if ((num & 8L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<bool>(ref this._flying, null, reader.ReadBool());
		}
		if ((num & 16L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<byte>(ref this._carpincho, null, reader.ReadByte());
		}
	}

	// Token: 0x040000BD RID: 189
	[SyncVar]
	private bool _spawned;

	// Token: 0x040000BE RID: 190
	[SyncVar]
	private Vector3 _syncPos;

	// Token: 0x040000BF RID: 191
	[SyncVar]
	private float _syncRot;

	// Token: 0x040000C0 RID: 192
	[SyncVar]
	private bool _flying;

	// Token: 0x040000C1 RID: 193
	[SyncVar]
	private byte _carpincho;

	// Token: 0x040000C2 RID: 194
	[SerializeField]
	private AnimationCurve _attackCurve;

	// Token: 0x040000C3 RID: 195
	[SerializeField]
	private AudioSource _attackSound;

	// Token: 0x040000C4 RID: 196
	[SerializeField]
	private float _positionLerp;

	// Token: 0x040000C5 RID: 197
	[SerializeField]
	private float _rotationLerp;

	// Token: 0x040000C6 RID: 198
	[SerializeField]
	private Material _appearMat;

	// Token: 0x040000C7 RID: 199
	[SerializeField]
	private float _appearSpeed;

	// Token: 0x040000C8 RID: 200
	[SerializeField]
	private float _heightTolerance;

	// Token: 0x040000C9 RID: 201
	[SerializeField]
	private GameObject _visuals;

	// Token: 0x040000CA RID: 202
	[SerializeField]
	private GameObject _capybara;

	// Token: 0x040000CB RID: 203
	[SerializeField]
	private Transform _attackMover;

	// Token: 0x040000CC RID: 204
	[SerializeField]
	private float _changePositionCooldown;

	// Token: 0x040000CD RID: 205
	[SerializeField]
	private float _huntingTime;

	// Token: 0x040000CE RID: 206
	[SerializeField]
	private float _pinataRadius;

	// Token: 0x040000CF RID: 207
	[SerializeField]
	private float _doorOffset;

	// Token: 0x040000D0 RID: 208
	[SerializeField]
	private float _raycastRange;

	// Token: 0x040000D1 RID: 209
	[SerializeField]
	private float _raycastHeight;

	// Token: 0x040000D2 RID: 210
	[SerializeField]
	private float _minDistance;

	// Token: 0x040000D3 RID: 211
	[SerializeField]
	private float _floorHeight;

	// Token: 0x040000D4 RID: 212
	[SerializeField]
	private float _targettingThickness;

	// Token: 0x040000D5 RID: 213
	[SerializeField]
	private float _targettingRange;

	// Token: 0x040000D6 RID: 214
	[SerializeField]
	private float _flyingSinIntensity;

	// Token: 0x040000D7 RID: 215
	[SerializeField]
	private float _flyingSinSpeed;

	// Token: 0x040000D8 RID: 216
	[SerializeField]
	private FacilityZone[] _spawnableZones;

	// Token: 0x040000D9 RID: 217
	private float _fade;

	// Token: 0x040000DA RID: 218
	private float _respawnTimer;

	// Token: 0x040000DB RID: 219
	private float _sequenceTimer;

	// Token: 0x040000DC RID: 220
	private float _attackTimer;

	// Token: 0x040000DD RID: 221
	private float _initialRot;

	// Token: 0x040000DE RID: 222
	private Vector3 _spawnPos;

	// Token: 0x040000DF RID: 223
	private Vector3 _initialPos;

	// Token: 0x040000E0 RID: 224
	private Transform _tr;

	// Token: 0x040000E1 RID: 225
	private Scp956Target _foundTarget;

	// Token: 0x040000E2 RID: 226
	private bool _attackTriggered;

	// Token: 0x040000E3 RID: 227
	private const float SqrCutoffPoint = 9f;

	// Token: 0x040000E4 RID: 228
	public static readonly HashSet<ReferenceHub> ActiveTargets = new HashSet<ReferenceHub>();

	// Token: 0x040000E5 RID: 229
	private static readonly Dictionary<FacilityZone, int> ZonesByIntensity = new Dictionary<FacilityZone, int>();

	// Token: 0x040000E6 RID: 230
	private static readonly List<DoorVariant> CompatibleDoors = new List<DoorVariant>();

	// Token: 0x040000E7 RID: 231
	private static bool _hasBeenGiftSpawned = false;

	// Token: 0x040000E8 RID: 232
	private static Scp956Pinata _instance;

	// Token: 0x040000E9 RID: 233
	public static Vector3 LastPosition;
}
