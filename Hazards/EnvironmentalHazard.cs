using System;
using System.Collections.Generic;
using GameCore;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using Mirror;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

namespace Hazards
{
	// Token: 0x0200084D RID: 2125
	public abstract class EnvironmentalHazard : NetworkBehaviour
	{
		// Token: 0x170008E0 RID: 2272
		// (get) Token: 0x0600304C RID: 12364 RVA: 0x000A1636 File Offset: 0x0009F836
		public List<ReferenceHub> AffectedPlayers { get; } = new List<ReferenceHub>();

		// Token: 0x170008E1 RID: 2273
		// (get) Token: 0x0600304D RID: 12365 RVA: 0x000A163E File Offset: 0x0009F83E
		// (set) Token: 0x0600304E RID: 12366 RVA: 0x000A1646 File Offset: 0x0009F846
		public virtual float MaxDistance { get; set; }

		// Token: 0x170008E2 RID: 2274
		// (get) Token: 0x0600304F RID: 12367 RVA: 0x000A164F File Offset: 0x0009F84F
		// (set) Token: 0x06003050 RID: 12368 RVA: 0x000A1657 File Offset: 0x0009F857
		public virtual float MaxHeightDistance { get; set; }

		// Token: 0x170008E3 RID: 2275
		// (get) Token: 0x06003051 RID: 12369 RVA: 0x000A1660 File Offset: 0x0009F860
		// (set) Token: 0x06003052 RID: 12370 RVA: 0x000A1668 File Offset: 0x0009F868
		public virtual Vector3 SourceOffset { get; set; }

		// Token: 0x170008E4 RID: 2276
		// (get) Token: 0x06003053 RID: 12371 RVA: 0x000A1671 File Offset: 0x0009F871
		// (set) Token: 0x06003054 RID: 12372 RVA: 0x000A1679 File Offset: 0x0009F879
		public virtual bool IsActive { get; set; } = true;

		// Token: 0x170008E5 RID: 2277
		// (get) Token: 0x06003055 RID: 12373 RVA: 0x000A1682 File Offset: 0x0009F882
		// (set) Token: 0x06003056 RID: 12374 RVA: 0x000A169A File Offset: 0x0009F89A
		public virtual Vector3 SourcePosition
		{
			get
			{
				return base.transform.position + this.SourceOffset;
			}
			set
			{
				base.transform.position = value;
			}
		}

		// Token: 0x06003057 RID: 12375 RVA: 0x000A16A8 File Offset: 0x0009F8A8
		public virtual bool OnEnter(ReferenceHub player)
		{
			PlayerEnteringHazardEventArgs playerEnteringHazardEventArgs = new PlayerEnteringHazardEventArgs(player, this);
			PlayerEvents.OnEnteringHazard(playerEnteringHazardEventArgs);
			if (!playerEnteringHazardEventArgs.IsAllowed)
			{
				return false;
			}
			this.AffectedPlayers.Add(player);
			return true;
		}

		// Token: 0x06003058 RID: 12376 RVA: 0x00002E9A File Offset: 0x0000109A
		public virtual void OnStay(ReferenceHub player)
		{
		}

		// Token: 0x06003059 RID: 12377 RVA: 0x000A16CD File Offset: 0x0009F8CD
		public virtual bool OnExit(ReferenceHub player)
		{
			PlayerLeavingHazardEventArgs playerLeavingHazardEventArgs = new PlayerLeavingHazardEventArgs(player, this);
			PlayerEvents.OnLeavingHazard(playerLeavingHazardEventArgs);
			if (!playerLeavingHazardEventArgs.IsAllowed)
			{
				return false;
			}
			this.AffectedPlayers.Remove(player);
			return true;
		}

		// Token: 0x0600305A RID: 12378 RVA: 0x000A16F3 File Offset: 0x0009F8F3
		public virtual bool IsInArea(Vector3 sourcePos, Vector3 targetPos)
		{
			return Mathf.Abs(targetPos.y - sourcePos.y) <= this.MaxHeightDistance && (sourcePos - targetPos).SqrMagnitudeIgnoreY() <= this.MaxDistance * this.MaxDistance;
		}

		// Token: 0x0600305B RID: 12379 RVA: 0x000A1730 File Offset: 0x0009F930
		protected virtual void UpdateTargets()
		{
			List<ReferenceHub> list = ListPool<ReferenceHub>.Shared.Rent();
			foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
			{
				IFpcRole fpcRole = referenceHub.roleManager.CurrentRole as IFpcRole;
				if (fpcRole != null)
				{
					bool flag = this.AffectedPlayers.Contains(referenceHub);
					if (this.IsInArea(this.SourcePosition, fpcRole.FpcModule.Position))
					{
						if (!flag)
						{
							this.OnEnter(referenceHub);
						}
						else
						{
							list.Add(referenceHub);
						}
					}
					else if (flag)
					{
						this.OnExit(referenceHub);
					}
				}
			}
			if (list.Count == 0)
			{
				ListPool<ReferenceHub>.Shared.Return(list);
				return;
			}
			PlayersStayingInHazardEventArgs playersStayingInHazardEventArgs = new PlayersStayingInHazardEventArgs(list, this);
			PlayerEvents.OnStayingInHazard(playersStayingInHazardEventArgs);
			list.Clear();
			foreach (Player player in playersStayingInHazardEventArgs.AffectedPlayers)
			{
				list.Add(player.ReferenceHub);
			}
			ListPool<Player>.Shared.Return(playersStayingInHazardEventArgs.AffectedPlayers);
			foreach (ReferenceHub player2 in list)
			{
				this.OnStay(player2);
			}
			ListPool<ReferenceHub>.Shared.Return(list);
		}

		// Token: 0x0600305C RID: 12380 RVA: 0x000A18B4 File Offset: 0x0009FAB4
		private void OnRoleChanged(ReferenceHub userHub, PlayerRoleBase prevRole, PlayerRoleBase newRole)
		{
			if (this.AffectedPlayers.Contains(userHub))
			{
				this.OnExit(userHub);
			}
		}

		// Token: 0x0600305D RID: 12381 RVA: 0x000A18CC File Offset: 0x0009FACC
		protected virtual void Awake()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			Action<EnvironmentalHazard> onAdded = EnvironmentalHazard.OnAdded;
			if (onAdded == null)
			{
				return;
			}
			onAdded(this);
		}

		// Token: 0x0600305E RID: 12382 RVA: 0x000A18E8 File Offset: 0x0009FAE8
		protected virtual void Start()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			NetworkServer.Spawn(base.gameObject, null);
			if (!RoundStart.RoundStarted)
			{
				GameCore.Console.AddDebugLog("MAPGEN", "Spawning hazard: \"" + base.gameObject.name + "\"", MessageImportance.LessImportant, true);
			}
			PlayerRoleManager.OnRoleChanged += this.OnRoleChanged;
		}

		// Token: 0x0600305F RID: 12383 RVA: 0x000A1947 File Offset: 0x0009FB47
		protected virtual void Update()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			this.UpdateTargets();
		}

		// Token: 0x06003060 RID: 12384 RVA: 0x000A1957 File Offset: 0x0009FB57
		protected virtual void OnDestroy()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			PlayerRoleManager.OnRoleChanged -= this.OnRoleChanged;
			Action<EnvironmentalHazard> onRemoved = EnvironmentalHazard.OnRemoved;
			if (onRemoved == null)
			{
				return;
			}
			onRemoved(this);
		}

		// Token: 0x06003062 RID: 12386 RVA: 0x00002DA5 File Offset: 0x00000FA5
		public override bool Weaved()
		{
			return true;
		}

		// Token: 0x0400257B RID: 9595
		public static Action<EnvironmentalHazard> OnAdded;

		// Token: 0x0400257C RID: 9596
		public static Action<EnvironmentalHazard> OnRemoved;
	}
}
