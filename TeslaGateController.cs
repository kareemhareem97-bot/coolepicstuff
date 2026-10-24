using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using MEC;
using Mirror;
using PlayerRoles;
using PlayerStatsSystem;
using Subtitles;
using UnityEngine;

// Token: 0x0200016E RID: 366
public class TeslaGateController : NetworkBehaviour
{
	// Token: 0x1700012D RID: 301
	// (get) Token: 0x0600075F RID: 1887 RVA: 0x0001C16F File Offset: 0x0001A36F
	// (set) Token: 0x06000760 RID: 1888 RVA: 0x0001C176 File Offset: 0x0001A376
	public static TeslaGateController Singleton { get; private set; }

	// Token: 0x06000761 RID: 1889 RVA: 0x0001C180 File Offset: 0x0001A380
	private static void ServerReceiveMessage(NetworkConnection conn, TeslaHitMsg msg)
	{
		ReferenceHub referenceHub;
		if (!ReferenceHub.TryGetHubNetID(conn.identity.netId, out referenceHub))
		{
			return;
		}
		if (msg.Gate == null)
		{
			referenceHub.gameConsoleTransmission.SendToClient("Received non-existing tesla gate!", "red");
			return;
		}
		if (Vector3.Distance(msg.Gate.transform.position, referenceHub.transform.position) > msg.Gate.sizeOfTrigger * 2.2f)
		{
			referenceHub.gameConsoleTransmission.SendToClient("You are too far from a tesla gate!", "red");
			return;
		}
		DamageHandlerBase.CassieAnnouncement cassieAnnouncement = new DamageHandlerBase.CassieAnnouncement
		{
			Announcement = "SUCCESSFULLY TERMINATED BY AUTOMATIC SECURITY SYSTEM",
			SubtitleParts = new SubtitlePart[]
			{
				new SubtitlePart(SubtitleType.TerminatedBySecuritySystem, null)
			}
		};
		referenceHub.playerStats.DealDamage(new UniversalDamageHandler((float)UnityEngine.Random.Range(200, 300), DeathTranslations.Tesla, cassieAnnouncement));
	}

	// Token: 0x06000762 RID: 1890 RVA: 0x0001C263 File Offset: 0x0001A463
	private void Awake()
	{
		TeslaGateController.Singleton = this;
	}

	// Token: 0x06000763 RID: 1891 RVA: 0x0001C26B File Offset: 0x0001A46B
	private void Start()
	{
		Timing.RunCoroutine(this.DelayedStopIdleParticles());
		Action<NetworkConnectionToClient, TeslaHitMsg> handler;
		if ((handler = TeslaGateController.<>O.<0>__ServerReceiveMessage) == null)
		{
			handler = (TeslaGateController.<>O.<0>__ServerReceiveMessage = new Action<NetworkConnectionToClient, TeslaHitMsg>(TeslaGateController.ServerReceiveMessage));
		}
		NetworkServer.ReplaceHandler<TeslaHitMsg>(handler, true);
	}

	// Token: 0x06000764 RID: 1892 RVA: 0x0001C29A File Offset: 0x0001A49A
	private IEnumerator<float> DelayedStopIdleParticles()
	{
		int j;
		for (int i = 0; i < 15; i = j + 1)
		{
			yield return float.NegativeInfinity;
			j = i;
		}
		using (HashSet<TeslaGate>.Enumerator enumerator = TeslaGate.AllGates.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				TeslaGate teslaGate = enumerator.Current;
				if (!(teslaGate == null) && teslaGate.windupParticles != null)
				{
					foreach (ParticleSystem particleSystem in teslaGate.windupParticles)
					{
						if (!(particleSystem == null))
						{
							particleSystem.Stop();
						}
					}
				}
			}
			yield break;
		}
		yield break;
	}

	// Token: 0x06000765 RID: 1893 RVA: 0x0001C2A4 File Offset: 0x0001A4A4
	public void FixedUpdate()
	{
		if (NetworkServer.active)
		{
			using (HashSet<TeslaGate>.Enumerator enumerator = TeslaGate.AllGates.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					TeslaGate teslaGate = enumerator.Current;
					if (teslaGate.isActiveAndEnabled)
					{
						if (teslaGate.InactiveTime > 0f)
						{
							teslaGate.NetworkInactiveTime = Mathf.Max(0f, teslaGate.InactiveTime - Time.fixedDeltaTime);
						}
						else
						{
							bool flag = false;
							bool flag2 = false;
							ReferenceHub hub = null;
							ReferenceHub hub2 = null;
							foreach (ReferenceHub referenceHub in ReferenceHub.AllHubs)
							{
								if (referenceHub.IsAlive())
								{
									if (!flag)
									{
										flag = teslaGate.IsInIdleRange(referenceHub);
										if (flag)
										{
											PlayerIdlingTeslaEventArgs playerIdlingTeslaEventArgs = new PlayerIdlingTeslaEventArgs(referenceHub, teslaGate);
											PlayerEvents.OnIdlingTesla(playerIdlingTeslaEventArgs);
											if (!playerIdlingTeslaEventArgs.IsAllowed)
											{
												flag = false;
											}
											else
											{
												hub = referenceHub;
											}
										}
									}
									if (!flag2 && teslaGate.PlayerInRange(referenceHub) && !teslaGate.InProgress)
									{
										PlayerTriggeringTeslaEventArgs playerTriggeringTeslaEventArgs = new PlayerTriggeringTeslaEventArgs(referenceHub, teslaGate);
										PlayerEvents.OnTriggeringTesla(playerTriggeringTeslaEventArgs);
										if (playerTriggeringTeslaEventArgs.IsAllowed)
										{
											hub2 = referenceHub;
											flag2 = true;
										}
									}
								}
							}
							if (flag2)
							{
								teslaGate.ServerSideCode();
								PlayerEvents.OnTriggeredTesla(new PlayerTriggeredTeslaEventArgs(hub2, teslaGate));
							}
							if (flag != teslaGate.isIdling)
							{
								teslaGate.ServerSideIdle(flag);
								if (flag)
								{
									PlayerEvents.OnIdledTesla(new PlayerIdledTeslaEventArgs(hub, teslaGate));
								}
							}
						}
					}
				}
				return;
			}
		}
		foreach (TeslaGate teslaGate2 in TeslaGate.AllGates)
		{
			teslaGate2.ClientSideCode();
		}
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool Weaved()
	{
		return true;
	}

	// Token: 0x0200016F RID: 367
	[CompilerGenerated]
	private static class <>O
	{
		// Token: 0x0400069D RID: 1693
		public static Action<NetworkConnectionToClient, TeslaHitMsg> <0>__ServerReceiveMessage;
	}
}
