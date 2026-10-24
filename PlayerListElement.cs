using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VoiceChat;

// Token: 0x020000E7 RID: 231
public class PlayerListElement : MonoBehaviour
{
	// Token: 0x06000465 RID: 1125 RVA: 0x00010C2C File Offset: 0x0000EE2C
	private void Start()
	{
		VoiceChatMutes.OnFlagsSet += this.RefreshMute;
		this.RefreshMute(this.instance, VoiceChatMutes.GetFlags(this.instance));
	}

	// Token: 0x06000466 RID: 1126 RVA: 0x00010C56 File Offset: 0x0000EE56
	private void OnDestroy()
	{
		VoiceChatMutes.OnFlagsSet -= this.RefreshMute;
	}

	// Token: 0x06000467 RID: 1127 RVA: 0x00010C69 File Offset: 0x0000EE69
	private void RefreshMute(ReferenceHub hub, VcMuteFlags flags)
	{
		if (hub != this.instance)
		{
			return;
		}
		if (!ReferenceHub.AllHubs.Contains(hub))
		{
			return;
		}
		this.ToggleMute.isOn = (flags > VcMuteFlags.None);
	}

	// Token: 0x06000468 RID: 1128 RVA: 0x00002E9A File Offset: 0x0000109A
	public void Mute(bool b)
	{
	}

	// Token: 0x06000469 RID: 1129 RVA: 0x00002E9A File Offset: 0x0000109A
	public void OpenSteamAccount()
	{
	}

	// Token: 0x0600046A RID: 1130 RVA: 0x00002E9A File Offset: 0x0000109A
	public void Report()
	{
	}

	// Token: 0x040003E0 RID: 992
	public ReferenceHub instance;

	// Token: 0x040003E1 RID: 993
	public TextMeshProUGUI TextNick;

	// Token: 0x040003E2 RID: 994
	public TextMeshProUGUI TextBadge;

	// Token: 0x040003E3 RID: 995
	public RawImage ImgVerified;

	// Token: 0x040003E4 RID: 996
	public Image ImgBackground;

	// Token: 0x040003E5 RID: 997
	public Toggle ToggleMute;

	// Token: 0x040003E6 RID: 998
	public GameObject OpenProfile;
}
