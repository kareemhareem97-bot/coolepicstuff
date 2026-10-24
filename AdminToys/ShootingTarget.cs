using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Interactables;
using Interactables.Verification;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using MapGeneration.StaticHelpers;
using Mirror;
using Mirror.RemoteCalls;
using PlayerStatsSystem;
using UnityEngine;
using UnityEngine.UI;

namespace AdminToys
{
	// Token: 0x02000AE0 RID: 2784
	public class ShootingTarget : AdminToyBase, IDestructible, IBlockStaticBatching, IClientInteractable, IInteractable, IServerInteractable
	{
		// Token: 0x17000E45 RID: 3653
		// (get) Token: 0x06003F13 RID: 16147 RVA: 0x00002B80 File Offset: 0x00000D80
		public uint NetworkId
		{
			get
			{
				return base.netId;
			}
		}

		// Token: 0x17000E46 RID: 3654
		// (get) Token: 0x06003F14 RID: 16148 RVA: 0x00003897 File Offset: 0x00001A97
		public IVerificationRule VerificationRule
		{
			get
			{
				return StandardDistanceVerification.Default;
			}
		}

		// Token: 0x17000E47 RID: 3655
		// (get) Token: 0x06003F15 RID: 16149 RVA: 0x000D0F8F File Offset: 0x000CF18F
		public Vector3 CenterOfMass
		{
			get
			{
				return this._bullsEye.position;
			}
		}

		// Token: 0x17000E48 RID: 3656
		// (get) Token: 0x06003F16 RID: 16150 RVA: 0x000D0F9C File Offset: 0x000CF19C
		public override string CommandName
		{
			get
			{
				return "Target" + this._targetName;
			}
		}

		// Token: 0x06003F17 RID: 16151 RVA: 0x000D0FB0 File Offset: 0x000CF1B0
		public override void OnSpawned(ReferenceHub admin, ArraySegment<string> arguments)
		{
			RaycastHit raycastHit;
			if (Physics.Raycast(admin.transform.position - admin.transform.forward, Vector3.down, out raycastHit, 2f))
			{
				base.transform.position = raycastHit.point;
				base.transform.rotation = Quaternion.Euler(Vector3.up * (Mathf.Round((admin.transform.rotation.eulerAngles.y + 90f) / 10f) * 10f));
			}
			base.OnSpawned(admin, arguments);
		}

		// Token: 0x06003F18 RID: 16152 RVA: 0x000D1050 File Offset: 0x000CF250
		public bool Damage(float damage, DamageHandlerBase handler, Vector3 exactHit)
		{
			AttackerDamageHandler attackerDamageHandler = handler as AttackerDamageHandler;
			if (attackerDamageHandler == null)
			{
				return false;
			}
			ReferenceHub hub = attackerDamageHandler.Attacker.Hub;
			if (hub == null)
			{
				return false;
			}
			PlayerDamagingShootingTargetEventArgs playerDamagingShootingTargetEventArgs = new PlayerDamagingShootingTargetEventArgs(hub, this, handler);
			PlayerEvents.OnDamagingShootingTarget(playerDamagingShootingTargetEventArgs);
			if (!playerDamagingShootingTargetEventArgs.IsAllowed)
			{
				return false;
			}
			float distance = Vector3.Distance(hub.transform.position, this._bullsEye.position);
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				if (this._syncMode || referenceHub == hub)
				{
					this.TargetRpcReceiveData(referenceHub.characterClassManager.connectionToClient, damage, distance, exactHit, handler);
				}
			}
			PlayerEvents.OnDamagedShootingTarget(new PlayerDamagedShootingTargetEventArgs(hub, this, handler));
			return true;
		}

		// Token: 0x06003F19 RID: 16153 RVA: 0x000D1128 File Offset: 0x000CF328
		public void ClientInteract(InteractableCollider collider)
		{
			this.UseButton((ShootingTarget.TargetButton)collider.ColliderId);
		}

		// Token: 0x06003F1A RID: 16154 RVA: 0x000D1138 File Offset: 0x000CF338
		private void UseButton(ShootingTarget.TargetButton tb)
		{
			switch (tb)
			{
			case ShootingTarget.TargetButton.IncreaseHP:
				this._maxHp = Mathf.Clamp(this._maxHp * 2, 1, 256);
				return;
			case ShootingTarget.TargetButton.DecreaseHP:
				this._maxHp /= 2;
				return;
			case ShootingTarget.TargetButton.IncreaseResetTime:
				this._autoDestroyTime = Mathf.Min(this._autoDestroyTime + 1, 10);
				return;
			case ShootingTarget.TargetButton.DecreaseResetTime:
				this._autoDestroyTime = Mathf.Max(this._autoDestroyTime - 1, 0);
				return;
			case ShootingTarget.TargetButton.ManualReset:
				this.ClearTarget();
				return;
			default:
				return;
			}
		}

		// Token: 0x06003F1B RID: 16155 RVA: 0x000D11BC File Offset: 0x000CF3BC
		private void ClearTarget()
		{
			foreach (GameObject obj in this._hits)
			{
				UnityEngine.Object.Destroy(obj);
			}
			this._hits.Clear();
			this._avg = 0f;
			this._hp = (float)this._maxHp;
		}

		// Token: 0x06003F1C RID: 16156 RVA: 0x000D1230 File Offset: 0x000CF430
		public void ServerInteract(ReferenceHub ply, byte colliderId)
		{
			if (!PermissionsHandler.IsPermitted(ply.serverRoles.Permissions, PlayerPermissions.FacilityManagement))
			{
				return;
			}
			PlayerInteractingShootingTargetEventArgs playerInteractingShootingTargetEventArgs = new PlayerInteractingShootingTargetEventArgs(ply, this);
			PlayerEvents.OnInteractingShootingTarget(playerInteractingShootingTargetEventArgs);
			if (!playerInteractingShootingTargetEventArgs.IsAllowed)
			{
				return;
			}
			if (colliderId != 5)
			{
				if (colliderId != 6)
				{
					if (this._syncMode && !ply.isLocalPlayer)
					{
						this.UseButton((ShootingTarget.TargetButton)colliderId);
						this.RpcSendInfo(this._maxHp, this._autoDestroyTime);
					}
				}
				else
				{
					this.Network_syncMode = !this._syncMode;
				}
			}
			else
			{
				NetworkServer.Destroy(base.gameObject);
			}
			PlayerEvents.OnInteractedShootingTarget(new PlayerInteractedShootingTargetEventArgs(ply, this));
		}

		// Token: 0x06003F1D RID: 16157 RVA: 0x000D12CC File Offset: 0x000CF4CC
		[TargetRpc]
		private void TargetRpcReceiveData(NetworkConnection conn, float damage, float distance, Vector3 pos, DamageHandlerBase handler)
		{
			NetworkWriterPooled writer = NetworkWriterPool.Get();
			writer.WriteFloat(damage);
			writer.WriteFloat(distance);
			writer.WriteVector3(pos);
			writer.WriteDamageHandler(handler);
			this.SendTargetRPCInternal(conn, "System.Void AdminToys.ShootingTarget::TargetRpcReceiveData(Mirror.NetworkConnection,System.Single,System.Single,UnityEngine.Vector3,PlayerStatsSystem.DamageHandlerBase)", -668080035, writer, 0);
			NetworkWriterPool.Return(writer);
		}

		// Token: 0x06003F1E RID: 16158 RVA: 0x000D1324 File Offset: 0x000CF524
		[ClientRpc]
		private void RpcSendInfo(int maxHp, int autoReset)
		{
			NetworkWriterPooled writer = NetworkWriterPool.Get();
			writer.WriteInt(maxHp);
			writer.WriteInt(autoReset);
			this.SendRPCInternal("System.Void AdminToys.ShootingTarget::RpcSendInfo(System.Int32,System.Int32)", -479456756, writer, 0, true);
			NetworkWriterPool.Return(writer);
		}

		// Token: 0x06003F1F RID: 16159 RVA: 0x000D1368 File Offset: 0x000CF568
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(this._bullsEye.position, this._bullsEyeRadius);
			Gizmos.DrawWireSphere(this._bullsEye.position, this._stepSize);
			foreach (Vector3 vector in this._bullsEyeBounds)
			{
				Gizmos.DrawWireCube(this._bullsEye.TransformPoint(new Vector3(0f, vector.y, vector.x)), new Vector3(0.04f, 1f, 1f) * vector.z);
			}
		}

		// Token: 0x06003F21 RID: 16161 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Weaved()
		{
			return true;
		}

		// Token: 0x17000E49 RID: 3657
		// (get) Token: 0x06003F22 RID: 16162 RVA: 0x000D1440 File Offset: 0x000CF640
		// (set) Token: 0x06003F23 RID: 16163 RVA: 0x000D1453 File Offset: 0x000CF653
		public bool Network_syncMode
		{
			get
			{
				return this._syncMode;
			}
			[param: In]
			set
			{
				base.GeneratedSyncVarSetter<bool>(value, ref this._syncMode, 32UL, null);
			}
		}

		// Token: 0x06003F24 RID: 16164 RVA: 0x000D1470 File Offset: 0x000CF670
		protected void UserCode_TargetRpcReceiveData__NetworkConnection__Single__Single__Vector3__DamageHandlerBase(NetworkConnection conn, float damage, float distance, Vector3 pos, DamageHandlerBase handler)
		{
			float num;
			if (this._bullsEyeBounds.Length == 0)
			{
				num = Vector3.Distance(this._bullsEye.position, pos);
			}
			else
			{
				num = float.PositiveInfinity;
				foreach (Vector3 vector in this._bullsEyeBounds)
				{
					Bounds bounds = new Bounds(this._bullsEye.TransformPoint(new Vector3(0f, vector.y, vector.x)), Vector3.one * vector.z);
					float num2 = Vector3.Distance(bounds.ClosestPoint(pos), pos);
					if (num2 < num)
					{
						num = num2;
					}
				}
			}
			num = Mathf.Max(0f, num - this._bullsEyeRadius);
			int num3 = Mathf.Min(Mathf.CeilToInt(num / this._stepSize), this._score.Length - 1);
			float num4 = 1f - (float)num3 / ((float)this._score.Length - 1f);
			this._avg += num4;
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			gameObject.GetComponent<Collider>().enabled = false;
			gameObject.GetComponent<MeshRenderer>().sharedMaterial = this._hitIndicator.GetComponent<MeshRenderer>().sharedMaterial;
			gameObject.transform.localScale = this._hitIndicator.transform.localScale;
			gameObject.transform.parent = this._hitIndicator.transform.parent;
			gameObject.transform.position = pos;
			this._hp -= damage;
			this._source.Stop();
			this._source.PlayOneShot(this._score[num3]);
			this._source.PlayOneShot((this._hp < 0f) ? this._killSound : this._hitSound);
			MeshRenderer meshRenderer;
			if (this._prevHit != null && this._prevHit.TryGetComponent<MeshRenderer>(out meshRenderer))
			{
				meshRenderer.sharedMaterial = this._prevHitMat;
			}
			this._prevHit = gameObject;
			if (this._autoDestroyTime > 0)
			{
				UnityEngine.Object.Destroy(gameObject, (float)this._autoDestroyTime);
				return;
			}
			this._hits.Add(gameObject);
		}

		// Token: 0x06003F25 RID: 16165 RVA: 0x000D168E File Offset: 0x000CF88E
		protected static void InvokeUserCode_TargetRpcReceiveData__NetworkConnection__Single__Single__Vector3__DamageHandlerBase(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("TargetRPC TargetRpcReceiveData called on server.");
				return;
			}
			((ShootingTarget)obj).UserCode_TargetRpcReceiveData__NetworkConnection__Single__Single__Vector3__DamageHandlerBase(null, reader.ReadFloat(), reader.ReadFloat(), reader.ReadVector3(), reader.ReadDamageHandler());
		}

		// Token: 0x06003F26 RID: 16166 RVA: 0x000D16CC File Offset: 0x000CF8CC
		protected void UserCode_RpcSendInfo__Int32__Int32(int maxHp, int autoReset)
		{
			this._maxHp = maxHp;
			this._autoDestroyTime = autoReset;
			this.ClearTarget();
		}

		// Token: 0x06003F27 RID: 16167 RVA: 0x000D16E2 File Offset: 0x000CF8E2
		protected static void InvokeUserCode_RpcSendInfo__Int32__Int32(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcSendInfo called on server.");
				return;
			}
			((ShootingTarget)obj).UserCode_RpcSendInfo__Int32__Int32(reader.ReadInt(), reader.ReadInt());
		}

		// Token: 0x06003F28 RID: 16168 RVA: 0x000D1714 File Offset: 0x000CF914
		static ShootingTarget()
		{
			RemoteProcedureCalls.RegisterRpc(typeof(ShootingTarget), "System.Void AdminToys.ShootingTarget::RpcSendInfo(System.Int32,System.Int32)", new RemoteCallDelegate(ShootingTarget.InvokeUserCode_RpcSendInfo__Int32__Int32));
			RemoteProcedureCalls.RegisterRpc(typeof(ShootingTarget), "System.Void AdminToys.ShootingTarget::TargetRpcReceiveData(Mirror.NetworkConnection,System.Single,System.Single,UnityEngine.Vector3,PlayerStatsSystem.DamageHandlerBase)", new RemoteCallDelegate(ShootingTarget.InvokeUserCode_TargetRpcReceiveData__NetworkConnection__Single__Single__Vector3__DamageHandlerBase));
		}

		// Token: 0x06003F29 RID: 16169 RVA: 0x000D1764 File Offset: 0x000CF964
		public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
		{
			base.SerializeSyncVars(writer, forceAll);
			if (forceAll)
			{
				writer.WriteBool(this._syncMode);
				return;
			}
			writer.WriteULong(base.syncVarDirtyBits);
			if ((base.syncVarDirtyBits & 32UL) != 0UL)
			{
				writer.WriteBool(this._syncMode);
			}
		}

		// Token: 0x06003F2A RID: 16170 RVA: 0x000D17BC File Offset: 0x000CF9BC
		public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
		{
			base.DeserializeSyncVars(reader, initialState);
			if (initialState)
			{
				base.GeneratedSyncVarDeserialize<bool>(ref this._syncMode, null, reader.ReadBool());
				return;
			}
			long num = (long)reader.ReadULong();
			if ((num & 32L) != 0L)
			{
				base.GeneratedSyncVarDeserialize<bool>(ref this._syncMode, null, reader.ReadBool());
			}
		}

		// Token: 0x04002DFC RID: 11772
		private float _hp = 10f;

		// Token: 0x04002DFD RID: 11773
		private int _maxHp = 10;

		// Token: 0x04002DFE RID: 11774
		private int _autoDestroyTime;

		// Token: 0x04002DFF RID: 11775
		private float _avg;

		// Token: 0x04002E00 RID: 11776
		private GameObject _prevHit;

		// Token: 0x04002E01 RID: 11777
		[SyncVar]
		private bool _syncMode;

		// Token: 0x04002E02 RID: 11778
		[SerializeField]
		private float _stepSize = 0.12f;

		// Token: 0x04002E03 RID: 11779
		[SerializeField]
		private string _targetName;

		// Token: 0x04002E04 RID: 11780
		[SerializeField]
		private AudioSource _source;

		// Token: 0x04002E05 RID: 11781
		[SerializeField]
		private AudioClip _hitSound;

		// Token: 0x04002E06 RID: 11782
		[SerializeField]
		private AudioClip _killSound;

		// Token: 0x04002E07 RID: 11783
		[SerializeField]
		private AudioClip[] _score;

		// Token: 0x04002E08 RID: 11784
		[SerializeField]
		private GameObject _hitIndicator;

		// Token: 0x04002E09 RID: 11785
		[SerializeField]
		private Transform _bullsEye;

		// Token: 0x04002E0A RID: 11786
		[SerializeField]
		private float _bullsEyeRadius;

		// Token: 0x04002E0B RID: 11787
		[SerializeField]
		private Vector3[] _bullsEyeBounds;

		// Token: 0x04002E0C RID: 11788
		[SerializeField]
		private Material _prevHitMat;

		// Token: 0x04002E0D RID: 11789
		[SerializeField]
		private Text _lastHitInfo;

		// Token: 0x04002E0E RID: 11790
		[SerializeField]
		private Text _syncText;

		// Token: 0x04002E0F RID: 11791
		[SerializeField]
		private Text _settingsWindow;

		// Token: 0x04002E10 RID: 11792
		private readonly List<GameObject> _hits = new List<GameObject>();

		// Token: 0x02000AE1 RID: 2785
		private enum TargetButton
		{
			// Token: 0x04002E12 RID: 11794
			IncreaseHP,
			// Token: 0x04002E13 RID: 11795
			DecreaseHP,
			// Token: 0x04002E14 RID: 11796
			IncreaseResetTime,
			// Token: 0x04002E15 RID: 11797
			DecreaseResetTime,
			// Token: 0x04002E16 RID: 11798
			ManualReset,
			// Token: 0x04002E17 RID: 11799
			Remove,
			// Token: 0x04002E18 RID: 11800
			GlobalResults
		}
	}
}
