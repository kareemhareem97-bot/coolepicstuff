using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CustomPlayerEffects;
using Footprinting;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.Usables.Scp244;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using MEC;
using Mirror;
using Mirror.RemoteCalls;
using PlayerRoles;
using RelativePositioning;
using UnityEngine;

namespace Hazards
{
	// Token: 0x02000851 RID: 2129
	public class TantrumEnvironmentalHazard : TemporaryHazard
	{
		// Token: 0x170008EE RID: 2286
		// (get) Token: 0x0600308E RID: 12430 RVA: 0x000A1EFE File Offset: 0x000A00FE
		// (set) Token: 0x0600308F RID: 12431 RVA: 0x000A169A File Offset: 0x0009F89A
		public override Vector3 SourcePosition
		{
			get
			{
				return this._correctPosition.position + this.SourceOffset;
			}
			set
			{
				base.transform.position = value;
			}
		}

		// Token: 0x170008EF RID: 2287
		// (get) Token: 0x06003090 RID: 12432 RVA: 0x000A1F16 File Offset: 0x000A0116
		// (set) Token: 0x06003091 RID: 12433 RVA: 0x000A1F1E File Offset: 0x000A011E
		public RelativePosition SynchronizedPosition
		{
			[CompilerGenerated]
			get
			{
				return this.<SynchronizedPosition>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				this.Network<SynchronizedPosition>k__BackingField = value;
			}
		}

		// Token: 0x170008F0 RID: 2288
		// (get) Token: 0x06003092 RID: 12434 RVA: 0x000A1F27 File Offset: 0x000A0127
		// (set) Token: 0x06003093 RID: 12435 RVA: 0x000A1F2F File Offset: 0x000A012F
		public bool PlaySizzle { get; set; }

		// Token: 0x170008F1 RID: 2289
		// (get) Token: 0x06003094 RID: 12436 RVA: 0x000A1F38 File Offset: 0x000A0138
		// (set) Token: 0x06003095 RID: 12437 RVA: 0x000A1F40 File Offset: 0x000A0140
		public override float HazardDuration { get; set; } = 180f;

		// Token: 0x170008F2 RID: 2290
		// (get) Token: 0x06003096 RID: 12438 RVA: 0x000A1F4C File Offset: 0x000A014C
		public override float DecaySpeed
		{
			get
			{
				if (this._decaySpeedOverride >= 0f)
				{
					return this._decaySpeedOverride;
				}
				float num = 1f;
				foreach (Scp244DeployablePickup scp244DeployablePickup in Scp244DeployablePickup.Instances)
				{
					num += scp244DeployablePickup.FogPercentForPoint(this.SourcePosition);
				}
				return num;
			}
		}

		// Token: 0x06003097 RID: 12439 RVA: 0x000A1FC4 File Offset: 0x000A01C4
		public override bool OnEnter(ReferenceHub player)
		{
			if (!this.IsActive || player.IsSCP(true))
			{
				return false;
			}
			if (!base.OnEnter(player))
			{
				return false;
			}
			player.playerEffectsController.EnableEffect<Stained>(1f, false);
			PlayerEvents.OnEnteredHazard(new PlayerEnteredHazardEventArgs(player, this));
			return true;
		}

		// Token: 0x06003098 RID: 12440 RVA: 0x000A2003 File Offset: 0x000A0203
		public override void OnStay(ReferenceHub player)
		{
			player.playerEffectsController.EnableEffect<Stained>(1f, false);
		}

		// Token: 0x06003099 RID: 12441 RVA: 0x000A2017 File Offset: 0x000A0217
		public override bool OnExit(ReferenceHub player)
		{
			if (!base.OnExit(player))
			{
				return false;
			}
			if (!this.IsActive || player.IsSCP(true))
			{
				return false;
			}
			player.playerEffectsController.EnableEffect<Stained>(2f, false);
			PlayerEvents.OnLeftHazard(new PlayerLeftHazardEventArgs(player, this));
			return true;
		}

		// Token: 0x0600309A RID: 12442 RVA: 0x000A2056 File Offset: 0x000A0256
		protected override void Start()
		{
			base.Start();
			this.ClientApplyDecalSize();
			if (!NetworkServer.active)
			{
				return;
			}
			TantrumEnvironmentalHazard.AllTantrums.Add(this);
			ExplosionGrenade.OnExploded += this.CheckExplosion;
		}

		// Token: 0x0600309B RID: 12443 RVA: 0x000A2088 File Offset: 0x000A0288
		[Server]
		public override void ServerDestroy()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void Hazards.TantrumEnvironmentalHazard::ServerDestroy()' called when server was not active");
				return;
			}
			base.ServerDestroy();
			this.RpcDespawn(this.PlaySizzle);
			this.ServerDelayedDestroy(6f);
		}

		// Token: 0x0600309C RID: 12444 RVA: 0x000A20BC File Offset: 0x000A02BC
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (!NetworkServer.active)
			{
				return;
			}
			TantrumEnvironmentalHazard.AllTantrums.Remove(this);
			ExplosionGrenade.OnExploded -= this.CheckExplosion;
		}

		// Token: 0x0600309D RID: 12445 RVA: 0x000A20EC File Offset: 0x000A02EC
		[ClientRpc]
		private void RpcDespawn(bool playSizzle)
		{
			NetworkWriterPooled writer = NetworkWriterPool.Get();
			writer.WriteBool(playSizzle);
			this.SendRPCInternal("System.Void Hazards.TantrumEnvironmentalHazard::RpcDespawn(System.Boolean)", -963326249, writer, 0, true);
			NetworkWriterPool.Return(writer);
		}

		// Token: 0x0600309E RID: 12446 RVA: 0x000A2126 File Offset: 0x000A0326
		private void ServerDelayedDestroy(float waitTime)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			Timing.RunCoroutine(this.DelayedPuddleRemoval(waitTime).CancelWith(base.gameObject));
		}

		// Token: 0x0600309F RID: 12447 RVA: 0x000A2148 File Offset: 0x000A0348
		private void LateUpdate()
		{
			this.SourcePosition = this.SynchronizedPosition.Position;
		}

		// Token: 0x060030A0 RID: 12448 RVA: 0x000A216C File Offset: 0x000A036C
		private void CheckExplosion(Footprint attacker, Vector3 pos, ExplosionGrenade grenade)
		{
			Vector3 position = this._correctPosition.position;
			if (Mathf.Abs(pos.y - position.y) > 5f)
			{
				return;
			}
			float num = this._explodeDistance * this._explodeDistance;
			if ((position - pos).SqrMagnitudeIgnoreY() > num)
			{
				return;
			}
			this.PlaySizzle = true;
			this.ServerDestroy();
		}

		// Token: 0x060030A1 RID: 12449 RVA: 0x00002E9A File Offset: 0x0000109A
		private void ClientApplyDecalSize()
		{
		}

		// Token: 0x060030A2 RID: 12450 RVA: 0x000A21CA File Offset: 0x000A03CA
		private IEnumerator<float> DelayedPuddleRemoval(float waitTime)
		{
			yield return Timing.WaitForSeconds(waitTime);
			NetworkServer.Destroy(base.gameObject);
			yield break;
		}

		// Token: 0x060030A4 RID: 12452 RVA: 0x000A2209 File Offset: 0x000A0409
		static TantrumEnvironmentalHazard()
		{
			RemoteProcedureCalls.RegisterRpc(typeof(TantrumEnvironmentalHazard), "System.Void Hazards.TantrumEnvironmentalHazard::RpcDespawn(System.Boolean)", new RemoteCallDelegate(TantrumEnvironmentalHazard.InvokeUserCode_RpcDespawn__Boolean));
		}

		// Token: 0x060030A5 RID: 12453 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Weaved()
		{
			return true;
		}

		// Token: 0x170008F3 RID: 2291
		// (get) Token: 0x060030A6 RID: 12454 RVA: 0x000A2238 File Offset: 0x000A0438
		// (set) Token: 0x060030A7 RID: 12455 RVA: 0x000A224B File Offset: 0x000A044B
		public RelativePosition Network<SynchronizedPosition>k__BackingField
		{
			get
			{
				return this.<SynchronizedPosition>k__BackingField;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<RelativePosition>(value, ref this.<SynchronizedPosition>k__BackingField, 1UL, null);
			}
		}

		// Token: 0x060030A8 RID: 12456 RVA: 0x00002E9A File Offset: 0x0000109A
		protected void UserCode_RpcDespawn__Boolean(bool playSizzle)
		{
		}

		// Token: 0x060030A9 RID: 12457 RVA: 0x000A2265 File Offset: 0x000A0465
		protected static void InvokeUserCode_RpcDespawn__Boolean(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcDespawn called on server.");
				return;
			}
			((TantrumEnvironmentalHazard)obj).UserCode_RpcDespawn__Boolean(reader.ReadBool());
		}

		// Token: 0x060030AA RID: 12458 RVA: 0x000A2290 File Offset: 0x000A0490
		public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
		{
			base.SerializeSyncVars(writer, forceAll);
			if (forceAll)
			{
				writer.WriteRelativePosition(this.<SynchronizedPosition>k__BackingField);
				return;
			}
			writer.WriteULong(base.syncVarDirtyBits);
			if ((base.syncVarDirtyBits & 1UL) != 0UL)
			{
				writer.WriteRelativePosition(this.<SynchronizedPosition>k__BackingField);
			}
		}

		// Token: 0x060030AB RID: 12459 RVA: 0x000A22E8 File Offset: 0x000A04E8
		public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
		{
			base.DeserializeSyncVars(reader, initialState);
			if (initialState)
			{
				base.GeneratedSyncVarDeserialize<RelativePosition>(ref this.<SynchronizedPosition>k__BackingField, null, reader.ReadRelativePosition());
				return;
			}
			long num = (long)reader.ReadULong();
			if ((num & 1L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<RelativePosition>(ref this.<SynchronizedPosition>k__BackingField, null, reader.ReadRelativePosition());
			}
		}

		// Token: 0x04002590 RID: 9616
		public static readonly List<TantrumEnvironmentalHazard> AllTantrums = new List<TantrumEnvironmentalHazard>();

		// Token: 0x04002591 RID: 9617
		private const float ExplosionHeight = 5f;

		// Token: 0x04002592 RID: 9618
		private const float DelayedDestroy = 6f;

		// Token: 0x04002596 RID: 9622
		private float _decaySpeedOverride = -1f;

		// Token: 0x04002597 RID: 9623
		private readonly float _explodeDistance = 5.25f;

		// Token: 0x04002598 RID: 9624
		[SerializeField]
		private Transform _correctPosition;
	}
}
