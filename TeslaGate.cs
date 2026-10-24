using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Hazards;
using MapGeneration;
using MEC;
using Mirror;
using Mirror.RemoteCalls;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using UnityEngine;

// Token: 0x02000168 RID: 360
public class TeslaGate : NetworkBehaviour
{
	// Token: 0x17000125 RID: 293
	// (get) Token: 0x0600071C RID: 1820 RVA: 0x0001B4BB File Offset: 0x000196BB
	public Vector3 Position
	{
		get
		{
			if (this._position == Vector3.zero)
			{
				this._position = base.transform.position;
			}
			return this._position;
		}
	}

	// Token: 0x1400001C RID: 28
	// (add) Token: 0x0600071D RID: 1821 RVA: 0x0001B4E8 File Offset: 0x000196E8
	// (remove) Token: 0x0600071E RID: 1822 RVA: 0x0001B51C File Offset: 0x0001971C
	public static event TeslaGate.BurstComplete OnBurstComplete;

	// Token: 0x1400001D RID: 29
	// (add) Token: 0x0600071F RID: 1823 RVA: 0x0001B550 File Offset: 0x00019750
	// (remove) Token: 0x06000720 RID: 1824 RVA: 0x0001B584 File Offset: 0x00019784
	public static event Action<TeslaGate> OnAdded;

	// Token: 0x1400001E RID: 30
	// (add) Token: 0x06000721 RID: 1825 RVA: 0x0001B5B8 File Offset: 0x000197B8
	// (remove) Token: 0x06000722 RID: 1826 RVA: 0x0001B5EC File Offset: 0x000197EC
	public static event Action<TeslaGate> OnRemoved;

	// Token: 0x1400001F RID: 31
	// (add) Token: 0x06000723 RID: 1827 RVA: 0x0001B620 File Offset: 0x00019820
	// (remove) Token: 0x06000724 RID: 1828 RVA: 0x0001B654 File Offset: 0x00019854
	public static event Action<TeslaGate> OnBursted;

	// Token: 0x06000725 RID: 1829 RVA: 0x0001B687 File Offset: 0x00019887
	public void ServerSideCode()
	{
		if (this.InProgress)
		{
			return;
		}
		Timing.RunCoroutine(this.ServerSideWaitForAnimation());
		this.RpcPlayAnimation();
	}

	// Token: 0x06000726 RID: 1830 RVA: 0x0001B6A4 File Offset: 0x000198A4
	private IEnumerator<float> ServerSideWaitForAnimation()
	{
		this.InProgress = true;
		yield return Timing.WaitForSeconds(this.windupTime);
		if (this.TantrumsToBeDestroyed.Count > 0)
		{
			this.TantrumsToBeDestroyed.ForEach(delegate(TantrumEnvironmentalHazard tantrum)
			{
				if (tantrum != null)
				{
					tantrum.PlaySizzle = true;
					tantrum.ServerDestroy();
				}
			});
			this.TantrumsToBeDestroyed.Clear();
		}
		yield return Timing.WaitForSeconds(this.cooldownTime);
		this.InProgress = false;
		yield break;
	}

	// Token: 0x06000727 RID: 1831 RVA: 0x0001B6B3 File Offset: 0x000198B3
	public void ServerSideIdle(bool shouldIdle)
	{
		if (shouldIdle)
		{
			this.RpcDoIdle();
			return;
		}
		this.RpcDoneIdling();
	}

	// Token: 0x06000728 RID: 1832 RVA: 0x00002E9A File Offset: 0x0000109A
	private void Update()
	{
	}

	// Token: 0x06000729 RID: 1833 RVA: 0x0001B6C5 File Offset: 0x000198C5
	private void Start()
	{
		TeslaGate.AllGates.Add(this);
		Action<TeslaGate> onAdded = TeslaGate.OnAdded;
		if (onAdded == null)
		{
			return;
		}
		onAdded(this);
	}

	// Token: 0x0600072A RID: 1834 RVA: 0x0001B6E3 File Offset: 0x000198E3
	private void OnDestroy()
	{
		TeslaGate.AllGates.Remove(this);
		Action<TeslaGate> onRemoved = TeslaGate.OnRemoved;
		if (onRemoved == null)
		{
			return;
		}
		onRemoved(this);
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x0001B704 File Offset: 0x00019904
	public void ClientSideCode()
	{
		base.transform.localPosition = this.localPosition;
		base.transform.localRotation = Quaternion.Euler(this.localRotation);
		if (this.ledLights != null)
		{
			this.ledLights.SetBool(TeslaGate._animatorShockHash, this.InProgress);
			this.ledLights.SetBool(TeslaGate._animatorIdleHash, this.isIdling);
		}
	}

	// Token: 0x0600072C RID: 1836 RVA: 0x0001B774 File Offset: 0x00019974
	[ClientRpc]
	private void RpcDoIdle()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		this.SendRPCInternal("System.Void TeslaGate::RpcDoIdle()", 482546151, writer, 0, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x0600072D RID: 1837 RVA: 0x0001B7A4 File Offset: 0x000199A4
	[ClientRpc]
	private void RpcDoneIdling()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		this.SendRPCInternal("System.Void TeslaGate::RpcDoneIdling()", -243325925, writer, 0, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x0600072E RID: 1838 RVA: 0x0001B7D4 File Offset: 0x000199D4
	[ClientRpc]
	private void RpcPlayAnimation()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		this.SendRPCInternal("System.Void TeslaGate::RpcPlayAnimation()", 2031582250, writer, 0, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x0600072F RID: 1839 RVA: 0x0001B804 File Offset: 0x00019A04
	[ClientRpc]
	public void RpcInstantBurst()
	{
		NetworkWriterPooled writer = NetworkWriterPool.Get();
		this.SendRPCInternal("System.Void TeslaGate::RpcInstantBurst()", -684941977, writer, 0, true);
		NetworkWriterPool.Return(writer);
	}

	// Token: 0x06000730 RID: 1840 RVA: 0x0001B834 File Offset: 0x00019A34
	private IEnumerator<float> _DoShock()
	{
		Action<TeslaGate> onBursted = TeslaGate.OnBursted;
		if (onBursted != null)
		{
			onBursted(this);
		}
		this.source.Stop();
		foreach (AudioClip audioClip in this.clipsShock)
		{
			if (audioClip != null)
			{
				this.source.PlayOneShot(audioClip);
			}
		}
		foreach (ParticleSystem particleSystem in this.windupParticles)
		{
			if (particleSystem != null)
			{
				particleSystem.Play();
			}
		}
		foreach (ParticleSystem particleSystem2 in this.shockParticles)
		{
			if (particleSystem2 != null)
			{
				particleSystem2.Play();
			}
		}
		ReferenceHub referenceHub;
		while (!ReferenceHub.TryGetLocalHub(out referenceHub))
		{
			yield return float.NegativeInfinity;
		}
		GameObject gameObject = referenceHub.gameObject;
		yield return Timing.WaitForSeconds(0.25f);
		yield return Timing.WaitForSeconds(0.25f);
		float num = 0f;
		foreach (ParticleSystem particleSystem3 in this.smokeParticles)
		{
			if (!(particleSystem3 == null))
			{
				if (particleSystem3.IsAlive() && num <= 0f)
				{
					num = particleSystem3.main.duration;
				}
				particleSystem3.Play();
			}
		}
		if (!this.isIdling)
		{
			foreach (ParticleSystem particleSystem4 in this.windupParticles)
			{
				if (particleSystem4 != null)
				{
					particleSystem4.Stop();
				}
			}
		}
		yield return Timing.WaitForSeconds(num);
		yield break;
	}

	// Token: 0x06000731 RID: 1841 RVA: 0x0001B843 File Offset: 0x00019A43
	private IEnumerator<float> _PlayAnimation()
	{
		bool is79 = this.next079burst;
		this.next079burst = false;
		if (!is79)
		{
			foreach (AudioClip clip in this.clipsWarmup)
			{
				this.source.PlayOneShot(clip);
			}
			ParticleSystem[] array2 = this.windupParticles;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].Play();
			}
			yield return Timing.WaitForSeconds(this.windupTime);
		}
		Timing.RunCoroutine(this._DoShock());
		yield return Timing.WaitForSeconds(is79 ? 0.5f : this.cooldownTime);
		yield break;
	}

	// Token: 0x06000732 RID: 1842 RVA: 0x0001B854 File Offset: 0x00019A54
	public bool PlayerInRange(ReferenceHub player)
	{
		ITeslaControllerRole teslaControllerRole = player.roleManager.CurrentRole as ITeslaControllerRole;
		if (teslaControllerRole != null && !teslaControllerRole.CanActivateShock)
		{
			return false;
		}
		IFpcRole fpcRole = player.roleManager.CurrentRole as IFpcRole;
		return fpcRole != null && this.InRange(fpcRole.FpcModule.Position);
	}

	// Token: 0x06000733 RID: 1843 RVA: 0x0001B8A6 File Offset: 0x00019AA6
	private bool InRange(Vector3 position)
	{
		return Vector3.Distance(this.Position, position) < this.sizeOfTrigger;
	}

	// Token: 0x06000734 RID: 1844 RVA: 0x0001B8BC File Offset: 0x00019ABC
	public bool IsInIdleRange(ReferenceHub player)
	{
		if (!player.IsAlive())
		{
			return false;
		}
		ITeslaControllerRole teslaControllerRole = player.roleManager.CurrentRole as ITeslaControllerRole;
		if (teslaControllerRole != null)
		{
			return teslaControllerRole.IsInIdleRange(this);
		}
		IFpcRole fpcRole = player.roleManager.CurrentRole as IFpcRole;
		return fpcRole != null && this.IsInIdleRange(fpcRole.FpcModule.Position);
	}

	// Token: 0x06000735 RID: 1845 RVA: 0x0001B916 File Offset: 0x00019B16
	public bool IsInIdleRange(Vector3 position)
	{
		return Vector3.Distance(this.Position, position) < this.distanceToIdle;
	}

	// Token: 0x06000736 RID: 1846 RVA: 0x0001B92C File Offset: 0x00019B2C
	private void OnDrawGizmosSelected()
	{
		if (!this.showGizmos)
		{
			return;
		}
		Gizmos.color = new Color(1f, 0f, 0f, 0.2f);
		GameObject[] array = this.killers;
		for (int i = 0; i < array.Length; i++)
		{
			Gizmos.DrawCube(array[i].transform.position + Vector3.up * (this.sizeOfKiller.y / 2f), this.sizeOfKiller);
		}
		Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
		Gizmos.DrawSphere(this.Position, this.sizeOfTrigger);
	}

	// Token: 0x06000738 RID: 1848 RVA: 0x0001B9F0 File Offset: 0x00019BF0
	static TeslaGate()
	{
		RemoteProcedureCalls.RegisterRpc(typeof(TeslaGate), "System.Void TeslaGate::RpcDoIdle()", new RemoteCallDelegate(TeslaGate.InvokeUserCode_RpcDoIdle));
		RemoteProcedureCalls.RegisterRpc(typeof(TeslaGate), "System.Void TeslaGate::RpcDoneIdling()", new RemoteCallDelegate(TeslaGate.InvokeUserCode_RpcDoneIdling));
		RemoteProcedureCalls.RegisterRpc(typeof(TeslaGate), "System.Void TeslaGate::RpcPlayAnimation()", new RemoteCallDelegate(TeslaGate.InvokeUserCode_RpcPlayAnimation));
		RemoteProcedureCalls.RegisterRpc(typeof(TeslaGate), "System.Void TeslaGate::RpcInstantBurst()", new RemoteCallDelegate(TeslaGate.InvokeUserCode_RpcInstantBurst));
	}

	// Token: 0x06000739 RID: 1849 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x17000126 RID: 294
	// (get) Token: 0x0600073A RID: 1850 RVA: 0x0001BAA8 File Offset: 0x00019CA8
	// (set) Token: 0x0600073B RID: 1851 RVA: 0x0001BABB File Offset: 0x00019CBB
	public float NetworkInactiveTime
	{
		get
		{
			return this.InactiveTime;
		}
		[param: In]
		set
		{
			base.GeneratedSyncVarSetter<float>(value, ref this.InactiveTime, 1UL, null);
		}
	}

	// Token: 0x0600073C RID: 1852 RVA: 0x0001BAD8 File Offset: 0x00019CD8
	protected void UserCode_RpcDoIdle()
	{
		if (!this.isIdling)
		{
			this.isIdling = true;
			this.loopSource.PlayOneShot(this.idleStart);
			this.loopSource.PlayDelayed(this.idleStart.length);
		}
		foreach (ParticleSystem particleSystem in this.windupParticles)
		{
			if (!particleSystem.isPlaying)
			{
				particleSystem.Play();
			}
		}
	}

	// Token: 0x0600073D RID: 1853 RVA: 0x0001BB42 File Offset: 0x00019D42
	protected static void InvokeUserCode_RpcDoIdle(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcDoIdle called on server.");
			return;
		}
		((TeslaGate)obj).UserCode_RpcDoIdle();
	}

	// Token: 0x0600073E RID: 1854 RVA: 0x0001BB68 File Offset: 0x00019D68
	protected void UserCode_RpcDoneIdling()
	{
		if (!this.isIdling)
		{
			return;
		}
		this.isIdling = false;
		this.loopSource.Stop();
		this.loopSource.PlayOneShot(this.idleEnd);
		ParticleSystem[] array = this.windupParticles;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop();
		}
	}

	// Token: 0x0600073F RID: 1855 RVA: 0x0001BBBE File Offset: 0x00019DBE
	protected static void InvokeUserCode_RpcDoneIdling(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcDoneIdling called on server.");
			return;
		}
		((TeslaGate)obj).UserCode_RpcDoneIdling();
	}

	// Token: 0x06000740 RID: 1856 RVA: 0x0001BBE1 File Offset: 0x00019DE1
	protected void UserCode_RpcPlayAnimation()
	{
		Timing.RunCoroutine(this._PlayAnimation(), Segment.FixedUpdate);
	}

	// Token: 0x06000741 RID: 1857 RVA: 0x0001BBF0 File Offset: 0x00019DF0
	protected static void InvokeUserCode_RpcPlayAnimation(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcPlayAnimation called on server.");
			return;
		}
		((TeslaGate)obj).UserCode_RpcPlayAnimation();
	}

	// Token: 0x06000742 RID: 1858 RVA: 0x0001BC13 File Offset: 0x00019E13
	protected void UserCode_RpcInstantBurst()
	{
		this.next079burst = true;
		Timing.RunCoroutine(this._PlayAnimation(), Segment.FixedUpdate);
	}

	// Token: 0x06000743 RID: 1859 RVA: 0x0001BC29 File Offset: 0x00019E29
	protected static void InvokeUserCode_RpcInstantBurst(NetworkBehaviour obj, NetworkReader reader, NetworkConnectionToClient senderConnection)
	{
		if (!NetworkClient.active)
		{
			Debug.LogError("RPC RpcInstantBurst called on server.");
			return;
		}
		((TeslaGate)obj).UserCode_RpcInstantBurst();
	}

	// Token: 0x06000744 RID: 1860 RVA: 0x0001BC4C File Offset: 0x00019E4C
	public override void SerializeSyncVars(NetworkWriter writer, bool forceAll)
	{
		base.SerializeSyncVars(writer, forceAll);
		if (forceAll)
		{
			writer.WriteFloat(this.InactiveTime);
			return;
		}
		writer.WriteULong(base.syncVarDirtyBits);
		if ((base.syncVarDirtyBits & 1UL) != 0UL)
		{
			writer.WriteFloat(this.InactiveTime);
		}
	}

	// Token: 0x06000745 RID: 1861 RVA: 0x0001BCA4 File Offset: 0x00019EA4
	public override void DeserializeSyncVars(NetworkReader reader, bool initialState)
	{
		base.DeserializeSyncVars(reader, initialState);
		if (initialState)
		{
			base.GeneratedSyncVarDeserialize<float>(ref this.InactiveTime, null, reader.ReadFloat());
			return;
		}
		long num = (long)reader.ReadULong();
		if ((num & 1L) != 0L)
		{
			base.GeneratedSyncVarDeserialize<float>(ref this.InactiveTime, null, reader.ReadFloat());
		}
	}

	// Token: 0x0400066D RID: 1645
	public Vector3 localPosition;

	// Token: 0x0400066E RID: 1646
	public Vector3 localRotation;

	// Token: 0x0400066F RID: 1647
	public GameObject[] killers;

	// Token: 0x04000670 RID: 1648
	public LayerMask killerMask;

	// Token: 0x04000671 RID: 1649
	public bool InProgress;

	// Token: 0x04000672 RID: 1650
	public Animator ledLights;

	// Token: 0x04000673 RID: 1651
	public ParticleSystem[] windupParticles;

	// Token: 0x04000674 RID: 1652
	public ParticleSystem[] shockParticles;

	// Token: 0x04000675 RID: 1653
	public ParticleSystem[] smokeParticles;

	// Token: 0x04000676 RID: 1654
	public static readonly HashSet<TeslaGate> AllGates = new HashSet<TeslaGate>();

	// Token: 0x04000677 RID: 1655
	public RoomIdentifier Room;

	// Token: 0x04000678 RID: 1656
	private Vector3 _position;

	// Token: 0x04000679 RID: 1657
	[Header("Parameters")]
	public Vector3 sizeOfKiller;

	// Token: 0x0400067A RID: 1658
	public float sizeOfTrigger;

	// Token: 0x0400067B RID: 1659
	public float distanceToIdle;

	// Token: 0x0400067C RID: 1660
	public float windupTime;

	// Token: 0x0400067D RID: 1661
	public float cooldownTime;

	// Token: 0x0400067E RID: 1662
	[Header("Idle Loop")]
	public bool isIdling;

	// Token: 0x0400067F RID: 1663
	public AudioSource loopSource;

	// Token: 0x04000680 RID: 1664
	public AudioClip idleStart;

	// Token: 0x04000681 RID: 1665
	public AudioClip idleLoop;

	// Token: 0x04000682 RID: 1666
	public AudioClip idleEnd;

	// Token: 0x04000683 RID: 1667
	[Header("Audio")]
	public AudioSource source;

	// Token: 0x04000684 RID: 1668
	public AudioClip[] clipsWarmup;

	// Token: 0x04000685 RID: 1669
	public AudioClip[] clipsShock;

	// Token: 0x04000686 RID: 1670
	public bool showGizmos;

	// Token: 0x04000687 RID: 1671
	[SyncVar]
	public float InactiveTime;

	// Token: 0x04000688 RID: 1672
	public List<TantrumEnvironmentalHazard> TantrumsToBeDestroyed = new List<TantrumEnvironmentalHazard>();

	// Token: 0x0400068D RID: 1677
	private bool next079burst;

	// Token: 0x0400068E RID: 1678
	private static readonly int _animatorShockHash = Animator.StringToHash("ShockActive");

	// Token: 0x0400068F RID: 1679
	private static readonly int _animatorIdleHash = Animator.StringToHash("IdleActive");

	// Token: 0x02000169 RID: 361
	// (Invoke) Token: 0x06000747 RID: 1863
	public delegate void BurstComplete(ReferenceHub hub, TeslaGate teslaGate);
}
