using System;
using System.Collections.Generic;
using GameCore;
using MapGeneration.RoomConnectors;
using MEC;
using Mirror;
using Mirror.RemoteCalls;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp1507;
using UnityEngine;
using Utils.NonAllocLINQ;

namespace Christmas.Scp2536
{
	// Token: 0x020010CA RID: 4298
	public class Scp2536Controller : NetworkBehaviour
	{
		// Token: 0x170013D8 RID: 5080
		// (get) Token: 0x06005FB5 RID: 24501 RVA: 0x00140098 File Offset: 0x0013E298
		// (set) Token: 0x06005FB6 RID: 24502 RVA: 0x0014009F File Offset: 0x0013E29F
		public static Scp2536Controller Singleton { get; private set; }

		// Token: 0x170013D9 RID: 5081
		// (get) Token: 0x06005FB7 RID: 24503 RVA: 0x001400A7 File Offset: 0x0013E2A7
		// (set) Token: 0x06005FB8 RID: 24504 RVA: 0x001400AF File Offset: 0x0013E2AF
		public Scp2536GiftController GiftController { get; private set; }

		// Token: 0x170013DA RID: 5082
		// (get) Token: 0x06005FB9 RID: 24505 RVA: 0x001400B8 File Offset: 0x0013E2B8
		private bool AreScpsGuaranteed
		{
			get
			{
				return this._manifestationTracker > 0 && this._manifestationTracker % 5 == 0;
			}
		}

		// Token: 0x06005FBA RID: 24506 RVA: 0x001400D0 File Offset: 0x0013E2D0
		public static bool PrevalidateTarget(ReferenceHub hub)
		{
			return !(hub.roleManager.CurrentRole is Scp1507Role);
		}

		// Token: 0x06005FBB RID: 24507 RVA: 0x001400E8 File Offset: 0x0013E2E8
		private bool ServerFindTarget(out ReferenceHub target, out Scp2536Spawnpoint spawnpoint)
		{
			target = null;
			spawnpoint = null;
			if (!NetworkServer.active)
			{
				return false;
			}
			List<Team> list = ListPool<Team>.Shared.Rent();
			foreach (ReferenceHub hub in ReferenceHub.AllHubs)
			{
				if (Scp2536Controller.PrevalidateTarget(hub))
				{
					Team team = hub.GetTeam();
					if (!list.Contains(team) && ((this.AreScpsGuaranteed && team == Team.SCPs) || Scp2536Controller.WhitelistedTeams.Contains(team)))
					{
						list.Add(team);
					}
				}
			}
			if (list.Count == 0)
			{
				ListPool<Team>.Shared.Return(list);
				return false;
			}
			Team team2 = this.AreScpsGuaranteed ? Team.SCPs : list.RandomItem<Team>();
			List<ReferenceHub> list2 = ListPool<ReferenceHub>.Shared.Rent();
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				if (referenceHub.GetTeam() == team2 && Scp2536Controller.PrevalidateTarget(referenceHub) && !this._ignoredPlayers.Contains(referenceHub.netId) && referenceHub.roleManager.CurrentRole is IFpcRole && referenceHub.GetRoleId() != RoleTypeId.Scp0492)
				{
					list2.Add(referenceHub);
				}
			}
			ListPool<Team>.Shared.Return(list);
			if (list2.Count == 0)
			{
				this._ignoredPlayers.Clear();
				ListPool<ReferenceHub>.Shared.Return(list2);
				return false;
			}
			target = list2.RandomItem<ReferenceHub>();
			ListPool<ReferenceHub>.Shared.Return(list2);
			return this.CanFindPosition(target, out spawnpoint);
		}

		// Token: 0x06005FBC RID: 24508 RVA: 0x00140294 File Offset: 0x0013E494
		private bool CanFindPosition(ReferenceHub target, out Scp2536Spawnpoint foundSpot)
		{
			foundSpot = null;
			IFpcRole fpcRole = target.roleManager.CurrentRole as IFpcRole;
			if (fpcRole == null)
			{
				return false;
			}
			float num = this._maxDistanceSqr;
			Bounds bounds = this._collider.bounds;
			Func<SpawnableClutterConnector, bool> <>9__0;
			foreach (Scp2536Spawnpoint scp2536Spawnpoint in Scp2536Spawnpoint.Spawnpoints)
			{
				List<SpawnableClutterConnector> instances = SpawnableClutterConnector.Instances;
				Func<SpawnableClutterConnector, bool> condition;
				if ((condition = <>9__0) == null)
				{
					condition = (<>9__0 = ((SpawnableClutterConnector c) => c.Intersects(bounds)));
				}
				if (!instances.Any(condition))
				{
					float sqrMagnitude = (fpcRole.FpcModule.Position - scp2536Spawnpoint.Position).sqrMagnitude;
					if (sqrMagnitude <= this._maxDistanceSqr && sqrMagnitude >= this._minDistanceSqr && sqrMagnitude < num)
					{
						num = sqrMagnitude;
						foundSpot = scp2536Spawnpoint;
					}
				}
			}
			return num < this._maxDistanceSqr;
		}

		// Token: 0x06005FBD RID: 24509 RVA: 0x00140394 File Offset: 0x0013E594
		[ClientRpc]
		public void RpcMoveTree(Vector3 newPos, Quaternion newRot, byte randomSeed)
		{
			NetworkWriterPooled writer = NetworkWriterPool.Get();
			writer.WriteVector3(newPos);
			writer.WriteQuaternion(newRot);
			writer.WriteByte(randomSeed);
			this.SendRPCInternal("System.Void Christmas.Scp2536.Scp2536Controller::RpcMoveTree(UnityEngine.Vector3,UnityEngine.Quaternion,System.Byte)", 1984975944, writer, 0, true);
			NetworkWriterPool.Return(writer);
		}

		// Token: 0x06005FBE RID: 24510 RVA: 0x001403E4 File Offset: 0x0013E5E4
		[ClientRpc]
		private void RpcHideTree()
		{
			NetworkWriterPooled writer = NetworkWriterPool.Get();
			this.SendRPCInternal("System.Void Christmas.Scp2536.Scp2536Controller::RpcHideTree()", 1400729660, writer, 0, true);
			NetworkWriterPool.Return(writer);
		}

		// Token: 0x06005FBF RID: 24511 RVA: 0x00140414 File Offset: 0x0013E614
		private void Awake()
		{
			Scp2536Controller.Singleton = this;
			this._manifestationTracker = 1;
			Timing.RunCoroutine(this.TeleportingLogicCoroutine().CancelWith(this));
			if (Scp2536Controller._init)
			{
				return;
			}
			Scp2536Controller._init = true;
			foreach (Scp2536Controller.SoundPerTeam soundPerTeam in this._sounds)
			{
				Scp2536Controller.TeamClips.Add(soundPerTeam.Team, soundPerTeam.Clip);
			}
		}

		// Token: 0x06005FC0 RID: 24512 RVA: 0x00002E9A File Offset: 0x0000109A
		private void Update()
		{
		}

		// Token: 0x06005FC1 RID: 24513 RVA: 0x00140484 File Offset: 0x0013E684
		[ClientRpc]
		private void RpcPlayDisappear()
		{
			NetworkWriterPooled writer = NetworkWriterPool.Get();
			this.SendRPCInternal("System.Void Christmas.Scp2536.Scp2536Controller::RpcPlayDisappear()", 295100853, writer, 0, true);
			NetworkWriterPool.Return(writer);
		}

		// Token: 0x06005FC2 RID: 24514 RVA: 0x001404B4 File Offset: 0x0013E6B4
		[ClientRpc]
		private void RpcPlayTeamSpawn(Team team, Vector3 position)
		{
			NetworkWriterPooled writer = NetworkWriterPool.Get();
			Mirror.GeneratedNetworkCode._Write_PlayerRoles.Team(writer, team);
			writer.WriteVector3(position);
			this.SendRPCInternal("System.Void Christmas.Scp2536.Scp2536Controller::RpcPlayTeamSpawn(PlayerRoles.Team,UnityEngine.Vector3)", -1071699852, writer, 0, true);
			NetworkWriterPool.Return(writer);
		}

		// Token: 0x06005FC3 RID: 24515 RVA: 0x001404F8 File Offset: 0x0013E6F8
		private IEnumerator<float> TeleportingLogicCoroutine()
		{
			while (!RoundStart.RoundStarted)
			{
				yield return float.NegativeInfinity;
			}
			yield return Timing.WaitForSeconds(60f);
			for (;;)
			{
				yield return Timing.WaitForSeconds(10f);
				ReferenceHub referenceHub;
				Scp2536Spawnpoint scp2536Spawnpoint;
				if (this.ServerFindTarget(out referenceHub, out scp2536Spawnpoint))
				{
					Vector3 spawnPos = scp2536Spawnpoint.Position;
					Quaternion spawnRot = scp2536Spawnpoint.Rotation;
					this._ignoredPlayers.Add(referenceHub.netId);
					this._manifestationTracker++;
					this.RpcPlayTeamSpawn(referenceHub.GetTeam(), scp2536Spawnpoint.Position);
					yield return Timing.WaitForSeconds(3f);
					this.GiftController.ServerPrepareGifts(true);
					this.RpcMoveTree(spawnPos, spawnRot, (byte)UnityEngine.Random.Range(0, 256));
					yield return Timing.WaitForSeconds(20f);
					this.RpcPlayDisappear();
					yield return Timing.WaitForSeconds(1.3f);
					this.RpcHideTree();
					yield return Timing.WaitForSeconds(60f);
					spawnPos = default(Vector3);
					spawnRot = default(Quaternion);
				}
			}
			yield break;
		}

		// Token: 0x06005FC5 RID: 24517 RVA: 0x00140558 File Offset: 0x0013E758
		static Scp2536Controller()
		{
			RemoteProcedureCalls.RegisterRpc(typeof(Scp2536Controller), "System.Void Christmas.Scp2536.Scp2536Controller::RpcMoveTree(UnityEngine.Vector3,UnityEngine.Quaternion,System.Byte)", new RemoteCallDelegate(Scp2536Controller.InvokeUserCode_RpcMoveTree__Vector3__Quaternion__Byte));
			RemoteProcedureCalls.RegisterRpc(typeof(Scp2536Controller), "System.Void Christmas.Scp2536.Scp2536Controller::RpcHideTree()", new RemoteCallDelegate(Scp2536Controller.InvokeUserCode_RpcHideTree));
			RemoteProcedureCalls.RegisterRpc(typeof(Scp2536Controller), "System.Void Christmas.Scp2536.Scp2536Controller::RpcPlayDisappear()", new RemoteCallDelegate(Scp2536Controller.InvokeUserCode_RpcPlayDisappear));
			RemoteProcedureCalls.RegisterRpc(typeof(Scp2536Controller), "System.Void Christmas.Scp2536.Scp2536Controller::RpcPlayTeamSpawn(PlayerRoles.Team,UnityEngine.Vector3)", new RemoteCallDelegate(Scp2536Controller.InvokeUserCode_RpcPlayTeamSpawn__Team__Vector3));
		}

		// Token: 0x06005FC6 RID: 24518 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Weaved()
		{
			return true;
		}

		// Token: 0x06005FC7 RID: 24519 RVA: 0x00140605 File Offset: 0x0013E805
		protected void UserCode_RpcMoveTree__Vector3__Quaternion__Byte(Vector3 newPos, Quaternion newRot, byte randomSeed)
		{
			base.transform.SetPositionAndRotation(newPos, newRot);
		}

		// Token: 0x06005FC8 RID: 24520 RVA: 0x00140614 File Offset: 0x0013E814
		protected static void InvokeUserCode_RpcMoveTree__Vector3__Quaternion__Byte(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcMoveTree called on server.");
				return;
			}
			((Scp2536Controller)obj).UserCode_RpcMoveTree__Vector3__Quaternion__Byte(reader.ReadVector3(), reader.ReadQuaternion(), reader.ReadByte());
		}

		// Token: 0x06005FC9 RID: 24521 RVA: 0x00002E9A File Offset: 0x0000109A
		protected void UserCode_RpcHideTree()
		{
		}

		// Token: 0x06005FCA RID: 24522 RVA: 0x00140649 File Offset: 0x0013E849
		protected static void InvokeUserCode_RpcHideTree(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcHideTree called on server.");
				return;
			}
			((Scp2536Controller)obj).UserCode_RpcHideTree();
		}

		// Token: 0x06005FCB RID: 24523 RVA: 0x00002E9A File Offset: 0x0000109A
		protected void UserCode_RpcPlayDisappear()
		{
		}

		// Token: 0x06005FCC RID: 24524 RVA: 0x0014066C File Offset: 0x0013E86C
		protected static void InvokeUserCode_RpcPlayDisappear(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcPlayDisappear called on server.");
				return;
			}
			((Scp2536Controller)obj).UserCode_RpcPlayDisappear();
		}

		// Token: 0x06005FCD RID: 24525 RVA: 0x00002E9A File Offset: 0x0000109A
		protected void UserCode_RpcPlayTeamSpawn__Team__Vector3(Team team, Vector3 position)
		{
		}

		// Token: 0x06005FCE RID: 24526 RVA: 0x0014068F File Offset: 0x0013E88F
		protected static void InvokeUserCode_RpcPlayTeamSpawn__Team__Vector3(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
		{
			if (!NetworkClient.active)
			{
				Debug.LogError("RPC RpcPlayTeamSpawn called on server.");
				return;
			}
			((Scp2536Controller)obj).UserCode_RpcPlayTeamSpawn__Team__Vector3(Mirror.GeneratedNetworkCode._Read_PlayerRoles.Team(reader), reader.ReadVector3());
		}

		// Token: 0x040045C7 RID: 17863
		private const int AppearanceLifetime = 20;

		// Token: 0x040045C8 RID: 17864
		private const int TimeBetweenAppearances = 60;

		// Token: 0x040045C9 RID: 17865
		private const float AppearDelay = 3f;

		// Token: 0x040045CA RID: 17866
		private const float DisappearDelay = 1.3f;

		// Token: 0x040045CB RID: 17867
		private const int ScpGuaranteeRoundFrequency = 5;

		// Token: 0x040045CC RID: 17868
		private const float TimeBetweenRetries = 10f;

		// Token: 0x040045CD RID: 17869
		private static readonly Team[] WhitelistedTeams = new Team[]
		{
			Team.ClassD,
			Team.Scientists,
			Team.FoundationForces,
			Team.ChaosInsurgency
		};

		// Token: 0x040045CF RID: 17871
		public static readonly Dictionary<Team, AudioClip> TeamClips = new Dictionary<Team, AudioClip>();

		// Token: 0x040045D0 RID: 17872
		private static bool _init;

		// Token: 0x040045D2 RID: 17874
		[SerializeField]
		private Scp2536Controller.SoundPerTeam[] _sounds;

		// Token: 0x040045D3 RID: 17875
		[SerializeField]
		private List<AudioClip> _musicClips;

		// Token: 0x040045D4 RID: 17876
		[SerializeField]
		private AudioClip _disappearClip;

		// Token: 0x040045D5 RID: 17877
		[SerializeField]
		private AudioSource _musicSource;

		// Token: 0x040045D6 RID: 17878
		[SerializeField]
		private List<Material> _mats = new List<Material>();

		// Token: 0x040045D7 RID: 17879
		[SerializeField]
		private float _minDistanceSqr = 2f;

		// Token: 0x040045D8 RID: 17880
		[SerializeField]
		private float _maxDistanceSqr = 300f;

		// Token: 0x040045D9 RID: 17881
		[SerializeField]
		private Collider _collider;

		// Token: 0x040045DA RID: 17882
		private readonly HashSet<uint> _ignoredPlayers = new HashSet<uint>();

		// Token: 0x040045DB RID: 17883
		private bool _shaderIncrease = true;

		// Token: 0x040045DC RID: 17884
		private bool _hidden = true;

		// Token: 0x040045DD RID: 17885
		private int _manifestationTracker;

		// Token: 0x020010CB RID: 4299
		[Serializable]
		private struct SoundPerTeam
		{
			// Token: 0x040045DE RID: 17886
			public Team Team;

			// Token: 0x040045DF RID: 17887
			public AudioClip Clip;
		}
	}
}
