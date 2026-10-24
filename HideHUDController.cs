using System;
using GameCore;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020000C1 RID: 193
public class HideHUDController : MonoBehaviour
{
	// Token: 0x1400000C RID: 12
	// (add) Token: 0x060003CE RID: 974 RVA: 0x0000F398 File Offset: 0x0000D598
	// (remove) Token: 0x060003CF RID: 975 RVA: 0x0000F3CC File Offset: 0x0000D5CC
	public static event Action<bool> ToggleHUD;

	// Token: 0x170000AB RID: 171
	// (get) Token: 0x060003D0 RID: 976 RVA: 0x0000F3FF File Offset: 0x0000D5FF
	public static bool IsHUDVisible
	{
		get
		{
			return HideHUDController._singleton == null || HideHUDController._singleton._showHUDElements;
		}
	}

	// Token: 0x060003D1 RID: 977 RVA: 0x0000F41A File Offset: 0x0000D61A
	private void Awake()
	{
		HideHUDController._singleton = this;
	}

	// Token: 0x060003D2 RID: 978 RVA: 0x0000F424 File Offset: 0x0000D624
	private void Update()
	{
		if (RoundStart.singleton == null || !RoundStart.RoundStarted || !Input.GetKeyDown(NewInput.GetKey(ActionName.HideGUI, KeyCode.None)))
		{
			return;
		}
		if (this._showHUDElements)
		{
			InputField[] array = UnityEngine.Object.FindObjectsOfType<InputField>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].isFocused)
				{
					return;
				}
			}
			TMP_InputField[] array2 = UnityEngine.Object.FindObjectsOfType<TMP_InputField>();
			for (int i = 0; i < array2.Length; i++)
			{
				if (array2[i].isFocused)
				{
					return;
				}
			}
		}
		this._showHUDElements = !this._showHUDElements;
		Action<bool> toggleHUD = HideHUDController.ToggleHUD;
		if (toggleHUD != null)
		{
			toggleHUD(this._showHUDElements);
		}
		GameMenu.singleton.hideHUDText.SetActive(!this._showHUDElements);
	}

	// Token: 0x04000366 RID: 870
	private static HideHUDController _singleton;

	// Token: 0x04000367 RID: 871
	private bool _showHUDElements = true;
}
