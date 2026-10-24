using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CustomPlayerEffects;
using Footprinting;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.Usables.Scp244;
using MEC;
using Mirror;
using Mirror.RemoteCalls;
using NorthwoodLib.Pools;
using PlayerRoles;
using RelativePositioning;
using UnityEngine;

namespace Hazards
{
	// Token: 0x0200084E RID: 2126
	public class PrismaticCloud : TemporaryHazard
	{
		// Token: 0x170008E6 RID: 2278
		// (get) Token: 0x06003063 RID: 12387 RVA: 0x000A199C File Offset: 0x0009FB9C
		// (set) Token: 0x06003064 RID: 12388 RVA: 0x000A19B4 File Offset: 0x0009FBB4
		public override Vector3 SourcePosition
		{
			get
			{
				return this._correctPosition.position + this.SourceOffset;
			}
			set
			{
				this._correctPosition.position = value;
			}
		}

		// Token: 0x170008E7 RID: 2279
		// (get) Token: 0x06003065 RID: 12389 RVA: 0x000A19C2 File Offset: 0x0009FBC2
		// (set) Token: 0x06003066 RID: 12390 RVA: 0x000A19CA File Offset: 0x0009FBCA
		public List<ReferenceHub> IgnoredTargets { get; private set; } = ListPool<ReferenceHub>.Shared.Rent();

		// Token: 0x170008E8 RID: 2280
		// (get) Token: 0x06003067 RID: 12391 RVA: 0x000A19D3 File Offset: 0x0009FBD3
		// (set) Token: 0x06003068 RID: 12392 RVA: 0x000A19DB File Offset: 0x0009FBDB
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

		// Token: 0x170008E9 RID: 2281
		// (get) Token: 0x06003069 RID: 12393 RVA: 0x000A19E4 File Offset: 0x0009FBE4
		// (set) Token: 0x0600306A RID: 12394 RVA: 0x000A19EC File Offset: 0x0009FBEC
		public override float HazardDuration { get; set; } = 30f;

		// Token: 0x170008EA RID: 2282
		// (get) Token: 0x0600306B RID: 12395 RVA: 0x000A19F8 File Offset: 0x0009FBF8
		public override float DecaySpeed
		{
			get
			{
				float num = 1f;
				foreach (Scp244DeployablePickup scp244DeployablePickup in Scp244DeployablePickup.Instances)
				{
					num += scp244DeployablePickup.FogPercentForPoint(this.SourcePosition);
				}
				return num;
			}
		}

		// Token: 0x0600306C RID: 12396 RVA: 0x000A1A5C File Offset: 0x0009FC5C
		public override bool OnEnter(ReferenceHub player)
		{
			if (!this.IsActive || player.IsSCP(true) || this.IgnoredTargets.Contains(player))
			{
				return false;
			}
			this.ServerEnableEffect(player, true);
			return base.OnEnter(player);
		}

		// Token: 0x0600306D RID: 12397 RVA: 0x000A1A8E File Offset: 0x0009FC8E
		public override void OnStay(ReferenceHub player)
		{
			if (this.IgnoredTargets.Contains(player))
			{
				return;
			}
			this.ServerEnableEffect(player, true);
		}

		// Token: 0x0600306E RID: 12398 RVA: 0x000A1AA7 File Offset: 0x0009FCA7
		public override bool OnExit(ReferenceHub player)
		{
			if (!this.IsActive || player.IsSCP(true) || this.IgnoredTargets.Contains(player))
			{
				return false;
			}
			this.ServerEnableEffect(player, false);
			return base.OnExit(player);
		}

		// Token: 0x0600306F RID: 12399 RVA: 0x000A1AD9 File Offset: 0x0009FCD9
		protected override void Start()
		{
			base.Start();
			if (!NetworkServer.active)
			{
				return;
			}
			PrismaticCloud.Instances.Add(this);
			ExplosionGrenade.OnExploded += this.CheckExplosion;
		}

		// Token: 0x06003070 RID: 12400 RVA: 0x000A1B05 File Offset: 0x0009FD05
		[Server]
		public override void ServerDestroy()
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void Hazards.PrismaticCloud::ServerDestroy()' called when server was not active");
				return;
			}
			base.ServerDestroy();
			this.ServerDelayedDestroy(15f);
			this.RpcDespawn();
		}

		// Token: 0x06003071 RID: 12401 RVA: 0x000A1B33 File Offset: 0x0009FD33
		protected override void OnDestroy()
		{
			base.OnDestroy();
			ListPool<ReferenceHub>.Shared.Return(this.IgnoredTargets);
			if (!NetworkServer.active)
			{
				return;
			}
			PrismaticCloud.Instances.Remove(this);
			ExplosionGrenade.OnExploded -= this.CheckExplosion;
		}

		// Token: 0x06003072 RID: 12402 RVA: 0x000A1B70 File Offset: 0x0009FD70
		[ClientRpc]
		private void RpcDespawn()
		{
			NetworkWriterPooled writer = NetworkWriterPool.Get();
			this.SendRPCInternal("System.Void Hazards.PrismaticCloud::RpcDespawn()", -2006036242, writer, 0, true);
			NetworkWriterPool.Return(writer);
		}

		// Token: 0x06003073 RID: 12403 RVA: 0x000A1BA0 File Offset: 0x0009FDA0
		private void ServerEnableEffect(ReferenceHub target, bool changeOrigin = true)
		{
			Prismatic prismatic = target.playerEffectsController.EnableEffect<Prismatic>(3f, false);
			if (!changeOrigin)
			{
				return;
			}
			prismatic.OriginCloud = this;
		}

		// Token: 0x06003074 RID: 12404 RVA: 0x000A1BCA File Offset: 0x0009FDCA
		private void ServerDelayedDestroy(float waitTime)
		{
			if (!NetworkServer.active)
			{
				return;
			}
			Timing.RunCoroutine(this.DelayedRemoval(waitTime).CancelWith(base.gameObject));
		}

		// Token: 0x06003075 RID: 12405 RVA: 0x000A1BEC File Offset: 0x0009FDEC
		private void LateUpdate()
		{
			this.SourcePosition = this.SynchronizedPosition.Position;
		}

		// Token: 0x06003076 RID: 12406 RVA: 0x000A1C10 File Offset: 0x0009FE10
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
			this.ServerDestroy();
		}

		// Token: 0x06003077 RID: 12407 RVA: 0x000A1C67 File Offset: 0x0009FE67
		private IEnumerator<float> DelayedRemoval(float waitTime)
		{
			yield return Timing.WaitForSeconds(waitTime);
			NetworkServer.Destroy(base.gameObject);
			yield break;
		}

		// Token: 0x06003079 RID: 12409 RVA: 0x000A1CAB File Offset: 0x0009FEAB
		static PrismaticCloud()
		{
			RemoteProcedureCalls.RegisterRpc(typeof(PrismaticCloud), "System.Void Hazards.PrismaticCloud::RpcDespawn()", new RemoteCallDelegate(PrismaticCloud.InvokeUserCode_RpcDespawn));
		}

		// Token: 0x0600307A RID: 12410 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Weaved()
		{
			return true;
		}

		// Token: 0x170008EB RID: 2283
		// (get) Token: 0x0600307B RID: 12411 RVA: 0x000A1CD8 File Offset: 0x0009FED8
		// (set) Token: 0x0600307C RID: 12412 RVA: 0x000A1CEB File Offset: 0x0009FEEB
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

		// Token: 0x0600307D RID: 12413 RVA: 0x000A1D05 File Offset: 0x0009FF05
		protected void UserCode_RpcDespawn()
		{
			this._particles.Stop();
		}

		// Token: 0x0600307E RID: 12414 RVA: 0x000A1D12 File Offset: 0x0009FF12
		protected static void InvokeUserCode_RpcDespawn(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcDespawn called on server.");
				return;
			}
			((PrismaticCloud)obj).UserCode_RpcDespawn();
		}

		// Token: 0x0600307F RID: 12415 RVA: 0x000A1D38 File Offset: 0x0009FF38
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

		// Token: 0x06003080 RID: 12416 RVA: 0x000A1D90 File Offset: 0x0009FF90
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

		// Token: 0x04002582 RID: 9602
		public static readonly List<PrismaticCloud> Instances = new List<PrismaticCloud>();

		// Token: 0x04002583 RID: 9603
		public const float EffectDuration = 3f;

		// Token: 0x04002584 RID: 9604
		private const float ExplosionHeight = 5f;

		// Token: 0x04002585 RID: 9605
		private const float DelayedDestroy = 15f;

		// Token: 0x04002589 RID: 9609
		private readonly float _explodeDistance = 5.25f;

		// Token: 0x0400258A RID: 9610
		[SerializeField]
		private Transform _correctPosition;

		// Token: 0x0400258B RID: 9611
		[SerializeField]
		private ParticleSystem _particles;
	}
}
