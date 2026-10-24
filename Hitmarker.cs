using System;
using CustomPlayerEffects;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using Mirror;
using PlayerRoles.Spectating;
using PlayerStatsSystem;
using UnityEngine;

// Token: 0x020000C3 RID: 195
public class Hitmarker : MonoBehaviour
{
	// Token: 0x060003D5 RID: 981 RVA: 0x00002E9A File Offset: 0x0000109A
	public static void PlayHitmarker(float size, bool playAudio = true, HitmarkerType hitmarkerType = HitmarkerType.Regular)
	{
	}

	// Token: 0x060003D6 RID: 982 RVA: 0x0000F4E8 File Offset: 0x0000D6E8
	public static void SendHitmarkerDirectly(ReferenceHub hub, float size, bool playAudio = true, HitmarkerType hitmarkerType = HitmarkerType.Regular)
	{
		if (hub == null)
		{
			return;
		}
		size = Mathf.Clamp(size, 0f, 2.55f);
		if (hub.isLocalPlayer)
		{
			Hitmarker.PlayHitmarker(size, playAudio, hitmarkerType);
			return;
		}
		PlayerSendingHitmarkerEventArgs playerSendingHitmarkerEventArgs = new PlayerSendingHitmarkerEventArgs(hub, size, playAudio, hitmarkerType);
		PlayerEvents.OnSendingHitmarker(playerSendingHitmarkerEventArgs);
		if (!playerSendingHitmarkerEventArgs.IsAllowed)
		{
			return;
		}
		size = playerSendingHitmarkerEventArgs.Size;
		playAudio = playerSendingHitmarkerEventArgs.PlayAudio;
		hub = playerSendingHitmarkerEventArgs.Player.ReferenceHub;
		hitmarkerType = playerSendingHitmarkerEventArgs.Hitmarker;
		new Hitmarker.HitmarkerMessage((byte)Mathf.RoundToInt(size / 2.55f * 255f), playAudio, hitmarkerType).SendToSpectatorsOf(hub, true);
		PlayerEvents.OnSentHitmarker(new PlayerSentHitmarkerEventArgs(hub, size, playAudio, hitmarkerType));
	}

	// Token: 0x060003D7 RID: 983 RVA: 0x0000F590 File Offset: 0x0000D790
	public static void SendHitmarkerDirectly(NetworkConnection conn, float size)
	{
		ReferenceHub hub;
		if (!ReferenceHub.TryGetHub(conn, out hub))
		{
			return;
		}
		Hitmarker.SendHitmarkerDirectly(hub, size, true, HitmarkerType.Regular);
	}

	// Token: 0x060003D8 RID: 984 RVA: 0x0000F5B1 File Offset: 0x0000D7B1
	public static void SendHitmarkerConditionally(float size, AttackerDamageHandler adh, ReferenceHub victim)
	{
		if (Hitmarker.CheckHitmarkerPerms(adh, victim))
		{
			Hitmarker.SendHitmarkerDirectly(adh.Attacker.Hub, size, true, HitmarkerType.Regular);
		}
	}

	// Token: 0x060003D9 RID: 985 RVA: 0x0000F5D0 File Offset: 0x0000D7D0
	public static bool CheckHitmarkerPerms(AttackerDamageHandler adh, ReferenceHub victim)
	{
		IHitmarkerPreventer hitmarkerPreventer = victim.roleManager.CurrentRole as IHitmarkerPreventer;
		if (hitmarkerPreventer != null && hitmarkerPreventer.TryPreventHitmarker(adh))
		{
			PlayerEvents.OnCheckedHitmarker(new PlayerCheckedHitmarkerEventArgs(adh.Attacker.Hub, adh, victim, false));
			return false;
		}
		PlayerEffectsController playerEffectsController = victim.playerEffectsController;
		for (int i = 0; i < playerEffectsController.EffectsLength; i++)
		{
			StatusEffectBase statusEffectBase = playerEffectsController.AllEffects[i];
			IHitmarkerPreventer hitmarkerPreventer2 = statusEffectBase as IHitmarkerPreventer;
			if (hitmarkerPreventer2 != null && statusEffectBase.IsEnabled && hitmarkerPreventer2.TryPreventHitmarker(adh))
			{
				PlayerEvents.OnCheckedHitmarker(new PlayerCheckedHitmarkerEventArgs(adh.Attacker.Hub, adh, victim, false));
				return false;
			}
		}
		PlayerEvents.OnCheckedHitmarker(new PlayerCheckedHitmarkerEventArgs(adh.Attacker.Hub, adh, victim, true));
		return true;
	}

	// Token: 0x04000368 RID: 872
	private const float MaxSize = 2.55f;

	// Token: 0x020000C4 RID: 196
	public struct HitmarkerMessage : NetworkMessage
	{
		// Token: 0x060003DB RID: 987 RVA: 0x0000F686 File Offset: 0x0000D886
		public HitmarkerMessage(byte size, bool playAudio = true, HitmarkerType hitmarkerType = HitmarkerType.Regular)
		{
			this.Size = size;
			this.Audio = playAudio;
			this.Hitmarker = hitmarkerType;
		}

		// Token: 0x04000369 RID: 873
		public byte Size;

		// Token: 0x0400036A RID: 874
		public bool Audio;

		// Token: 0x0400036B RID: 875
		public HitmarkerType Hitmarker;
	}
}
