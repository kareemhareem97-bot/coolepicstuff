using System;
using System.Collections.Generic;
using CustomPlayerEffects;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using Mirror;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.PlayableScps.Scp106;
using PlayerStatsSystem;
using UnityEngine;

// Token: 0x02000148 RID: 328
public class PocketDimensionTeleport : NetworkBehaviour
{
	// Token: 0x14000017 RID: 23
	// (add) Token: 0x0600063D RID: 1597 RVA: 0x000179E4 File Offset: 0x00015BE4
	// (remove) Token: 0x0600063E RID: 1598 RVA: 0x00017A18 File Offset: 0x00015C18
	public static event Action<PocketDimensionTeleport> OnAdded;

	// Token: 0x14000018 RID: 24
	// (add) Token: 0x0600063F RID: 1599 RVA: 0x00017A4C File Offset: 0x00015C4C
	// (remove) Token: 0x06000640 RID: 1600 RVA: 0x00017A80 File Offset: 0x00015C80
	public static event Action<PocketDimensionTeleport> OnRemoved;

	// Token: 0x14000019 RID: 25
	// (add) Token: 0x06000641 RID: 1601 RVA: 0x00017AB4 File Offset: 0x00015CB4
	// (remove) Token: 0x06000642 RID: 1602 RVA: 0x00017AE8 File Offset: 0x00015CE8
	public static event Action OnInstancesUpdated;

	// Token: 0x1400001A RID: 26
	// (add) Token: 0x06000643 RID: 1603 RVA: 0x00017B1C File Offset: 0x00015D1C
	// (remove) Token: 0x06000644 RID: 1604 RVA: 0x00017B50 File Offset: 0x00015D50
	public static event PocketDimensionTeleport.PlayerEscapePocketDimension OnPlayerEscapePocketDimension;

	// Token: 0x06000645 RID: 1605 RVA: 0x00017B83 File Offset: 0x00015D83
	public void SetType(PocketDimensionTeleport.PDTeleportType t)
	{
		this._type = t;
	}

	// Token: 0x06000646 RID: 1606 RVA: 0x00017B8C File Offset: 0x00015D8C
	public PocketDimensionTeleport.PDTeleportType GetTeleportType()
	{
		return this._type;
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x00017B94 File Offset: 0x00015D94
	public static void TryExit(PocketDimensionTeleport instance, ReferenceHub hub)
	{
		if (!(hub.roleManager.CurrentRole is IFpcRole))
		{
			return;
		}
		PlayerLeavingPocketDimensionEventArgs playerLeavingPocketDimensionEventArgs = new PlayerLeavingPocketDimensionEventArgs(hub, instance, true);
		PlayerEvents.OnLeavingPocketDimension(playerLeavingPocketDimensionEventArgs);
		if (!playerLeavingPocketDimensionEventArgs.IsAllowed)
		{
			return;
		}
		if (!playerLeavingPocketDimensionEventArgs.IsSuccessful)
		{
			PocketDimensionTeleport.Kill(instance, hub);
			return;
		}
		PocketDimensionTeleport.Exit(instance, hub);
	}

	// Token: 0x06000648 RID: 1608 RVA: 0x00017BE4 File Offset: 0x00015DE4
	public static void Exit(PocketDimensionTeleport instance, ReferenceHub hub)
	{
		IFpcRole fpcRole = hub.roleManager.CurrentRole as IFpcRole;
		if (fpcRole == null)
		{
			return;
		}
		fpcRole.FpcModule.ServerOverridePosition(Scp106PocketExitFinder.GetBestExitPosition(fpcRole));
		hub.playerEffectsController.EnableEffect<Disabled>(10f, true);
		hub.playerEffectsController.EnableEffect<Traumatized>(0f, false);
		hub.playerEffectsController.DisableEffect<PocketCorroding>();
		hub.playerEffectsController.DisableEffect<Corroding>();
		PocketDimensionTeleport.PlayerEscapePocketDimension onPlayerEscapePocketDimension = PocketDimensionTeleport.OnPlayerEscapePocketDimension;
		if (onPlayerEscapePocketDimension != null)
		{
			onPlayerEscapePocketDimension(hub);
		}
		PocketDimensionGenerator.RandomizeTeleports();
		PlayerEvents.OnLeftPocketDimension(new PlayerLeftPocketDimensionEventArgs(hub, instance, true));
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x00017C78 File Offset: 0x00015E78
	public static void TryKill(PocketDimensionTeleport instance, ReferenceHub hub)
	{
		PlayerLeavingPocketDimensionEventArgs playerLeavingPocketDimensionEventArgs = new PlayerLeavingPocketDimensionEventArgs(hub, instance, false);
		PlayerEvents.OnLeavingPocketDimension(playerLeavingPocketDimensionEventArgs);
		if (!playerLeavingPocketDimensionEventArgs.IsAllowed)
		{
			return;
		}
		if (playerLeavingPocketDimensionEventArgs.IsSuccessful)
		{
			PocketDimensionTeleport.Exit(instance, hub);
			return;
		}
		PocketDimensionTeleport.Kill(instance, hub);
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x00017CB4 File Offset: 0x00015EB4
	public static void Kill(PocketDimensionTeleport instance, ReferenceHub hub)
	{
		hub.playerStats.DealDamage(new UniversalDamageHandler(-1f, DeathTranslations.PocketDecay, null));
		PlayerEvents.OnLeftPocketDimension(new PlayerLeftPocketDimensionEventArgs(hub, instance, false));
	}

	// Token: 0x0600064B RID: 1611 RVA: 0x00017CDF File Offset: 0x00015EDF
	private void Awake()
	{
		PocketDimensionTeleport._anyModified = true;
		PocketDimensionTeleport.AllInstances.Add(this);
	}

	// Token: 0x0600064C RID: 1612 RVA: 0x00017CF3 File Offset: 0x00015EF3
	private void Start()
	{
		Action<PocketDimensionTeleport> onAdded = PocketDimensionTeleport.OnAdded;
		if (onAdded == null)
		{
			return;
		}
		onAdded(this);
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x00017D05 File Offset: 0x00015F05
	private void OnDestroy()
	{
		PocketDimensionTeleport._anyModified = true;
		PocketDimensionTeleport.AllInstances.Remove(this);
		Action<PocketDimensionTeleport> onRemoved = PocketDimensionTeleport.OnRemoved;
		if (onRemoved == null)
		{
			return;
		}
		onRemoved(this);
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x00017D29 File Offset: 0x00015F29
	private void Update()
	{
		if (!PocketDimensionTeleport._anyModified)
		{
			return;
		}
		Action onInstancesUpdated = PocketDimensionTeleport.OnInstancesUpdated;
		if (onInstancesUpdated != null)
		{
			onInstancesUpdated();
		}
		PocketDimensionTeleport._anyModified = false;
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x00017D4C File Offset: 0x00015F4C
	[ServerCallback]
	private void OnTriggerEnter(Collider other)
	{
		if (!NetworkServer.active)
		{
			return;
		}
		NetworkIdentity component = other.GetComponent<NetworkIdentity>();
		if (component == null)
		{
			return;
		}
		ReferenceHub referenceHub;
		if (!ReferenceHub.TryGetHubNetID(component.netId, out referenceHub))
		{
			return;
		}
		if (referenceHub.roleManager.CurrentRole.ActiveTime < 1f)
		{
			return;
		}
		if ((this._type == PocketDimensionTeleport.PDTeleportType.Killer || AlphaWarheadController.Detonated) && !PocketDimensionTeleport.DebugBool)
		{
			PocketDimensionTeleport.TryKill(this, referenceHub);
			return;
		}
		if (this._type == PocketDimensionTeleport.PDTeleportType.Exit || PocketDimensionTeleport.DebugBool)
		{
			PocketDimensionTeleport.TryExit(this, referenceHub);
		}
	}

	// Token: 0x06000652 RID: 1618 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x040005CB RID: 1483
	public static readonly HashSet<PocketDimensionTeleport> AllInstances = new HashSet<PocketDimensionTeleport>();

	// Token: 0x040005CE RID: 1486
	public const float DisabledDuration = 10f;

	// Token: 0x040005CF RID: 1487
	public static bool DebugBool;

	// Token: 0x040005D0 RID: 1488
	public static bool RefreshExit;

	// Token: 0x040005D1 RID: 1489
	private const float MinAliveDuration = 1f;

	// Token: 0x040005D2 RID: 1490
	private static bool _anyModified;

	// Token: 0x040005D3 RID: 1491
	private PocketDimensionTeleport.PDTeleportType _type;

	// Token: 0x02000149 RID: 329
	// (Invoke) Token: 0x06000654 RID: 1620
	public delegate void PlayerEscapePocketDimension(ReferenceHub hub);

	// Token: 0x0200014A RID: 330
	public enum PDTeleportType
	{
		// Token: 0x040005D5 RID: 1493
		Killer,
		// Token: 0x040005D6 RID: 1494
		Exit
	}
}
