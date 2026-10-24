using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000BF RID: 191
public class GameplayOptionsMenu : MonoBehaviour
{
	// Token: 0x060003CA RID: 970 RVA: 0x0000F10C File Offset: 0x0000D30C
	public void Awake()
	{
		this.classIntroFastFadeSlider.value = (PlayerPrefsSl.Get("ClassIntroFastFade", false) ? 1f : 0f);
		this.headBobSlider.value = (PlayerPrefsSl.Get("HeadBob", true) ? 1f : 0f);
		this.toggleSprintSlider.value = (PlayerPrefsSl.Get("ToggleSprint", false) ? 1f : 0f);
		this.modeSwitchToggle079.value = (PlayerPrefsSl.Get("ModeSwitchSetting079", false) ? 1f : 0f);
		this.postProcessing079.value = (PlayerPrefsSl.Get("PostProcessing079", true) ? 1f : 0f);
		this.healthBarShowsExact.value = (PlayerPrefsSl.Get("HealthBarShowsExact", true) ? 1f : 0f);
		this.richPresence.value = (PlayerPrefsSl.Get("RichPresence", true) ? 1f : 0f);
		this.publicLobby.value = (PlayerPrefsSl.Get("PublicLobby", true) ? 1f : 0f);
		this.hideIP.value = (PlayerPrefsSl.Get("HideIP", false) ? 1f : 0f);
		this.toggleSearch.value = (PlayerPrefsSl.Get("ToggleSearch", false) ? 1f : 0f);
		this._isAwake = true;
	}

	// Token: 0x060003CB RID: 971 RVA: 0x0000F288 File Offset: 0x0000D488
	public void SaveSettings()
	{
		if (!this._isAwake)
		{
			return;
		}
		PlayerPrefsSl.Set("ClassIntroFastFade", (int)this.classIntroFastFadeSlider.value == 1);
		PlayerPrefsSl.Set("HeadBob", (int)this.headBobSlider.value == 1);
		PlayerPrefsSl.Set("ToggleSprint", (int)this.toggleSprintSlider.value == 1);
		PlayerPrefsSl.Set("ModeSwitchSetting079", (int)this.modeSwitchToggle079.value == 1);
		PlayerPrefsSl.Set("PostProcessing079", (int)this.postProcessing079.value == 1);
		PlayerPrefsSl.Set("HealthBarShowsExact", (int)this.healthBarShowsExact.value == 1);
		PlayerPrefsSl.Set("RichPresence", (int)this.richPresence.value == 1);
		PlayerPrefsSl.Set("PublicLobby", (int)this.publicLobby.value == 1);
		PlayerPrefsSl.Set("HideIP", (int)this.hideIP.value == 1);
		PlayerPrefsSl.Set("ToggleSearch", (int)this.toggleSearch.value == 1);
	}

	// Token: 0x0400035A RID: 858
	public Slider classIntroFastFadeSlider;

	// Token: 0x0400035B RID: 859
	public Slider headBobSlider;

	// Token: 0x0400035C RID: 860
	public Slider toggleSprintSlider;

	// Token: 0x0400035D RID: 861
	public Slider modeSwitchToggle079;

	// Token: 0x0400035E RID: 862
	public Slider postProcessing079;

	// Token: 0x0400035F RID: 863
	public Slider healthBarShowsExact;

	// Token: 0x04000360 RID: 864
	public Slider richPresence;

	// Token: 0x04000361 RID: 865
	public Slider publicLobby;

	// Token: 0x04000362 RID: 866
	public Slider hideIP;

	// Token: 0x04000363 RID: 867
	public Slider toggleSearch;

	// Token: 0x04000364 RID: 868
	private bool _isAwake;
}
