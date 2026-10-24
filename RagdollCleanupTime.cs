using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000E0 RID: 224
public class RagdollCleanupTime : MonoBehaviour
{
	// Token: 0x06000444 RID: 1092 RVA: 0x000105B4 File Offset: 0x0000E7B4
	public void Start()
	{
		this.DisabledString = TranslationReader.Get("NewMainMenu", 73, "Disabled");
		this.SecondsString = TranslationReader.Get("NewMainMenu", 74, "Seconds");
		this.SecondString = TranslationReader.Get("NewMainMenu", 86, "Second");
		int num = PlayerPrefsSl.Get("ragdoll_cleanup", 0);
		if (num <= 0)
		{
			this.Text.SetTextWithoutNotify(string.Empty);
			this.SecondsText.text = this.DisabledString;
		}
		else
		{
			this.SecondsText.text = ((num == 1) ? this.SecondString : this.SecondsString);
			this.Text.SetTextWithoutNotify(num.ToString());
			Timing.RunCoroutine(this.SetPositionOneFrameLater());
		}
		this.Slider.SetValueWithoutNotify((float)num);
	}

	// Token: 0x06000445 RID: 1093 RVA: 0x00010684 File Offset: 0x0000E884
	public void OnValueChanged(float value)
	{
		Mathf.Clamp(value, 0f, 604800f);
		if (value <= 0f)
		{
			this.Text.SetTextWithoutNotify(string.Empty);
			this.SecondsText.text = this.DisabledString;
		}
		else
		{
			this.SecondsText.text = ((value == 1f) ? this.SecondString : this.SecondsString);
			this.Text.SetTextWithoutNotify(value.ToString());
		}
		Timing.RunCoroutine(this.SetPositionOneFrameLater());
		this.Slider.SetValueWithoutNotify(value);
		PlayerPrefsSl.Set("ragdoll_cleanup", (int)value);
	}

	// Token: 0x06000446 RID: 1094 RVA: 0x00010724 File Offset: 0x0000E924
	private IEnumerator<float> SetPositionOneFrameLater()
	{
		yield return float.NegativeInfinity;
		this.SecondsText.rectTransform.anchoredPosition = new Vector2(this.Text.textComponent.preferredWidth + 10f, 0f);
		yield break;
	}

	// Token: 0x06000447 RID: 1095 RVA: 0x00010734 File Offset: 0x0000E934
	public void OnStringValueChanged(string value)
	{
		if (value.Length == 0)
		{
			this.OnValueChanged(0f);
		}
		float value2;
		if (float.TryParse(value, out value2))
		{
			this.OnValueChanged(value2);
		}
	}

	// Token: 0x040003C3 RID: 963
	private string DisabledString;

	// Token: 0x040003C4 RID: 964
	private string SecondsString;

	// Token: 0x040003C5 RID: 965
	private string SecondString;

	// Token: 0x040003C6 RID: 966
	public InputField Text;

	// Token: 0x040003C7 RID: 967
	public Text SecondsText;

	// Token: 0x040003C8 RID: 968
	public Slider Slider;
}
