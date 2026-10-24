using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x020000D6 RID: 214
[RequireComponent(typeof(TMP_Dropdown))]
[DisallowMultipleComponent]
public class DropDownController : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x06000425 RID: 1061 RVA: 0x00010021 File Offset: 0x0000E221
	private void Awake()
	{
		this._dropdown = base.GetComponent<TMP_Dropdown>();
	}

	// Token: 0x06000426 RID: 1062 RVA: 0x00010030 File Offset: 0x0000E230
	public void OnPointerClick(PointerEventData eventData)
	{
		Canvas componentInChildren = base.GetComponentInChildren<Canvas>();
		if (!componentInChildren)
		{
			return;
		}
		Toggle[] componentsInChildren = componentInChildren.GetComponentsInChildren<Toggle>(true);
		for (int i = 1; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].interactable = !this.indexesToDisable.Contains(i - 1);
		}
	}

	// Token: 0x06000427 RID: 1063 RVA: 0x0001007C File Offset: 0x0000E27C
	public void EnableOption(int index, bool enable)
	{
		if (index < 1 || index >= this._dropdown.options.Count)
		{
			Debug.LogWarning("Index out of range -> ignored!", this);
			return;
		}
		if (enable)
		{
			if (this.indexesToDisable.Contains(index))
			{
				this.indexesToDisable.Remove(index);
			}
		}
		else if (!this.indexesToDisable.Contains(index))
		{
			this.indexesToDisable.Add(index);
		}
		Canvas componentInChildren = base.GetComponentInChildren<Canvas>();
		if (!componentInChildren)
		{
			return;
		}
		componentInChildren.GetComponentsInChildren<Toggle>(true)[index].interactable = enable;
	}

	// Token: 0x06000428 RID: 1064 RVA: 0x00010108 File Offset: 0x0000E308
	public void EnableOption(string label, bool enable)
	{
		int num = this._dropdown.options.FindIndex((TMP_Dropdown.OptionData o) => string.Equals(o.text, label));
		this.EnableOption(num + 1, enable);
	}

	// Token: 0x040003AD RID: 941
	[Tooltip("Indexes that should be ignored. Indexes are 0 based.")]
	public List<int> indexesToDisable = new List<int>();

	// Token: 0x040003AE RID: 942
	private TMP_Dropdown _dropdown;
}
