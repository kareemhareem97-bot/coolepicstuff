using System;
using System.Collections.Generic;
using MEC;
using Mirror;
using NorthwoodLib;
using TMPro;
using ToggleableMenus;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020000BD RID: 189
public class GameMenu : SimpleToggleableMenu
{
	// Token: 0x170000A7 RID: 167
	// (get) Token: 0x060003B9 RID: 953 RVA: 0x00002DA5 File Offset: 0x00000FA5
	public override bool CanToggle
	{
		get
		{
			return true;
		}
	}

	// Token: 0x170000A8 RID: 168
	// (get) Token: 0x060003BA RID: 954 RVA: 0x0000ED78 File Offset: 0x0000CF78
	public override bool LockMovement
	{
		get
		{
			GameObject currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
			TMP_InputField tmp_InputField;
			return currentSelectedGameObject != null && currentSelectedGameObject.TryGetComponent<TMP_InputField>(out tmp_InputField) && tmp_InputField.isFocused;
		}
	}

	// Token: 0x060003BB RID: 955 RVA: 0x0000EDAB File Offset: 0x0000CFAB
	protected override void Awake()
	{
		base.Awake();
		GameMenu.singleton = this;
	}

	// Token: 0x060003BC RID: 956 RVA: 0x0000EDB9 File Offset: 0x0000CFB9
	private void Update()
	{
		if (HideHUDController.IsHUDVisible)
		{
			return;
		}
		this.hideHUDText.GetComponent<TextMeshProUGUI>().text = "Warning: HUD is hidden\n" + string.Format("Press <b>{0}</b> to enable HUD", NewInput.GetKey(ActionName.HideGUI, KeyCode.None));
	}

	// Token: 0x060003BD RID: 957 RVA: 0x0000EDF4 File Offset: 0x0000CFF4
	private IEnumerator<float> _ShowServerInfo(string id)
	{
		this.infoText.text = "";
		if (id.Contains('/'))
		{
			this.infoText.text = "The URL isn't directing to pastebin site. Please contact server owner.";
		}
		else
		{
			using (HttpCoroutine http = HttpQuery.GetCoroutine("https://pastebin.com/raw/" + id))
			{
				yield return http.Wait();
				try
				{
					http.CheckResult();
					string text = http.GetText();
					if (text.Contains("<title>Pastebin.com - Locked Paste</title>", StringComparison.OrdinalIgnoreCase))
					{
						this.infoText.text = "The provided paste is locked via password and cannot be displayed. Please contact the server owner.";
						yield break;
					}
					if (text.Length > 5000)
					{
						text = text.TruncateToLast(5000, '\n');
						text = text + "...\n<i><color=#87CEFA><u><link=\"https://pastebin.com/" + id + "\">(Click here for full content)</link></u></color></i>";
					}
					this.infoText.text = text;
					this._pastebinDisplayed = true;
				}
				catch (Exception ex)
				{
					this.infoText.text = "Error getting server info: " + ex.Message;
				}
			}
			HttpCoroutine http = null;
		}
		yield break;
		yield break;
	}

	// Token: 0x060003BE RID: 958 RVA: 0x0000EE0C File Offset: 0x0000D00C
	protected override void OnToggled()
	{
		base.OnToggled();
		foreach (GameObject gameObject in this.minors)
		{
			if (gameObject.activeSelf)
			{
				gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x060003BF RID: 959 RVA: 0x0000EE48 File Offset: 0x0000D048
	public void SelectMinor(int id)
	{
		foreach (GameObject gameObject in this.minors)
		{
			if (gameObject.activeSelf)
			{
				gameObject.SetActive(false);
			}
		}
		this.minors[id].SetActive(true);
		ReferenceHub referenceHub;
		if (id == 2 && !this._pastebinDisplayed && ReferenceHub.TryGetHostHub(out referenceHub))
		{
			Timing.RunCoroutine(this._ShowServerInfo(referenceHub.characterClassManager.Pastebin), Segment.FixedUpdate);
		}
	}

	// Token: 0x060003C0 RID: 960 RVA: 0x0000EEB8 File Offset: 0x0000D0B8
	public void Disconnect()
	{
		if (NetworkServer.active)
		{
			NetworkManager.singleton.StopHost();
			return;
		}
		NetworkManager.singleton.StopClient();
	}

	// Token: 0x060003C1 RID: 961 RVA: 0x0000EED6 File Offset: 0x0000D0D6
	public void Exit()
	{
		this.IsEnabled = false;
	}

	// Token: 0x0400034F RID: 847
	public static GameMenu singleton;

	// Token: 0x04000350 RID: 848
	public GameObject[] minors;

	// Token: 0x04000351 RID: 849
	public Graphic[] colorableElements;

	// Token: 0x04000352 RID: 850
	public TextMeshProUGUI infoText;

	// Token: 0x04000353 RID: 851
	public GameObject hideHUDText;

	// Token: 0x04000354 RID: 852
	private bool _pastebinDisplayed;
}
